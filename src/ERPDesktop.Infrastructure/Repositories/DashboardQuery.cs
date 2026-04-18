using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class DashboardQuery : IDashboardQuery
{
    private readonly IDbConnectionFactory _factory;

    public DashboardQuery(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public DashboardResumoDia ObterResumoDiaLocal(DateTime diaLocal)
    {
        using var conn = _factory.CreateOpenConnection();
        var d = diaLocal.ToString("yyyy-MM-dd");

        var vendasHoje = conn.ExecuteScalar<decimal>(
            """
            SELECT IFNULL(SUM(valor_total), 0) FROM vendas
            WHERE excluido = 0 AND situacao = 'Finalizada' AND substr(data_emissao, 1, 10) = @d;
            """,
            new { d });

        var pares = conn.ExecuteScalar<decimal>(
            """
            SELECT IFNULL(SUM(vi.quantidade), 0)
            FROM venda_itens vi
            INNER JOIN vendas v ON v.id = vi.venda_id
            WHERE v.excluido = 0 AND v.situacao = 'Finalizada' AND substr(v.data_emissao, 1, 10) = @d;
            """,
            new { d });

        var qtdVendas = conn.ExecuteScalar<long>(
            """
            SELECT COUNT(1) FROM vendas
            WHERE excluido = 0 AND situacao = 'Finalizada' AND substr(data_emissao, 1, 10) = @d;
            """,
            new { d });

        var ticket = qtdVendas > 0 ? vendasHoje / qtdVendas : 0m;

        var skuBaixo = conn.ExecuteScalar<int>(
            """
            SELECT COUNT(1) FROM produtos
            WHERE excluido = 0 AND ativo = 1 AND estoque_atual <= estoque_minimo;
            """);

        var crAberto = conn.ExecuteScalar<decimal>(
            """
            SELECT IFNULL(SUM(valor - valor_recebido), 0) FROM titulos_receber
            WHERE excluido = 0 AND situacao = 'Aberto';
            """);

        return new DashboardResumoDia
        {
            VendasHoje = vendasHoje,
            ParesVendidosHoje = pares,
            TicketMedioHoje = ticket,
            SkuEstoqueBaixo = skuBaixo,
            ContasReceberAberto = crAberto
        };
    }

    public IReadOnlyList<VendaPorDiaPonto> ObterVendasUltimos7DiasLocal()
    {
        using var conn = _factory.CreateOpenConnection();
        var fim = DateTime.Today;
        var ini = fim.AddDays(-6);
        var rows = conn.Query<(string Dia, decimal Total)>(
            """
            SELECT substr(v.data_emissao, 1, 10) AS Dia, IFNULL(SUM(v.valor_total), 0) AS Total
            FROM vendas v
            WHERE v.excluido = 0 AND v.situacao = 'Finalizada'
              AND substr(v.data_emissao, 1, 10) >= @ini AND substr(v.data_emissao, 1, 10) <= @fim
            GROUP BY substr(v.data_emissao, 1, 10)
            ORDER BY Dia;
            """,
            new { ini = ini.ToString("yyyy-MM-dd"), fim = fim.ToString("yyyy-MM-dd") }).ToList();

        var map = rows.ToDictionary(r => r.Dia, r => r.Total);
        var lista = new List<VendaPorDiaPonto>();
        for (var dt = ini; dt <= fim; dt = dt.AddDays(1))
        {
            var key = dt.ToString("yyyy-MM-dd");
            lista.Add(new VendaPorDiaPonto { Dia = dt, Total = map.TryGetValue(key, out var t) ? t : 0m });
        }

        return lista;
    }

    public IReadOnlyList<VendaListagemRow> ObterUltimasVendas(int limite)
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<VendaListaDb>(
            """
            SELECT
              v.id AS Id,
              v.numero AS Numero,
              v.data_emissao AS DataEmissao,
              IFNULL(c.nome_razao_social, '') AS ClienteNome,
              v.valor_total AS ValorTotal,
              v.situacao AS Situacao
            FROM vendas v
            LEFT JOIN clientes c ON c.id = v.cliente_id
            WHERE v.excluido = 0
            ORDER BY v.id DESC
            LIMIT @lim;
            """,
            new { lim = limite });

        return rows.Select(r => new VendaListagemRow
        {
            Id = r.Id,
            Numero = r.Numero,
            DataEmissao = DateTime.TryParse(r.DataEmissao, out var dx) ? dx : DateTime.Today,
            ClienteNome = r.ClienteNome,
            ValorTotal = r.ValorTotal,
            Situacao = r.Situacao
        }).ToList();
    }

    public IReadOnlyList<EstoqueBaixoRow> ObterEstoqueBaixo(int limite)
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<EstoqueBaixoRow>(
            """
            SELECT
              id AS Id,
              codigo AS Codigo,
              descricao AS Descricao,
              estoque_atual AS EstoqueAtual,
              estoque_minimo AS EstoqueMinimo
            FROM produtos
            WHERE excluido = 0 AND ativo = 1 AND estoque_atual <= estoque_minimo
            ORDER BY estoque_atual ASC, descricao COLLATE NOCASE
            LIMIT @lim;
            """,
            new { lim = limite });
        return rows.ToList();
    }

    private sealed class VendaListaDb
    {
        public long Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string DataEmissao { get; set; } = string.Empty;
        public string ClienteNome { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string Situacao { get; set; } = string.Empty;
    }
}
