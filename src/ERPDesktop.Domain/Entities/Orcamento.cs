namespace ERPDesktop.Domain.Entities;

public sealed class Orcamento
{
    public long Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public long? ClienteId { get; set; }
    public DateTime DataEmissao { get; set; } = DateTime.Today;
    public string Situacao { get; set; } = "Aberto";
    public decimal ValorTotal { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public bool Excluido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
