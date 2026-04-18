namespace ERPDesktop.Application.DTOs;

public sealed class ProdutoGridRow
{
    public long Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Unidade { get; init; } = string.Empty;
    public decimal PrecoAvista { get; init; }
    public decimal PrecoAprazo { get; init; }
    public decimal EstoqueAtual { get; init; }
    public decimal EstoqueMinimo { get; init; }
    public string Categoria { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
    public string ReferenciaFabrica { get; init; } = string.Empty;
    public string AtivoTexto { get; init; } = string.Empty;
}
