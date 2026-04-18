using System.Data;
using Dapper;
using ERPDesktop.Application.Abstractions;

namespace ERPDesktop.Infrastructure.Data;

public sealed class DataSeeder : IDataSeeder
{
    private readonly IDbConnectionFactory _factory;

    public DataSeeder(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public void ExecutarSeNecessario()
    {
        using var conn = _factory.CreateOpenConnection();

        var vendedores = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM vendedores;");
        if (vendedores == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO vendedores(codigo, nome, ativo, criado_em, atualizado_em) VALUES
                  ('001', 'Vendedor Padrão', 1, @a, @a),
                  ('002', 'Maria Souza', 1, @a, @a),
                  ('003', 'João Lima', 1, @a, @a);
                """,
                new { a = agora });
        }

        var fp = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM formas_pagamento;");
        if (fp == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO formas_pagamento(codigo, descricao, ativo, criado_em) VALUES
                  ('DIN', 'Dinheiro', 1, @a),
                  ('PIX', 'PIX', 1, @a),
                  ('CCR', 'Cartão Crédito', 1, @a),
                  ('CDB', 'Cartão Débito', 1, @a),
                  ('BLT', 'Boleto', 1, @a);
                """,
                new { a = agora });
        }

        var cfg = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM configuracoes;");
        if (cfg == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO configuracoes(chave, valor, atualizado_em) VALUES
                  ('empresa.razao_social', 'ERP Desktop Demonstração', @a),
                  ('empresa.cidade', 'Sua Cidade', @a);
                """,
                new { a = agora });
        }

        var cli = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM clientes WHERE excluido = 0;");
        if (cli == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            var vid = conn.ExecuteScalar<long?>("SELECT id FROM vendedores ORDER BY id LIMIT 1;");
            conn.Execute(
                """
                INSERT INTO clientes(
                  codigo, data_cadastro, status, nome_razao_social, nome_fantasia, tipo_cadastro, origem_marketing, vendedor_id,
                  endereco, numero, complemento, bairro, cidade, uf, cep,
                  telefone1, telefone2, celular, whatsapp, whatsapp_abordagem, email, contato, rede_social,
                  cpf, rg, orgao_emissor, cnpj, ie, im,
                  observacoes, observacoes_financeiras, historico_texto, status_financeiro,
                  limite_credito, bloqueado, restrito, foto_path, excluido, criado_em, atualizado_em
                ) VALUES
                (
                  '000001', @hoje, 0, 'Cliente Exemplo LTDA', 'Loja Exemplo', 'Pessoa Jurídica', 'Indicação', @vid,
                  'Rua das Flores', '1200', 'Sala 01', 'Centro', 'Porto Alegre', 'RS', '90000000',
                  '(51) 3000-0000', '', '(51) 98888-7777', '(51) 98888-7777', 'Comercial', 'contato@exemplo.com', 'Fulano', '@instagram',
                  '', '', '', '12.345.678/0001-90', 'ISENTO', '',
                  'Cliente criado automaticamente para demonstração.', '', '', 'Em dia',
                  5000, 0, 0, '', 0, @a, @a
                ),
                (
                  '000002', @hoje, 0, 'Maria Aparecida da Silva', 'Maria Calçados', 'Pessoa Física', 'Instagram', @vid,
                  'Av. Brasil', '45', '', 'Moinhos', 'Canoas', 'RS', '92000000',
                  '', '', '(51) 97777-6655', '(51) 97777-6655', 'Pós-venda', 'maria@email.com', 'Maria', '',
                  '000.111.222-33', '1234567890', 'SSP/RS', '', '', '',
                  '', 'Sem restrições.', '', 'Em dia',
                  1500, 0, 0, '', 0, @a, @a
                );
                """,
                new { a = agora, hoje = DateTime.Today.ToString("yyyy-MM-dd"), vid });
        }

        var pr = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM produtos WHERE excluido = 0;");
        if (pr == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO produtos(
                  codigo, descricao, unidade, preco_avista, preco_aprazo, estoque_atual, estoque_minimo,
                  categoria, marca, referencia_fabrica, ativo, excluido, criado_em, atualizado_em
                ) VALUES
                  ('P00001', 'Tênis Esportivo Modelo A', 'UN', 199.90, 219.90, 24, 5, 'Calçados', 'Marca Demo', 'REF-A1', 1, 0, @a, @a),
                  ('P00002', 'Sandália Comfort Line', 'UN', 89.90, 99.90, 40, 8, 'Calçados', 'Marca Demo', 'REF-S2', 1, 0, @a, @a),
                  ('P00003', 'Bota Couro Premium', 'UN', 349.00, 389.00, 6, 2, 'Calçados', 'Couro RS', 'REF-B3', 1, 0, @a, @a),
                  ('P00004', 'Meia Algodão (par)', 'UN', 12.90, 14.90, 200, 50, 'Acessórios', 'Têxtil', 'REF-M4', 1, 0, @a, @a);
                """,
                new { a = agora });
        }

