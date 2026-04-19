using ERPDesktop.App.Ui;
using ERPDesktop.Shared.Paths;
using Microsoft.Extensions.DependencyInjection;

namespace ERPDesktop.App.Forms;

public partial class MainForm : Form
{
    private IServiceProvider? _provider;
    private readonly System.Windows.Forms.Timer _relogio = new() { Interval = 1000 };
    private bool _abriuInicio;

    public MainForm()
    {
        InitializeComponent();
        ToolbarSpriteHelper.Apply(_tool, this);
        DpiChanged += (_, _) => ToolbarSpriteHelper.Apply(_tool, this);
        ErpChrome.Aplicar(this, _menu, _tool, _status);
        _header.BackColor = Color.FromArgb(27, 43, 75);
        _footerAtalhos.BackColor = Color.FromArgb(27, 43, 75);
        Text = "ERP Loja de Calçados — Desktop (local)";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        IsMdiContainer = true;
        KeyPreview = true;
        KeyDown += MainForm_KeyDown;

        _statusData.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
        _statusCaminhoDb.Text = $"Banco: {AppPaths.DatabaseFilePath}";

        _relogio.Tick += (_, _) => _statusHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        _relogio.Start();

        _tree.AfterSelect += Tree_AfterSelect;
        Shown += MainForm_Shown;
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.F2 or Keys.F3)
            AbrirModuloPdv();
        else if (e.KeyCode == Keys.F4)
            AbrirModuloProdutos();
        else if (e.KeyCode == Keys.F5)
            AbrirModuloClientes();
        else if (e.KeyCode == Keys.F6)
            AbrirModuloEstoque();
        else if (e.KeyCode == Keys.F7)
            AbrirModuloCaixa();
        else if (e.KeyCode == Keys.F8)
        {
            AlternarMenuLateral();
            e.Handled = true;
        }
    }

    private void AlternarMenuLateral()
    {
        _miExibirSidebar.Checked = !_miExibirSidebar.Checked;
        _sidebar.Visible = _miExibirSidebar.Checked;
    }

    private void menuExibirSidebar_Click(object? sender, EventArgs e) =>
        _sidebar.Visible = _miExibirSidebar.Checked;

    private void Tree_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is not string tag)
            return;
        Navegar(tag);
    }

    private void Navegar(string tag)
    {
        switch (tag)
        {
            case "INICIO":
                AbrirModuloInicio();
                break;
            case "CLI":
                AbrirModuloClientes();
                break;
            case "PROD":
                AbrirModuloProdutos();
                break;
            case "FORN":
                AbrirModuloFornecedores();
                break;
            case "PDV":
                AbrirModuloPdv();
                break;
            case "ORC":
                AbrirModuloOrcamentos();
                break;
            case "MENUVEN":
                AbrirModal<MenuVendasForm>();
                break;
            case "EST":
                AbrirModuloEstoque();
                break;
            case "COMP":
                AbrirModuloCompras();
                break;
            case "MENUFIN":
                AbrirModal<MenuFinancasForm>();
                break;
            case "CR":
                AbrirModuloContasReceber();
                break;
            case "CP":
                AbrirModuloContasPagar();
                break;
            case "CAIXA":
                AbrirModuloCaixa();
                break;
            case "REL":
                AbrirModuloRelatorios();
                break;
            case "ETI":
                AbrirModuloEtiquetas();
                break;
            case "PROMO":
                AbrirModuloPromocoes();
                break;
            case "CFG":
                AbrirModuloConfiguracoes();
                break;
        }
    }

    public void ConfigurarProvider(IServiceProvider provider)
    {
        _provider = provider;
    }

    private void MainForm_Shown(object? sender, EventArgs e)
    {
        foreach (Control c in Controls)
        {
            if (c is MdiClient m)
            {
                m.BackColor = ErpTheme.MdiWorkspace;
                break;
            }
        }

        if (_provider is null || _abriuInicio)
            return;
        _abriuInicio = true;
        AbrirFilho<DashboardForm>();
    }

    private T Criar<T>() where T : Form => (T)_provider!.GetService(typeof(T))!;

    private void AbrirModal<T>() where T : Form
    {
        if (_provider is null)
            return;

        using var f = _provider.GetRequiredService<T>();
        f.Owner = this;
        f.ShowDialog(this);
    }

    private void AbrirFilho<T>(Action<T>? configurar = null) where T : Form
    {
        if (_provider is null)
            return;

        var existente = MdiChildren.OfType<T>().FirstOrDefault();
        if (existente is not null)
        {
            existente.WindowState = FormWindowState.Maximized;
            existente.BringToFront();
            existente.Focus();
            configurar?.Invoke(existente);
            return;
        }

        var f = Criar<T>();
        f.MdiParent = this;
        configurar?.Invoke(f);
        f.WindowState = FormWindowState.Maximized;
        f.Show();
    }

    public void AbrirModuloInicio() => AbrirFilho<DashboardForm>();
    public void AbrirModuloClientes() => AbrirFilho<ClientesPesquisaForm>();
    public void AbrirModuloProdutos() => AbrirFilho<ProdutosPesquisaForm>();
    public void AbrirModuloFornecedores() => AbrirFilho<FornecedoresPesquisaForm>();
    public void AbrirModuloPdv() => AbrirFilho<PdvVendaBalcaoForm>();
    public void AbrirModuloOrcamentos() => AbrirFilho<OrcamentosPedidosForm>();
    public void AbrirModuloEstoque() => AbrirFilho<EstoqueConsultaForm>();
    public void AbrirModuloCompras() => AbrirFilho<ComprasForm>();
    public void AbrirModuloContasReceber() => AbrirFilho<ContasReceberForm>();
    public void AbrirModuloContasPagar() => AbrirFilho<ContasPagarForm>();
    public void AbrirModuloCaixa() => AbrirFilho<CaixaForm>();
    public void AbrirModuloRelatorios() => AbrirFilho<RelatoriosForm>();
    public void AbrirModuloEtiquetas() => AbrirFilho<EtiquetasForm>();
    public void AbrirModuloPromocoes() => AbrirFilho<PromocoesForm>();
    public void AbrirModuloConfiguracoes() => AbrirFilho<ConfiguracoesForm>();

    private void menuInicio_Click(object? sender, EventArgs e) => AbrirModuloInicio();
    private void menuCadastrosClientes_Click(object? sender, EventArgs e) => AbrirModuloClientes();
    private void menuCadastrosProdutos_Click(object? sender, EventArgs e) => AbrirModuloProdutos();
    private void menuCadastrosFornecedores_Click(object? sender, EventArgs e) => AbrirModuloFornecedores();
    private void menuVendasPdv_Click(object? sender, EventArgs e) => AbrirModuloPdv();
    private void menuVendasOrcamentos_Click(object? sender, EventArgs e) => AbrirModuloOrcamentos();
    private void menuVendasMenu_Click(object? sender, EventArgs e) => AbrirModal<MenuVendasForm>();
    private void menuEstoque_Click(object? sender, EventArgs e) => AbrirModuloEstoque();
    private void menuCompras_Click(object? sender, EventArgs e) => AbrirModuloCompras();
    private void menuFinancasMenu_Click(object? sender, EventArgs e) => AbrirModal<MenuFinancasForm>();
    private void menuFinCr_Click(object? sender, EventArgs e) => AbrirModuloContasReceber();
    private void menuFinCp_Click(object? sender, EventArgs e) => AbrirModuloContasPagar();
    private void menuFinCaixa_Click(object? sender, EventArgs e) => AbrirModuloCaixa();
    private void menuRelatorios_Click(object? sender, EventArgs e) => AbrirModuloRelatorios();
    private void menuEtiquetas_Click(object? sender, EventArgs e) => AbrirModuloEtiquetas();
    private void menuPromocoes_Click(object? sender, EventArgs e) => AbrirModuloPromocoes();
    private void menuConfig_Click(object? sender, EventArgs e) => AbrirModuloConfiguracoes();

    private void menuAjudaSobre_Click(object? sender, EventArgs e) =>
        MessageBox.Show(
            "ERP Loja de Calçados — Desktop\r\n.NET 8 + WinForms + SQLite (offline).\r\nBase modular em camadas (App / Application / Domain / Infrastructure).",
            "Sobre",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

    private void tsInicio_Click(object? sender, EventArgs e) => AbrirModuloInicio();
    private void tsClientes_Click(object? sender, EventArgs e) => AbrirModuloClientes();
    private void tsProdutos_Click(object? sender, EventArgs e) => AbrirModuloProdutos();
    private void tsPdv_Click(object? sender, EventArgs e) => AbrirModuloPdv();
    private void tsOrcamentos_Click(object? sender, EventArgs e) => AbrirModuloOrcamentos();
    private void tsEstoque_Click(object? sender, EventArgs e) => AbrirModuloEstoque();
    private void tsCompras_Click(object? sender, EventArgs e) => AbrirModuloCompras();
    private void tsFornecedores_Click(object? sender, EventArgs e) => AbrirModuloFornecedores();
    private void tsFinanceiro_Click(object? sender, EventArgs e) => AbrirModal<MenuFinancasForm>();
    private void tsCr_Click(object? sender, EventArgs e) => AbrirModuloContasReceber();
    private void tsCp_Click(object? sender, EventArgs e) => AbrirModuloContasPagar();
    private void tsRelatorios_Click(object? sender, EventArgs e) => AbrirModuloRelatorios();
    private void tsEtiquetas_Click(object? sender, EventArgs e) => AbrirModuloEtiquetas();
    private void tsPromocoes_Click(object? sender, EventArgs e) => AbrirModuloPromocoes();
    private void tsConfig_Click(object? sender, EventArgs e) => AbrirModuloConfiguracoes();
    private void tsSair_Click(object? sender, EventArgs e) => Close();

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        System.Windows.Forms.Application.Idle -= Application_Idle;
        _relogio.Dispose();
        base.OnFormClosed(e);
    }
}
