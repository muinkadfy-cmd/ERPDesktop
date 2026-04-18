using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class TituloPagarRepository : ITituloPagarRepository
{
    private readonly IDbConnectionFactory _factory;

    public TituloPagarRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<TituloPagarGridRow> ListarParaGrid()
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<TituloPagarGridRow>(
            """
            SELECT
              t.id AS Id,
              IFNULL(f.razao_social, '') AS FornecedorNome,
              t.descricao AS Descricao,
              t.data_vencimento AS Vencimento,
              t.valor AS Valor,
              t.valor_pago AS ValorPago,
              t.situacao AS Situacao
            FROM titulos_pagar t
            LEFT JOIN fornecedores f ON f.id = t.fornecedor_id
            WHERE t.excluido = 0
            ORDER BY t.data_vencimento DESC, t.id DESC;
            """).ToList();
    }

    public IReadOnlyList<TituloPagar> Listar()
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<TituloPagarDb>(
            """
            SELECT
              id AS Id, fornecedor_id AS FornecedorId, descricao AS Descricao,
              data_emissao AS DataEmissao, data_vencimento AS DataVencimento,
              valor AS Valor, valor_pago AS ValorPago, situacao AS Situacao,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM titulos_pagar
            WHERE excluido = 0
            ORDER BY data_vencimento DESC, id DESC;
            """);
        return rows.Select(Map).ToList();
    }

    public TituloPagar? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<TituloPagarDb>(
            """
            SELECT
              id AS Id, fornecedor_id AS FornecedorId, descricao AS Descricao,
              data_emissao AS DataEmissao, data_vencimento AS DataVencimento,
              valor AS Valor, valor_pago AS ValorPago, situacao AS Situacao,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM titulos_pagar WHERE id = @id AND excluido = 0;
            """,
            new { id });
        return row is null ? null : Map(row);
    }

    public long Inserir(TituloPagar t)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            INSERT INTO titulos_pagar(
              fornecedor_id, descricao, data_emissao, data_vencimento, valor, valor_pago, situacao, observacoes,
              excluido, criado_em, atualizado_em
            ) VALUES (
              @FornecedorId, @Descricao, @DataEmissao, @DataVencimento, @Valor, @ValorPago, @Situacao, @Observacoes,
              0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;
        return conn.ExecuteScalar<long>(sql, ToParams(t));
    }

    public void Atualizar(TituloPagar t)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            """
            UPDATE titulos_pagar SET
              fornecedor_id=@FornecedorId,
              descricao=@Descricao,
              data_emissao=@DataEmissao,
              data_vencimento=@DataVencimento,
              valor=@Valor,
              valor_pago=@ValorPago,
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
            "UPDATE titulos_pagar SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    public void RegistrarPagamento(long id, decimal valor)
    {
        using var conn = _factory.CreateOpenConnection();
        var t = ObterPorId(id);
        if (t is null)
            return;
        var novo = t.ValorPago + valor;
        var sit = novo >= t.Valor ? "Quitado" : "Aberto";
        conn.Execute(
            """
            UPDATE titulos_pagar SET
              valor_pago = valor_pago + @v,
              situacao = @s,
              atualizado_em = @a
            WHERE id = @id AND excluido = 0;
            """,
            new { id, v = valor, s = sit, a = DateTime.UtcNow.ToString("O") });
    }

    private static object ToParams(TituloPagar t) => new
    {
        t.Id,
        t.FornecedorId,
        t.Descricao,
        DataEmissao = t.DataEmissao.ToString("yyyy-MM-dd"),
        DataVencimento = t.DataVencimento.ToString("yyyy-MM-dd"),
        t.Valor,
        t.ValorPago,
        t.Situacao,
        t.Observacoes,
        CriadoEm = t.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = t.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static TituloPagar Map(TituloPagarDb r) => new()
    {
        Id = r.Id,
        FornecedorId = r.FornecedorId,
        Descricao = r.Descricao,
        DataEmissao = DateTime.TryParse(r.DataEmissao, out var de) ? de : DateTime.Today,
        DataVencimento = DateTime.TryParse(r.DataVencimento, out var dv) ? dv : DateTime.Today,
        Valor = r.Valor,
        ValorPago = r.ValorPago,
        Situacao = r.Situacao,
        Observacoes = r.Observacoes,
        Excluido = r.Excluido != 0,
        CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
        AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private sealed class TituloPagarDb
    {
        public long Id { get; set; }
        public long? FornecedorId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string DataEmissao { get; set; } = string.Empty;
        public string DataVencimento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal ValorPago { get; set; }
        public string Situacao { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
