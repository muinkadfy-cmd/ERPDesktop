namespace ERPDesktop.Application.Abstractions;

public interface IConfiguracaoRepository
{
    string? ObterValor(string chave);
    void DefinirValor(string chave, string valor);
}
