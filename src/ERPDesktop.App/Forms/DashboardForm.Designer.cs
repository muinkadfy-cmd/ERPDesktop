using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

partial class DashboardForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel _layoutRoot = null!;
    private FlowLayoutPanel _painelCards = null!;
    private Panel _pnlGrafico = null!;
    private SplitContainer _splitListas = null!;
    private DataGridView _gridVendas = null!;
    private DataGridView _gridEstoque = null!;
    private FlowLayoutPanel _painelAcoes = null!;
    private Label _lblTitulo = null!;
    private StatusStrip _statusDash = null!;
    private ToolStripStatusLabel _statusMsg = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _layoutRoot = new TableLayoutPanel();
        _lblTitulo = new Label();
        _painelCards = new FlowLayoutPanel();
        _pnlGrafico = new Panel();
        _splitListas = new SplitContainer();
        _gridVendas = new DataGridView();
        _gridEstoque = new DataGridView();
        _painelAcoes = new FlowLayoutPanel();
        _statusDash = new StatusStrip();
        _statusMsg = new ToolStripStatusLabel();

        SuspendLayout();

        Text = ">>> INÍCIO / DASHBOARD <<<";
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        StartPosition = FormStartPosition.WindowsDefaultLocation;
        MinimumSize = new Size(960, 620);

        _layoutRoot.Dock = DockStyle.Fill;
        _layoutRoot.Padding = new Padding(12, 10, 12, 8);
        _layoutRoot.ColumnCount = 1;
        _layoutRoot.RowCount = 5;
        _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
        _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
        _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

        _lblTitulo.Dock = DockStyle.Fill;
        _lblTitulo.Text = "Painel do dia — visão geral da operação";
        _lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
        _lblTitulo.Font = ErpTheme.UiTitle(12f);
        _lblTitulo.ForeColor = Color.FromArgb(27, 43, 75);

        _painelCards.Dock = DockStyle.Fill;
        _painelCards.WrapContents = false;
        _painelCards.FlowDirection = FlowDirection.LeftToRight;
        _painelCards.Padding = new Padding(0, 4, 0, 4);
        _painelCards.AutoScroll = true;

        _pnlGrafico.Dock = DockStyle.Fill;
        _pnlGrafico.BackColor = Color.White;
        _pnlGrafico.BorderStyle = BorderStyle.FixedSingle;
        _pnlGrafico.Padding = new Padding(8);

        _splitListas.Dock = DockStyle.Fill;
        _splitListas.Orientation = Orientation.Horizontal;
        _splitListas.SplitterDistance = 200;
        _splitListas.Panel1.Padding = new Padding(0, 0, 0, 4);
        _splitListas.Panel2.Padding = new Padding(0, 4, 0, 0);
        _gridVendas.Dock = DockStyle.Fill;
        _gridEstoque.Dock = DockStyle.Fill;

        _painelAcoes.Dock = DockStyle.Fill;
        _painelAcoes.FlowDirection = FlowDirection.LeftToRight;
        _painelAcoes.WrapContents = false;
        _painelAcoes.Padding = new Padding(0, 6, 0, 0);

        _statusDash.Items.Add(_statusMsg);
        _statusMsg.Spring = true;
        _statusMsg.TextAlign = ContentAlignment.MiddleLeft;

        _layoutRoot.Controls.Add(_lblTitulo, 0, 0);
        _layoutRoot.Controls.Add(_painelCards, 0, 1);
        _layoutRoot.Controls.Add(_pnlGrafico, 0, 2);
        _layoutRoot.Controls.Add(_splitListas, 0, 3);
        _layoutRoot.Controls.Add(_painelAcoes, 0, 4);

        _splitListas.Panel1.Controls.Add(MkListaHost("Últimas vendas", _gridVendas));
        _splitListas.Panel2.Controls.Add(MkListaHost("Estoque baixo", _gridEstoque));

        Controls.Add(_layoutRoot);
        Controls.Add(_statusDash);
        _statusDash.Dock = DockStyle.Bottom;

        ResumeLayout(false);
        PerformLayout();
    }

    private static Panel MkListaHost(string titulo, Control grid)
    {
        var p = new Panel { Dock = DockStyle.Fill };
        var l = new Label
        {
            Text = titulo,
            Dock = DockStyle.Top,
            Height = 24,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = ErpTheme.UiTitle(10f),
            ForeColor = ErpTheme.TextMuted
        };
        grid.Dock = DockStyle.Fill;
        p.Controls.Add(grid);
        p.Controls.Add(l);
        return p;
    }
}
