using ERPDesktop.Application.DTOs;

namespace ERPDesktop.Application.Abstractions;

public interface IMovimentacaoFinanceiraRepository
{
    IReadOnlyList<MovimentacaoFinanceiraGridRow> ListarUltimas(int limite);
}
