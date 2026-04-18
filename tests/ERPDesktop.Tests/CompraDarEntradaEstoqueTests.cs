using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Infrastructure.Data;
using ERPDesktop.Infrastructure.Repositories;
using Xunit;

namespace ERPDesktop.Tests;

/// <summary>Integração SQLite em ficheiro temporário: migração + entrada de estoque na compra.</summary>
public sealed class CompraDarEntradaEstoqueTests : IDisposable
{
    private readonly string _dbPath;

    public CompraDarEntradaEstoqueTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), "erp_tests_" + Guid.NewGuid().ToString("N") + ".db");
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_dbPath))
                File.Delete(_dbPath);
        }
        catch
        {
            // ignorar bloqueio eventual no Windows
        }
    }

    [Fact]
    public void DarEntradaEstoque_incrementa_produto_e_marca_recebido()
    {
        IDbConnectionFactory factory = new SqliteConnectionFactory(_dbPath);
        new DatabaseMigrator(factory).AplicarPendentes();

        using (var conn = factory.CreateOpenConnection())
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO fornecedores(codigo, razao_social, nome_fantasia, cnpj, telefone, cidade, uf, email, observacoes, excluido, criado_em, atualizado_em)
                VALUES('F1', 'Forn', '', '', '', '', '', '', '', 0, @a, @a);
                """,
                new { a = agora });
            conn.Execute(
                """
                INSERT INTO produtos(codigo, descricao, unidade, preco_avista, preco_aprazo, estoque_atual, estoque_minimo, categoria, marca, referencia_fabrica, ativo, excluido, criado_em, atualizado_em)
                VALUES('SKU1', 'Prod', 'UN', 10, 10, 2, 0, '', '', '', 1, 0, @a, @a);
                """,
                new { a = agora });
            conn.Execute(
                """
                INSERT INTO compras(numero, fornecedor_id, data_emissao, situacao, valor_total, observacoes, excluido, criado_em, atualizado_em)
                VALUES('C00001', 1, date('now'), 'Pedido', 0, '', 0, @a, @a);
                """,
                new { a = agora });
            conn.Execute(
                """
                INSERT INTO compra_itens(compra_id, produto_id, sequencia, descricao, quantidade, valor_unitario, valor_total, criado_em)
                VALUES(1, 1, 1, 'Prod', 4, 5, 20, @a);
                """,
                new { a = agora });
        }

        var repo = new CompraRepository(factory);
        var err = repo.DarEntradaEstoque(1);

        Assert.Null(err);

        using var conn2 = factory.CreateOpenConnection();
        var estoque = conn2.ExecuteScalar<decimal>("SELECT estoque_atual FROM produtos WHERE id = 1;");
        Assert.Equal(6, estoque);

        var sit = conn2.ExecuteScalar<string>("SELECT situacao FROM compras WHERE id = 1;");
        Assert.Equal("Recebido", sit);
    }

    [Fact]
    public void DarEntradaEstoque_segunda_vez_retorna_erro()
    {
        IDbConnectionFactory factory = new SqliteConnectionFactory(_dbPath);
        new DatabaseMigrator(factory).AplicarPendentes();

        using (var conn = factory.CreateOpenConnection())
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO fornecedores(codigo, razao_social, nome_fantasia, cnpj, telefone, cidade, uf, email, observacoes, excluido, criado_em, atualizado_em)
                VALUES('F1', 'Forn', '', '', '', '', '', '', '', 0, @a, @a);
                """,
                new { a = agora });
            conn.Execute(
                """
                INSERT INTO produtos(codigo, descricao, unidade, preco_avista, preco_aprazo, estoque_atual, estoque_minimo, categoria, marca, referencia_fabrica, ativo, excluido, criado_em, atualizado_em)
                VALUES('SKU1', 'Prod', 'UN', 10, 10, 10, 0, '', '', '', 1, 0, @a, @a);
                """,
                new { a = agora });
            conn.Execute(
                """
                INSERT INTO compras(numero, fornecedor_id, data_emissao, situacao, valor_total, observacoes, excluido, criado_em, atualizado_em)
                VALUES('C00001', 1, date('now'), 'Pedido', 20, '', 0, @a, @a);
                """,
                new { a = agora });
            conn.Execute(
                """
                INSERT INTO compra_itens(compra_id, produto_id, sequencia, descricao, quantidade, valor_unitario, valor_total, criado_em)
                VALUES(1, 1, 1, 'Prod', 1, 1, 1, @a);
                """,
                new { a = agora });
        }

        var repo = new CompraRepository(factory);
        Assert.Null(repo.DarEntradaEstoque(1));
        var err = repo.DarEntradaEstoque(1);
        Assert.NotNull(err);
        Assert.Contains("já teve entrada", err, StringComparison.OrdinalIgnoreCase);
    }
}
