namespace ERPDesktop.Application.DTOs;

public sealed class CompraGridRow
{
    public long Id { get; init; }
    public string Numero { get; init; } = string.Empty;
    public DateTime DataEmissao { get; init; }
    public string FornecedorNome { get; init; } = string.Empty;
    public decimal ValorTotal { get; init; }
    public string Situacao { get; init; } = string.Empty;
}
