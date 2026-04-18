namespace ERPDesktop.App.Forms;

partial class ComprasForm
{
    private System.ComponentModel.IContainer components = null!;
    private ToolStrip _ts = null!;
    private SplitContainer _splitMain = null!;
    private DataGridView _gridCompra = null!;
    private GroupBox _gbItens = null!;
    private DataGridView _gridItens = null!;
    private GroupBox _gbIncluir = null!;
    private TextBox _txtCodItem = null!;
    private NumericUpDown _numQtdItem = null!;
    private Button _btnAddItem = null!;
    private Button _btnRemItem = null!;
    private Button _btnSalvarItens = null!;
    private Label _lblTotItens = null!;
    private Label _lblCompraSel = null!;
    private StatusStrip _st = null!;
    private ToolStripStatusLabel _stMsg = null!;
    private TableLayoutPanel _pDetItens = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _ts = new ToolStrip();
        _splitMain = new SplitContainer();
        _gridCompra = new DataGridView();
        _gbItens = new GroupBox();
        _gridItens = new DataGridView();
        _gbIncluir = new GroupBox();
        _txtCodItem = new TextBox();
        _numQtdItem = new NumericUpDown();
        _btnAddItem = new Button();
        _btnRemItem = new Button();
        _btnSalvarItens = new Button();
        _lblTotItens = new Label();
        _lblCompraSel = new Label();
        _st = new StatusStrip();
        _stMsg = new ToolStripStatusLabel();
        SuspendLayout();

        Text = ">>> COMPRAS <<<";
        MinimumSize = new Size(880, 500);
        StartPosition = FormStartPosition.WindowsDefaultLocation;

        _ts.Dock = DockStyle.Top;
        _ts.GripStyle = ToolStripGripStyle.Hidden;

        _splitMain.Dock = DockStyle.Fill;
        _splitMain.Orientation = Orientation.Horizontal;
        _splitMain.SplitterDistance = 220;
        _splitMain.FixedPanel = FixedPanel.None;
        _splitMain.Panel1MinSize = 96;
        _splitMain.Panel2MinSize = 220;
        _splitMain.SplitterWidth = 6;

        _gridCompra.Dock = DockStyle.Fill;
        _gridCompra.BorderStyle = BorderStyle.None;

        _pDetItens = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(4, 2, 4, 4)
        };
        _pDetItens.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _pDetItens.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 288F));

        _gbItens.Text = "Itens do pedido selecionado";
        _gbItens.Dock = DockStyle.Fill;
        _gbItens.Padding = new Padding(8);
        _gridItens.Dock = DockStyle.Fill;
        _gbItens.Controls.Add(_gridItens);

        _gbIncluir.Text = "Incluir por código de produto";
        _gbIncluir.Dock = DockStyle.Fill;
        _gbIncluir.Padding = new Padding(8);
        var tInc = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7 };
        for (var i = 0; i < 7; i++)
            tInc.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tInc.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88));
        tInc.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        void L(int row, string tit, Control c)
        {
            tInc.Controls.Add(new Label { Text = tit, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            c.Dock = DockStyle.Fill;
            tInc.Controls.Add(c, 1, row);
        }
        _numQtdItem.Minimum = 0.01m;
        _numQtdItem.Maximum = 99999;
        _numQtdItem.DecimalPlaces = 2;
        _numQtdItem.Value = 1;
        _btnAddItem.Text = "Adicionar";
        _btnAddItem.BackColor = Color.FromArgb(25, 118, 210);
        _btnAddItem.ForeColor = Color.White;
        _btnAddItem.FlatStyle = FlatStyle.Flat;
        _btnRemItem.Text = "Remover linha";
        _btnSalvarItens.Text = "Gravar itens e total";
        _btnSalvarItens.BackColor = Color.FromArgb(46, 125, 50);
        _btnSalvarItens.ForeColor = Color.White;
        _btnSalvarItens.FlatStyle = FlatStyle.Flat;
        _lblTotItens.Text = "Total itens: R$ 0,00";
        _lblTotItens.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        _lblCompraSel.Text = "Selecione um pedido na lista superior.";
        _lblCompraSel.ForeColor = Color.DimGray;
        _lblCompraSel.AutoEllipsis = true;
        L(0, "Código", _txtCodItem);
        L(1, "Quantidade", _numQtdItem);
        tInc.Controls.Add(_btnAddItem, 1, 2);
        tInc.Controls.Add(_btnRemItem, 1, 3);
        tInc.Controls.Add(_btnSalvarItens, 1, 4);
        tInc.Controls.Add(_lblTotItens, 0, 5);
        tInc.SetColumnSpan(_lblTotItens, 2);
        tInc.Controls.Add(_lblCompraSel, 0, 6);
        tInc.SetColumnSpan(_lblCompraSel, 2);
        _gbIncluir.Controls.Add(tInc);

        _pDetItens.Controls.Add(_gbItens, 0, 0);
        _pDetItens.Controls.Add(_gbIncluir, 1, 0);

        _splitMain.Panel1.Controls.Add(_gridCompra);
        _splitMain.Panel2.Controls.Add(_pDetItens);

        _st.Items.Add(_stMsg);
        _stMsg.Spring = true;
        _st.Dock = DockStyle.Bottom;

        Controls.Add(_splitMain);
        Controls.Add(_ts);
        Controls.Add(_st);

        ResumeLayout(false);
    }
}
