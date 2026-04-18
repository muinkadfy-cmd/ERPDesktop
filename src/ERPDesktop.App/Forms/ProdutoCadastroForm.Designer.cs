#nullable enable
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

partial class ProdutoCadastroForm
{
    private Label _lblId = null!;
    private TextBox _txtCodigo = null!;
    private TextBox _txtDesc = null!;
    private ComboBox _cbUnidade = null!;
    private NumericUpDown _numAvista = null!;
    private NumericUpDown _numAprazo = null!;
    private NumericUpDown _numEst = null!;
    private NumericUpDown _numEstMin = null!;
    private TextBox _txtCat = null!;
    private TextBox _txtMarca = null!;
    private TextBox _txtRef = null!;
    private CheckBox _chkAtivo = null!;

    private void InitializeComponent()
    {
        SuspendLayout();

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(14)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var gb = new GroupBox
        {
            Text = "Dados do produto",
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            Font = ErpTheme.UiFont(8.5f, FontStyle.Bold),
            ForeColor = ErpTheme.TextMuted
        };

        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7 };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        for (var i = 0; i < 7; i++)
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

        _lblId = new Label { AutoSize = true, Font = ErpTheme.UiFont(10f, FontStyle.Bold) };
        _txtCodigo = new TextBox { BorderStyle = BorderStyle.FixedSingle };
        _txtDesc = new TextBox { BorderStyle = BorderStyle.FixedSingle };
        _cbUnidade = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        _cbUnidade.Items.AddRange(new object[] { "UN", "PC", "CX", "KG", "MT" });
        _numAvista = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };
        _numAprazo = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };
        _numEst = new NumericUpDown { DecimalPlaces = 2, Maximum = 100000000, Minimum = 0 };
        _numEstMin = new NumericUpDown { DecimalPlaces = 2, Maximum = 100000000, Minimum = 0 };
        _txtCat = new TextBox { BorderStyle = BorderStyle.FixedSingle };
        _txtMarca = new TextBox { BorderStyle = BorderStyle.FixedSingle };
        _txtRef = new TextBox { BorderStyle = BorderStyle.FixedSingle };
        _chkAtivo = new CheckBox { Text = "Ativo", AutoSize = true, Margin = new Padding(6, 10, 6, 0) };

        void L(int col, int row, string titulo, Control c)
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var lab = new Label { Text = titulo, Dock = DockStyle.Top, AutoSize = true, ForeColor = ErpTheme.TextMuted };
            c.Dock = DockStyle.Fill;
            p.Controls.Add(c);
            p.Controls.Add(lab);
            t.Controls.Add(p, col, row);
        }

        L(0, 0, "Registro interno", _lblId);
        L(1, 0, "Código", _txtCodigo);
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var lab = new Label { Text = "Descrição", Dock = DockStyle.Top, AutoSize = true, ForeColor = ErpTheme.TextMuted };
            _txtDesc.Dock = DockStyle.Fill;
            p.Controls.Add(_txtDesc);
            p.Controls.Add(lab);
            t.Controls.Add(p, 0, 1);
            t.SetColumnSpan(p, 2);
        }
        L(0, 2, "Unidade", _cbUnidade);
        L(1, 2, "Ativo", _chkAtivo);
        L(0, 3, "Preço à vista", _numAvista);
        L(1, 3, "Preço a prazo", _numAprazo);
        L(0, 4, "Estoque atual", _numEst);
        L(1, 4, "Estoque mínimo", _numEstMin);
        L(0, 5, "Categoria", _txtCat);
        L(1, 5, "Marca", _txtMarca);
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var lab = new Label { Text = "Referência fábrica", Dock = DockStyle.Top, AutoSize = true, ForeColor = ErpTheme.TextMuted };
            _txtRef.Dock = DockStyle.Fill;
            p.Controls.Add(_txtRef);
            p.Controls.Add(lab);
            t.Controls.Add(p, 0, 6);
            t.SetColumnSpan(p, 2);
        }

        gb.Controls.Add(t);

        var side = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(8, 8, 0, 0) };
        var bSalvar = new Button { Text = "Salvar", Width = 120, Height = 40, BackColor = Color.FromArgb(44, 118, 92), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        bSalvar.FlatAppearance.BorderSize = 0;
        bSalvar.Click += BtnSalvar_Click;
        var bSair = new Button { Text = "Sair", Width = 120, Height = 36, FlatStyle = FlatStyle.Flat };
        bSair.FlatAppearance.BorderColor = ErpTheme.BorderSubtle;
        bSair.Click += BtnSair_Click;
        side.Controls.Add(bSalvar);
        side.Controls.Add(bSair);

        root.Controls.Add(gb, 0, 0);
        root.Controls.Add(side, 1, 0);

        Controls.Add(root);

        ClientSize = new Size(760, 520);
        ResumeLayout(false);
    }
}
