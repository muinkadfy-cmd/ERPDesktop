using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class TituloReceberRepository : ITituloReceberRepository
{
    private readonly IDbConnectionFactory _factory;

    public TituloReceberRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<TituloReceberGridRow> ListarParaGrid()
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<TituloReceberGridRow>(
            """
            SELECT
              t.id AS Id,
              IFNULL(c.nome_razao_social, '') AS ClienteNome,
              t.descricao AS Descricao,
              t.data_vencimento AS Vencimento,
              t.valor AS Valor,
              t.valor_recebido AS ValorRecebido,
              t.situacao AS Situacao
            FROM titulos_receber t
            LEFT JOIN clientes c ON c.id = t.cliente_id
            WHERE t.excluido = 0
            ORDER BY t.data_vencimento DESC, t.id DESC;
            """).ToList();
    }

    public IReadOnlyList<TituloReceber> Listar()
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<TituloReceberDb>(
            """
            SELECT
              id AS Id, cliente_id AS ClienteId, descricao AS Descricao,
              data_emissao AS DataEmissao, data_vencimento AS DataVencimento,
              valor AS Valor, valor_recebido AS ValorRecebido, situacao AS Situacao,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM titulos_receber
            WHERE excluido = 0
            ORDER BY data_vencimento DESC, id DESC;
            """);
        return rows.Select(Map).ToList();
    }

    public TituloReceber? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<TituloReceberDb>(
            """
            SELECT
              id AS Id, cliente_id AS ClienteId, descricao AS Descricao,
              data_emissao AS DataEmissao, data_vencimento AS DataVencimento,
              valor AS Valor, valor_recebido AS ValorRecebido, situacao AS Situacao,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM titulos_receber WHERE id = @id AND excluido = 0;
            """,
            new { id });
        return row is null ? null : Map(row);
    }

    public long Inserir(TituloReceber t)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            INSERT INTO titulos_receber(
              cliente_id, descricao, data_emissao, data_vencimento, valor, valor_recebido, situacao, observacoes,
              excluido, criado_em, atualizado_em
            ) VALUES (
              @ClienteId, @Descricao, @DataEmissao, @DataVencimento, @Valor, @ValorRecebido, @Situacao, @Observacoes,
              0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;
        return conn.ExecuteScalar<long>(sql, ToParams(t));
    }

    public void Atualizar(TituloReceber t)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            """
            UPDATE titulos_receber SET
              cliente_id=@ClienteId,
              descricao=@Descricao,
              data_emissao=@DataEmissao,
              data_vencimento=@DataVencimento,
              valor=@Valor,
              valor_recebido=@ValorRecebido,
              situacao=@Situacao,
              observacoes=@Observacoes,
              atualizado_em=@AtualizadoEm
            WHERE id=@Id AND excluido=0;
            """,
            ToParams(t));
    }

    public void MarcarExcluido(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            "UPDATE titulos_receber SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    public void RegistrarRecebimento(long id, decimal valor)
    {
        using var conn = _factory.CreateOpenConnection();
        var t = ObterPorId(id);
        if (t is null)
            return;
        var novo = t.ValorRecebido + valor;
        var sit = novo >= t.Valor ? "Quitado" : "Aberto";
        conn.Execute(
            """
            UPDATE titulos_receber SET
              valor_recebido = valor_recebido + @v,
              situacao = @s,
              atualizado_em = @a
            WHERE id = @id AND excluido = 0;
            """,
            new { id, v = valor, s = sit, a = DateTime.UtcNow.ToString("O") });
    }

    private static object ToParams(TituloReceber t) => new
    {
        t.Id,
        t.ClienteId,
        t.Descricao,
        DataEmissao = t.DataEmissao.ToString("yyyy-MM-dd"),
        DataVencimento = t.DataVencimento.ToString("yyyy-MM-dd"),
        t.Valor,
        t.ValorRecebido,
        t.Situacao,
        t.Observacoes,
        CriadoEm = t.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = t.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static TituloReceber Map(TituloReceberDb r) => new()
    {
        Id = r.Id,
        ClienteId = r.ClienteId,
        Descricao = r.Descricao,
        DataEmissao = DateTime.TryParse(r.DataEmissao, out var de) ? de : DateTime.Today,
        DataVencimento = DateTime.TryParse(r.DataVencimento, out var dv) ? dv : DateTime.Today,
        Valor = r.Valor,
        ValorRecebido = r.ValorRecebido,
        Situacao = r.Situacao,
        Observacoes = r.Observacoes,
        Excluido = r.Excluido != 0,
        CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
        AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private sealed class TituloReceberDb
    {
        public long Id { get; set; }
        public long? ClienteId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string DataEmissao { get; set; } = string.Empty;
        public string DataVencimento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal ValorRecebido { get; set; }
        public string Situacao { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
