namespace ERPDesktop.Application.DTOs;

public sealed class DashboardResumoDia
{
    public decimal VendasHoje { get; init; }
    public decimal ParesVendidosHoje { get; init; }
    public decimal TicketMedioHoje { get; init; }
    public int SkuEstoqueBaixo { get; init; }
    public decimal ContasReceberAberto { get; init; }
}

public sealed class VendaPorDiaPonto
{
    public DateTime Dia { get; init; }
    public decimal Total { get; init; }
}

public sealed class VendaListagemRow
{
    public long Id { get; init; }
    public string Numero { get; init; } = string.Empty;
    public DateTime DataEmissao { get; init; }
    public string ClienteNome { get; init; } = string.Empty;
    public decimal ValorTotal { get; init; }
    public string Situacao { get; init; } = string.Empty;
}

public sealed class EstoqueBaixoRow
{
    public long Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public decimal EstoqueAtual { get; init; }
    public decimal EstoqueMinimo { get; init; }
}
