namespace ERPDesktop.Domain.Entities;

public sealed class TituloReceber
{
    public long Id { get; set; }
    public long? ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; } = DateTime.Today;
    public DateTime DataVencimento { get; set; } = DateTime.Today;
    public decimal Valor { get; set; }
    public decimal ValorRecebido { get; set; }
    public string Situacao { get; set; } = "Aberto";
    public string Observacoes { get; set; } = string.Empty;
    public bool Excluido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
