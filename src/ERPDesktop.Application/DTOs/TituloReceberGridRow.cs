namespace ERPDesktop.Application.DTOs;

public sealed class TituloReceberGridRow
{
    public long Id { get; init; }
    public string ClienteNome { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public DateTime Vencimento { get; init; }
    public decimal Valor { get; init; }
    public decimal ValorRecebido { get; init; }
    public string Situacao { get; init; } = string.Empty;
}
