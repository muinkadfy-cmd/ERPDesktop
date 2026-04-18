using ERPDesktop.Application.Abstractions;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Application.Services;

public sealed class VendaPdvAppService
{
    private readonly IVendaRepository _vendas;
    private readonly IProdutoRepository _produtos;
    private readonly IOrcamentoRepository _orcamentos;

    public VendaPdvAppService(IVendaRepository vendas, IProdutoRepository produtos, IOrcamentoRepository orcamentos)
    {
        _vendas = vendas;
        _produtos = produtos;
        _orcamentos = orcamentos;
    }

    public Produto? BuscarProdutoPorCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;
        var f = new ProdutoFiltro { Codigo = codigo.Trim(), SomenteAtivos = true };
        var lista = _produtos.Pesquisar(f);
        return lista.FirstOrDefault(p => p.Codigo.Equals(codigo.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public ResultadoOperacao FinalizarBalcao(long? clienteId, long? vendedorId, IReadOnlyList<PdvItemLinha> linhas)
    {
        if (linhas.Count == 0)
            return ResultadoOperacao.Fail("Inclua ao menos um item no cupom.");

        foreach (var l in linhas)
        {
            var p = _produtos.ObterPorId(l.ProdutoId);
            if (p is null)
                return ResultadoOperacao.Fail($"Produto inválido na linha: {l.Descricao}.");
            if (p.EstoqueAtual < l.Quantidade)
                return ResultadoOperacao.Fail($"Estoque insuficiente para {p.Descricao}. Disponível: {p.EstoqueAtual}.");
        }

        var agora = DateTime.UtcNow;
        var total = linhas.Sum(l => l.Quantidade * l.PrecoUnitario);
        var venda = new Venda
        {
            Numero = _vendas.GerarProximoNumero(),
            DataEmissao = DateTime.Today,
            ClienteId = clienteId,
            VendedorId = vendedorId,
            Situacao = "Finalizada",
            ValorTotal = total,
            Observacoes = "PDV Balcão",
            Excluido = false,
            CriadoEm = agora,
            AtualizadoEm = agora
        };

        var seq = 1;
        var itens = linhas.Select(l => new VendaItem
        {
            Sequencia = seq++,
            ProdutoId = l.ProdutoId,
            Descricao = l.Descricao,
            Quantidade = l.Quantidade,
            ValorUnitario = l.PrecoUnitario,
            ValorTotal = l.Quantidade * l.PrecoUnitario,
            CriadoEm = agora
        }).ToList();

        _vendas.InserirVendaComItens(venda, itens);
        return ResultadoOperacao.Sucesso();
    }

    /// <summary>Gera venda com baixa de estoque a partir dos itens gravados do orçamento e marca o orçamento como Faturado.</summary>
    public ResultadoOperacao FaturarOrcamento(long orcamentoId)
    {
        var o = _orcamentos.ObterPorId(orcamentoId);
        if (o is null)
            return ResultadoOperacao.Fail("Orçamento não encontrado.");
        if (string.Equals(o.Situacao, "Cancelado", StringComparison.OrdinalIgnoreCase))
            return ResultadoOperacao.Fail("Orçamento cancelado não pode ser faturado.");
        if (string.Equals(o.Situacao, "Faturado", StringComparison.OrdinalIgnoreCase))
            return ResultadoOperacao.Fail("Este orçamento já foi faturado.");

        var linhas = _orcamentos.ListarItensParaGrid(orcamentoId);
        if (linhas.Count == 0)
            return ResultadoOperacao.Fail("Grave os itens do orçamento antes de faturar.");

        foreach (var row in linhas)
        {
            if (!row.ProdutoId.HasValue || row.ProdutoId.Value <= 0)
                continue;
            var pid = row.ProdutoId.Value;
            var p = _produtos.ObterPorId(pid);
            if (p is null)
                return ResultadoOperacao.Fail($"Produto inválido na linha: {row.Descricao}.");
            if (p.EstoqueAtual < row.Quantidade)
                return ResultadoOperacao.Fail($"Estoque insuficiente para {p.Descricao}. Disponível: {p.EstoqueAtual}.");
        }

        var agora = DateTime.UtcNow;
        var total = linhas.Sum(r => r.ValorTotal);
        var venda = new Venda
        {
            Numero = _vendas.GerarProximoNumero(),
            DataEmissao = DateTime.Today,
            ClienteId = o.ClienteId,
            VendedorId = null,
            Situacao = "Finalizada",
            ValorTotal = total,
            Observacoes = $"Venda gerada do orçamento {o.Numero}.",
            Excluido = false,
            CriadoEm = agora,
            AtualizadoEm = agora
        };

        var seq = 1;
        var itens = linhas.Select(r => new VendaItem
        {
            Sequencia = seq++,
            ProdutoId = r.ProdutoId,
            Descricao = r.Descricao,
            Quantidade = r.Quantidade,
            ValorUnitario = r.ValorUnitario,
            ValorTotal = r.ValorTotal,
            CriadoEm = agora
        }).ToList();

        _vendas.InserirVendaComItens(venda, itens);
        o.Situacao = "Faturado";
        o.AtualizadoEm = agora;
        _orcamentos.Atualizar(o);
        return ResultadoOperacao.Sucesso();
    }
}

public sealed class PdvItemLinha
{
    public long ProdutoId { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public decimal Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
}
