using ERPDesktop.Application.DTOs;

namespace ERPDesktop.Application.Abstractions;

public interface IDashboardQuery
{
    DashboardResumoDia ObterResumoDiaLocal(DateTime diaLocal);
    IReadOnlyList<VendaPorDiaPonto> ObterVendasUltimos7DiasLocal();
    IReadOnlyList<VendaListagemRow> ObterUltimasVendas(int limite);
    IReadOnlyList<EstoqueBaixoRow> ObterEstoqueBaixo(int limite);
}
