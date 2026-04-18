namespace ERPDesktop.Domain.Entities;

public sealed class Venda
{
    public long Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; } = DateTime.Today;
    public long? ClienteId { get; set; }
    public long? VendedorId { get; set; }
    public string Situacao { get; set; } = "Rascunho";
    public decimal ValorTotal { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public bool Excluido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
