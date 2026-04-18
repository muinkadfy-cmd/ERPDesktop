namespace ERPDesktop.App.Forms;

partial class EstoqueConsultaForm
{
    private System.ComponentModel.IContainer components = null!;
    private SplitContainer _split = null!;
    private FlowLayoutPanel _filtros = null!;
    private DataGridView _grid = null!;
    private GroupBox _gbAjuste = null!;
    private NumericUpDown _numNovo = null!;
    private Button _btnAplicar = null!;
    private Label _lblProd = null!;
    private StatusStrip _st = null!;
    private ToolStripStatusLabel _stMsg = null!;
    private TextBox _txtCod = null!;
    private TextBox _txtDesc = null!;
    private Button _btnFiltrar = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _split = new SplitContainer();
        _filtros = new FlowLayoutPanel();
        _grid = new DataGridView();
        _gbAjuste = new GroupBox();
        _numNovo = new NumericUpDown();
        _btnAplicar = new Button();
        _lblProd = new Label();
        _st = new StatusStrip();
        _stMsg = new ToolStripStatusLabel();
        _txtCod = new TextBox();
        _txtDesc = new TextBox();
        _btnFiltrar = new Button();
        SuspendLayout();
        Text = ">>> ESTOQUE — CONSULTA / AJUSTE <<<";
        MinimumSize = new Size(960, 560);
        _filtros.Dock = DockStyle.Top;
        _filtros.Height = 44;
        _filtros.Padding = new Padding(10, 8, 10, 8);
        _filtros.WrapContents = false;
        _filtros.Controls.Add(new Label { Text = "Código:", AutoSize = true, Margin = new Padding(0, 6, 4, 0) });
        _txtCod.Width = 120;
        _txtCod.Margin = new Padding(0, 4, 16, 0);
        _filtros.Controls.Add(_txtCod);
        _filtros.Controls.Add(new Label { Text = "Descrição:", AutoSize = true, Margin = new Padding(0, 6, 4, 0) });
        _txtDesc.Width = 280;
        _txtDesc.Margin = new Padding(0, 4, 16, 0);
        _filtros.Controls.Add(_txtDesc);
        _btnFiltrar.Text = "Filtrar";
        _btnFiltrar.AutoSize = true;
        _btnFiltrar.Margin = new Padding(0, 2, 0, 0);
        _filtros.Controls.Add(_btnFiltrar);
        _split.Dock = DockStyle.Fill;
        _split.Orientation = Orientation.Horizontal;
        _split.SplitterDistance = 360;
        _grid.Dock = DockStyle.Fill;
        _gbAjuste.Text = "Ajuste de estoque (produto selecionado)";
        _gbAjuste.Dock = DockStyle.Fill;
        _gbAjuste.Padding = new Padding(12);
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2 };
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _lblProd.Dock = DockStyle.Fill;
        _lblProd.Text = "Nenhum produto selecionado.";
        _numNovo.DecimalPlaces = 2;
        _numNovo.Maximum = 999999;
        _numNovo.Dock = DockStyle.Fill;
        _btnAplicar.Text = "Aplicar novo estoque";
        _btnAplicar.Dock = DockStyle.Fill;
        _btnAplicar.BackColor = Color.FromArgb(46, 125, 50);
        _btnAplicar.ForeColor = Color.White;
        _btnAplicar.FlatStyle = FlatStyle.Flat;
        t.Controls.Add(_lblProd, 0, 0);
        t.SetColumnSpan(_lblProd, 3);
        t.Controls.Add(new Label { Text = "Quantidade física", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        t.Controls.Add(_numNovo, 1, 1);
        t.Controls.Add(_btnAplicar, 2, 1);
        _gbAjuste.Controls.Add(t);
        _split.Panel1.Controls.Add(_grid);
        _split.Panel2.Controls.Add(_gbAjuste);
        _st.Items.Add(_stMsg);
        _stMsg.Spring = true;
        _st.Dock = DockStyle.Bottom;
        Controls.Add(_split);
        Controls.Add(_filtros);
        Controls.Add(_st);
        ResumeLayout(false);
    }
}
