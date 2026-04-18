namespace ERPDesktop.App.Forms;

partial class ClienteCadastroForm
{
    private SplitContainer _split = null!;
    private PictureBox _pic = null!;
    private Button _btnFoto = null!;
    private Button _btnLimparFoto = null!;

    private Label _lblRegistro = null!;
    private DateTimePicker _dtCadastro = null!;
    private RadioButton _rbLiberado = null!;
    private RadioButton _rbRestrito = null!;
    private RadioButton _rbBloqueado = null!;
    private ComboBox _cbVendedor = null!;
    private TextBox _txtCodigo = null!;
    private TextBox _txtNome = null!;
    private TextBox _txtFantasia = null!;

    private TabControl _tabs = null!;
    private TextBox _txtEndereco = null!;
    private TextBox _txtNumero = null!;
    private TextBox _txtComplemento = null!;
    private TextBox _txtBairro = null!;
    private TextBox _txtCidade = null!;
    private ComboBox _cbUf = null!;
    private TextBox _txtCep = null!;
    private TextBox _txtTel1 = null!;
    private TextBox _txtTel2 = null!;
    private TextBox _txtCel = null!;
    private TextBox _txtWa = null!;
    private TextBox _txtWaAbordagem = null!;
    private TextBox _txtEmail = null!;
    private TextBox _txtContato = null!;
    private TextBox _txtRede = null!;

    private ComboBox _cbTipoCadastro = null!;
    private TextBox _txtOrigem = null!;
    private NumericUpDown _numLimite = null!;
    private TextBox _txtStatusFin = null!;
    private CheckBox _chkRestrito = null!;
    private CheckBox _chkBloqueado = null!;
    private TextBox _txtObsFin = null!;

    private TextBox _txtCpf = null!;
    private TextBox _txtRg = null!;
    private TextBox _txtOrgao = null!;
    private TextBox _txtCnpj = null!;
    private TextBox _txtIe = null!;
    private TextBox _txtIm = null!;
    private TextBox _txtHistorico = null!;

    private TextBox _txtObs = null!;
    private Button _btnSalvar = null!;
    private Button _btnImprimir = null!;
    private Button _btnSair = null!;

