namespace ERPDesktop.App.Forms;

partial class FornecedorCadastroForm
{
    private System.ComponentModel.IContainer components = null!;
    private TabControl _tabs = null!;
    private TableLayoutPanel _tpDados = null!;
    private TextBox _txtCodigo = null!;
    private TextBox _txtRazao = null!;
    private TextBox _txtFantasia = null!;
    private TextBox _txtCnpj = null!;
    private TextBox _txtTel = null!;
    private TextBox _txtCidade = null!;
    private TextBox _txtUf = null!;
    private TextBox _txtEmail = null!;
    private TextBox _txtObs = null!;
    private Button _btnSalvar = null!;
    private Button _btnSair = null!;

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
        _tpDados = new TableLayoutPanel();
        _txtCodigo = new TextBox();
        _txtRazao = new TextBox();
        _txtFantasia = new TextBox();
        _txtCnpj = new TextBox();
        _txtTel = new TextBox();
        _txtCidade = new TextBox();
        _txtUf = new TextBox();
        _txtEmail = new TextBox();
        _txtObs = new TextBox();
        _btnSalvar = new Button();
        _btnSair = new Button();
        SuspendLayout();
        Text = "Cadastro — Fornecedor";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(560, 420);
        _tabs.Dock = DockStyle.Fill;
        var pg1 = new TabPage("Dados principais");
        _tpDados.Dock = DockStyle.Fill;
        _tpDados.ColumnCount = 2;
        _tpDados.RowCount = 8;
        _tpDados.Padding = new Padding(10);
        for (var i = 0; i < 8; i++)
            _tpDados.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        _tpDados.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        _tpDados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        void L(int row, string tit, Control c)
        {
            _tpDados.Controls.Add(new Label { Text = tit, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            c.Dock = DockStyle.Fill;
            _tpDados.Controls.Add(c, 1, row);
        }
        L(0, "Código", _txtCodigo);
        L(1, "Razão social", _txtRazao);
        L(2, "Fantasia", _txtFantasia);
        L(3, "CNPJ", _txtCnpj);
        L(4, "Telefone", _txtTel);
        L(5, "Cidade", _txtCidade);
        L(6, "UF", _txtUf);
        L(7, "E-mail", _txtEmail);
        pg1.Controls.Add(_tpDados);
        var pg2 = new TabPage("Observações");
        _txtObs.Multiline = true;
        _txtObs.Dock = DockStyle.Fill;
        _txtObs.ScrollBars = ScrollBars.Vertical;
        pg2.Padding = new Padding(10);
        pg2.Controls.Add(_txtObs);
        _tabs.TabPages.Add(pg1);
        _tabs.TabPages.Add(pg2);
        var pBtn = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        _btnSair.Text = "Sair";
        _btnSair.AutoSize = true;
        _btnSalvar.Text = "Salvar";
        _btnSalvar.AutoSize = true;
        _btnSalvar.BackColor = Color.FromArgb(46, 125, 50);
        _btnSalvar.ForeColor = Color.White;
        _btnSalvar.FlatStyle = FlatStyle.Flat;
        pBtn.Controls.Add(_btnSair);
        pBtn.Controls.Add(_btnSalvar);
        Controls.Add(_tabs);
        Controls.Add(pBtn);
        ResumeLayout(false);
    }
}
