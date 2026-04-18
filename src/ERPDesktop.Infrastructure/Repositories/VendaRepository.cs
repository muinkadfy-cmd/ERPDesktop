using System.Data;
using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class VendaRepository : IVendaRepository
{
    private readonly IDbConnectionFactory _factory;

    public VendaRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public string GerarProximoNumero()
    {
        using var conn = _factory.CreateOpenConnection();
        var next = conn.ExecuteScalar<long>("SELECT IFNULL(MAX(id), 0) + 1 FROM vendas;");
        return "V" + next.ToString("D6");
    }

    public long InserirVendaComItens(Venda venda, IReadOnlyList<VendaItem> itens)
    {
        using var conn = _factory.CreateOpenConnection();
        using var tx = conn.BeginTransaction();

        var agora = DateTime.UtcNow.ToString("O");
        var dataEmissao = venda.DataEmissao.ToString("yyyy-MM-dd");

        conn.Execute(
            """
            INSERT INTO vendas(
              numero, data_emissao, cliente_id, vendedor_id, situacao, valor_total, observacoes,
              excluido, criado_em, atualizado_em
            ) VALUES (
              @Numero, @DataEmissao, @ClienteId, @VendedorId, @Situacao, @ValorTotal, @Observacoes,
              0, @CriadoEm, @AtualizadoEm
            );
            """,
            new
            {
                venda.Numero,
                DataEmissao = dataEmissao,
                venda.ClienteId,
                venda.VendedorId,
                venda.Situacao,
                venda.ValorTotal,
                venda.Observacoes,
                CriadoEm = agora,
                AtualizadoEm = agora
            },
            tx);

        var vendaId = conn.ExecuteScalar<long>("SELECT last_insert_rowid();", transaction: tx);

        var seq = 1;
        foreach (var it in itens)
        {
            conn.Execute(
                """
                INSERT INTO venda_itens(
                  venda_id, sequencia, descricao, quantidade, valor_unitario, valor_total, criado_em, produto_id
                ) VALUES (
                  @VendaId, @Sequencia, @Descricao, @Quantidade, @ValorUnitario, @ValorTotal, @CriadoEm, @ProdutoId
                );
                """,
                new
                {
                    VendaId = vendaId,
                    Sequencia = seq++,
                    it.Descricao,
                    it.Quantidade,
                    it.ValorUnitario,
                    it.ValorTotal,
                    CriadoEm = agora,
                    ProdutoId = it.ProdutoId
                },
                tx);

            if (it.ProdutoId is { } pid && pid > 0)
            {
                conn.Execute(
                    """
                    UPDATE produtos SET
                      estoque_atual = estoque_atual - @q,
                      atualizado_em = @a
                    WHERE id = @id AND excluido = 0;
                    """,
                    new { id = pid, q = it.Quantidade, a = agora },
                    tx);
            }
        }

        tx.Commit();
        return vendaId;
    }
}
