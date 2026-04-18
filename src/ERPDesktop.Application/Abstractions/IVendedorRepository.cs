using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface IVendedorRepository
{
    IReadOnlyList<Vendedor> ListarAtivos();
}
