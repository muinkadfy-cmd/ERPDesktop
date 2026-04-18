using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class MovimentacaoFinanceiraRepository : IMovimentacaoFinanceiraRepository
{
    private readonly IDbConnectionFactory _factory;

    public MovimentacaoFinanceiraRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<MovimentacaoFinanceiraGridRow> ListarUltimas(int limite)
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<RowDb>(
            """
            SELECT
              id AS Id,
              data_movimento AS DataMovimento,
              tipo AS Tipo,
              historico AS Historico,
              valor AS Valor
            FROM movimentacoes_financeiras
            ORDER BY id DESC
            LIMIT @lim;
            """,
            new { lim = limite });

        return rows.Select(r => new MovimentacaoFinanceiraGridRow
        {
            Id = r.Id,
            DataMovimento = DateTime.TryParse(r.DataMovimento, out var d) ? d : DateTime.Today,
            Tipo = r.Tipo,
            Historico = r.Historico,
            Valor = r.Valor
        }).ToList();
    }

    private sealed class RowDb
    {
        public long Id { get; set; }
        public string DataMovimento { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Historico { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
