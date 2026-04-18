using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Abstractions;

public interface IProdutoRepository
{
    IReadOnlyList<Produto> Pesquisar(ProdutoFiltro filtro);
    Produto? ObterPorId(long id);
    long Inserir(Produto produto);
    void Atualizar(Produto produto);
    void MarcarExcluido(long id);
    string GerarProximoCodigo();
    void AlterarEstoqueDelta(long produtoId, decimal delta);
}

public sealed class ProdutoFiltro
{
    public string Ordenacao { get; set; } = "Descricao";
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public bool? SomenteAtivos { get; set; } = true;
}
