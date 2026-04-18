using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class EtiquetasForm : Form
{
    private readonly ProdutoAppService _produtos;

    public EtiquetasForm(ProdutoAppService produtos)
    {
        _produtos = produtos;
        InitializeComponent();
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        _st.BackColor = ErpTheme.StatusBack;
        _btnBuscar.Click += (_, _) => AtualizarPreview();
        _pnlPrev.Paint += PnlPrev_Paint;
        foreach (Control c in _acoes.Controls)
        {
            if (c is Button { Name: "btnImp" } b)
                b.Click += (_, _) => MessageBox.Show("Integração com impressora de etiquetas na próxima etapa.", "Etiquetas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (c is Button { Name: "btnPdf" } b2)
                b2.Click += (_, _) => MessageBox.Show("Exportação PDF será ligada ao motor de relatórios.", "Etiquetas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        Shown += (_, _) => _stMsg.Text = "Informe o código e visualize a prévia.";
    }

    private string _nome = "";
    private string _codigo = "";
    private decimal _preco;

    private void AtualizarPreview()
    {
        var p = _produtos.ObterPorCodigoOuNull(_txtCod.Text.Trim());
        if (p is null)
        {
            MessageBox.Show("Produto não encontrado.", "Etiquetas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _nome = p.Descricao;
        _codigo = p.Codigo;
        _preco = p.PrecoAvista;
        _pnlPrev.Invalidate();
        _stMsg.Text = $"Prévia: {_codigo}";
    }

    private void PnlPrev_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Color.White);
        using var pen = new Pen(Color.Black, 2);
        g.DrawRectangle(pen, 40, 40, _pnlPrev.Width - 81, 140);
        using var f = ErpTheme.UiTitle(11f);
        g.DrawString(_nome, f, Brushes.Black, 50, 52);
        using var f2 = ErpTheme.UiFont(10f);
        g.DrawString(_codigo, f2, Brushes.DimGray, 50, 88);
        g.DrawString(_preco.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")), ErpTheme.UiTitle(14f), Brushes.DarkGreen, 50, 118);
        using var f3 = ErpTheme.UiFont(8f);
        g.DrawString("CODE128 (simulado)", f3, Brushes.Gray, 50, 150);
    }
}
