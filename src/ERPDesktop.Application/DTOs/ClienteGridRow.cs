namespace ERPDesktop.Application.DTOs;

public sealed class ClienteGridRow
{
    public long Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string NomeRazaoSocial { get; init; } = string.Empty;
    public string NomeFantasia { get; init; } = string.Empty;
    public string TipoCadastro { get; init; } = string.Empty;
    public string OrigemMarketing { get; init; } = string.Empty;
    public string Whatsapp { get; init; } = string.Empty;
    public string WhatsappAbordagem { get; init; } = string.Empty;
    public string Telefone1 { get; init; } = string.Empty;
    public string Celular { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string VendedorNome { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string StatusTexto { get; init; } = string.Empty;
}
