using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

partial class MenuFinancasForm
{
    private Button _btnMovCaixa = null!;
    private Button _btnAbertura = null!;
    private Button _btnReceber = null!;
    private Button _btnPagar = null!;
    private Button _btnRecibos = null!;
    private Button _btnFluxo = null!;
    private Button _btnRelatorio = null!;
    private Button _btnPainel = null!;
    private Button _btnConsultas = null!;
    private Button _btnSair = null!;

    private void InitializeComponent()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(14),
            BackColor = ErpTheme.FormBack
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6,
            Padding = new Padding(0, 6, 14, 0)
        };
        for (var i = 0; i < 6; i++)
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66F));
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        _btnMovCaixa = Mk("Movimento de Caixa");
        _btnAbertura = Mk("Abertura / Fechamento de Caixa");
        _btnReceber = Mk("Contas a Receber");
        _btnPagar = Mk("Contas a Pagar");
        _btnRecibos = Mk("Recibos");
        _btnFluxo = Mk("Fluxo / Posição");
        _btnRelatorio = Mk("Relatórios Financeiros");
        _btnPainel = Mk("Painel Gerencial");
        _btnConsultas = Mk("Consultas e Extratos");
        _btnSair = Mk("Sair do Menu Financeiro", accent: false);

        left.Controls.Add(_btnMovCaixa, 0, 0);
        left.Controls.Add(_btnAbertura, 1, 0);
        left.Controls.Add(_btnReceber, 0, 1);
        left.Controls.Add(_btnPagar, 1, 1);
        left.Controls.Add(_btnRecibos, 0, 2);
        left.Controls.Add(_btnFluxo, 1, 2);
        left.Controls.Add(_btnRelatorio, 0, 3);
        left.Controls.Add(_btnPainel, 1, 3);
        left.Controls.Add(_btnConsultas, 0, 4);
        left.SetColumnSpan(_btnConsultas, 2);
        left.Controls.Add(_btnSair, 0, 5);
        left.SetColumnSpan(_btnSair, 2);

        var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 242, 232), Padding = new Padding(14) };
        var title = new Label
        {
            Dock = DockStyle.Top,
            Height = 52,
            Text = "MENU FINANCEIRO",
            Font = ErpTheme.UiTitle(14f),
            ForeColor = Color.FromArgb(78, 52, 28),
            TextAlign = ContentAlignment.MiddleCenter
        };
        var sub = new Label
        {
            Dock = DockStyle.Top,
            Height = 44,
            Text = "Caixa, contas e relatórios (ligação ao banco na sequência).",
            Font = ErpTheme.UiFont(8.75f),
            ForeColor = ErpTheme.TextMuted,
            TextAlign = ContentAlignment.TopCenter
        };
        var art = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(242, 232, 216) };
        art.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(190, 160, 120), 2);
            g.DrawRectangle(pen, new Rectangle(20, 20, art.Width - 41, art.Height - 41));
            using var f = ErpTheme.UiFont(10f, FontStyle.Italic);
            g.DrawString("Resumo financeiro (em breve)", f, Brushes.DimGray, 32, 40);
        };

        right.Controls.Add(art);
        right.Controls.Add(sub);
        right.Controls.Add(title);

        root.Controls.Add(left, 0, 0);
        root.Controls.Add(right, 1, 0);

        Controls.Add(root);
    }

    private Button Mk(string text, bool accent = true)
    {
        var b = new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            Margin = new Padding(5, 4, 5, 4),
            Font = ErpTheme.UiFont(9.5f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0),
            UseVisualStyleBackColor = false,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 1;
        b.FlatAppearance.BorderColor = ErpTheme.BorderSubtle;
        b.FlatAppearance.MouseOverBackColor = Color.FromArgb(246, 236, 220);
        b.FlatAppearance.MouseDownBackColor = Color.FromArgb(236, 222, 198);
        b.BackColor = accent ? Color.White : Color.FromArgb(250, 246, 240);
        b.ForeColor = Color.FromArgb(28, 32, 40);
        b.Click += Btn_Click;
        return b;
    }
}
