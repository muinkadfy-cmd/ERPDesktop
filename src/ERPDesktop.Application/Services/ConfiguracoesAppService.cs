using ERPDesktop.Application.Abstractions;

namespace ERPDesktop.Application.Services;

public sealed class ConfiguracoesAppService
{
    private readonly IConfiguracaoRepository _cfg;

    public ConfiguracoesAppService(IConfiguracaoRepository cfg)
    {
        _cfg = cfg;
    }

    public string Obter(string chave, string padrao = "") =>
        _cfg.ObterValor(chave) ?? padrao;

    public void Salvar(string chave, string valor) =>
        _cfg.DefinirValor(chave, valor);
}
