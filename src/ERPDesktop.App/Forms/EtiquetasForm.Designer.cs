namespace ERPDesktop.App.Forms;

partial class EtiquetasForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel _root = null!;
    private TextBox _txtCod = null!;
    private Button _btnBuscar = null!;
    private Panel _pnlPrev = null!;
    private FlowLayoutPanel _acoes = null!;
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
        _txtCod = new TextBox();
        _btnBuscar = new Button();
        _pnlPrev = new Panel();
        _acoes = new FlowLayoutPanel();
        _st = new StatusStrip();
        _stMsg = new ToolStripStatusLabel();
        SuspendLayout();
        Text = ">>> ETIQUETAS / CÓDIGO DE BARRAS <<<";
        MinimumSize = new Size(720, 480);
        _root.Dock = DockStyle.Fill;
        _root.Padding = new Padding(12);
        _root.ColumnCount = 1;
        _root.RowCount = 4;
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
        var pBusca = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        pBusca.Controls.Add(new Label { Text = "Código:", AutoSize = true, Margin = new Padding(0, 8, 6, 0) });
        _txtCod.Width = 200;
        _txtCod.Margin = new Padding(0, 4, 12, 0);
        _btnBuscar.Text = "Carregar produto";
        _btnBuscar.AutoSize = true;
        pBusca.Controls.Add(_txtCod);
        pBusca.Controls.Add(_btnBuscar);
        _pnlPrev.Dock = DockStyle.Fill;
        _pnlPrev.BackColor = Color.White;
        _pnlPrev.BorderStyle = BorderStyle.FixedSingle;
        _acoes.Dock = DockStyle.Fill;
        _acoes.FlowDirection = FlowDirection.LeftToRight;
        var b1 = new Button { Text = "Imprimir (simulado)", AutoSize = true, Margin = new Padding(0, 4, 8, 0) };
        var b2 = new Button { Text = "Gerar PDF (em breve)", AutoSize = true, Margin = new Padding(0, 4, 8, 0) };
        _acoes.Controls.Add(b1);
        _acoes.Controls.Add(b2);
        b1.Name = "btnImp";
        b2.Name = "btnPdf";
        _root.Controls.Add(pBusca, 0, 0);
        _root.Controls.Add(_pnlPrev, 0, 1);
        _root.Controls.Add(_acoes, 0, 2);
        _st.Items.Add(_stMsg);
        _stMsg.Spring = true;
        _st.Dock = DockStyle.Bottom;
        Controls.Add(_root);
        Controls.Add(_st);
        ResumeLayout(false);
    }
}
