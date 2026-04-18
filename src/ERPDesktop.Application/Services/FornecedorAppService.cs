using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Services;

public sealed class FornecedorAppService
{
    private readonly IFornecedorRepository _repo;

    public FornecedorAppService(IFornecedorRepository repo)
    {
        _repo = repo;
    }

    public IReadOnlyList<FornecedorGridRow> PesquisarGrid() => _repo.ListarParaGrid();

    public Fornecedor? Obter(long id) => _repo.ObterPorId(id);

    public Fornecedor CriarNovoVazio() => new()
    {
        Codigo = _repo.GerarProximoCodigo()
    };

    public ResultadoOperacao Salvar(Fornecedor f)
    {
        if (string.IsNullOrWhiteSpace(f.RazaoSocial))
            return ResultadoOperacao.Fail("Informe a razão social.");

        var agora = DateTime.UtcNow;
        if (f.Id <= 0)
        {
            f.CriadoEm = agora;
            f.AtualizadoEm = agora;
            f.Id = _repo.Inserir(f);
        }
        else
        {
            f.AtualizadoEm = agora;
            _repo.Atualizar(f);
        }

        return ResultadoOperacao.Sucesso();
    }

    public ResultadoOperacao Excluir(long id)
    {
        if (id <= 0)
            return ResultadoOperacao.Fail("Selecione um fornecedor.");
        _repo.MarcarExcluido(id);
        return ResultadoOperacao.Sucesso();
    }
}
