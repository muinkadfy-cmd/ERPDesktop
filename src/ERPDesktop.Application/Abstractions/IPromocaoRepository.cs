using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface IPromocaoRepository
{
    IReadOnlyList<PromocaoGridRow> ListarParaGrid();
    IReadOnlyList<Promocao> Listar();
    Promocao? ObterPorId(long id);
    long Inserir(Promocao p);
    void Atualizar(Promocao p);
    void MarcarExcluido(long id);
}
