using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class PromocaoRepository : IPromocaoRepository
{
    private readonly IDbConnectionFactory _factory;

    public PromocaoRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<PromocaoGridRow> ListarParaGrid()
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<PromocaoGridRow>(
            """
            SELECT
              id AS Id,
              nome AS Nome,
              percentual_desconto AS Percentual,
              data_inicio AS Inicio,
              data_fim AS Fim,
              CASE WHEN ativo = 1 THEN 'Sim' ELSE 'Não' END AS AtivoTexto
            FROM promocoes
            WHERE excluido = 0
            ORDER BY id DESC;
            """).ToList();
    }

    public IReadOnlyList<Promocao> Listar()
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<PromocaoDb>(
            """
            SELECT
              id AS Id, nome AS Nome, percentual_desconto AS PercentualDesconto,
              data_inicio AS DataInicio, data_fim AS DataFim, ativo AS Ativo,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM promocoes WHERE excluido = 0 ORDER BY id DESC;
            """);
        return rows.Select(Map).ToList();
    }

    public Promocao? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<PromocaoDb>(
            """
            SELECT
              id AS Id, nome AS Nome, percentual_desconto AS PercentualDesconto,
              data_inicio AS DataInicio, data_fim AS DataFim, ativo AS Ativo,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM promocoes WHERE id = @id AND excluido = 0;
            """,
            new { id });
        return row is null ? null : Map(row);
    }

    public long Inserir(Promocao p)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            INSERT INTO promocoes(
              nome, percentual_desconto, data_inicio, data_fim, ativo, observacoes,
              excluido, criado_em, atualizado_em
            ) VALUES (
              @Nome, @PercentualDesconto, @DataInicio, @DataFim, @Ativo, @Observacoes,
              0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;
        return conn.ExecuteScalar<long>(sql, ToParams(p));
    }

    public void Atualizar(Promocao p)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            """
            UPDATE promocoes SET
              nome=@Nome,
              percentual_desconto=@PercentualDesconto,
              data_inicio=@DataInicio,
              data_fim=@DataFim,
              ativo=@Ativo,
              observacoes=@Observacoes,
              atualizado_em=@AtualizadoEm
            WHERE id=@Id AND excluido=0;
            """,
            ToParams(p));
    }

    public void MarcarExcluido(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            "UPDATE promocoes SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    private static object ToParams(Promocao p) => new
    {
        p.Id,
        p.Nome,
        p.PercentualDesconto,
        DataInicio = p.DataInicio.ToString("yyyy-MM-dd"),
        DataFim = p.DataFim.ToString("yyyy-MM-dd"),
        Ativo = p.Ativo ? 1 : 0,
        p.Observacoes,
        CriadoEm = p.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = p.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static Promocao Map(PromocaoDb r) => new()
    {
        Id = r.Id,
        Nome = r.Nome,
        PercentualDesconto = r.PercentualDesconto,
        DataInicio = DateTime.TryParse(r.DataInicio, out var di) ? di : DateTime.Today,
        DataFim = DateTime.TryParse(r.DataFim, out var df) ? df : DateTime.Today,
        Ativo = r.Ativo != 0,
        Observacoes = r.Observacoes,
        Excluido = r.Excluido != 0,
        CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
        AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private sealed class PromocaoDb
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal PercentualDesconto { get; set; }
        public string DataInicio { get; set; } = string.Empty;
        public string DataFim { get; set; } = string.Empty;
        public int Ativo { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
