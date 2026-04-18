using System.Drawing;
using System.Drawing.Drawing2D;

namespace ERPDesktop.Shared.Ui;

public static class ToolbarIcons
{
    public static Image Create(int size, Color back, Color fore, string glyph)
    {
        var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(back);

        using var pen = new Pen(fore, Math.Max(2, size / 16f))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };

        var pad = size * 0.18f;
        var r = new RectangleF(pad, pad, size - pad * 2, size - pad * 2);

        switch (glyph)
        {
            case "users":
                g.FillEllipse(new SolidBrush(fore), r.X + r.Width * 0.08f, r.Y + r.Height * 0.22f, r.Width * 0.34f, r.Height * 0.34f);
                g.FillEllipse(new SolidBrush(fore), r.X + r.Width * 0.52f, r.Y + r.Height * 0.22f, r.Width * 0.34f, r.Height * 0.34f);
                g.FillPie(new SolidBrush(fore), r.X, r.Y + r.Height * 0.48f, r.Width, r.Height * 0.62f, 0, 180);
                break;
            case "cart":
            {
                var bx = r.X + r.Width * 0.14f;
                var by = r.Y + r.Height * 0.38f;
                var bw = r.Width * 0.72f;
                var bh = r.Height * 0.44f;
                var pts = new[]
                {
                    new PointF(bx, by + bh * 0.15f),
                    new PointF(bx + bw * 0.08f, by),
                    new PointF(bx + bw * 0.92f, by),
                    new PointF(bx + bw, by + bh * 0.15f),
                    new PointF(bx + bw, by + bh),
                    new PointF(bx, by + bh)
                };
                g.FillPolygon(new SolidBrush(Color.FromArgb(240, 245, 255)), pts);
                g.DrawPolygon(pen, pts);
                g.DrawArc(pen, bx + bw * 0.22f, by - r.Height * 0.22f, bw * 0.56f, r.Height * 0.34f, 0, 180);
                break;
            }
            case "money":
            {
                g.DrawEllipse(pen, r.X + r.Width * 0.18f, r.Y + r.Height * 0.18f, r.Width * 0.64f, r.Height * 0.64f);
                using var fontMoeda = new Font(FontFamily.GenericSansSerif, size * 0.35f, FontStyle.Bold, GraphicsUnit.Pixel);
                g.DrawString("$", fontMoeda, new SolidBrush(fore), r.X + r.Width * 0.34f, r.Y + r.Height * 0.22f);
                break;
            }
            case "box":
                g.DrawPolygon(pen, new[]
                {
                    new PointF(r.X + r.Width * 0.5f, r.Y + r.Height * 0.10f),
                    new PointF(r.X + r.Width * 0.88f, r.Y + r.Height * 0.30f),
                    new PointF(r.X + r.Width * 0.88f, r.Y + r.Height * 0.70f),
                    new PointF(r.X + r.Width * 0.5f, r.Y + r.Height * 0.90f),
                    new PointF(r.X + r.Width * 0.12f, r.Y + r.Height * 0.70f),
                    new PointF(r.X + r.Width * 0.12f, r.Y + r.Height * 0.30f),
                });
                break;
            case "chart":
                g.DrawLine(pen, r.X + r.Width * 0.12f, r.Y + r.Height * 0.82f, r.X + r.Width * 0.88f, r.Y + r.Height * 0.82f);
                g.DrawLine(pen, r.X + r.Width * 0.12f, r.Y + r.Height * 0.18f, r.X + r.Width * 0.12f, r.Y + r.Height * 0.82f);
                g.FillRectangle(new SolidBrush(fore), r.X + r.Width * 0.20f, r.Y + r.Height * 0.55f, r.Width * 0.14f, r.Height * 0.27f);
                g.FillRectangle(new SolidBrush(fore), r.X + r.Width * 0.42f, r.Y + r.Height * 0.38f, r.Width * 0.14f, r.Height * 0.44f);
                g.FillRectangle(new SolidBrush(fore), r.X + r.Width * 0.64f, r.Y + r.Height * 0.28f, r.Width * 0.14f, r.Height * 0.54f);
                break;
            case "door":
                g.DrawRectangle(pen, r.X + r.Width * 0.22f, r.Y + r.Height * 0.18f, r.Width * 0.56f, r.Height * 0.64f);
                g.FillEllipse(new SolidBrush(fore), r.X + r.Width * 0.62f, r.Y + r.Height * 0.48f, r.Width * 0.10f, r.Height * 0.10f);
                break;
            default:
            {
                using var font = new Font(FontFamily.GenericSansSerif, size * 0.42f, FontStyle.Bold, GraphicsUnit.Pixel);
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(glyph, font, new SolidBrush(fore), r, sf);
                break;
            }
        }

        return bmp;
    }
}
