using System.Data;
using System;
using Dapper;
using ERPDesktop.Application.Abstractions;

namespace ERPDesktop.Infrastructure.Data;

public sealed class DatabaseMigrator : IDatabaseMigrator
{
    private readonly IDbConnectionFactory _factory;

    public DatabaseMigrator(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public void AplicarPendentes()
    {
        using var conn = _factory.CreateOpenConnection();

        conn.Execute(
            """
            CREATE TABLE IF NOT EXISTS schema_migrations (
              version INTEGER PRIMARY KEY,
              name TEXT NOT NULL,
              applied_at TEXT NOT NULL
            );
            """);

        Aplicar(conn, 1, "inicial", SqlInicial.V1);
        Aplicar(conn, 2, "produtos", SqlInicial.V2);
        Aplicar(conn, 3, "modulos_erp", SqlInicial.V3, AplicarPosV3);
        Aplicar(conn, 4, "itens_orcamento_compra", SqlInicial.V4);
    }

    private static void AplicarPosV3(IDbConnection conn, IDbTransaction tx)
    {
        var temColuna = conn.ExecuteScalar<int>(
            "SELECT COUNT(1) FROM pragma_table_info('venda_itens') WHERE name='produto_id';",
            transaction: tx) > 0;
        if (!temColuna)
        {
            conn.Execute("ALTER TABLE venda_itens ADD COLUMN produto_id INTEGER NULL REFERENCES produtos(id);", transaction: tx);
        }
    }

    private static void Aplicar(IDbConnection conn, int version, string name, string sql, Action<IDbConnection, IDbTransaction>? pos = null)
    {
        var existe = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM schema_migrations WHERE version = @v;", new { v = version }) > 0;
        if (existe)
            return;

        using var tx = conn.BeginTransaction();
        conn.Execute(sql, transaction: tx);
        pos?.Invoke(conn, tx);
        conn.Execute(
            "INSERT INTO schema_migrations(version, name, applied_at) VALUES(@version, @name, @applied);",
            new { version, name, applied = DateTime.UtcNow.ToString("O") },
            tx);
        tx.Commit();
    }
}

internal static class SqlInicial
{
    public const string V1 =
        """
        CREATE TABLE IF NOT EXISTS vendedores (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          codigo TEXT NOT NULL UNIQUE,
          nome TEXT NOT NULL,
          ativo INTEGER NOT NULL DEFAULT 1,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS formas_pagamento (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          codigo TEXT NOT NULL UNIQUE,
          descricao TEXT NOT NULL,
          ativo INTEGER NOT NULL DEFAULT 1,
          criado_em TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS configuracoes (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          chave TEXT NOT NULL UNIQUE,
          valor TEXT NOT NULL,
          atualizado_em TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS clientes (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          codigo TEXT NOT NULL UNIQUE,
          data_cadastro TEXT NOT NULL,
          status INTEGER NOT NULL DEFAULT 0,
          nome_razao_social TEXT NOT NULL DEFAULT '',
          nome_fantasia TEXT NOT NULL DEFAULT '',
          tipo_cadastro TEXT NOT NULL DEFAULT '',
          origem_marketing TEXT NOT NULL DEFAULT '',
          vendedor_id INTEGER NULL,
          endereco TEXT NOT NULL DEFAULT '',
          numero TEXT NOT NULL DEFAULT '',
          complemento TEXT NOT NULL DEFAULT '',
          bairro TEXT NOT NULL DEFAULT '',
          cidade TEXT NOT NULL DEFAULT '',
          uf TEXT NOT NULL DEFAULT '',
          cep TEXT NOT NULL DEFAULT '',
          telefone1 TEXT NOT NULL DEFAULT '',
          telefone2 TEXT NOT NULL DEFAULT '',
          celular TEXT NOT NULL DEFAULT '',
          whatsapp TEXT NOT NULL DEFAULT '',
          whatsapp_abordagem TEXT NOT NULL DEFAULT '',
          email TEXT NOT NULL DEFAULT '',
          contato TEXT NOT NULL DEFAULT '',
          rede_social TEXT NOT NULL DEFAULT '',
          cpf TEXT NOT NULL DEFAULT '',
          rg TEXT NOT NULL DEFAULT '',
          orgao_emissor TEXT NOT NULL DEFAULT '',
          cnpj TEXT NOT NULL DEFAULT '',
          ie TEXT NOT NULL DEFAULT '',
          im TEXT NOT NULL DEFAULT '',
          observacoes TEXT NOT NULL DEFAULT '',
          observacoes_financeiras TEXT NOT NULL DEFAULT '',
          historico_texto TEXT NOT NULL DEFAULT '',
          status_financeiro TEXT NOT NULL DEFAULT '',
          limite_credito REAL NOT NULL DEFAULT 0,
          bloqueado INTEGER NOT NULL DEFAULT 0,
          restrito INTEGER NOT NULL DEFAULT 0,
          foto_path TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL,
          FOREIGN KEY(vendedor_id) REFERENCES vendedores(id)
        );

        CREATE INDEX IF NOT EXISTS idx_clientes_nome ON clientes(nome_razao_social);
        CREATE INDEX IF NOT EXISTS idx_clientes_fantasia ON clientes(nome_fantasia);
        CREATE INDEX IF NOT EXISTS idx_clientes_excluido ON clientes(excluido);

        CREATE TABLE IF NOT EXISTS vendas (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          numero TEXT NOT NULL UNIQUE,
          data_emissao TEXT NOT NULL,
          cliente_id INTEGER NULL,
          vendedor_id INTEGER NULL,
          situacao TEXT NOT NULL DEFAULT 'Rascunho',
          valor_total REAL NOT NULL DEFAULT 0,
          observacoes TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL,
          FOREIGN KEY(cliente_id) REFERENCES clientes(id),
          FOREIGN KEY(vendedor_id) REFERENCES vendedores(id)
        );

        CREATE TABLE IF NOT EXISTS venda_itens (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          venda_id INTEGER NOT NULL,
          sequencia INTEGER NOT NULL,
          descricao TEXT NOT NULL DEFAULT '',
          quantidade REAL NOT NULL DEFAULT 0,
          valor_unitario REAL NOT NULL DEFAULT 0,
          valor_total REAL NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          FOREIGN KEY(venda_id) REFERENCES vendas(id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS movimentacoes_financeiras (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          data_movimento TEXT NOT NULL,
          tipo TEXT NOT NULL DEFAULT '',
          historico TEXT NOT NULL DEFAULT '',
          valor REAL NOT NULL DEFAULT 0,
          forma_pagamento_id INTEGER NULL,
          venda_id INTEGER NULL,
          cliente_id INTEGER NULL,
          criado_em TEXT NOT NULL,
          FOREIGN KEY(forma_pagamento_id) REFERENCES formas_pagamento(id),
          FOREIGN KEY(venda_id) REFERENCES vendas(id),
          FOREIGN KEY(cliente_id) REFERENCES clientes(id)
        );
        """;

