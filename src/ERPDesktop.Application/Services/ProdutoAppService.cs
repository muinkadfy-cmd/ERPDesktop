using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Services;

public sealed class ProdutoAppService
{
    private readonly IProdutoRepository _repo;

    public ProdutoAppService(IProdutoRepository repo)
    {
        _repo = repo;
    }

    public IReadOnlyList<ProdutoGridRow> PesquisarParaGrid(ProdutoFiltro filtro) =>
        _repo.Pesquisar(filtro).Select(Map).ToList();

    public Produto? Obter(long id) => _repo.ObterPorId(id);

    public Produto? ObterPorCodigoOuNull(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;
        var lista = _repo.Pesquisar(new ProdutoFiltro { Codigo = codigo.Trim(), SomenteAtivos = null });
        return lista.FirstOrDefault(p => p.Codigo.Equals(codigo.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public Produto CriarNovoVazio() => new()
    {
        Codigo = _repo.GerarProximoCodigo(),
        Unidade = "UN",
        Ativo = true
    };

    public ResultadoOperacao Salvar(Produto p)
    {
        if (string.IsNullOrWhiteSpace(p.Descricao))
            return ResultadoOperacao.Fail("Informe a descrição do produto.");

        if (p.PrecoAvista < 0 || p.PrecoAprazo < 0)
            return ResultadoOperacao.Fail("Preços inválidos.");

        if (p.EstoqueAtual < 0 || p.EstoqueMinimo < 0)
            return ResultadoOperacao.Fail("Estoque inválido.");

        var agora = DateTime.UtcNow;
        if (p.Id <= 0)
        {
            p.CriadoEm = agora;
            p.AtualizadoEm = agora;
            p.Id = _repo.Inserir(p);
        }
        else
        {
            p.AtualizadoEm = agora;
            _repo.Atualizar(p);
        }

        return ResultadoOperacao.Sucesso();
    }

    public ResultadoOperacao ExcluirSeguro(long id)
    {
        if (id <= 0)
            return ResultadoOperacao.Fail("Selecione um produto.");

        _repo.MarcarExcluido(id);
        return ResultadoOperacao.Sucesso();
    }

    private static ProdutoGridRow Map(Produto p) => new()
    {
        Id = p.Id,
        Codigo = p.Codigo,
        Descricao = p.Descricao,
        Unidade = p.Unidade,
        PrecoAvista = p.PrecoAvista,
        PrecoAprazo = p.PrecoAprazo,
        EstoqueAtual = p.EstoqueAtual,
        EstoqueMinimo = p.EstoqueMinimo,
        Categoria = p.Categoria,
        Marca = p.Marca,
        ReferenciaFabrica = p.ReferenciaFabrica,
        AtivoTexto = p.Ativo ? "Sim" : "Não"
    };
}
