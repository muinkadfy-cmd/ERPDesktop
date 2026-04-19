using System.Diagnostics;
using ERPDesktop.App.Forms;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.Services;
using ERPDesktop.Infrastructure.DependencyInjection;
using ERPDesktop.Shared.Paths;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using WinApp = System.Windows.Forms.Application;

namespace ERPDesktop.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        WinApp.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        Directory.CreateDirectory(AppPaths.LogsDirectory);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(AppPaths.LogsDirectory, "erp-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            RunInternal();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void RunInternal()
    {
        WinApp.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        WinApp.ThreadException += (_, e) => TratarExcecaoNaoTratada("Erro na interface", e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            if (ex is not null)
                Log.Fatal(ex, "UnhandledException (IsTerminating={IsTerminating})", e.IsTerminating);
            else
                Log.Fatal("UnhandledException: {Obj} (IsTerminating={IsTerminating})", e.ExceptionObject, e.IsTerminating);
            Trace.TraceError(
                "[ERP Desktop] UnhandledException (IsTerminating={0}): {1}",
                e.IsTerminating,
                ex ?? (object)e.ExceptionObject ?? "Desconhecido");
        };

        var services = new ServiceCollection();
        services.AddLogging(b =>
        {
            b.ClearProviders();
            b.AddSerilog();
        });
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
            Log.Fatal(ex, "Falha ao inicializar base de dados local");
            MessageBox.Show(
                $"Não foi possível inicializar o banco de dados local.\r\n\r\n{ex.Message}\r\n\r\nLog: {AppPaths.LogsDirectory}",
                "ERP Desktop",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        var main = provider.GetRequiredService<MainForm>();
        main.ConfigurarProvider(provider);

        WinApp.Run(main);
    }

    private static void TratarExcecaoNaoTratada(string titulo, Exception ex)
    {
        Log.Error(ex, "{Titulo}", titulo);
        Trace.TraceError("[ERP Desktop] {0}: {1}", titulo, ex);
        MessageBox.Show(
            $"{titulo}.\r\n\r\n{ex.Message}\r\n\r\nDetalhes: pasta de logs em\r\n{AppPaths.LogsDirectory}",
            "ERP Desktop",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