    public const string V2 =
        """
        CREATE TABLE IF NOT EXISTS produtos (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          codigo TEXT NOT NULL UNIQUE,
          descricao TEXT NOT NULL,
          unidade TEXT NOT NULL DEFAULT 'UN',
          preco_avista REAL NOT NULL DEFAULT 0,
          preco_aprazo REAL NOT NULL DEFAULT 0,
          estoque_atual REAL NOT NULL DEFAULT 0,
          estoque_minimo REAL NOT NULL DEFAULT 0,
          categoria TEXT NOT NULL DEFAULT '',
          marca TEXT NOT NULL DEFAULT '',
          referencia_fabrica TEXT NOT NULL DEFAULT '',
          ativo INTEGER NOT NULL DEFAULT 1,
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL
        );

        CREATE INDEX IF NOT EXISTS idx_produtos_descricao ON produtos(descricao);
        CREATE INDEX IF NOT EXISTS idx_produtos_marca ON produtos(marca);
        CREATE INDEX IF NOT EXISTS idx_produtos_excluido ON produtos(excluido);
        """;

    public const string V3 =
        """
        CREATE TABLE IF NOT EXISTS fornecedores (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          codigo TEXT NOT NULL UNIQUE,
          razao_social TEXT NOT NULL DEFAULT '',
          nome_fantasia TEXT NOT NULL DEFAULT '',
          cnpj TEXT NOT NULL DEFAULT '',
          telefone TEXT NOT NULL DEFAULT '',
          cidade TEXT NOT NULL DEFAULT '',
          uf TEXT NOT NULL DEFAULT '',
          email TEXT NOT NULL DEFAULT '',
          observacoes TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL
        );

        CREATE INDEX IF NOT EXISTS idx_fornecedores_razao ON fornecedores(razao_social);

        CREATE TABLE IF NOT EXISTS titulos_receber (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          cliente_id INTEGER NULL,
          descricao TEXT NOT NULL DEFAULT '',
          data_emissao TEXT NOT NULL,
          data_vencimento TEXT NOT NULL,
          valor REAL NOT NULL DEFAULT 0,
          valor_recebido REAL NOT NULL DEFAULT 0,
          situacao TEXT NOT NULL DEFAULT 'Aberto',
          observacoes TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL,
          FOREIGN KEY(cliente_id) REFERENCES clientes(id)
        );

        CREATE INDEX IF NOT EXISTS idx_tr_cliente ON titulos_receber(cliente_id);
        CREATE INDEX IF NOT EXISTS idx_tr_venc ON titulos_receber(data_vencimento);

        CREATE TABLE IF NOT EXISTS titulos_pagar (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          fornecedor_id INTEGER NULL,
          descricao TEXT NOT NULL DEFAULT '',
          data_emissao TEXT NOT NULL,
          data_vencimento TEXT NOT NULL,
          valor REAL NOT NULL DEFAULT 0,
          valor_pago REAL NOT NULL DEFAULT 0,
          situacao TEXT NOT NULL DEFAULT 'Aberto',
          observacoes TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL,
          FOREIGN KEY(fornecedor_id) REFERENCES fornecedores(id)
        );

        CREATE INDEX IF NOT EXISTS idx_tp_fornecedor ON titulos_pagar(fornecedor_id);

        CREATE TABLE IF NOT EXISTS orcamentos (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          numero TEXT NOT NULL UNIQUE,
          cliente_id INTEGER NULL,
          data_emissao TEXT NOT NULL,
          situacao TEXT NOT NULL DEFAULT 'Aberto',
          valor_total REAL NOT NULL DEFAULT 0,
          observacoes TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL,
          FOREIGN KEY(cliente_id) REFERENCES clientes(id)
        );

        CREATE TABLE IF NOT EXISTS promocoes (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          nome TEXT NOT NULL,
          percentual_desconto REAL NOT NULL DEFAULT 0,
          data_inicio TEXT NOT NULL,
          data_fim TEXT NOT NULL,
          ativo INTEGER NOT NULL DEFAULT 1,
          observacoes TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS compras (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          numero TEXT NOT NULL UNIQUE,
          fornecedor_id INTEGER NULL,
          data_emissao TEXT NOT NULL,
          situacao TEXT NOT NULL DEFAULT 'Pedido',
          valor_total REAL NOT NULL DEFAULT 0,
          observacoes TEXT NOT NULL DEFAULT '',
          excluido INTEGER NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          atualizado_em TEXT NOT NULL,
          FOREIGN KEY(fornecedor_id) REFERENCES fornecedores(id)
        );
        """;

