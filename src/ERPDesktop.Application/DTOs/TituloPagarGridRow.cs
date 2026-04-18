namespace ERPDesktop.Application.DTOs;

public sealed class TituloPagarGridRow
{
    public long Id { get; init; }
    public string FornecedorNome { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public DateTime Vencimento { get; init; }
    public decimal Valor { get; init; }
    public decimal ValorPago { get; init; }
    public string Situacao { get; init; } = string.Empty;
}
