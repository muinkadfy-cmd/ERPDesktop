namespace ERPDesktop.Domain.Entities;

public sealed class MovimentacaoFinanceira
{
    public long Id { get; set; }
    public DateTime DataMovimento { get; set; } = DateTime.Today;
    public string Tipo { get; set; } = string.Empty;
    public string Historico { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public long? FormaPagamentoId { get; set; }
    public long? VendaId { get; set; }
    public long? ClienteId { get; set; }
    public DateTime CriadoEm { get; set; }
}
