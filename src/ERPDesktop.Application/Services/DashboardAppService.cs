using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;

namespace ERPDesktop.Application.Services;

public sealed class DashboardAppService
{
    private readonly IDashboardQuery _query;

    public DashboardAppService(IDashboardQuery query)
    {
        _query = query;
    }

    public DashboardResumoDia ResumoHoje() => _query.ObterResumoDiaLocal(DateTime.Today);

    public IReadOnlyList<VendaPorDiaPonto> Vendas7Dias() => _query.ObterVendasUltimos7DiasLocal();

    public IReadOnlyList<VendaListagemRow> UltimasVendas(int n = 12) => _query.ObterUltimasVendas(n);

    public IReadOnlyList<EstoqueBaixoRow> EstoqueBaixo(int n = 10) => _query.ObterEstoqueBaixo(n);
}
