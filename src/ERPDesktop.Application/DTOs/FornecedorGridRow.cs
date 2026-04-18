namespace ERPDesktop.Application.DTOs;

public sealed class FornecedorGridRow
{
    public long Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string RazaoSocial { get; init; } = string.Empty;
    public string NomeFantasia { get; init; } = string.Empty;
    public string Cidade { get; init; } = string.Empty;
    public string Uf { get; init; } = string.Empty;
    public string Telefone { get; init; } = string.Empty;
}
