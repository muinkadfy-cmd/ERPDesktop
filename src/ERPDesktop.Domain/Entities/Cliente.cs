using ERPDesktop.Domain.Enums;

namespace ERPDesktop.Domain.Entities;

public sealed class Cliente
{
    public long Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.Today;
    public ClienteStatusCadastro Status { get; set; } = ClienteStatusCadastro.Liberado;

    public string NomeRazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string TipoCadastro { get; set; } = string.Empty;
    public string OrigemMarketing { get; set; } = string.Empty;
    public long? VendedorId { get; set; }

    /// <summary>Preenchido em consultas com JOIN (não persistido diretamente).</summary>
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
    public bool Bloqueado { get; set; }
    public bool Restrito { get; set; }

    public string FotoPath { get; set; } = string.Empty;

    public bool Excluido { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
