using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class CompraRepository : ICompraRepository
{
    private readonly IDbConnectionFactory _factory;

    public CompraRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public string GerarProximoNumero()
    {
        using var conn = _factory.CreateOpenConnection();
        var next = conn.ExecuteScalar<long>("SELECT IFNULL(MAX(id), 0) + 1 FROM compras;");
        return "C" + next.ToString("D5");
    }

    public IReadOnlyList<CompraGridRow> ListarParaGrid()
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<CompraGridRow>(
            """
            SELECT
              c.id AS Id,
              c.numero AS Numero,
              c.data_emissao AS DataEmissao,
              IFNULL(f.razao_social, '') AS FornecedorNome,
              c.valor_total AS ValorTotal,
              c.situacao AS Situacao
            FROM compras c
            LEFT JOIN fornecedores f ON f.id = c.fornecedor_id
            WHERE c.excluido = 0
            ORDER BY c.id DESC;
            """).ToList();
    }

    public IReadOnlyList<Compra> Listar()
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<CompraDb>(
            """
            SELECT
              id AS Id, numero AS Numero, fornecedor_id AS FornecedorId, data_emissao AS DataEmissao,
              situacao AS Situacao, valor_total AS ValorTotal, observacoes AS Observacoes,
              excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM compras WHERE excluido = 0 ORDER BY id DESC;
            """);
        return rows.Select(Map).ToList();
    }

    public Compra? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<CompraDb>(
            """
            SELECT
              id AS Id, numero AS Numero, fornecedor_id AS FornecedorId, data_emissao AS DataEmissao,
              situacao AS Situacao, valor_total AS ValorTotal, observacoes AS Observacoes,
              excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM compras WHERE id = @id AND excluido = 0;
            """,
            new { id });
        return row is null ? null : Map(row);
    }

    public long Inserir(Compra c)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            INSERT INTO compras(
              numero, fornecedor_id, data_emissao, situacao, valor_total, observacoes,
              excluido, criado_em, atualizado_em
            ) VALUES (
              @Numero, @FornecedorId, @DataEmissao, @Situacao, @ValorTotal, @Observacoes,
              0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;
        return conn.ExecuteScalar<long>(sql, ToParams(c));
    }

    public void Atualizar(Compra c)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            """
            UPDATE compras SET
              numero=@Numero,
              fornecedor_id=@FornecedorId,
              data_emissao=@DataEmissao,
              situacao=@Situacao,
              valor_total=@ValorTotal,
              observacoes=@Observacoes,
              atualizado_em=@AtualizadoEm
            WHERE id=@Id AND excluido=0;
            """,
            ToParams(c));
    }

    public void MarcarExcluido(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute("DELETE FROM compra_itens WHERE compra_id = @id;", new { id });
        conn.Execute(
            "UPDATE compras SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    public IReadOnlyList<PedidoItemGridRow> ListarItensParaGrid(long compraId)
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<PedidoItemGridRow>(
            """
            SELECT
              ci.id AS Id,
              ci.produto_id AS ProdutoId,
              ci.sequencia AS Sequencia,
              IFNULL(p.codigo, '') AS CodigoProduto,
              ci.descricao AS Descricao,
              ci.quantidade AS Quantidade,
              ci.valor_unitario AS ValorUnitario,
              ci.valor_total AS ValorTotal
            FROM compra_itens ci
            LEFT JOIN produtos p ON p.id = ci.produto_id
            INNER JOIN compras c ON c.id = ci.compra_id AND c.excluido = 0
            WHERE ci.compra_id = @id
            ORDER BY ci.sequencia;
            """,
            new { id = compraId }).ToList();
    }

    public void SalvarItens(long compraId, IReadOnlyList<ItemPedidoLinha> linhas)
    {
        using var conn = _factory.CreateOpenConnection();
        var existe = conn.ExecuteScalar<int>(
            "SELECT COUNT(1) FROM compras WHERE id = @id AND excluido = 0;",
            new { id = compraId }) > 0;
        if (!existe)
            return;

        var sitBloq = conn.ExecuteScalar<string?>(
            "SELECT situacao FROM compras WHERE id = @id AND excluido = 0;",
            new { id = compraId });
        if (sitBloq is not null &&
            (string.Equals(sitBloq, "Recebido", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(sitBloq, "Cancelado", StringComparison.OrdinalIgnoreCase)))
            return;

        using var tx = conn.BeginTransaction();
        conn.Execute("DELETE FROM compra_itens WHERE compra_id = @id;", new { id = compraId }, tx);
        var agora = DateTime.UtcNow.ToString("O");
        var seq = 1;
        foreach (var l in linhas)
        {
            if (l.Quantidade <= 0)
                continue;
            conn.Execute(
                """
                INSERT INTO compra_itens(
                  compra_id, produto_id, sequencia, descricao, quantidade, valor_unitario, valor_total, criado_em
                ) VALUES (
                  @cid, @pid, @seq, @desc, @q, @vu, @vt, @a
                );
                """,
                new
                {
                    cid = compraId,
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
            "SELECT IFNULL(SUM(valor_total), 0) FROM compra_itens WHERE compra_id = @id;",
            new { id = compraId },
            tx);
        conn.Execute(
            "UPDATE compras SET valor_total = @t, atualizado_em = @a WHERE id = @id AND excluido = 0;",
            new { id = compraId, t = total, a = agora },
            tx);
        tx.Commit();
    }

    public string? DarEntradaEstoque(long compraId)
    {
        using var conn = _factory.CreateOpenConnection();
        using var tx = conn.BeginTransaction();
        var situacao = conn.ExecuteScalar<string?>(
            "SELECT situacao FROM compras WHERE id = @id AND excluido = 0;",
            new { id = compraId },
            tx);
        if (situacao is null)
            return "Pedido não encontrado.";
        if (string.Equals(situacao, "Recebido", StringComparison.OrdinalIgnoreCase))
            return "Este pedido já teve entrada de estoque registrada.";
        if (string.Equals(situacao, "Cancelado", StringComparison.OrdinalIgnoreCase))
            return "Pedido cancelado — não é possível dar entrada no estoque.";

        var linhas = conn.Query<(long Pid, decimal Q)>(
            """
            SELECT produto_id AS Pid, quantidade AS Q
            FROM compra_itens
            WHERE compra_id = @id AND produto_id IS NOT NULL AND quantidade > 0;
            """,
            new { id = compraId },
            tx).ToList();
        if (linhas.Count == 0)
            return "Grave os itens do pedido com produto vinculado antes de dar entrada no estoque.";

        var agora = DateTime.UtcNow.ToString("O");
        foreach (var (pid, q) in linhas)
        {
            var n = conn.Execute(
                """
                UPDATE produtos SET
                  estoque_atual = estoque_atual + @q,
                  atualizado_em = @a
                WHERE id = @id AND excluido = 0;
                """,
                new { id = pid, q, a = agora },
                tx);
            if (n == 0)
            {
                tx.Rollback();
                return "Produto inválido ou excluído em uma das linhas do pedido.";
            }
        }

        conn.Execute(
            """
            UPDATE compras SET situacao = 'Recebido', atualizado_em = @a
            WHERE id = @id AND excluido = 0;
            """,
            new { id = compraId, a = agora },
            tx);
        tx.Commit();
        return null;
    }

    private static object ToParams(Compra c) => new
    {
        c.Id,
        c.Numero,
        c.FornecedorId,
        DataEmissao = c.DataEmissao.ToString("yyyy-MM-dd"),
        c.Situacao,
        c.ValorTotal,
        c.Observacoes,
        CriadoEm = c.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = c.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static Compra Map(CompraDb r) => new()
    {
        Id = r.Id,
        Numero = r.Numero,
        FornecedorId = r.FornecedorId,
        DataEmissao = DateTime.TryParse(r.DataEmissao, out var d) ? d : DateTime.Today,
        Situacao = r.Situacao,
        ValorTotal = r.ValorTotal,
        Observacoes = r.Observacoes,
        Excluido = r.Excluido != 0,
        CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
        AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private sealed class CompraDb
    {
        public long Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public long? FornecedorId { get; set; }
        public string DataEmissao { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
