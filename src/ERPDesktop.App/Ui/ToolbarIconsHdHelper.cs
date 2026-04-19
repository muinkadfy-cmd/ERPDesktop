using System.Drawing;

namespace ERPDesktop.App.Ui;

internal static class ToolbarIconsHdHelper
{
    public static void Apply(ToolStrip toolStrip, Form form)
    {
        var dpi = form.IsHandleCreated ? form.DeviceDpi : 96;
        if (dpi <= 0)
            dpi = 96;
        var px = Math.Clamp((int)Math.Round(40 * dpi / 96.0), 36, 80);
        toolStrip.ImageScalingSize = new Size(px, px);

        var spec = new (ToolbarIconKind Kind, Color Accent)[]
        {
            (ToolbarIconKind.Home, Color.FromArgb(25, 118, 210)),
            (ToolbarIconKind.Clients, ErpTheme.ToolbarClientes),
            (ToolbarIconKind.Shoes, ErpTheme.ToolbarProdutos),
            (ToolbarIconKind.Pdv, ErpTheme.ToolbarVendas),
            (ToolbarIconKind.Quotes, Color.FromArgb(124, 88, 42)),
            (ToolbarIconKind.Stock, Color.FromArgb(0, 131, 143)),
            (ToolbarIconKind.Purchases, Color.FromArgb(93, 64, 55)),
            (ToolbarIconKind.Suppliers, Color.FromArgb(81, 45, 168)),
            (ToolbarIconKind.Finance, ErpTheme.ToolbarFinancas),
            (ToolbarIconKind.Receivable, Color.FromArgb(106, 27, 154)),
            (ToolbarIconKind.Payable, Color.FromArgb(183, 28, 28)),
            (ToolbarIconKind.Reports, Color.FromArgb(69, 90, 100)),
            (ToolbarIconKind.Labels, Color.FromArgb(0, 121, 107)),
            (ToolbarIconKind.Promotions, Color.FromArgb(245, 124, 0)),
            (ToolbarIconKind.Settings, Color.FromArgb(69, 90, 100)),
            (ToolbarIconKind.Exit, ErpTheme.ToolbarSair),
        };

        var buttons = toolStrip.Items.OfType<ToolStripButton>().ToList();
        for (var i = 0; i < spec.Length && i < buttons.Count; i++)
        {
            var old = buttons[i].Image;
            buttons[i].Image = ToolbarIconsHd.Create(px, spec[i].Accent, spec[i].Kind);
            old?.Dispose();
        }
    }
}
