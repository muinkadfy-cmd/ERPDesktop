namespace ERPDesktop.Domain.Entities;

public sealed class Promocao
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal PercentualDesconto { get; set; }
    public DateTime DataInicio { get; set; } = DateTime.Today;
    public DateTime DataFim { get; set; } = DateTime.Today.AddMonths(1);
    public bool Ativo { get; set; } = true;
    public string Observacoes { get; set; } = string.Empty;
    public bool Excluido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
