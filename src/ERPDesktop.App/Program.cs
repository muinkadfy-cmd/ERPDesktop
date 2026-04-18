using ERPDesktop.App.Forms;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.Services;
using ERPDesktop.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WinApp = System.Windows.Forms.Application;

namespace ERPDesktop.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        WinApp.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        var services = new ServiceCollection();
        services.AddErpInfrastructure();
        services.AddSingleton<ClienteAppService>();
        services.AddSingleton<ProdutoAppService>();
        services.AddSingleton<DashboardAppService>();
        services.AddSingleton<VendaPdvAppService>();
        services.AddSingleton<FornecedorAppService>();
        services.AddSingleton<ConfiguracoesAppService>();
        services.AddTransient<MainForm>();
        services.AddTransient<MenuVendasForm>();
        services.AddTransient<MenuFinancasForm>();
        services.AddTransient<ClientesPesquisaForm>();
        services.AddTransient<ClienteCadastroForm>();
        services.AddTransient<ProdutosPesquisaForm>();
        services.AddTransient<ProdutoCadastroForm>();
        services.AddTransient<DashboardForm>();
        services.AddTransient<PdvVendaBalcaoForm>();
        services.AddTransient<OrcamentosPedidosForm>();
        services.AddTransient<EstoqueConsultaForm>();
        services.AddTransient<ComprasForm>();
        services.AddTransient<FornecedoresPesquisaForm>();
        services.AddTransient<FornecedorCadastroForm>();
        services.AddTransient<ContasReceberForm>();
        services.AddTransient<ContasPagarForm>();
        services.AddTransient<CaixaForm>();
        services.AddTransient<RelatoriosForm>();
        services.AddTransient<EtiquetasForm>();
        services.AddTransient<PromocoesForm>();
        services.AddTransient<ConfiguracoesForm>();

        using var provider = services.BuildServiceProvider();

        try
        {
            provider.GetRequiredService<IDatabaseMigrator>().AplicarPendentes();
            provider.GetRequiredService<IDataSeeder>().ExecutarSeNecessario();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível inicializar o banco de dados local.\r\n\r\n{ex.Message}",
                "ERP Desktop",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        var main = provider.GetRequiredService<MainForm>();
        main.ConfigurarProvider(provider);

        WinApp.Run(main);
    }
}
