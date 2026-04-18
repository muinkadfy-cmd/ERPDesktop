using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface ICompraRepository
{
    IReadOnlyList<CompraGridRow> ListarParaGrid();
    IReadOnlyList<Compra> Listar();
    Compra? ObterPorId(long id);
    long Inserir(Compra c);
    void Atualizar(Compra c);
    void MarcarExcluido(long id);
    string GerarProximoNumero();
    IReadOnlyList<PedidoItemGridRow> ListarItensParaGrid(long compraId);
    void SalvarItens(long compraId, IReadOnlyList<ItemPedidoLinha> linhas);

    /// <summary>Incrementa estoque pelos itens gravados e define situação Recebido. Retorna null se OK.</summary>
    string? DarEntradaEstoque(long compraId);
}
