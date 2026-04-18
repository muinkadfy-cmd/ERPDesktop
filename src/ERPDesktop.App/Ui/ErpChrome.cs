using System.Drawing;
using System.Windows.Forms;

namespace ERPDesktop.App.Ui;

internal static class ErpChrome
{
    public static void AplicarBarraFilha(ToolStrip tool, int imageSize = 22)
    {
        tool.BackColor = ErpTheme.ToolbarBack;
        tool.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        tool.Renderer = new ToolStripProfessionalRenderer(new ErpToolbarColorTable());
        tool.ImageScalingSize = new Size(imageSize, imageSize);
        tool.GripStyle = ToolStripGripStyle.Hidden;
        tool.Padding = new Padding(6, 2, 6, 2);
    }

    public static void Aplicar(Form form, MenuStrip menu, ToolStrip tool, StatusStrip status)
    {
        form.BackColor = ErpTheme.FormBack;
        menu.BackColor = ErpTheme.MenuBack;
        menu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        menu.Renderer = new ToolStripProfessionalRenderer(new ErpMenuColorTable());

        tool.BackColor = ErpTheme.ToolbarBack;
        tool.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        tool.Renderer = new ToolStripProfessionalRenderer(new ErpToolbarColorTable());
        tool.Padding = new Padding(8, 4, 8, 4);

        status.BackColor = ErpTheme.StatusBack;
        status.SizingGrip = true;
        foreach (ToolStripItem it in status.Items)
        {
            if (it is ToolStripStatusLabel l)
            {
                l.BorderSides = ToolStripStatusLabelBorderSides.None;
                l.Margin = new Padding(6, 2, 6, 2);
            }
        }
    }

    private sealed class ErpToolbarColorTable : ProfessionalColorTable
    {
        public override Color ToolStripGradientBegin => ErpTheme.ToolbarBack;
        public override Color ToolStripGradientMiddle => ErpTheme.ToolbarBack;
        public override Color ToolStripGradientEnd => ErpTheme.ToolbarBack;
        public override Color ButtonCheckedGradientBegin => Color.FromArgb(225, 232, 244);
        public override Color ButtonCheckedGradientMiddle => Color.FromArgb(225, 232, 244);
        public override Color ButtonCheckedGradientEnd => Color.FromArgb(225, 232, 244);
        public override Color ButtonSelectedGradientBegin => Color.FromArgb(220, 228, 242);
        public override Color ButtonSelectedGradientMiddle => Color.FromArgb(220, 228, 242);
        public override Color ButtonSelectedGradientEnd => Color.FromArgb(220, 228, 242);
        public override Color ButtonPressedGradientBegin => Color.FromArgb(210, 220, 236);
        public override Color ButtonPressedGradientMiddle => Color.FromArgb(210, 220, 236);
        public override Color ButtonPressedGradientEnd => Color.FromArgb(210, 220, 236);
    }

    private sealed class ErpMenuColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => ErpTheme.MenuBack;
        public override Color MenuStripGradientEnd => ErpTheme.MenuBack;
        public override Color MenuItemSelected => Color.FromArgb(220, 228, 242);
        public override Color MenuItemBorder => ErpTheme.BorderSubtle;
        public override Color MenuBorder => ErpTheme.BorderSubtle;
    }
}
