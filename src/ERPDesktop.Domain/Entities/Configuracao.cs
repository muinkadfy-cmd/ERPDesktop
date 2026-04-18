namespace ERPDesktop.Domain.Entities;

public sealed class Configuracao
{
    public long Id { get; set; }
    public string Chave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public DateTime AtualizadoEm { get; set; }
}
