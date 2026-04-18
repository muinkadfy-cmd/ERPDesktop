using System.Data;
using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Domain.Entities;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class VendedorRepository : IVendedorRepository
{
    private readonly IDbConnectionFactory _factory;

    public VendedorRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<Vendedor> ListarAtivos()
    {
        using var conn = _factory.CreateOpenConnection();
        var rows = conn.Query<VendedorDb>("""
            SELECT id AS Id, codigo AS Codigo, nome AS Nome, ativo AS Ativo,
                   criado_em AS CriadoEm, atualizado_em AS AtualizadoEm
            FROM vendedores
            WHERE ativo = 1
            ORDER BY nome;
            """);

        return rows.Select(r => new Vendedor
        {
            Id = r.Id,
            Codigo = r.Codigo,
            Nome = r.Nome,
            Ativo = r.Ativo != 0,
            CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
            AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
        }).ToList();
    }

    private sealed class VendedorDb
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Ativo { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
