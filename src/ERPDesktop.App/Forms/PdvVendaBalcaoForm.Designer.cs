using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

partial class PdvVendaBalcaoForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel _root = null!;
    private GroupBox _gbEntrada = null!;
    private Label _lblCod = null!;
    private TextBox _txtCodigo = null!;
    private Label _lblQtd = null!;
    private NumericUpDown _numQtd = null!;
    private Button _btnAdd = null!;
    private GroupBox _gbCliente = null!;
    private ComboBox _cbCliente = null!;
    private DataGridView _grid = null!;
    private TableLayoutPanel _pnlResumo = null!;
    private Label _lblSubTit = null!;
    private Label _lblSub = null!;
    private Label _lblTotTit = null!;
    private Label _lblTotal = null!;
    private Button _btnFinalizar = null!;
    private ToolStrip _ts = null!;
    private StatusStrip _st = null!;
    private ToolStripStatusLabel _stMsg = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _root = new TableLayoutPanel();
        _ts = new ToolStrip();
        _gbEntrada = new GroupBox();
        _lblCod = new Label();
        _txtCodigo = new TextBox();
        _lblQtd = new Label();
        _numQtd = new NumericUpDown();
        _btnAdd = new Button();
        _gbCliente = new GroupBox();
        _cbCliente = new ComboBox();
        _grid = new DataGridView();
        _pnlResumo = new TableLayoutPanel();
        _lblSubTit = new Label();
        _lblSub = new Label();
        _lblTotTit = new Label();
        _lblTotal = new Label();
        _btnFinalizar = new Button();
        _st = new StatusStrip();
        _stMsg = new ToolStripStatusLabel();

        SuspendLayout();

        Text = ">>> PDV / VENDA BALCÃO <<<";
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        MinimumSize = new Size(920, 560);
        StartPosition = FormStartPosition.WindowsDefaultLocation;

        _ts.GripStyle = ToolStripGripStyle.Hidden;
        _ts.Dock = DockStyle.Top;
        _ts.Items.Add(new ToolStripLabel("Esc — Sair  |  Enter no código — adicionar  |  F12 — Finalizar") { ForeColor = ErpTheme.TextMuted });

        _root.Dock = DockStyle.Fill;
        _root.Padding = new Padding(12, 8, 12, 8);
        _root.ColumnCount = 2;
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        _root.RowCount = 2;
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
        _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _gbEntrada.Text = "Código / leitura";
        _gbEntrada.Dock = DockStyle.Fill;
        _gbEntrada.Padding = new Padding(10);
        var tEnt = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 2 };
        tEnt.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
        tEnt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tEnt.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        tEnt.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        tEnt.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        tEnt.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        tEnt.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        tEnt.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        _lblCod.Text = "Código:";
        _lblCod.Dock = DockStyle.Fill;
        _lblCod.TextAlign = ContentAlignment.MiddleLeft;
        _txtCodigo.Dock = DockStyle.Fill;
        _txtCodigo.Margin = new Padding(4);
        _lblQtd.Text = "Qtd:";
        _lblQtd.Dock = DockStyle.Fill;
        _lblQtd.TextAlign = ContentAlignment.MiddleRight;
        _numQtd.Minimum = 0.01m;
        _numQtd.Maximum = 9999;
        _numQtd.DecimalPlaces = 2;
        _numQtd.Increment = 1;
        _numQtd.Value = 1;
        _numQtd.Dock = DockStyle.Fill;
        _btnAdd.Text = "Adicionar (Enter)";
        _btnAdd.Dock = DockStyle.Fill;
        _btnAdd.BackColor = Color.FromArgb(25, 118, 210);
        _btnAdd.ForeColor = Color.White;
        _btnAdd.FlatStyle = FlatStyle.Flat;
        tEnt.Controls.Add(_lblCod, 0, 0);
        tEnt.Controls.Add(_txtCodigo, 1, 0);
        tEnt.SetColumnSpan(_txtCodigo, 3);
        tEnt.Controls.Add(_lblQtd, 0, 1);
        tEnt.Controls.Add(_numQtd, 1, 1);
        tEnt.Controls.Add(_btnAdd, 2, 1);
        tEnt.SetColumnSpan(_btnAdd, 4);
        _gbEntrada.Controls.Add(tEnt);

        _gbCliente.Text = "Cliente (opcional)";
        _gbCliente.Dock = DockStyle.Fill;
        _gbCliente.Padding = new Padding(10);
        _cbCliente.Dock = DockStyle.Fill;
        _cbCliente.DropDownStyle = ComboBoxStyle.DropDownList;
        _gbCliente.Controls.Add(_cbCliente);

        var pTopo = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        pTopo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
        pTopo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
        pTopo.Controls.Add(_gbEntrada, 0, 0);
        pTopo.Controls.Add(_gbCliente, 1, 0);

        _grid.Dock = DockStyle.Fill;
        _pnlResumo.Dock = DockStyle.Fill;
        _pnlResumo.ColumnCount = 1;
        _pnlResumo.RowCount = 5;
        _pnlResumo.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        _pnlResumo.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        _pnlResumo.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        _pnlResumo.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        _pnlResumo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _lblSubTit.Text = "Subtotal";
        _lblSubTit.ForeColor = ErpTheme.TextMuted;
        _lblSubTit.Dock = DockStyle.Fill;
        _lblSub.Text = "R$ 0,00";
        _lblSub.Font = ErpTheme.UiTitle(11f);
        _lblSub.Dock = DockStyle.Fill;
        _lblTotTit.Text = "Total a pagar";
        _lblTotTit.ForeColor = ErpTheme.TextMuted;
        _lblTotTit.Dock = DockStyle.Fill;
        _lblTotal.Text = "R$ 0,00";
        _lblTotal.Font = ErpTheme.UiTitle(16f);
        _lblTotal.Dock = DockStyle.Fill;
        _btnFinalizar.Text = "Finalizar — F12";
        _btnFinalizar.Dock = DockStyle.Top;
        _btnFinalizar.Height = 44;
        _btnFinalizar.BackColor = Color.FromArgb(46, 125, 50);
        _btnFinalizar.ForeColor = Color.White;
        _btnFinalizar.FlatStyle = FlatStyle.Flat;
        _btnFinalizar.Font = ErpTheme.UiFont(10f, FontStyle.Bold);
        _pnlResumo.Controls.Add(_lblSubTit, 0, 0);
        _pnlResumo.Controls.Add(_lblSub, 0, 1);
        _pnlResumo.Controls.Add(_lblTotTit, 0, 2);
        _pnlResumo.Controls.Add(_lblTotal, 0, 3);
        _pnlResumo.Controls.Add(_btnFinalizar, 0, 4);

        _root.Controls.Add(pTopo, 0, 0);
        _root.SetColumnSpan(pTopo, 2);
        _root.Controls.Add(_grid, 0, 1);
        _root.Controls.Add(_pnlResumo, 1, 1);

        _st.Items.Add(_stMsg);
        _stMsg.Spring = true;
        _stMsg.TextAlign = ContentAlignment.MiddleLeft;
        _st.Dock = DockStyle.Bottom;
        _st.BackColor = ErpTheme.StatusBack;

        Controls.Add(_root);
        Controls.Add(_ts);
        Controls.Add(_st);

        ResumeLayout(false);
        PerformLayout();
    }
}
