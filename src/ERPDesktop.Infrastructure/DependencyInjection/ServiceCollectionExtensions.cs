using ERPDesktop.Application.Abstractions;
using ERPDesktop.Infrastructure.Data;
using ERPDesktop.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ERPDesktop.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddErpInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
        services.AddSingleton<IDatabaseMigrator, DatabaseMigrator>();
        services.AddSingleton<IDataSeeder, DataSeeder>();
        services.AddSingleton<IClienteRepository, ClienteRepository>();
        services.AddSingleton<IProdutoRepository, ProdutoRepository>();
        services.AddSingleton<IVendedorRepository, VendedorRepository>();
        services.AddSingleton<IDashboardQuery, DashboardQuery>();
        services.AddSingleton<IVendaRepository, VendaRepository>();
        services.AddSingleton<IFornecedorRepository, FornecedorRepository>();
        services.AddSingleton<ITituloReceberRepository, TituloReceberRepository>();
        services.AddSingleton<ITituloPagarRepository, TituloPagarRepository>();
        services.AddSingleton<IOrcamentoRepository, OrcamentoRepository>();
        services.AddSingleton<IPromocaoRepository, PromocaoRepository>();
        services.AddSingleton<ICompraRepository, CompraRepository>();
        services.AddSingleton<IConfiguracaoRepository, ConfiguracaoRepository>();
        services.AddSingleton<IMovimentacaoFinanceiraRepository, MovimentacaoFinanceiraRepository>();
        return services;
    }
}
