using System.Drawing.Drawing2D;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class DashboardForm : Form
{
    private readonly DashboardAppService _dash;
    private readonly List<Panel> _cards = new();
    private IReadOnlyList<VendaPorDiaPonto> _pontosGrafico = Array.Empty<VendaPorDiaPonto>();

    public DashboardForm(DashboardAppService dash)
    {
        _dash = dash;
        InitializeComponent();
        ErpGridStyle.Aplicar(_gridVendas);
        ErpGridStyle.Aplicar(_gridEstoque);
        _statusDash.BackColor = ErpTheme.StatusBack;
        _statusDash.SizingGrip = false;
        _pnlGrafico.Paint += PnlGrafico_Paint;
        _pnlGrafico.Resize += (_, _) => _pnlGrafico.Invalidate();
        MontarCards();
        MontarBotoesAtalho();
        Shown += (_, _) => AtualizarTudo();
        KeyPreview = true;
        KeyDown += DashboardForm_KeyDown;
    }

    private void DashboardForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F2)
            AbrirPdv();
        else if (e.KeyCode == Keys.F3)
            AbrirProdutos();
        else if (e.KeyCode == Keys.F4)
            AbrirClientes();
    }

    private void MontarCards()
    {
        _painelCards.Controls.Clear();
        _cards.Clear();
        void AddCard(string titulo, string subtitulo, Color borda, Color fundo)
        {
            var p = new Panel
            {
                Width = 168,
                Height = 96,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = fundo
            };
            p.Paint += (_, a) =>
            {
                using var br = new SolidBrush(borda);
                a.Graphics.FillRectangle(br, 0, 0, 5, p.Height);
                using var pen = new Pen(ErpTheme.BorderSubtle);
                a.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
            var lt = new Label
            {
                Text = titulo,
                Location = new Point(14, 10),
                AutoSize = true,
                Font = ErpTheme.UiFont(8.25f),
                ForeColor = ErpTheme.TextMuted
            };
            var lv = new Label
            {
                Name = "Valor",
                Text = "—",
                Location = new Point(14, 34),
                AutoSize = true,
                Font = ErpTheme.UiTitle(14f),
                ForeColor = Color.FromArgb(30, 36, 48)
            };
            var ls = new Label
            {
                Text = subtitulo,
                Location = new Point(14, 68),
                AutoSize = true,
                Font = ErpTheme.UiFont(8f),
                ForeColor = ErpTheme.TextMuted
            };
            p.Controls.Add(lt);
            p.Controls.Add(lv);
            p.Controls.Add(ls);
            _painelCards.Controls.Add(p);
            _cards.Add(p);
        }

        AddCard("Vendas hoje", "Total faturado", Color.FromArgb(46, 125, 50), Color.FromArgb(245, 252, 246));
        AddCard("Pares / unid.", "Volume do dia", Color.FromArgb(25, 118, 210), Color.FromArgb(245, 249, 255));
        AddCard("Ticket médio", "Por cupom", Color.FromArgb(200, 150, 40), Color.FromArgb(255, 252, 240));
        AddCard("Estoque baixo", "SKUs críticos", Color.FromArgb(198, 40, 40), Color.FromArgb(255, 245, 245));
        AddCard("Contas a receber", "Em aberto", Color.FromArgb(106, 27, 154), Color.FromArgb(250, 245, 255));
    }

    private void SetCardValor(int index, string valor)
    {
        if (index < 0 || index >= _cards.Count)
            return;
        foreach (Control c in _cards[index].Controls)
        {
            if (c is Label { Name: "Valor" } l)
                l.Text = valor;
        }
    }

    private void MontarBotoesAtalho()
    {
        void Btn(string texto, Color fundo, Action act)
        {
            var b = new Button
            {
                Text = texto,
                AutoSize = true,
                Padding = new Padding(18, 8, 18, 8),
                Margin = new Padding(0, 0, 8, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = fundo,
                ForeColor = Color.White,
                Font = ErpTheme.UiFont(9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(60, 70, 90);
            b.Click += (_, _) => act();
            _painelAcoes.Controls.Add(b);
        }

        Btn("Nova venda (F2)", Color.FromArgb(25, 118, 210), AbrirPdv);
        Btn("PDV balcão (F3)", Color.FromArgb(0, 137, 123), AbrirPdv);
        Btn("Produtos (F4)", Color.FromArgb(86, 72, 132), AbrirProdutos);
        Btn("Clientes (F5)", Color.FromArgb(52, 88, 138), AbrirClientes);
        Btn("Estoque (F6)", Color.FromArgb(0, 131, 143), AbrirEstoque);
        Btn("Orçamentos", Color.FromArgb(124, 88, 42), AbrirOrcamentos);
    }

    private void AbrirPdv()
    {
        if (MdiParent is MainForm m)
            m.AbrirModuloPdv();
    }

    private void AbrirProdutos()
    {
        if (MdiParent is MainForm m)
            m.AbrirModuloProdutos();
    }

    private void AbrirClientes()
    {
        if (MdiParent is MainForm m)
            m.AbrirModuloClientes();
    }

    private void AbrirEstoque()
    {
        if (MdiParent is MainForm m)
            m.AbrirModuloEstoque();
    }

    private void AbrirOrcamentos()
    {
        if (MdiParent is MainForm m)
            m.AbrirModuloOrcamentos();
    }

    private void AtualizarTudo()
    {
        try
        {
            var r = _dash.ResumoHoje();
            SetCardValor(0, r.VendasHoje.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")));
            SetCardValor(1, r.ParesVendidosHoje.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")));
            SetCardValor(2, r.TicketMedioHoje.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")));
            SetCardValor(3, r.SkuEstoqueBaixo.ToString());
            SetCardValor(4, r.ContasReceberAberto.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")));

            _pontosGrafico = _dash.Vendas7Dias();
            _pnlGrafico.Invalidate();

            _gridVendas.AutoGenerateColumns = false;
            _gridVendas.Columns.Clear();
            _gridVendas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Número", DataPropertyName = nameof(VendaListagemRow.Numero), FillWeight = 20 });
            _gridVendas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Data", DataPropertyName = nameof(VendaListagemRow.DataEmissao), FillWeight = 18, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            _gridVendas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", DataPropertyName = nameof(VendaListagemRow.ClienteNome), FillWeight = 35 });
            _gridVendas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = nameof(VendaListagemRow.ValorTotal), FillWeight = 15, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            _gridVendas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Situação", DataPropertyName = nameof(VendaListagemRow.Situacao), FillWeight = 12 });
            _gridVendas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridVendas.DataSource = _dash.UltimasVendas(12).ToList();

            _gridEstoque.AutoGenerateColumns = false;
            _gridEstoque.Columns.Clear();
            _gridEstoque.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(EstoqueBaixoRow.Codigo), FillWeight = 18 });
            _gridEstoque.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descrição", DataPropertyName = nameof(EstoqueBaixoRow.Descricao), FillWeight = 50 });
            _gridEstoque.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Atual", DataPropertyName = nameof(EstoqueBaixoRow.EstoqueAtual), FillWeight = 16, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            _gridEstoque.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mín.", DataPropertyName = nameof(EstoqueBaixoRow.EstoqueMinimo), FillWeight = 16, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            _gridEstoque.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridEstoque.DataSource = _dash.EstoqueBaixo(12).ToList();

            _statusMsg.Text = $"Atualizado em {DateTime.Now:dd/MM/yyyy HH:mm} — últimos 7 dias no gráfico.";
        }
        catch (Exception ex)
        {
            _statusMsg.Text = "Erro ao carregar painel.";
            MessageBox.Show(ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void PnlGrafico_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.White);
        var titulo = "Vendas nos últimos 7 dias (finalizadas)";
        using var ft = ErpTheme.UiTitle(10f);
        g.DrawString(titulo, ft, Brushes.DimGray, 4, 4);

        if (_pontosGrafico.Count == 0)
            return;

        var max = _pontosGrafico.Max(p => p.Total);
        if (max <= 0)
            max = 1;

        var rect = new RectangleF(40, 32, _pnlGrafico.Width - 56, _pnlGrafico.Height - 52);
        using var penEixo = new Pen(Color.FromArgb(200, 205, 215), 1);
        g.DrawLine(penEixo, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
        g.DrawLine(penEixo, rect.Left, rect.Top, rect.Left, rect.Bottom);

        var n = _pontosGrafico.Count;
        var step = n > 1 ? rect.Width / (n - 1) : rect.Width;
        var pts = new List<PointF>();
        for (var i = 0; i < n; i++)
        {
            var v = (double)_pontosGrafico[i].Total;
            var x = rect.Left + i * step;
            var y = (float)(rect.Bottom - v / (double)max * rect.Height);
            pts.Add(new PointF(x, y));
            using var ft2 = ErpTheme.UiFont(7.5f);
            g.DrawString(_pontosGrafico[i].Dia.ToString("dd/MM"), ft2, Brushes.Gray, x - 12, rect.Bottom + 2);
        }

        if (pts.Count >= 2)
            using (var penLinha = new Pen(Color.FromArgb(25, 118, 210), 2))
                g.DrawLines(penLinha, pts.ToArray());
        foreach (var pt in pts)
            g.FillEllipse(new SolidBrush(Color.FromArgb(25, 118, 210)), pt.X - 4, pt.Y - 4, 8, 8);
    }
}