    private void InitializeComponent()
    {
        SuspendLayout();

        _split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            FixedPanel = FixedPanel.Panel1,
            SplitterWidth = 6,
            SplitterDistance = 240,
            BackColor = Color.FromArgb(245, 245, 245)
        };

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(10)
        };
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        _pic = new PictureBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            SizeMode = PictureBoxSizeMode.Zoom
        };

        _btnFoto = new Button { Text = "Carregar foto…", Dock = DockStyle.Fill, Margin = new Padding(0, 6, 0, 0) };
        _btnFoto.Click += BtnFoto_Click;
        _btnLimparFoto = new Button { Text = "Limpar", Dock = DockStyle.Fill, Margin = new Padding(0, 6, 0, 0) };
        _btnLimparFoto.Click += BtnLimparFoto_Click;

        var fotoBtns = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        fotoBtns.Controls.Add(_btnFoto);
        fotoBtns.Controls.Add(_btnLimparFoto);

        left.Controls.Add(_pic, 0, 0);
        left.Controls.Add(fotoBtns, 0, 1);

        _split.Panel1.Controls.Add(left);

        var rightRoot = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(10, 10, 10, 10)
        };
        rightRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rightRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        rightRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rightRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rightRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));

        var header = new GroupBox
        {
            Text = "Identificação",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 10, 10)
        };
        var ht = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 4 };
        ht.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        ht.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        ht.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        ht.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        ht.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        ht.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

        ht.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        ht.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        ht.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        ht.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        _lblRegistro = new Label { AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        _dtCadastro = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill };

        _rbLiberado = new RadioButton { Text = "LIBERAR", AutoSize = true, Checked = true };
        _rbRestrito = new RadioButton { Text = "RESTRINGIR", AutoSize = true };
        _rbBloqueado = new RadioButton { Text = "BLOQUEAR", AutoSize = true };
        _rbLiberado.CheckedChanged += Status_Changed;
        _rbRestrito.CheckedChanged += Status_Changed;
        _rbBloqueado.CheckedChanged += Status_Changed;

        var pStatus = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        pStatus.Controls.Add(_rbLiberado);
        pStatus.Controls.Add(_rbRestrito);
        pStatus.Controls.Add(_rbBloqueado);

        _cbVendedor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        _txtCodigo = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.WhiteSmoke };
        _txtNome = new TextBox { Dock = DockStyle.Fill };
        _txtFantasia = new TextBox { Dock = DockStyle.Fill };

        void L(int col, int row, string t, Control c, int colspan = 1)
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var lab = new Label { Text = t, Dock = DockStyle.Top, AutoSize = true, ForeColor = Color.FromArgb(60, 60, 60) };
            c.Dock = DockStyle.Fill;
            p.Controls.Add(c);
            p.Controls.Add(lab);
            ht.Controls.Add(p, col, row);
            if (colspan > 1)
                ht.SetColumnSpan(p, colspan);
        }

        L(0, 0, "Registro", _lblRegistro, 1);
        L(1, 0, "Data cadastro", _dtCadastro, 1);
        L(2, 0, "Status", pStatus, 4);

        L(0, 1, "Código", _txtCodigo, 2);
        L(2, 1, "Vendedor", _cbVendedor, 4);

        L(0, 2, "Nome / Razão completo", _txtNome, 6);
        L(0, 3, "Nome fantasia / apelido", _txtFantasia, 6);

        header.Controls.Add(ht);

        _tabs = new TabControl { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0) };

        var tab1 = new TabPage("Endereço e Contatos");
        tab1.Padding = new Padding(10);
        var t1 = MkTable(8, 2);
        _txtEndereco = MkTxt();
        _txtNumero = MkTxt();
        _txtComplemento = MkTxt();
        _txtBairro = MkTxt();
        _txtCidade = MkTxt();
        _cbUf = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        _cbUf.Items.AddRange(new object[]
        {
            "","AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
        });
        _txtCep = MkTxt();
        _txtTel1 = MkTxt();
        _txtTel2 = MkTxt();
        _txtCel = MkTxt();
        _txtWa = MkTxt();
        _txtWaAbordagem = MkTxt();
        _txtEmail = MkTxt();
        _txtContato = MkTxt();
        _txtRede = MkTxt();

        AddL(t1, 0, 0, "Endereço", _txtEndereco);
        AddL(t1, 1, 0, "Número", _txtNumero);
        AddL(t1, 0, 1, "Complemento", _txtComplemento);
        AddL(t1, 1, 1, "Bairro", _txtBairro);
        AddL(t1, 0, 2, "Cidade", _txtCidade);
        AddL(t1, 1, 2, "UF", _cbUf);
        AddL(t1, 0, 3, "CEP", _txtCep);
        AddL(t1, 1, 3, "1º telefone", _txtTel1);
        AddL(t1, 0, 4, "2º telefone", _txtTel2);
        AddL(t1, 1, 4, "Celular", _txtCel);
        AddL(t1, 0, 5, "WhatsApp", _txtWa);
        AddL(t1, 1, 5, "Abordagem no WhatsApp", _txtWaAbordagem);
        AddL(t1, 0, 6, "E-mail", _txtEmail);
        AddL(t1, 1, 6, "Contato", _txtContato);
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var lab = new Label { Text = "Rede social", Dock = DockStyle.Top, AutoSize = true, ForeColor = Color.FromArgb(60, 60, 60) };
            _txtRede.Dock = DockStyle.Fill;
            p.Controls.Add(_txtRede);
            p.Controls.Add(lab);
            t1.Controls.Add(p, 0, 7);
            t1.SetColumnSpan(p, 2);
        }
        tab1.Controls.Add(t1);

        var tab2 = new TabPage("Filiação e Avaliação Financeira");
        tab2.Padding = new Padding(10);
        var t2 = MkTable(3, 2);
        _cbTipoCadastro = new ComboBox { Dock = DockStyle.Fill };
        _cbTipoCadastro.Items.AddRange(new object[] { "Pessoa Física", "Pessoa Jurídica" });
        _cbTipoCadastro.DropDownStyle = ComboBoxStyle.DropDownList;
        _txtOrigem = MkTxt();
        _numLimite = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 2, Maximum = 100000000, Minimum = 0 };
        _txtStatusFin = MkTxt();
        _chkRestrito = new CheckBox { Text = "Restrito (financeiro)", AutoSize = true, Enabled = false };
        _chkBloqueado = new CheckBox { Text = "Bloqueado (financeiro)", AutoSize = true, Enabled = false };
        _txtObsFin = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };

        AddL(t2, 0, 0, "Tipo de cadastro", _cbTipoCadastro);
        AddL(t2, 1, 0, "Origem / marketing", _txtOrigem);
        AddL(t2, 0, 1, "Limite de crédito", _numLimite);
        AddL(t2, 1, 1, "Status financeiro", _txtStatusFin);

        var pChk = new Panel { Dock = DockStyle.Fill };
        pChk.Controls.Add(_chkBloqueado);
        pChk.Controls.Add(_chkRestrito);
        _chkRestrito.Top = 6;
        _chkBloqueado.Top = 28;
        AddL(t2, 0, 2, "Situação (somente leitura)", pChk);
        AddL(t2, 1, 2, "Observações financeiras", _txtObsFin);
        tab2.Controls.Add(t2);

        var tab3 = new TabPage("Informações / Observações / Histórico");
        tab3.Padding = new Padding(10);
        var t3 = MkTable(4, 2);
        _txtCpf = MkTxt();
        _txtRg = MkTxt();
        _txtOrgao = MkTxt();
        _txtCnpj = MkTxt();
        _txtIe = MkTxt();
        _txtIm = MkTxt();
        _txtHistorico = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };

        AddL(t3, 0, 0, "CPF", _txtCpf);
        AddL(t3, 1, 0, "RG", _txtRg);
        AddL(t3, 0, 1, "Órgão emissor", _txtOrgao);
        AddL(t3, 1, 1, "CNPJ", _txtCnpj);
        AddL(t3, 0, 2, "IE", _txtIe);
        AddL(t3, 1, 2, "IM", _txtIm);
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var lab = new Label { Text = "Histórico textual", Dock = DockStyle.Top, AutoSize = true, ForeColor = Color.FromArgb(60, 60, 60) };
            _txtHistorico.Dock = DockStyle.Fill;
            p.Controls.Add(_txtHistorico);
            p.Controls.Add(lab);
            t3.Controls.Add(p, 0, 3);
            t3.SetColumnSpan(p, 2);
        }
        tab3.Controls.Add(t3);
        t3.RowStyles[3] = new RowStyle(SizeType.Absolute, 180);

        _tabs.TabPages.Add(tab1);
        _tabs.TabPages.Add(tab2);
        _tabs.TabPages.Add(tab3);

        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));

        _txtObs = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Margin = new Padding(0, 0, 10, 0),
            PlaceholderText = "Observações gerais do cliente…"
        };

        var side = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
        _btnSalvar = new Button { Text = "Salvar cadastro", Width = 130, Height = 44, Margin = new Padding(0, 0, 0, 10), BackColor = Color.FromArgb(0, 130, 70), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnSalvar.FlatAppearance.BorderSize = 0;
        _btnSalvar.Click += BtnSalvar_Click;
        _btnImprimir = new Button { Text = "Imprimir ficha", Width = 130, Height = 40, Margin = new Padding(0, 0, 0, 10) };
        _btnImprimir.Click += BtnImprimir_Click;
        _btnSair = new Button { Text = "Sair", Width = 130, Height = 40 };
        _btnSair.Click += BtnSair_Click;
        side.Controls.Add(_btnSalvar);
        side.Controls.Add(_btnImprimir);
        side.Controls.Add(_btnSair);

        bottom.Controls.Add(_txtObs, 0, 0);
        bottom.Controls.Add(side, 1, 0);

        rightRoot.Controls.Add(header, 0, 0);
        rightRoot.SetColumnSpan(header, 2);
        rightRoot.Controls.Add(_tabs, 0, 1);
        rightRoot.SetColumnSpan(_tabs, 2);
        rightRoot.Controls.Add(bottom, 0, 2);
        rightRoot.SetColumnSpan(bottom, 2);

        _split.Panel2.Controls.Add(rightRoot);

        Controls.Add(_split);

        ClientSize = new Size(1040, 720);
        Text = "Cadastro de Clientes";
        ResumeLayout(false);
    }

    private static TableLayoutPanel MkTable(int rows, int cols)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = cols, RowCount = rows };
        for (var c = 0; c < cols; c++)
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        for (var r = 0; r < rows; r++)
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        return t;
    }

    private static TextBox MkTxt() => new() { Dock = DockStyle.Fill };

    private static void AddL(TableLayoutPanel t, int col, int row, string titulo, Control c)
    {
        var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
        var lab = new Label { Text = titulo, Dock = DockStyle.Top, AutoSize = true, ForeColor = Color.FromArgb(60, 60, 60) };
        c.Dock = DockStyle.Fill;
        p.Controls.Add(c);
        p.Controls.Add(lab);
        t.Controls.Add(p, col, row);
    }
}
