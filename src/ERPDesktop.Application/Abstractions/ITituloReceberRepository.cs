using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface ITituloReceberRepository
{
    IReadOnlyList<TituloReceberGridRow> ListarParaGrid();
    IReadOnlyList<TituloReceber> Listar();
    TituloReceber? ObterPorId(long id);
    long Inserir(TituloReceber t);
    void Atualizar(TituloReceber t);
    void MarcarExcluido(long id);
    void RegistrarRecebimento(long id, decimal valor);
}
