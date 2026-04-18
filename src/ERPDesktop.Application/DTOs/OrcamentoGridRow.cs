namespace ERPDesktop.Application.DTOs;

public sealed class OrcamentoGridRow
{
    public long Id { get; init; }
    public string Numero { get; init; } = string.Empty;
    public DateTime DataEmissao { get; init; }
    public string ClienteNome { get; init; } = string.Empty;
    public decimal ValorTotal { get; init; }
    public string Situacao { get; init; } = string.Empty;
}
