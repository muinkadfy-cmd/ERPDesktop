using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface IOrcamentoRepository
{
    IReadOnlyList<OrcamentoGridRow> ListarParaGrid();
    IReadOnlyList<Orcamento> Listar();
    Orcamento? ObterPorId(long id);
    long Inserir(Orcamento o);
    void Atualizar(Orcamento o);
    void MarcarExcluido(long id);
    string GerarProximoNumero();
    IReadOnlyList<PedidoItemGridRow> ListarItensParaGrid(long orcamentoId);
    void SalvarItens(long orcamentoId, IReadOnlyList<ItemPedidoLinha> linhas);
}
