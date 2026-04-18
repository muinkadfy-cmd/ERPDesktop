namespace ERPDesktop.Application.DTOs;

public sealed class PromocaoGridRow
{
    public long Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public decimal Percentual { get; init; }
    public DateTime Inicio { get; init; }
    public DateTime Fim { get; init; }
    public string AtivoTexto { get; init; } = string.Empty;
}
