using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface IClienteRepository
{
    IReadOnlyList<Cliente> Pesquisar(ClienteFiltro filtro);
    Cliente? ObterPorId(long id);
    long Inserir(Cliente cliente);
    void Atualizar(Cliente cliente);
    void MarcarExcluido(long id);
    string GerarProximoCodigo();
}

public sealed class ClienteFiltro
{
    public string Ordenacao { get; set; } = "Nome";
    public string Nome { get; set; } = string.Empty;
    public string Fantasia { get; set; } = string.Empty;
    public string RastrearNome { get; set; } = string.Empty;
    public string RastrearEndereco { get; set; } = string.Empty;
    public string RastrearTelefone { get; set; } = string.Empty;
    public string RastrearCpf { get; set; } = string.Empty;
    public string RastrearCnpj { get; set; } = string.Empty;
    public string TipoCadastro { get; set; } = string.Empty;
    public string OrigemMarketing { get; set; } = string.Empty;
}
