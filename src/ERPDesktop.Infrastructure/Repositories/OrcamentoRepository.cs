using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class OrcamentoRepository : IOrcamentoRepository
{
    private readonly IDbConnectionFactory _factory;

    public OrcamentoRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public string GerarProximoNumero()
    {
        using var conn = _factory.CreateOpenConnection();
        var next = conn.ExecuteScalar<long>("SELECT IFNULL(MAX(id), 0) + 1 FROM orcamentos;");
        return "ORC" + next.ToString("D5");
    }

    public IReadOnlyList<OrcamentoGridRow> ListarParaGrid()
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<OrcamentoGridRow>(
            """
            SELECT
              o.id AS Id,
              o.numero AS Numero,
              o.data_emissao AS DataEmissao,
              IFNULL(c.nome_razao_social, '') AS ClienteNome,
              o.valor_total AS ValorTotal,
              o.situacao AS Situacao
            FROM orcamentos o
            LEFT JOIN clientes c ON c.id = o.cliente_id
            WHERE o.excluido = 0
            ORDER BY o.id DESC;
            """).ToList();
    }

    public IReadOnlyList<Orcamento> Listar()
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<OrcamentoDb>(
            """
            SELECT
              id AS Id, numero AS Numero, cliente_id AS ClienteId, data_emissao AS DataEmissao,
              situacao AS Situacao, valor_total AS ValorTotal, observacoes AS Observacoes,
              excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM orcamentos WHERE excluido = 0 ORDER BY id DESC;
            """);
        return rows.Select(Map).ToList();
    }

    public Orcamento? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<OrcamentoDb>(
            """
            SELECT
              id AS Id, numero AS Numero, cliente_id AS ClienteId, data_emissao AS DataEmissao,
              situacao AS Situacao, valor_total AS ValorTotal, observacoes AS Observacoes,
              excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM orcamentos WHERE id = @id AND excluido = 0;
            """,
            new { id });
        return row is null ? null : Map(row);
    }

    public long Inserir(Orcamento o)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            INSERT INTO orcamentos(
              numero, cliente_id, data_emissao, situacao, valor_total, observacoes,
              excluido, criado_em, atualizado_em
            ) VALUES (
              @Numero, @ClienteId, @DataEmissao, @Situacao, @ValorTotal, @Observacoes,
              0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;
        return conn.ExecuteScalar<long>(sql, ToParams(o));
    }

    public void Atualizar(Orcamento o)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            """
            UPDATE orcamentos SET
              numero=@Numero,
              cliente_id=@ClienteId,
              data_emissao=@DataEmissao,
              situacao=@Situacao,
              valor_total=@ValorTotal,
              observacoes=@Observacoes,
              atualizado_em=@AtualizadoEm
            WHERE id=@Id AND excluido=0;
            """,
            ToParams(o));
    }

    public void MarcarExcluido(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute("DELETE FROM orcamento_itens WHERE orcamento_id = @id;", new { id });
        conn.Execute(
            "UPDATE orcamentos SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    public IReadOnlyList<PedidoItemGridRow> ListarItensParaGrid(long orcamentoId)
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<PedidoItemGridRow>(
            """
            SELECT
              oi.id AS Id,
              oi.produto_id AS ProdutoId,
              oi.sequencia AS Sequencia,
              IFNULL(p.codigo, '') AS CodigoProduto,
              oi.descricao AS Descricao,
              oi.quantidade AS Quantidade,
              oi.valor_unitario AS ValorUnitario,
              oi.valor_total AS ValorTotal
            FROM orcamento_itens oi
            LEFT JOIN produtos p ON p.id = oi.produto_id
            INNER JOIN orcamentos o ON o.id = oi.orcamento_id AND o.excluido = 0
            WHERE oi.orcamento_id = @id
            ORDER BY oi.sequencia;
            """,
            new { id = orcamentoId }).ToList();
    }

    public void SalvarItens(long orcamentoId, IReadOnlyList<ItemPedidoLinha> linhas)
    {
        using var conn = _factory.CreateOpenConnection();
        var existe = conn.ExecuteScalar<int>(
            "SELECT COUNT(1) FROM orcamentos WHERE id = @id AND excluido = 0;",
            new { id = orcamentoId }) > 0;
        if (!existe)
            return;

        var sitBloq = conn.ExecuteScalar<string?>(
            "SELECT situacao FROM orcamentos WHERE id = @id AND excluido = 0;",
            new { id = orcamentoId });
        if (sitBloq is not null &&
            (string.Equals(sitBloq, "Faturado", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(sitBloq, "Cancelado", StringComparison.OrdinalIgnoreCase)))
            return;

        using var tx = conn.BeginTransaction();
        conn.Execute("DELETE FROM orcamento_itens WHERE orcamento_id = @id;", new { id = orcamentoId }, tx);
        var agora = DateTime.UtcNow.ToString("O");
        var seq = 1;
        foreach (var l in linhas)
        {
            if (l.Quantidade <= 0)
                continue;
            conn.Execute(
                """
                INSERT INTO orcamento_itens(
                  orcamento_id, produto_id, sequencia, descricao, quantidade, valor_unitario, valor_total, criado_em
                ) VALUES (
                  @oid, @pid, @seq, @desc, @q, @vu, @vt, @a
                );
                """,
                new
                {
                    oid = orcamentoId,
                    pid = l.ProdutoId,
                    seq = seq++,
                    desc = l.Descricao,
                    q = l.Quantidade,
                    vu = l.ValorUnitario,
                    vt = l.ValorTotal,
                    a = agora
                },
                tx);
        }

        var total = conn.ExecuteScalar<decimal>(
            "SELECT IFNULL(SUM(valor_total), 0) FROM orcamento_itens WHERE orcamento_id = @id;",
            new { id = orcamentoId },
            tx);
        conn.Execute(
            "UPDATE orcamentos SET valor_total = @t, atualizado_em = @a WHERE id = @id AND excluido = 0;",
            new { id = orcamentoId, t = total, a = agora },
            tx);
        tx.Commit();
    }

    private static object ToParams(Orcamento o) => new
    {
        o.Id,
        o.Numero,
        o.ClienteId,
        DataEmissao = o.DataEmissao.ToString("yyyy-MM-dd"),
        o.Situacao,
        o.ValorTotal,
        o.Observacoes,
        CriadoEm = o.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = o.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static Orcamento Map(OrcamentoDb r) => new()
    {
        Id = r.Id,
        Numero = r.Numero,
        ClienteId = r.ClienteId,
        DataEmissao = DateTime.TryParse(r.DataEmissao, out var d) ? d : DateTime.Today,
        Situacao = r.Situacao,
        ValorTotal = r.ValorTotal,
        Observacoes = r.Observacoes,
        Excluido = r.Excluido != 0,
        CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
        AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private sealed class OrcamentoDb
    {
        public long Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public long? ClienteId { get; set; }
        public string DataEmissao { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
