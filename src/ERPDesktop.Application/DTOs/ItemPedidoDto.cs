namespace ERPDesktop.Application.DTOs;

/// <summary>Linha para persistir itens de orçamento ou compra.</summary>
public sealed class ItemPedidoLinha
{
    public long? ProdutoId { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public decimal Quantidade { get; init; }
    public decimal ValorUnitario { get; init; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
}

public sealed class PedidoItemGridRow
{
    public long Id { get; init; }
    public long? ProdutoId { get; init; }
    public int Sequencia { get; init; }
    public string CodigoProduto { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public decimal Quantidade { get; init; }
    public decimal ValorUnitario { get; init; }
    public decimal ValorTotal { get; init; }
}
