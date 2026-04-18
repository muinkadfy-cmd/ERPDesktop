namespace ERPDesktop.Domain.Entities;

public sealed class Compra
{
    public long Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public long? FornecedorId { get; set; }
    public DateTime DataEmissao { get; set; } = DateTime.Today;
    public string Situacao { get; set; } = "Pedido";
    public decimal ValorTotal { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public bool Excluido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
