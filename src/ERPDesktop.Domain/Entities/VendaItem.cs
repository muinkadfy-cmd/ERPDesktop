namespace ERPDesktop.Domain.Entities;

public sealed class VendaItem
{
    public long Id { get; set; }
    public long VendaId { get; set; }
    public long? ProdutoId { get; set; }
    public int Sequencia { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime CriadoEm { get; set; }
}
