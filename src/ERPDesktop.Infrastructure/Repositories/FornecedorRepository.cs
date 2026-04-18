using System.Data;
using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class FornecedorRepository : IFornecedorRepository
{
    private readonly IDbConnectionFactory _factory;

    public FornecedorRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public string GerarProximoCodigo()
    {
        using var conn = _factory.CreateOpenConnection();
        var next = conn.ExecuteScalar<long>("SELECT IFNULL(MAX(id), 0) + 1 FROM fornecedores;");
        return "F" + next.ToString("D5");
    }

    public IReadOnlyList<FornecedorGridRow> ListarParaGrid()
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.Query<FornecedorGridRow>(
            """
            SELECT
              id AS Id,
              codigo AS Codigo,
              razao_social AS RazaoSocial,
              nome_fantasia AS NomeFantasia,
              cidade AS Cidade,
              uf AS Uf,
              telefone AS Telefone
            FROM fornecedores
            WHERE excluido = 0
            ORDER BY razao_social COLLATE NOCASE;
            """).ToList();
    }

    public IReadOnlyList<Fornecedor> ListarAtivos()
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<FornecedorDb>(
            """
            SELECT
              id AS Id, codigo AS Codigo, razao_social AS RazaoSocial, nome_fantasia AS NomeFantasia,
              cnpj AS Cnpj, telefone AS Telefone, cidade AS Cidade, uf AS Uf, email AS Email,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM fornecedores
            WHERE excluido = 0
            ORDER BY razao_social COLLATE NOCASE;
            """);
        return rows.Select(Map).ToList();
    }

    public Fornecedor? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<FornecedorDb>(
            """
            SELECT
              id AS Id, codigo AS Codigo, razao_social AS RazaoSocial, nome_fantasia AS NomeFantasia,
              cnpj AS Cnpj, telefone AS Telefone, cidade AS Cidade, uf AS Uf, email AS Email,
              observacoes AS Observacoes, excluido AS Excluido, criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM fornecedores WHERE id = @id AND excluido = 0;
            """,
            new { id });
        return row is null ? null : Map(row);
    }

    public long Inserir(Fornecedor f)
    {
        using var conn = _factory.CreateOpenConnection();
        const string sql =
            """
            INSERT INTO fornecedores(
              codigo, razao_social, nome_fantasia, cnpj, telefone, cidade, uf, email, observacoes,
              excluido, criado_em, atualizado_em
            ) VALUES (
              @Codigo, @RazaoSocial, @NomeFantasia, @Cnpj, @Telefone, @Cidade, @Uf, @Email, @Observacoes,
              0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;
        return conn.ExecuteScalar<long>(sql, ToParams(f));
    }

    public void Atualizar(Fornecedor f)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            """
            UPDATE fornecedores SET
              codigo=@Codigo,
              razao_social=@RazaoSocial,
              nome_fantasia=@NomeFantasia,
              cnpj=@Cnpj,
              telefone=@Telefone,
              cidade=@Cidade,
              uf=@Uf,
              email=@Email,
              observacoes=@Observacoes,
              atualizado_em=@AtualizadoEm
            WHERE id=@Id AND excluido=0;
            """,
            ToParams(f));
    }

    public void MarcarExcluido(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            "UPDATE fornecedores SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    private static object ToParams(Fornecedor f) => new
    {
        f.Id,
        f.Codigo,
        f.RazaoSocial,
        f.NomeFantasia,
        f.Cnpj,
        f.Telefone,
        f.Cidade,
        f.Uf,
        f.Email,
        f.Observacoes,
        CriadoEm = f.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = f.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static Fornecedor Map(FornecedorDb r) => new()
    {
        Id = r.Id,
        Codigo = r.Codigo,
        RazaoSocial = r.RazaoSocial,
        NomeFantasia = r.NomeFantasia,
        Cnpj = r.Cnpj,
        Telefone = r.Telefone,
        Cidade = r.Cidade,
        Uf = r.Uf,
        Email = r.Email,
        Observacoes = r.Observacoes,
        Excluido = r.Excluido != 0,
        CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
        AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private sealed class FornecedorDb
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
