using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class MenuVendasForm : Form
{
    public MenuVendasForm()
    {
        InitializeComponent();
        ErpModuloModal.AplicarShell(this, ">>> MENU DE VENDAS <<<");
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
    }

    private void Btn_Click(object? sender, EventArgs e)
    {
        if (sender is not Button b)
            return;

        if (b == _btnSair)
        {
            Close();
            return;
        }

        if (Owner is not MainForm mf)
        {
            MessageBox.Show("Abra este menu a partir da janela principal.", "Vendas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (b == _btnNovaVenda || b == _btnEmitir)
        {
            mf.AbrirModuloPdv();
            Close();
            return;
        }

        if (b == _btnPendente || b == _btnFinalizada || b == _btnGeral || b == _btnRelatorio)
        {
            mf.AbrirModuloOrcamentos();
            Close();
            return;
        }

        if (b == _btnAlterar || b == _btnFinalizar)
        {
            mf.AbrirModuloPdv();
            Close();
            return;
        }

        if (b == _btnCancelar)
        {
            MessageBox.Show("Cancelamento de venda utilizará o fluxo de estorno no módulo fiscal (evolução).", "Vendas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        MessageBox.Show(
            $"Função: {b.Text}\r\n\r\nUse o PDV, Orçamentos ou as pesquisas de cadastro conectadas ao SQLite.",
            "Vendas",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
