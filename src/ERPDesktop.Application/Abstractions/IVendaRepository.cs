using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface IVendaRepository
{
    string GerarProximoNumero();
    long InserirVendaComItens(Venda venda, IReadOnlyList<VendaItem> itens);
}
