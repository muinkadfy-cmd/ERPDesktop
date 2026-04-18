using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;
using ERPDesktop.Domain.Enums;

namespace ERPDesktop.Application.Services;

public sealed class ClienteAppService
{
    private readonly IClienteRepository _clientes;

    public ClienteAppService(IClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public IReadOnlyList<ClienteGridRow> PesquisarParaGrid(ClienteFiltro filtro)
    {
        var lista = _clientes.Pesquisar(filtro);
        return lista.Select(MapRow).ToList();
    }

    public Cliente? Obter(long id) => _clientes.ObterPorId(id);

    public Cliente CriarNovoVazio()
    {
        return new Cliente
        {
            Codigo = _clientes.GerarProximoCodigo(),
            DataCadastro = DateTime.Today,
            Status = ClienteStatusCadastro.Liberado
        };
    }

    public ResultadoOperacao Salvar(Cliente cliente)
    {
        var validacao = Validar(cliente);
        if (!validacao.Ok)
            return validacao;

        var agora = DateTime.UtcNow;
        if (cliente.Id <= 0)
        {
            cliente.CriadoEm = agora;
            cliente.AtualizadoEm = agora;
            cliente.Id = _clientes.Inserir(cliente);
        }
        else
        {
            cliente.AtualizadoEm = agora;
            _clientes.Atualizar(cliente);
        }

        return ResultadoOperacao.Sucesso();
    }

    public ResultadoOperacao ExcluirSeguro(long id)
    {
        if (id <= 0)
            return ResultadoOperacao.Fail("Selecione um cliente.");

        _clientes.MarcarExcluido(id);
        return ResultadoOperacao.Sucesso();
    }

    private static ResultadoOperacao Validar(Cliente c)
    {
        if (string.IsNullOrWhiteSpace(c.NomeRazaoSocial))
            return ResultadoOperacao.Fail("Informe o Nome / Razão Social.");

        if (c.LimiteCredito < 0)
            return ResultadoOperacao.Fail("Limite de crédito inválido.");

        return ResultadoOperacao.Sucesso();
    }

    private static ClienteGridRow MapRow(Cliente c)
    {
        return new ClienteGridRow
        {
            Id = c.Id,
            Codigo = c.Codigo,
            NomeRazaoSocial = c.NomeRazaoSocial,
            NomeFantasia = c.NomeFantasia,
            TipoCadastro = c.TipoCadastro,
            OrigemMarketing = c.OrigemMarketing,
            Whatsapp = c.Whatsapp,
            WhatsappAbordagem = c.WhatsappAbordagem,
            Telefone1 = c.Telefone1,
            Celular = c.Celular,
            Email = c.Email,
            VendedorNome = c.VendedorNome,
            Status = c.Status.ToString(),
            StatusTexto = c.Status switch
            {
                ClienteStatusCadastro.Bloqueado => "Bloqueado",
                ClienteStatusCadastro.Restrito => "Restrito",
                _ => "Liberado"
            }
        };
    }
}

public sealed class ResultadoOperacao
{
    public bool Ok { get; private init; }
    public string Mensagem { get; private init; } = string.Empty;

    public static ResultadoOperacao Sucesso() => new() { Ok = true, Mensagem = string.Empty };
    public static ResultadoOperacao Fail(string mensagem) => new() { Ok = false, Mensagem = mensagem };
}
