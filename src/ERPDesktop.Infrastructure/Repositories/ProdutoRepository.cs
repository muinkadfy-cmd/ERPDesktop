using System.Data;
using System.Text;
using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly IDbConnectionFactory _factory;

    public ProdutoRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public string GerarProximoCodigo()
    {
        using var conn = _factory.CreateOpenConnection();
        var next = conn.ExecuteScalar<long>("SELECT IFNULL(MAX(id), 0) + 1 FROM produtos;");
        return "P" + next.ToString("D5");
    }

    public IReadOnlyList<Produto> Pesquisar(ProdutoFiltro filtro)
    {
        using var conn = _factory.CreateOpenConnection();
        var sql = new StringBuilder(
            """
            SELECT
              id AS Id,
              codigo AS Codigo,
              descricao AS Descricao,
              unidade AS Unidade,
              preco_avista AS PrecoAvista,
              preco_aprazo AS PrecoAprazo,
              estoque_atual AS EstoqueAtual,
              estoque_minimo AS EstoqueMinimo,
              categoria AS Categoria,
              marca AS Marca,
              referencia_fabrica AS ReferenciaFabrica,
              ativo AS Ativo,
              excluido AS Excluido,
              criado_em AS CriadoEm,
              atualizado_em AS AtualizadoEm
            FROM produtos
            WHERE excluido = 0
            """);

        var dp = new DynamicParameters();

        void Like(string col, string param, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            sql.Append($" AND {col} LIKE @{param} ESCAPE '\\'");
            dp.Add(param, "%" + EscapeLike(value) + "%");
        }

        Like("codigo", "fCodigo", filtro.Codigo);
        Like("descricao", "fDesc", filtro.Descricao);
        Like("marca", "fMarca", filtro.Marca);
        Like("categoria", "fCat", filtro.Categoria);

        if (filtro.SomenteAtivos == true)
            sql.Append(" AND ativo = 1");

        var order = (filtro.Ordenacao ?? "Descricao").Trim().ToLowerInvariant() switch
        {
            "codigo" => "codigo",
            "marca" => "marca",
            "estoque" => "estoque_atual",
            _ => "descricao"
        };

        sql.Append(" ORDER BY ").Append(order).Append(" COLLATE NOCASE;");
        var rows = conn.Query<ProdutoDb>(sql.ToString(), dp);
        return rows.Select(Map).ToList();
    }

    public Produto? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<ProdutoDb>(
            """
            SELECT
              id AS Id,
              codigo AS Codigo,
              descricao AS Descricao,
              unidade AS Unidade,
              preco_avista AS PrecoAvista,
              preco_aprazo AS PrecoAprazo,
              estoque_atual AS EstoqueAtual,
              estoque_minimo AS EstoqueMinimo,
              categoria AS Categoria,
              marca AS Marca,
              referencia_fabrica AS ReferenciaFabrica,
              ativo AS Ativo,
              excluido AS Excluido,
              criado_em AS CriadoEm,
              atualizado_em AS AtualizadoEm
            FROM produtos
            WHERE id = @id AND excluido = 0;
            """,
            new { id });

        return row is null ? null : Map(row);
    }

    public long Inserir(Produto p)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            INSERT INTO produtos(
              codigo, descricao, unidade, preco_avista, preco_aprazo, estoque_atual, estoque_minimo,
              categoria, marca, referencia_fabrica, ativo, excluido, criado_em, atualizado_em
            ) VALUES (
              @Codigo, @Descricao, @Unidade, @PrecoAvista, @PrecoAprazo, @EstoqueAtual, @EstoqueMinimo,
              @Categoria, @Marca, @ReferenciaFabrica, @Ativo, 0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;
        return conn.ExecuteScalar<long>(sql, ToParams(p));
    }

    public void Atualizar(Produto p)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            UPDATE produtos SET
              codigo=@Codigo,
              descricao=@Descricao,
              unidade=@Unidade,
              preco_avista=@PrecoAvista,
              preco_aprazo=@PrecoAprazo,
              estoque_atual=@EstoqueAtual,
              estoque_minimo=@EstoqueMinimo,
              categoria=@Categoria,
              marca=@Marca,
              referencia_fabrica=@ReferenciaFabrica,
              ativo=@Ativo,
              atualizado_em=@AtualizadoEm
            WHERE id=@Id AND excluido=0;
            """;
        conn.Execute(sql, ToParams(p));
    }

    public void MarcarExcluido(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            "UPDATE produtos SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    public void AlterarEstoqueDelta(long produtoId, decimal delta)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            """
            UPDATE produtos SET
              estoque_atual = estoque_atual + @d,
              atualizado_em = @a
            WHERE id = @id AND excluido = 0;
            """,
            new { id = produtoId, d = delta, a = DateTime.UtcNow.ToString("O") });
    }

    private static object ToParams(Produto p) => new
    {
        p.Id,
        p.Codigo,
        p.Descricao,
        p.Unidade,
        p.PrecoAvista,
        p.PrecoAprazo,
        p.EstoqueAtual,
        p.EstoqueMinimo,
        p.Categoria,
        p.Marca,
        ReferenciaFabrica = p.ReferenciaFabrica,
        Ativo = p.Ativo ? 1 : 0,
        CriadoEm = p.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = p.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static string EscapeLike(string input) =>
        input.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);

    private static Produto Map(ProdutoDb r) => new()
    {
        Id = r.Id,
        Codigo = r.Codigo,
        Descricao = r.Descricao,
        Unidade = r.Unidade,
        PrecoAvista = r.PrecoAvista,
        PrecoAprazo = r.PrecoAprazo,
        EstoqueAtual = r.EstoqueAtual,
        EstoqueMinimo = r.EstoqueMinimo,
        Categoria = r.Categoria,
        Marca = r.Marca,
        ReferenciaFabrica = r.ReferenciaFabrica,
        Ativo = r.Ativo != 0,
        Excluido = r.Excluido != 0,
        CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
        AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private sealed class ProdutoDb
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Unidade { get; set; } = string.Empty;
        public decimal PrecoAvista { get; set; }
        public decimal PrecoAprazo { get; set; }
        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string ReferenciaFabrica { get; set; } = string.Empty;
        public int Ativo { get; set; }
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
