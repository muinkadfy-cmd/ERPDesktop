using System.Data;
using System.Text;
using Dapper;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Domain.Entities;
using ERPDesktop.Domain.Enums;

namespace ERPDesktop.Infrastructure.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly IDbConnectionFactory _factory;

    public ClienteRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public string GerarProximoCodigo()
    {
        using var conn = _factory.CreateOpenConnection();
        var next = conn.ExecuteScalar<long>("SELECT IFNULL(MAX(id), 0) + 1 FROM clientes;");
        return next.ToString("D6");
    }

    public IReadOnlyList<Cliente> Pesquisar(ClienteFiltro filtro)
    {
        using var conn = _factory.CreateOpenConnection();

        var sql = new StringBuilder(
            """
            SELECT
              c.id AS Id,
              c.codigo AS Codigo,
              c.data_cadastro AS DataCadastro,
              c.status AS Status,
              c.nome_razao_social AS NomeRazaoSocial,
              c.nome_fantasia AS NomeFantasia,
              c.tipo_cadastro AS TipoCadastro,
              c.origem_marketing AS OrigemMarketing,
              c.vendedor_id AS VendedorId,
              IFNULL(v.nome, '') AS VendedorNome,
              c.endereco AS Endereco,
              c.numero AS Numero,
              c.complemento AS Complemento,
              c.bairro AS Bairro,
              c.cidade AS Cidade,
              c.uf AS Uf,
              c.cep AS Cep,
              c.telefone1 AS Telefone1,
              c.telefone2 AS Telefone2,
              c.celular AS Celular,
              c.whatsapp AS Whatsapp,
              c.whatsapp_abordagem AS WhatsappAbordagem,
              c.email AS Email,
              c.contato AS Contato,
              c.rede_social AS RedeSocial,
              c.cpf AS Cpf,
              c.rg AS Rg,
              c.orgao_emissor AS OrgaoEmissor,
              c.cnpj AS Cnpj,
              c.ie AS Ie,
              c.im AS Im,
              c.observacoes AS Observacoes,
              c.observacoes_financeiras AS ObservacoesFinanceiras,
              c.historico_texto AS HistoricoTexto,
              c.status_financeiro AS StatusFinanceiro,
              c.limite_credito AS LimiteCredito,
              c.bloqueado AS Bloqueado,
              c.restrito AS Restrito,
              c.foto_path AS FotoPath,
              c.excluido AS Excluido,
              c.criado_em AS CriadoEm,
              c.atualizado_em AS AtualizadoEm
            FROM clientes c
            LEFT JOIN vendedores v ON v.id = c.vendedor_id
            WHERE c.excluido = 0
            """);

        var dp = new DynamicParameters();

        void Like(string expr, string paramName, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            sql.Append($" AND {expr} LIKE @{paramName} ESCAPE '\\'");
            dp.Add(paramName, "%" + EscapeLike(value) + "%");
        }

        Like("c.nome_razao_social", "fNome", filtro.Nome);
        Like("c.nome_fantasia", "fFantasia", filtro.Fantasia);
        Like("c.nome_razao_social", "fRastNome", filtro.RastrearNome);
        Like("c.endereco || ' ' || c.bairro || ' ' || c.cidade", "fRastEnd", filtro.RastrearEndereco);
        Like("c.telefone1 || c.telefone2 || c.celular || c.whatsapp", "fRastFone", filtro.RastrearTelefone);
        Like("c.cpf", "fRastCpf", filtro.RastrearCpf);
        Like("c.cnpj", "fRastCnpj", filtro.RastrearCnpj);

        if (!string.IsNullOrWhiteSpace(filtro.TipoCadastro))
        {
            sql.Append(" AND c.tipo_cadastro = @tipoCadastro");
            dp.Add("tipoCadastro", filtro.TipoCadastro.Trim());
        }

        if (!string.IsNullOrWhiteSpace(filtro.OrigemMarketing))
        {
            sql.Append(" AND c.origem_marketing = @origem");
            dp.Add("origem", filtro.OrigemMarketing.Trim());
        }

        var order = (filtro.Ordenacao ?? "Nome").Trim().ToLowerInvariant() switch
        {
            "codigo" => "c.codigo",
            "fantasia" => "c.nome_fantasia",
            _ => "c.nome_razao_social"
        };

        sql.Append(" ORDER BY ").Append(order).Append(" COLLATE NOCASE;");

        var rows = conn.Query<ClienteDbRow>(sql.ToString(), dp);
        return rows.Select(Map).ToList();
    }

    public Cliente? ObterPorId(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        var row = conn.QuerySingleOrDefault<ClienteDbRow>(
            """
            SELECT
              c.id AS Id,
              c.codigo AS Codigo,
              c.data_cadastro AS DataCadastro,
              c.status AS Status,
              c.nome_razao_social AS NomeRazaoSocial,
              c.nome_fantasia AS NomeFantasia,
              c.tipo_cadastro AS TipoCadastro,
              c.origem_marketing AS OrigemMarketing,
              c.vendedor_id AS VendedorId,
              IFNULL(v.nome, '') AS VendedorNome,
              c.endereco AS Endereco,
              c.numero AS Numero,
              c.complemento AS Complemento,
              c.bairro AS Bairro,
              c.cidade AS Cidade,
              c.uf AS Uf,
              c.cep AS Cep,
              c.telefone1 AS Telefone1,
              c.telefone2 AS Telefone2,
              c.celular AS Celular,
              c.whatsapp AS Whatsapp,
              c.whatsapp_abordagem AS WhatsappAbordagem,
              c.email AS Email,
              c.contato AS Contato,
              c.rede_social AS RedeSocial,
              c.cpf AS Cpf,
              c.rg AS Rg,
              c.orgao_emissor AS OrgaoEmissor,
              c.cnpj AS Cnpj,
              c.ie AS Ie,
              c.im AS Im,
              c.observacoes AS Observacoes,
              c.observacoes_financeiras AS ObservacoesFinanceiras,
              c.historico_texto AS HistoricoTexto,
              c.status_financeiro AS StatusFinanceiro,
              c.limite_credito AS LimiteCredito,
              c.bloqueado AS Bloqueado,
              c.restrito AS Restrito,
              c.foto_path AS FotoPath,
              c.excluido AS Excluido,
              c.criado_em AS CriadoEm,
              c.atualizado_em AS AtualizadoEm
            FROM clientes c
            LEFT JOIN vendedores v ON v.id = c.vendedor_id
            WHERE c.id = @id AND c.excluido = 0;
            """,
            new { id });

        return row is null ? null : Map(row);
    }

    public long Inserir(Cliente cliente)
    {
        using var conn = _factory.CreateOpenConnection();
        SincronizarFlagsPorStatus(cliente);

        const string sql =
            """
            INSERT INTO clientes(
              codigo, data_cadastro, status, nome_razao_social, nome_fantasia, tipo_cadastro, origem_marketing, vendedor_id,
              endereco, numero, complemento, bairro, cidade, uf, cep,
              telefone1, telefone2, celular, whatsapp, whatsapp_abordagem, email, contato, rede_social,
              cpf, rg, orgao_emissor, cnpj, ie, im,
              observacoes, observacoes_financeiras, historico_texto, status_financeiro,
              limite_credito, bloqueado, restrito, foto_path, excluido, criado_em, atualizado_em
            ) VALUES (
              @Codigo, @DataCadastro, @Status, @NomeRazaoSocial, @NomeFantasia, @TipoCadastro, @OrigemMarketing, @VendedorId,
              @Endereco, @Numero, @Complemento, @Bairro, @Cidade, @Uf, @Cep,
              @Telefone1, @Telefone2, @Celular, @Whatsapp, @WhatsappAbordagem, @Email, @Contato, @RedeSocial,
              @Cpf, @Rg, @OrgaoEmissor, @Cnpj, @Ie, @Im,
              @Observacoes, @ObservacoesFinanceiras, @HistoricoTexto, @StatusFinanceiro,
              @LimiteCredito, @Bloqueado, @Restrito, @FotoPath, 0, @CriadoEm, @AtualizadoEm
            );
            SELECT last_insert_rowid();
            """;

        var p = ToParams(cliente);
        return conn.ExecuteScalar<long>(sql, p);
    }

    public void Atualizar(Cliente cliente)
    {
        using var conn = _factory.CreateOpenConnection();
        SincronizarFlagsPorStatus(cliente);

        const string sql =
            """
            UPDATE clientes SET
              codigo=@Codigo,
              data_cadastro=@DataCadastro,
              status=@Status,
              nome_razao_social=@NomeRazaoSocial,
              nome_fantasia=@NomeFantasia,
              tipo_cadastro=@TipoCadastro,
              origem_marketing=@OrigemMarketing,
              vendedor_id=@VendedorId,
              endereco=@Endereco,
              numero=@Numero,
              complemento=@Complemento,
              bairro=@Bairro,
              cidade=@Cidade,
              uf=@Uf,
              cep=@Cep,
              telefone1=@Telefone1,
              telefone2=@Telefone2,
              celular=@Celular,
              whatsapp=@Whatsapp,
              whatsapp_abordagem=@WhatsappAbordagem,
              email=@Email,
              contato=@Contato,
              rede_social=@RedeSocial,
              cpf=@Cpf,
              rg=@Rg,
              orgao_emissor=@OrgaoEmissor,
              cnpj=@Cnpj,
              ie=@Ie,
              im=@Im,
              observacoes=@Observacoes,
              observacoes_financeiras=@ObservacoesFinanceiras,
              historico_texto=@HistoricoTexto,
              status_financeiro=@StatusFinanceiro,
              limite_credito=@LimiteCredito,
              bloqueado=@Bloqueado,
              restrito=@Restrito,
              foto_path=@FotoPath,
              atualizado_em=@AtualizadoEm
            WHERE id=@Id AND excluido=0;
            """;

        conn.Execute(sql, ToParams(cliente));
    }

    public void MarcarExcluido(long id)
    {
        using var conn = _factory.CreateOpenConnection();
        conn.Execute(
            "UPDATE clientes SET excluido = 1, atualizado_em = @a WHERE id = @id;",
            new { id, a = DateTime.UtcNow.ToString("O") });
    }

    private static object ToParams(Cliente c) => new
    {
        c.Id,
        c.Codigo,
        DataCadastro = c.DataCadastro.ToString("yyyy-MM-dd"),
        Status = (int)c.Status,
        c.NomeRazaoSocial,
        c.NomeFantasia,
        c.TipoCadastro,
        c.OrigemMarketing,
        c.VendedorId,
        c.Endereco,
        c.Numero,
        c.Complemento,
        c.Bairro,
        c.Cidade,
        c.Uf,
        c.Cep,
        c.Telefone1,
        c.Telefone2,
        c.Celular,
        c.Whatsapp,
        c.WhatsappAbordagem,
        c.Email,
        c.Contato,
        c.RedeSocial,
        c.Cpf,
        c.Rg,
        c.OrgaoEmissor,
        c.Cnpj,
        c.Ie,
        c.Im,
        c.Observacoes,
        c.ObservacoesFinanceiras,
        c.HistoricoTexto,
        c.StatusFinanceiro,
        c.LimiteCredito,
        Bloqueado = c.Bloqueado ? 1 : 0,
        Restrito = c.Restrito ? 1 : 0,
        c.FotoPath,
        CriadoEm = c.CriadoEm.ToUniversalTime().ToString("O"),
        AtualizadoEm = c.AtualizadoEm.ToUniversalTime().ToString("O")
    };

    private static void SincronizarFlagsPorStatus(Cliente c)
    {
        switch (c.Status)
        {
            case ClienteStatusCadastro.Bloqueado:
                c.Bloqueado = true;
                c.Restrito = false;
                break;
            case ClienteStatusCadastro.Restrito:
                c.Bloqueado = false;
                c.Restrito = true;
                break;
            default:
                c.Bloqueado = false;
                c.Restrito = false;
                break;
        }
    }

    private static string EscapeLike(string input)
    {
        return input.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
    }

    private static Cliente Map(ClienteDbRow r)
    {
        return new Cliente
        {
            Id = r.Id,
            Codigo = r.Codigo,
            DataCadastro = DateTime.Parse(r.DataCadastro, null, System.Globalization.DateTimeStyles.AssumeLocal),
            Status = (ClienteStatusCadastro)r.Status,
            NomeRazaoSocial = r.NomeRazaoSocial,
            NomeFantasia = r.NomeFantasia,
            TipoCadastro = r.TipoCadastro,
            OrigemMarketing = r.OrigemMarketing,
            VendedorId = r.VendedorId,
            VendedorNome = r.VendedorNome,
            Endereco = r.Endereco,
            Numero = r.Numero,
            Complemento = r.Complemento,
            Bairro = r.Bairro,
            Cidade = r.Cidade,
            Uf = r.Uf,
            Cep = r.Cep,
            Telefone1 = r.Telefone1,
            Telefone2 = r.Telefone2,
            Celular = r.Celular,
            Whatsapp = r.Whatsapp,
            WhatsappAbordagem = r.WhatsappAbordagem,
            Email = r.Email,
            Contato = r.Contato,
            RedeSocial = r.RedeSocial,
            Cpf = r.Cpf,
            Rg = r.Rg,
            OrgaoEmissor = r.OrgaoEmissor,
            Cnpj = r.Cnpj,
            Ie = r.Ie,
            Im = r.Im,
            Observacoes = r.Observacoes,
            ObservacoesFinanceiras = r.ObservacoesFinanceiras,
            HistoricoTexto = r.HistoricoTexto,
            StatusFinanceiro = r.StatusFinanceiro,
            LimiteCredito = r.LimiteCredito,
            Bloqueado = r.Bloqueado != 0,
            Restrito = r.Restrito != 0,
            FotoPath = r.FotoPath,
            Excluido = r.Excluido != 0,
            CriadoEm = DateTime.Parse(r.CriadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind),
            AtualizadoEm = DateTime.Parse(r.AtualizadoEm, null, System.Globalization.DateTimeStyles.RoundtripKind)
        };
    }

    private sealed class ClienteDbRow
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string DataCadastro { get; set; } = string.Empty;
        public int Status { get; set; }
        public string NomeRazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string TipoCadastro { get; set; } = string.Empty;
        public string OrigemMarketing { get; set; } = string.Empty;
        public long? VendedorId { get; set; }
        public string VendedorNome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone1 { get; set; } = string.Empty;
        public string Telefone2 { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string WhatsappAbordagem { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        public string RedeSocial { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Rg { get; set; } = string.Empty;
        public string OrgaoEmissor { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Ie { get; set; } = string.Empty;
        public string Im { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public string ObservacoesFinanceiras { get; set; } = string.Empty;
        public string HistoricoTexto { get; set; } = string.Empty;
        public string StatusFinanceiro { get; set; } = string.Empty;
        public decimal LimiteCredito { get; set; }
        public int Bloqueado { get; set; }
        public int Restrito { get; set; }
        public string FotoPath { get; set; } = string.Empty;
        public int Excluido { get; set; }
        public string CriadoEm { get; set; } = string.Empty;
        public string AtualizadoEm { get; set; } = string.Empty;
    }
}
