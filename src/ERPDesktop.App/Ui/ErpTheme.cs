using System.Drawing;

namespace ERPDesktop.App.Ui;

internal static class ErpTheme
{
    public static Color FormBack => Color.FromArgb(250, 251, 253);
    public static Color ToolbarBack => Color.FromArgb(241, 244, 249);
    public static Color MenuBack => Color.FromArgb(248, 249, 252);
    public static Color StatusBack => Color.FromArgb(236, 239, 245);
    public static Color MdiWorkspace => Color.FromArgb(232, 236, 242);
    public static Color BorderSubtle => Color.FromArgb(205, 212, 222);
    public static Color TextMuted => Color.FromArgb(90, 98, 110);

    public static Color ToolbarClientes => Color.FromArgb(52, 88, 138);
    public static Color ToolbarVendas => Color.FromArgb(44, 118, 92);
    public static Color ToolbarProdutos => Color.FromArgb(86, 72, 132);
    public static Color ToolbarFinancas => Color.FromArgb(124, 88, 42);
    public static Color ToolbarSair => Color.FromArgb(128, 48, 48);

    public static Color GridHeaderBack => Color.FromArgb(234, 238, 245);
    public static Color GridHeaderFore => Color.FromArgb(45, 52, 64);
    public static Color GridAltRow => Color.FromArgb(242, 245, 250);
    public static Color GridLine => Color.FromArgb(220, 225, 233);

    public static Font UiFont(float size = 9f, FontStyle style = FontStyle.Regular) =>
        new("Segoe UI", size, style, GraphicsUnit.Point);

    public static Font UiTitle(float size = 11f) =>
        new("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point);
}
