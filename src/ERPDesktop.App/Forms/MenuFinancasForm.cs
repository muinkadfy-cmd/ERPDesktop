using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class MenuFinancasForm : Form
{
    public MenuFinancasForm()
    {
        InitializeComponent();
        ErpModuloModal.AplicarShell(this, ">>> MENU FINANCEIRO <<<");
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
            MessageBox.Show("Abra este menu a partir da janela principal.", "Financeiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (b == _btnReceber)
        {
            mf.AbrirModuloContasReceber();
            Close();
            return;
        }

        if (b == _btnPagar)
        {
            mf.AbrirModuloContasPagar();
            Close();
            return;
        }

        if (b == _btnMovCaixa || b == _btnAbertura)
        {
            mf.AbrirModuloCaixa();
            Close();
            return;
        }

        if (b == _btnRelatorio || b == _btnPainel || b == _btnFluxo || b == _btnConsultas)
        {
            mf.AbrirModuloRelatorios();
            Close();
            return;
        }

        MessageBox.Show(
            $"Função: {b.Text}\r\n\r\nContas a receber/pagar e caixa já estão disponíveis nos atalhos deste menu.",
            "Financeiro",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