    public const string V4 =
        """
        CREATE TABLE IF NOT EXISTS orcamento_itens (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          orcamento_id INTEGER NOT NULL,
          produto_id INTEGER NULL,
          sequencia INTEGER NOT NULL,
          descricao TEXT NOT NULL DEFAULT '',
          quantidade REAL NOT NULL DEFAULT 0,
          valor_unitario REAL NOT NULL DEFAULT 0,
          valor_total REAL NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          FOREIGN KEY(orcamento_id) REFERENCES orcamentos(id) ON DELETE CASCADE,
          FOREIGN KEY(produto_id) REFERENCES produtos(id)
        );

        CREATE INDEX IF NOT EXISTS idx_oi_orc ON orcamento_itens(orcamento_id);

        CREATE TABLE IF NOT EXISTS compra_itens (
          id INTEGER PRIMARY KEY AUTOINCREMENT,
          compra_id INTEGER NOT NULL,
          produto_id INTEGER NULL,
          sequencia INTEGER NOT NULL,
          descricao TEXT NOT NULL DEFAULT '',
          quantidade REAL NOT NULL DEFAULT 0,
          valor_unitario REAL NOT NULL DEFAULT 0,
          valor_total REAL NOT NULL DEFAULT 0,
          criado_em TEXT NOT NULL,
          FOREIGN KEY(compra_id) REFERENCES compras(id) ON DELETE CASCADE,
          FOREIGN KEY(produto_id) REFERENCES produtos(id)
        );

        CREATE INDEX IF NOT EXISTS idx_ci_compra ON compra_itens(compra_id);
        """;
}