        var forn = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM fornecedores WHERE excluido = 0;");
        if (forn == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO fornecedores(
                  codigo, razao_social, nome_fantasia, cnpj, telefone, cidade, uf, email, observacoes,
                  excluido, criado_em, atualizado_em
                ) VALUES
                  ('F00001', 'Distribuidora Sul de Calçados LTDA', 'Sul Distrib.', '11.222.333/0001-44', '(51) 3333-4444', 'Porto Alegre', 'RS', 'compras@suldist.com.br', '', 0, @a, @a),
                  ('F00002', 'Importadora Nordeste ME', 'Nordeste Shoes', '98.765.432/0001-10', '(81) 4000-9000', 'Recife', 'PE', 'vendas@nordeste.com', '', 0, @a, @a);
                """,
                new { a = agora });
        }

        var tr = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM titulos_receber WHERE excluido = 0;");
        if (tr == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            var cid = conn.ExecuteScalar<long?>("SELECT id FROM clientes WHERE excluido = 0 ORDER BY id LIMIT 1;");
            if (cid is null)
                goto PulaTitulosReceber;
            conn.Execute(
                """
                INSERT INTO titulos_receber(
                  cliente_id, descricao, data_emissao, data_vencimento, valor, valor_recebido, situacao, observacoes,
                  excluido, criado_em, atualizado_em
                ) VALUES
                  (@cid, 'Duplicata venda balcão', @hoje, @v1, 450.00, 0, 'Aberto', '', 0, @a, @a),
                  (@cid, 'Serviços manutenção', @hoje, @v2, 120.00, 50.00, 'Aberto', '', 0, @a, @a);
                """,
                new
                {
                    cid,
                    hoje = DateTime.Today.ToString("yyyy-MM-dd"),
                    v1 = DateTime.Today.AddDays(10).ToString("yyyy-MM-dd"),
                    v2 = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd"),
                    a = agora
                });
        PulaTitulosReceber:
            ;
        }

        var tp = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM titulos_pagar WHERE excluido = 0;");
        if (tp == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            var fid = conn.ExecuteScalar<long?>("SELECT id FROM fornecedores WHERE excluido = 0 ORDER BY id LIMIT 1;");
            if (fid is null)
                goto PulaTitulosPagar;
            conn.Execute(
                """
                INSERT INTO titulos_pagar(
                  fornecedor_id, descricao, data_emissao, data_vencimento, valor, valor_pago, situacao, observacoes,
                  excluido, criado_em, atualizado_em
                ) VALUES
                  (@fid, 'NF compra calçados', @hoje, @v1, 2800.00, 0, 'Aberto', '', 0, @a, @a),
                  (@fid, 'Frete importação', @hoje, @v2, 350.00, 350.00, 'Quitado', '', 0, @a, @a);
                """,
                new
                {
                    fid,
                    hoje = DateTime.Today.ToString("yyyy-MM-dd"),
                    v1 = DateTime.Today.AddDays(15).ToString("yyyy-MM-dd"),
                    v2 = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"),
                    a = agora
                });
        PulaTitulosPagar:
            ;
        }

        var orc = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM orcamentos WHERE excluido = 0;");
        if (orc == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            var cid = conn.ExecuteScalar<long?>("SELECT id FROM clientes WHERE excluido = 0 ORDER BY id LIMIT 1;");
            if (cid is null)
                goto PulaOrc;
            conn.Execute(
                """
                INSERT INTO orcamentos(numero, cliente_id, data_emissao, situacao, valor_total, observacoes, excluido, criado_em, atualizado_em)
                VALUES
                  ('ORC00001', @cid, @hoje, 'Aberto', 1299.90, 'Kit escolar — aguardando aprovação.', 0, @a, @a),
                  ('ORC00002', @cid, @hoje, 'Aprovado', 540.00, 'Reposição loja.', 0, @a, @a);
                """,
                new { cid, hoje = DateTime.Today.ToString("yyyy-MM-dd"), a = agora });
        PulaOrc:
            ;
        }

        var prm = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM promocoes WHERE excluido = 0;");
        if (prm == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            conn.Execute(
                """
                INSERT INTO promocoes(nome, percentual_desconto, data_inicio, data_fim, ativo, observacoes, excluido, criado_em, atualizado_em)
                VALUES
                  ('Liquidação de inverno', 15, @i1, @f1, 1, 'Válido para calçados da linha comfort.', 0, @a, @a),
                  ('PIX 5%', 5, @i1, @f1, 1, 'Desconto adicional no PIX.', 0, @a, @a);
                """,
                new
                {
                    i1 = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"),
                    f1 = DateTime.Today.AddDays(30).ToString("yyyy-MM-dd"),
                    a = agora
                });
        }

        var cmp = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM compras WHERE excluido = 0;");
        if (cmp == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            var fid = conn.ExecuteScalar<long?>("SELECT id FROM fornecedores WHERE excluido = 0 ORDER BY id LIMIT 1;");
            if (fid is null)
                goto PulaComp;
            conn.Execute(
                """
                INSERT INTO compras(numero, fornecedor_id, data_emissao, situacao, valor_total, observacoes, excluido, criado_em, atualizado_em)
                VALUES
                  ('C00001', @fid, @hoje, 'Pedido', 4200.00, 'Pedido de coleção nova.', 0, @a, @a);
                """,
                new { fid, hoje = DateTime.Today.ToString("yyyy-MM-dd"), a = agora });
        PulaComp:
            ;
        }

        var vn = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM vendas WHERE excluido = 0;");
        if (vn == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            var cid = conn.ExecuteScalar<long?>("SELECT id FROM clientes WHERE excluido = 0 ORDER BY id LIMIT 1;");
            var vid = conn.ExecuteScalar<long?>("SELECT id FROM vendedores ORDER BY id LIMIT 1;");
            var p1 = conn.ExecuteScalar<long?>("SELECT id FROM produtos WHERE excluido = 0 ORDER BY id LIMIT 1;");
            var p2 = conn.ExecuteScalar<long?>("SELECT id FROM produtos WHERE excluido = 0 ORDER BY id DESC LIMIT 1;");
            conn.Execute(
                """
                INSERT INTO vendas(numero, data_emissao, cliente_id, vendedor_id, situacao, valor_total, observacoes, excluido, criado_em, atualizado_em)
                VALUES
                  ('V000001', @hoje, @cid, @vid, 'Finalizada', 389.80, 'Venda demonstração', 0, @a, @a),
                  ('V000002', @ontem, @cid, @vid, 'Finalizada', 199.90, 'Venda demonstração', 0, @a, @a);
                """,
                new
                {
                    cid,
                    vid,
                    hoje = DateTime.Today.ToString("yyyy-MM-dd"),
                    ontem = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
                    a = agora
                });

            var v1 = conn.ExecuteScalar<long>("SELECT id FROM vendas WHERE numero = 'V000001' LIMIT 1;");
            var v2 = conn.ExecuteScalar<long>("SELECT id FROM vendas WHERE numero = 'V000002' LIMIT 1;");
            conn.Execute(
                """
                INSERT INTO venda_itens(venda_id, sequencia, descricao, quantidade, valor_unitario, valor_total, criado_em, produto_id)
                VALUES
                  (@v1, 1, 'Item PDV', 2, 99.95, 199.90, @a, @p1),
                  (@v1, 2, 'Item PDV', 1, 189.90, 189.90, @a, @p2),
                  (@v2, 1, 'Item PDV', 1, 199.90, 199.90, @a, @p1);
                """,
                new { v1, v2, p1, p2, a = agora });

            if (p1 is not null)
            {
                conn.Execute(
                    "UPDATE produtos SET estoque_atual = estoque_atual - 3, atualizado_em = @a WHERE id = @p1;",
                    new { p1, a = agora });
            }

            if (p2 is not null)
            {
                conn.Execute(
                    "UPDATE produtos SET estoque_atual = estoque_atual - 1, atualizado_em = @a WHERE id = @p2;",
                    new { p2, a = agora });
            }
        }

        var mf = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM movimentacoes_financeiras;");
        if (mf == 0)
        {
            var agora = DateTime.UtcNow.ToString("O");
            var fpId = conn.ExecuteScalar<long?>("SELECT id FROM formas_pagamento ORDER BY id LIMIT 1;");
            conn.Execute(
                """
                INSERT INTO movimentacoes_financeiras(data_movimento, tipo, historico, valor, forma_pagamento_id, venda_id, cliente_id, criado_em)
                VALUES
                  (@hoje, 'Entrada', 'Abertura de caixa (demonstração)', 500.00, @fpId, NULL, NULL, @a),
                  (@hoje, 'Entrada', 'Venda balcão — demonstração', 199.90, @fpId, NULL, NULL, @a);
                """,
                new { hoje = DateTime.Today.ToString("yyyy-MM-dd"), fpId, a = agora });
        }
    }
}
