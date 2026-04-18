using System.Drawing;

namespace ERPDesktop.App.Ui;

/// <summary>Padrão dos modais “quadro de menu” (referência ~600×300, ex.: Menu Vendas / Menu Financeiro).</summary>
internal static class ErpModuloModal
{
    /// <summary>Tamanho único do cliente para menus de módulo (Vendas e Finanças idênticos).</summary>
    public static Size TamanhoCliente => new(600, 300);

    public static void AplicarShell(Form form, string tituloJanela)
    {
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;
        form.ClientSize = TamanhoCliente;
        form.Text = tituloJanela;
        form.StartPosition = FormStartPosition.CenterParent;
    }
}
