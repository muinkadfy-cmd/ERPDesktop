using Dapper;
using ERPDesktop.Application.Abstractions;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class ConfiguracaoRepository : IConfiguracaoRepository
{
    private readonly IDbConnectionFactory _factory;

    public ConfiguracaoRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public string? ObterValor(string chave)
    {
        using var conn = _factory.CreateOpenConnection();
        return conn.ExecuteScalar<string?>(
            "SELECT valor FROM configuracoes WHERE chave = @c LIMIT 1;",
            new { c = chave });
    }

    public void DefinirValor(string chave, string valor)
    {
        using var conn = _factory.CreateOpenConnection();
        var agora = DateTime.UtcNow.ToString("O");
        conn.Execute(
            """
            INSERT INTO configuracoes(chave, valor, atualizado_em) VALUES(@chave, @valor, @a)
            ON CONFLICT(chave) DO UPDATE SET valor = excluded.valor, atualizado_em = excluded.atualizado_em;
            """,
            new { chave, valor, a = agora });
    }
}
