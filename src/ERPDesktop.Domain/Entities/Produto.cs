namespace ERPDesktop.Domain.Entities;

public sealed class Produto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Unidade { get; set; } = "UN";
    public decimal PrecoAvista { get; set; }
    public decimal PrecoAprazo { get; set; }
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string ReferenciaFabrica { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public bool Excluido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
