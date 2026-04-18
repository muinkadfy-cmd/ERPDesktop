using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

partial class MenuVendasForm
{
    private Button _btnNovaVenda = null!;
    private Button _btnAlterar = null!;
    private Button _btnCancelar = null!;
    private Button _btnFinalizar = null!;
    private Button _btnRelatorio = null!;
    private Button _btnEmitir = null!;
    private Button _btnPendente = null!;
    private Button _btnFinalizada = null!;
    private Button _btnGeral = null!;
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

        _btnNovaVenda = Mk("Nova Venda");
        _btnAlterar = Mk("Alterar / Consultar");
        _btnCancelar = Mk("Cancelar Venda");
        _btnFinalizar = Mk("Finalizar Venda");
        _btnRelatorio = Mk("Relatório das Vendas");
        _btnEmitir = Mk("Emitir Venda");
        _btnPendente = Mk("Pesquisa Venda Pendente");
        _btnFinalizada = Mk("Pesquisa Venda Finalizada");
        _btnGeral = Mk("Pesquisar Venda Geral");
        _btnSair = Mk("Sair do Menu de Vendas", accent: false);

        left.Controls.Add(_btnNovaVenda, 0, 0);
        left.Controls.Add(_btnAlterar, 1, 0);
        left.Controls.Add(_btnCancelar, 0, 1);
        left.Controls.Add(_btnFinalizar, 1, 1);
        left.Controls.Add(_btnRelatorio, 0, 2);
        left.Controls.Add(_btnEmitir, 1, 2);
        left.Controls.Add(_btnPendente, 0, 3);
        left.Controls.Add(_btnFinalizada, 1, 3);
        left.Controls.Add(_btnGeral, 0, 4);
        left.SetColumnSpan(_btnGeral, 2);
        left.Controls.Add(_btnSair, 0, 5);
        left.SetColumnSpan(_btnSair, 2);

        var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(236, 241, 248), Padding = new Padding(18) };
        var title = new Label
        {
            Dock = DockStyle.Top,
            Height = 52,
            Text = "MENU DE VENDAS",
            Font = ErpTheme.UiTitle(14f),
            ForeColor = Color.FromArgb(34, 52, 78),
            TextAlign = ContentAlignment.MiddleCenter
        };
        var sub = new Label
        {
            Dock = DockStyle.Top,
            Height = 44,
            Text = "Atalhos do fluxo comercial (PDV/pedidos na sequência).",
            Font = ErpTheme.UiFont(8.75f),
            ForeColor = ErpTheme.TextMuted,
            TextAlign = ContentAlignment.TopCenter
        };
        var art = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(228, 234, 243) };
        art.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(150, 168, 190), 2);
            g.DrawRectangle(pen, new Rectangle(24, 24, art.Width - 49, art.Height - 49));
            using var f = ErpTheme.UiFont(10f, FontStyle.Italic);
            g.DrawString("Resumo / últimos pedidos (em breve)", f, new SolidBrush(ErpTheme.TextMuted), 36, 44);
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
            Padding = new Padding(14, 0, 0, 0),
            UseVisualStyleBackColor = false,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 1;
        b.FlatAppearance.BorderColor = ErpTheme.BorderSubtle;
        b.FlatAppearance.MouseOverBackColor = Color.FromArgb(228, 235, 246);
        b.FlatAppearance.MouseDownBackColor = Color.FromArgb(214, 224, 240);
        b.BackColor = accent ? Color.White : Color.FromArgb(244, 246, 250);
        b.ForeColor = Color.FromArgb(28, 32, 40);
        b.Click += Btn_Click;
        return b;
    }
}
