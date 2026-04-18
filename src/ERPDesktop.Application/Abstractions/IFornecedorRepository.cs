using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface IFornecedorRepository
{
    IReadOnlyList<FornecedorGridRow> ListarParaGrid();
    IReadOnlyList<Fornecedor> ListarAtivos();
    Fornecedor? ObterPorId(long id);
    long Inserir(Fornecedor f);
    void Atualizar(Fornecedor f);
    void MarcarExcluido(long id);
    string GerarProximoCodigo();
}
