namespace ERPDesktop.Application.DTOs;

public sealed class MovimentacaoFinanceiraGridRow
{
    public long Id { get; init; }
    public DateTime DataMovimento { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string Historico { get; init; } = string.Empty;
    public decimal Valor { get; init; }
}
