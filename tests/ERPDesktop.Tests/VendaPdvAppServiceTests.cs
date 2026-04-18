using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.Domain.Entities;
using Moq;
using Xunit;

namespace ERPDesktop.Tests;

public sealed class VendaPdvAppServiceTests
{
    [Fact]
    public void FinalizarBalcao_sem_itens_retorna_falha()
    {
        var svc = new VendaPdvAppService(
            Mock.Of<IVendaRepository>(),
            Mock.Of<IProdutoRepository>(),
            Mock.Of<IOrcamentoRepository>());

        var r = svc.FinalizarBalcao(null, null, Array.Empty<PdvItemLinha>());

        Assert.False(r.Ok);
        Assert.Contains("cupom", r.Mensagem, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FinalizarBalcao_estoque_insuficiente_retorna_falha()
    {
        var produtos = new Mock<IProdutoRepository>();
        produtos.Setup(p => p.ObterPorId(1))
            .Returns(new Produto
            {
                Id = 1,
                Codigo = "P1",
                Descricao = "Teste",
                EstoqueAtual = 1,
                Ativo = true
            });

        var svc = new VendaPdvAppService(
            Mock.Of<IVendaRepository>(),
            produtos.Object,
            Mock.Of<IOrcamentoRepository>());

        var linhas = new[]
        {
            new PdvItemLinha { ProdutoId = 1, Descricao = "Teste", Quantidade = 5, PrecoUnitario = 10 }
        };

        var r = svc.FinalizarBalcao(null, null, linhas);

        Assert.False(r.Ok);
        Assert.Contains("Estoque insuficiente", r.Mensagem, StringComparison.Ordinal);
    }

    [Fact]
    public void FinalizarBalcao_ok_chama_inserir_venda()
    {
        var vendas = new Mock<IVendaRepository>();
        vendas.Setup(v => v.GerarProximoNumero()).Returns("V000001");

        var produtos = new Mock<IProdutoRepository>();
        produtos.Setup(p => p.ObterPorId(1))
            .Returns(new Produto
            {
                Id = 1,
                Codigo = "P1",
                Descricao = "Teste",
                EstoqueAtual = 100,
                Ativo = true
            });

        var svc = new VendaPdvAppService(
            vendas.Object,
            produtos.Object,
            Mock.Of<IOrcamentoRepository>());

        var linhas = new[]
        {
            new PdvItemLinha { ProdutoId = 1, Descricao = "Teste", Quantidade = 2, PrecoUnitario = 10 }
        };

        var r = svc.FinalizarBalcao(5, 7, linhas);

        Assert.True(r.Ok);
        vendas.Verify(
            v => v.InserirVendaComItens(
                It.Is<Venda>(x => x.ClienteId == 5 && x.VendedorId == 7 && x.ValorTotal == 20),
                It.Is<IReadOnlyList<VendaItem>>(it => it.Count == 1 && it[0].Quantidade == 2)),
            Times.Once);
    }

    [Fact]
    public void FaturarOrcamento_nao_encontrado_retorna_falha()
    {
        var orc = new Mock<IOrcamentoRepository>();
        orc.Setup(o => o.ObterPorId(99)).Returns((Orcamento?)null);

        var svc = new VendaPdvAppService(
            Mock.Of<IVendaRepository>(),
            Mock.Of<IProdutoRepository>(),
            orc.Object);

        var r = svc.FaturarOrcamento(99);

        Assert.False(r.Ok);
        Assert.Contains("não encontrado", r.Mensagem, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FaturarOrcamento_cancelado_retorna_falha()
    {
        var orc = new Mock<IOrcamentoRepository>();
        orc.Setup(o => o.ObterPorId(1))
            .Returns(new Orcamento { Id = 1, Numero = "O1", Situacao = "Cancelado" });

        var svc = new VendaPdvAppService(
            Mock.Of<IVendaRepository>(),
            Mock.Of<IProdutoRepository>(),
            orc.Object);

        var r = svc.FaturarOrcamento(1);

        Assert.False(r.Ok);
        Assert.Contains("cancelado", r.Mensagem, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FaturarOrcamento_sem_itens_retorna_falha()
    {
        var orc = new Mock<IOrcamentoRepository>();
        orc.Setup(o => o.ObterPorId(1))
            .Returns(new Orcamento { Id = 1, Numero = "O1", Situacao = "Aberto" });
        orc.Setup(o => o.ListarItensParaGrid(1)).Returns(Array.Empty<PedidoItemGridRow>());

        var svc = new VendaPdvAppService(
            Mock.Of<IVendaRepository>(),
            Mock.Of<IProdutoRepository>(),
            orc.Object);

        var r = svc.FaturarOrcamento(1);

        Assert.False(r.Ok);
        Assert.Contains("Grave os itens", r.Mensagem, StringComparison.Ordinal);
    }

    [Fact]
    public void FaturarOrcamento_ok_atualiza_situacao_e_insere_venda()
    {
        var agora = DateTime.UtcNow;
        var orcamento = new Orcamento
        {
            Id = 10,
            Numero = "O00010",
            ClienteId = 3,
            Situacao = "Aberto",
            CriadoEm = agora,
            AtualizadoEm = agora
        };

        var orc = new Mock<IOrcamentoRepository>();
        orc.Setup(o => o.ObterPorId(10)).Returns(orcamento);
        orc.Setup(o => o.ListarItensParaGrid(10))
            .Returns(new List<PedidoItemGridRow>
            {
                new()
                {
                    Id = 1,
                    ProdutoId = 100,
                    Sequencia = 1,
                    Descricao = "Item",
                    Quantidade = 1,
                    ValorUnitario = 50,
                    ValorTotal = 50
                }
            });

        var produtos = new Mock<IProdutoRepository>();
        produtos.Setup(p => p.ObterPorId(100))
            .Returns(new Produto
            {
                Id = 100,
                Codigo = "X",
                Descricao = "Item",
                EstoqueAtual = 10,
                Ativo = true
            });

        var vendas = new Mock<IVendaRepository>();
        vendas.Setup(v => v.GerarProximoNumero()).Returns("V000099");

        var svc = new VendaPdvAppService(vendas.Object, produtos.Object, orc.Object);

        var r = svc.FaturarOrcamento(10);

        Assert.True(r.Ok);
        vendas.Verify(
            v => v.InserirVendaComItens(
                It.Is<Venda>(x => x.ClienteId == 3 && x.ValorTotal == 50),
                It.Is<IReadOnlyList<VendaItem>>(it => it.Count == 1 && it[0].ProdutoId == 100)),
            Times.Once);
        orc.Verify(o => o.Atualizar(It.Is<Orcamento>(x => x.Situacao == "Faturado")), Times.Once);
    }
}
