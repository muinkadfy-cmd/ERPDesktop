using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

partial class ConfiguracoesForm
{
    private System.ComponentModel.IContainer components = null!;
    private TabControl _tabs = null!;
    private TextBox _txtRazao = null!;
    private TextBox _txtCidade = null!;
    private TextBox _txtFone = null!;
    private Button _btnSalvarEmp = null!;
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
        _tabs = new TabControl();
        _txtRazao = new TextBox();
        _txtCidade = new TextBox();
        _txtFone = new TextBox();
        _btnSalvarEmp = new Button();
        _st = new StatusStrip();
        _stMsg = new ToolStripStatusLabel();
        SuspendLayout();
        Text = ">>> CONFIGURAÇÕES <<<";
        ClientSize = new Size(560, 400);
        MinimumSize = new Size(520, 360);
        FormBorderStyle = FormBorderStyle.Sizable;
        _tabs.Dock = DockStyle.Fill;
        var pg = new TabPage("Empresa / identificação");
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, RowCount = 5 };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < 5; i++)
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        void L(int row, string tit, Control c)
        {
            t.Controls.Add(new Label { Text = tit, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            c.Dock = DockStyle.Fill;
            t.Controls.Add(c, 1, row);
        }
        L(0, "Razão social", _txtRazao);
        L(1, "Cidade", _txtCidade);
        L(2, "Telefone loja", _txtFone);
        _btnSalvarEmp.Text = "Salvar dados da empresa";
        _btnSalvarEmp.Height = 36;
        t.Controls.Add(_btnSalvarEmp, 1, 3);
        var lbl = new Label
        {
            Text = "Valores gravados na tabela configuracoes (SQLite local).",
            Dock = DockStyle.Fill,
            ForeColor = ErpTheme.TextMuted
        };
        t.Controls.Add(lbl, 1, 4);
        pg.Controls.Add(t);
        var pg2 = new TabPage("Fiscal / NF-e");
        var l2 = new Label
        {
            Text = "Módulo fiscal será integrado em etapa posterior. Aqui ficam apenas parâmetros gerais.",
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            TextAlign = ContentAlignment.TopLeft
        };
        pg2.Controls.Add(l2);
        _tabs.TabPages.Add(pg);
        _tabs.TabPages.Add(pg2);
        _st.Items.Add(_stMsg);
        _stMsg.Spring = true;
        _st.Dock = DockStyle.Bottom;
        Controls.Add(_tabs);
        Controls.Add(_st);
        ResumeLayout(false);
    }
}
