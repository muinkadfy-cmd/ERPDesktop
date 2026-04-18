using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface ITituloPagarRepository
{
    IReadOnlyList<TituloPagarGridRow> ListarParaGrid();
    IReadOnlyList<TituloPagar> Listar();
    TituloPagar? ObterPorId(long id);
    long Inserir(TituloPagar t);
    void Atualizar(TituloPagar t);
    void MarcarExcluido(long id);
    void RegistrarPagamento(long id, decimal valor);
}
