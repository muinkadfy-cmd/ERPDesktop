using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ERPDesktop.App.Ui;

internal enum ToolbarIconKind
{
    Home,
    Clients,
    Shoes,
    Pdv,
    Quotes,
    Stock,
    Purchases,
    Suppliers,
    Finance,
    Receivable,
    Payable,
    Reports,
    Labels,
    Promotions,
    Settings,
    Exit
}

/// <summary>Ícones da barra desenhados em alta resolução (estilo moderno / Fluent).</summary>
internal static class ToolbarIconsHd
{
    private const int HiRes = 128;

    public static Bitmap Create(int displayPx, Color accent, ToolbarIconKind kind)
    {
        displayPx = Math.Max(24, displayPx);
        using var hi = new Bitmap(HiRes, HiRes, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(hi))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.Transparent);
            DrawPlate(g, HiRes, accent);
            DrawGlyph(g, HiRes, kind);
        }

        var final = new Bitmap(displayPx, displayPx, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(final))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(hi, new Rectangle(0, 0, displayPx, displayPx));
        }

        return final;
    }

    private static void DrawPlate(Graphics g, int s, Color accent)
    {
        var pad = s * 0.08f;
        var box = new RectangleF(pad, pad, s - pad * 2, s - pad * 2);
        var r = s * 0.18f;
        using var path = RoundedRect(box, r);
        var top = Blend(accent, Color.White, 0.22f);
        var bot = Blend(accent, Color.Black, 0.12f);
        using var br = new LinearGradientBrush(box, top, bot, LinearGradientMode.Vertical);
        g.FillPath(br, path);
        using var hi = new SolidBrush(Color.FromArgb(55, Color.White));
        var hiBox = new RectangleF(box.X + box.Width * 0.12f, box.Y + box.Height * 0.08f, box.Width * 0.76f, box.Height * 0.28f);
        using var hiPath = RoundedRect(hiBox, r * 0.6f);
        g.FillPath(hi, hiPath);
        using var edge = new Pen(Color.FromArgb(90, Color.White), Math.Max(1f, s / 64f))
        {
            LineJoin = LineJoin.Round
        };
        g.DrawPath(edge, path);
        using var shade = new Pen(Color.FromArgb(45, 0, 0, 0), Math.Max(1f, s / 80f));
        using var pathSh = RoundedRect(new RectangleF(box.X + 1, box.Y + 1, box.Width, box.Height), r);
        g.DrawPath(shade, pathSh);
    }

    private static void DrawGlyph(Graphics g, int s, ToolbarIconKind kind)
    {
        var w = s * 0.52f;
        var cx = s / 2f;
        var cy = s / 2f + s * 0.02f;
        using var pen = new Pen(Color.White, Math.Max(2.2f, s / 28f))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };
        using var fill = new SolidBrush(Color.White);

        switch (kind)
        {
            case ToolbarIconKind.Home:
                DrawBars(g, cx, cy, w, fill);
                break;
            case ToolbarIconKind.Clients:
                DrawPeople(g, cx, cy, w, fill, pen);
                break;
            case ToolbarIconKind.Shoes:
                DrawShoe(g, cx, cy, w, pen);
                break;
            case ToolbarIconKind.Pdv:
                DrawTerminal(g, cx, cy, w, fill, pen);
                break;
            case ToolbarIconKind.Quotes:
                DrawClipboard(g, cx, cy, w, fill, pen);
                break;
            case ToolbarIconKind.Stock:
                DrawBarcode(g, cx, cy, w, fill);
                break;
            case ToolbarIconKind.Purchases:
                DrawCart(g, cx, cy, w, pen);
                break;
            case ToolbarIconKind.Suppliers:
                DrawHandshake(g, cx, cy, w, pen);
                break;
            case ToolbarIconKind.Finance:
                DrawCoins(g, cx, cy, w, fill, pen);
                break;
            case ToolbarIconKind.Receivable:
                DrawInArrow(g, cx, cy, w, pen, fill);
                break;
            case ToolbarIconKind.Payable:
                DrawOutArrow(g, cx, cy, w, pen, fill);
                break;
            case ToolbarIconKind.Reports:
                DrawReport(g, cx, cy, w, fill, pen);
                break;
            case ToolbarIconKind.Labels:
                DrawTag(g, cx, cy, w, pen, fill);
                break;
            case ToolbarIconKind.Promotions:
                DrawPercent(g, cx, cy, w, pen);
                break;
            case ToolbarIconKind.Settings:
                DrawGear(g, cx, cy, w * 0.85f, pen);
                break;
            case ToolbarIconKind.Exit:
                DrawDoor(g, cx, cy, w, pen);
                break;
        }
    }

    private static void DrawBars(Graphics g, float cx, float cy, float w, Brush fill)
    {
        var bw = w * 0.16f;
        var gap = w * 0.08f;
        var h3 = w * 0.35f;
        var h2 = w * 0.5f;
        var h1 = w * 0.65f;
        var baseY = cy + w * 0.28f;
        g.FillRoundedRect(fill, cx - bw * 1.5f - gap - bw, baseY - h1, bw, h1, bw * 0.35f);
        g.FillRoundedRect(fill, cx - bw * 0.5f, baseY - h2, bw, h2, bw * 0.35f);
        g.FillRoundedRect(fill, cx + bw * 0.5f + gap, baseY - h3, bw, h3, bw * 0.35f);
    }

    private static void DrawPeople(Graphics g, float cx, float cy, float w, Brush fill, Pen pen)
    {
        var r = w * 0.14f;
        g.FillEllipse(fill, cx - w * 0.26f - r, cy - w * 0.22f, r * 2, r * 2);
        g.FillEllipse(fill, cx + w * 0.06f - r, cy - w * 0.22f, r * 2, r * 2);
        using var arc = new GraphicsPath();
        arc.AddArc(cx - w * 0.42f, cy - w * 0.02f, w * 0.84f, w * 0.55f, 0, 180);
        g.FillPath(fill, arc);
        g.DrawEllipse(pen, cx - w * 0.26f - r, cy - w * 0.22f, r * 2, r * 2);
        g.DrawEllipse(pen, cx + w * 0.06f - r, cy - w * 0.22f, r * 2, r * 2);
    }

    private static void DrawShoe(Graphics g, float cx, float cy, float w, Pen pen)
    {
        using var path = new GraphicsPath();
        var x0 = cx - w * 0.38f;
        var y0 = cy - w * 0.08f;
        path.AddBezier(
            new PointF(x0, y0),
            new PointF(cx - w * 0.1f, cy - w * 0.32f),
            new PointF(cx + w * 0.28f, cy - w * 0.18f),
            new PointF(cx + w * 0.38f, cy + w * 0.12f));
        path.AddLine(cx + w * 0.38f, cy + w * 0.12f, cx + w * 0.32f, cy + w * 0.22f);
        path.AddBezier(
            new PointF(cx + w * 0.32f, cy + w * 0.22f),
            new PointF(cx + w * 0.05f, cy + w * 0.28f),
            new PointF(cx - w * 0.32f, cy + w * 0.2f),
            new PointF(x0, y0 + w * 0.08f));
        path.CloseFigure();
        g.DrawPath(pen, path);
    }

    private static void DrawTerminal(Graphics g, float cx, float cy, float w, Brush fill, Pen pen)
    {
        var rw = w * 0.42f;
        var rh = w * 0.48f;
        var x = cx - rw / 2;
        var y = cy - rh / 2 - w * 0.06f;
        g.FillRoundedRect(fill, x, y, rw, rh * 0.55f, w * 0.06f);
        g.DrawRoundedRect(pen, x, y, rw, rh * 0.55f, w * 0.06f);
        g.FillRoundedRect(fill, x + rw * 0.08f, y + rh * 0.58f, rw * 0.84f, rh * 0.38f, w * 0.05f);
        g.DrawRoundedRect(pen, x + rw * 0.08f, y + rh * 0.58f, rw * 0.84f, rh * 0.38f, w * 0.05f);
        g.FillEllipse(fill, cx - w * 0.06f, y + rh * 0.72f, w * 0.12f, w * 0.12f);
    }

    private static void DrawClipboard(Graphics g, float cx, float cy, float w, Brush fill, Pen pen)
    {
        var cw = w * 0.38f;
        var ch = w * 0.52f;
        var x = cx - cw / 2;
        var y = cy - ch / 2;
        g.FillRoundedRect(fill, x, y, cw, ch, w * 0.06f);
        g.DrawRoundedRect(pen, x, y, cw, ch, w * 0.06f);
        g.FillRoundedRect(fill, cx - cw * 0.18f, y - w * 0.08f, cw * 0.36f, w * 0.12f, w * 0.04f);
        var ly = y + w * 0.14f;
        var lw = cw * 0.62f;
        var lx = cx - lw / 2;
        using var ln = new Pen(Color.FromArgb(200, 255, 255, 255), w * 0.035f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
        g.DrawLine(ln, lx, ly, lx + lw, ly);
        g.DrawLine(ln, lx, ly + w * 0.1f, lx + lw * 0.78f, ly + w * 0.1f);
        g.DrawLine(ln, lx, ly + w * 0.2f, lx + lw * 0.55f, ly + w * 0.2f);
    }

    private static void DrawBarcode(Graphics g, float cx, float cy, float w, Brush fill)
    {
        var x0 = cx - w * 0.32f;
        var y0 = cy - w * 0.28f;
        var h = w * 0.56f;
        var rnd = new[] { 0.06f, 0.1f, 0.05f, 0.12f, 0.07f, 0.09f, 0.05f, 0.11f, 0.06f };
        var xp = x0;
        foreach (var t in rnd)
        {
            var bw = w * t;
            g.FillRoundedRect(fill, xp, y0, bw, h, bw * 0.15f);
            xp += bw + w * 0.02f;
        }
    }

    private static void DrawCart(Graphics g, float cx, float cy, float w, Pen pen)
    {
        var bx = cx - w * 0.28f;
        var by = cy - w * 0.02f;
        var bw = w * 0.56f;
        var bh = w * 0.32f;
        using var body = new GraphicsPath();
        body.AddLine(bx, by + bh * 0.2f, bx + bw * 0.12f, by);
        body.AddLine(bx + bw * 0.88f, by, bx + bw, by + bh * 0.2f);
        body.AddLine(bx + bw, by + bh, bx, by + bh);
        body.CloseFigure();
        g.DrawPath(pen, body);
        g.DrawArc(pen, bx + bw * 0.18f, by - w * 0.2f, bw * 0.64f, w * 0.28f, 0, 180);
        g.FillEllipse(Brushes.White, bx + w * 0.06f, by + bh - w * 0.02f, w * 0.12f, w * 0.12f);
        g.DrawEllipse(pen, bx + w * 0.06f, by + bh - w * 0.02f, w * 0.12f, w * 0.12f);
        g.FillEllipse(Brushes.White, bx + bw - w * 0.2f, by + bh - w * 0.02f, w * 0.12f, w * 0.12f);
        g.DrawEllipse(pen, bx + bw - w * 0.2f, by + bh - w * 0.02f, w * 0.12f, w * 0.12f);
    }

    private static void DrawHandshake(Graphics g, float cx, float cy, float w, Pen pen)
    {
        var pen2 = new Pen(pen.Color, pen.Width * 0.9f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
        g.DrawBezier(pen2,
            cx - w * 0.35f, cy + w * 0.12f,
            cx - w * 0.1f, cy - w * 0.28f,
            cx + w * 0.05f, cy + w * 0.22f,
            cx + w * 0.38f, cy - w * 0.08f);
        g.DrawBezier(pen2,
            cx + w * 0.35f, cy + w * 0.12f,
            cx + w * 0.1f, cy - w * 0.28f,
            cx - w * 0.05f, cy + w * 0.22f,
            cx - w * 0.38f, cy - w * 0.08f);
        pen2.Dispose();
    }

    private static void DrawCoins(Graphics g, float cx, float cy, float w, Brush fill, Pen pen)
    {
        var ew = w * 0.22f;
        var eh = w * 0.1f;
        g.FillEllipse(fill, cx - w * 0.08f, cy - w * 0.18f, ew, ew);
        g.DrawEllipse(pen, cx - w * 0.08f, cy - w * 0.18f, ew, ew);
        g.FillEllipse(fill, cx - w * 0.22f, cy - w * 0.02f, ew, ew);
        g.DrawEllipse(pen, cx - w * 0.22f, cy - w * 0.02f, ew, ew);
        g.FillEllipse(fill, cx + w * 0.02f, cy + w * 0.08f, ew, ew);
        g.DrawEllipse(pen, cx + w * 0.02f, cy + w * 0.08f, ew, ew);
        g.FillRoundedRect(fill, cx - w * 0.12f, cy - w * 0.28f, w * 0.36f, eh, eh * 0.4f);
        g.DrawRoundedRect(pen, cx - w * 0.12f, cy - w * 0.28f, w * 0.36f, eh, eh * 0.4f);
    }

    private static void DrawInArrow(Graphics g, float cx, float cy, float w, Pen pen, Brush fill)
    {
        var tray = w * 0.42f;
        var x = cx - tray / 2;
        var y = cy + w * 0.08f;
        using var trayPath = new GraphicsPath();
        trayPath.AddLine(x, y, x + tray, y);
        trayPath.AddLine(x + tray, y, x + tray * 0.88f, y + w * 0.14f);
        trayPath.AddLine(x + tray * 0.88f, y + w * 0.14f, x + tray * 0.12f, y + w * 0.14f);
        trayPath.AddLine(x + tray * 0.12f, y + w * 0.14f, x, y);
        g.FillPath(fill, trayPath);
        g.DrawPath(pen, trayPath);
        g.DrawLine(pen, cx, cy - w * 0.22f, cx, cy - w * 0.02f);
        g.DrawLine(pen, cx, cy - w * 0.02f, cx - w * 0.12f, cy - w * 0.12f);
        g.DrawLine(pen, cx, cy - w * 0.02f, cx + w * 0.12f, cy - w * 0.12f);
    }

    private static void DrawOutArrow(Graphics g, float cx, float cy, float w, Pen pen, Brush fill)
    {
        var tray = w * 0.42f;
        var x = cx - tray / 2;
        var y = cy - w * 0.18f;
        using var trayPath = new GraphicsPath();
        trayPath.AddLine(x + tray * 0.12f, y, x + tray * 0.88f, y);
        trayPath.AddLine(x + tray * 0.88f, y, x + tray, y + w * 0.14f);
        trayPath.AddLine(x + tray, y + w * 0.14f, x, y + w * 0.14f);
        trayPath.AddLine(x, y + w * 0.14f, x + tray * 0.12f, y);
        g.FillPath(fill, trayPath);
        g.DrawPath(pen, trayPath);
        g.DrawLine(pen, cx, cy + w * 0.06f, cx, cy + w * 0.26f);
        g.DrawLine(pen, cx, cy + w * 0.26f, cx - w * 0.12f, cy + w * 0.16f);
        g.DrawLine(pen, cx, cy + w * 0.26f, cx + w * 0.12f, cy + w * 0.16f);
    }

    private static void DrawReport(Graphics g, float cx, float cy, float w, Brush fill, Pen pen)
    {
        var pw = w * 0.36f;
        var ph = w * 0.48f;
        var x = cx - pw / 2 - w * 0.08f;
        var y = cy - ph / 2;
        g.FillRoundedRect(fill, x, y, pw, ph, w * 0.05f);
        g.DrawRoundedRect(pen, x, y, pw, ph, w * 0.05f);
        var bx = cx + w * 0.02f;
        var by = cy - w * 0.18f;
        g.FillRoundedRect(fill, bx, by, w * 0.22f, w * 0.38f, w * 0.04f);
        g.DrawRoundedRect(pen, bx, by, w * 0.22f, w * 0.38f, w * 0.04f);
        g.FillRoundedRect(fill, bx + w * 0.04f, by + w * 0.28f, w * 0.08f, w * 0.18f, w * 0.02f);
        g.FillRoundedRect(fill, bx + w * 0.14f, by + w * 0.18f, w * 0.08f, w * 0.28f, w * 0.02f);
    }

    private static void DrawTag(Graphics g, float cx, float cy, float w, Pen pen, Brush fill)
    {
        using var path = new GraphicsPath();
        var x0 = cx - w * 0.22f;
        var y0 = cy - w * 0.28f;
        path.AddLine(x0, y0, cx + w * 0.22f, y0 - w * 0.12f);
        path.AddLine(cx + w * 0.22f, y0 - w * 0.12f, cx + w * 0.18f, cy + w * 0.26f);
        path.AddLine(cx + w * 0.18f, cy + w * 0.26f, x0 - w * 0.08f, cy + w * 0.18f);
        path.CloseFigure();
        g.FillPath(fill, path);
        g.DrawPath(pen, path);
        g.FillEllipse(Brushes.White, cx - w * 0.02f, cy - w * 0.18f, w * 0.1f, w * 0.1f);
        g.DrawEllipse(pen, cx - w * 0.02f, cy - w * 0.18f, w * 0.1f, w * 0.1f);
    }

    private static void DrawPercent(Graphics g, float cx, float cy, float w, Pen pen)
    {
        using var font = new Font("Segoe UI", w * 0.52f, FontStyle.Bold, GraphicsUnit.Pixel);
        using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString("%", font, Brushes.White, new RectangleF(cx - w * 0.4f, cy - w * 0.4f, w * 0.8f, w * 0.8f), sf);
        g.DrawArc(pen, cx - w * 0.32f, cy - w * 0.32f, w * 0.64f, w * 0.64f, -35, 110);
    }

    private static void DrawGear(Graphics g, float cx, float cy, float w, Pen pen)
    {
        var teeth = 6;
        var ro = w * 0.32f;
        var ri = w * 0.2f;
        using var path = new GraphicsPath();
        for (var i = 0; i < teeth; i++)
        {
            var a0 = (float)(i * 2 * Math.PI / teeth - Math.PI / teeth);
            var a1 = (float)(i * 2 * Math.PI / teeth);
            var a2 = (float)(i * 2 * Math.PI / teeth + Math.PI / teeth);
            path.AddLine(
                cx + MathF.Cos(a0) * ri, cy + MathF.Sin(a0) * ri,
                cx + MathF.Cos(a1) * ro, cy + MathF.Sin(a1) * ro);
            path.AddLine(
                cx + MathF.Cos(a1) * ro, cy + MathF.Sin(a1) * ro,
                cx + MathF.Cos(a2) * ri, cy + MathF.Sin(a2) * ri);
        }

        path.CloseFigure();
        g.DrawPath(pen, path);
        g.FillEllipse(Brushes.White, cx - ri * 0.75f, cy - ri * 0.75f, ri * 1.5f, ri * 1.5f);
        g.DrawEllipse(pen, cx - ri * 0.75f, cy - ri * 0.75f, ri * 1.5f, ri * 1.5f);
    }

    private static void DrawDoor(Graphics g, float cx, float cy, float w, Pen pen)
    {
        var dw = w * 0.32f;
        var dh = w * 0.5f;
        var x = cx - dw / 2 - w * 0.06f;
        var y = cy - dh / 2;
        g.DrawRectangle(pen, x, y, dw, dh);
        g.FillEllipse(Brushes.White, x + dw * 0.68f, y + dh * 0.52f, w * 0.08f, w * 0.08f);
        g.DrawEllipse(pen, x + dw * 0.68f, y + dh * 0.52f, w * 0.08f, w * 0.08f);
        g.DrawLine(pen, cx + w * 0.12f, cy - w * 0.08f, cx + w * 0.28f, cy);
        g.DrawLine(pen, cx + w * 0.28f, cy, cx + w * 0.12f, cy + w * 0.08f);
    }

    private static Color Blend(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0, 1);
        return Color.FromArgb(
            (int)(a.R + (b.R - a.R) * t),
            (int)(a.G + (b.G - a.G) * t),
            (int)(a.B + (b.B - a.B) * t));
    }

    private static GraphicsPath RoundedRect(RectangleF b, float r)
    {
        r = Math.Min(r, Math.Min(b.Width, b.Height) / 2);
        var d = r * 2;
        var p = new GraphicsPath();
        p.AddArc(b.X, b.Y, d, d, 180, 90);
        p.AddArc(b.Right - d, b.Y, d, d, 270, 90);
        p.AddArc(b.Right - d, b.Bottom - d, d, d, 0, 90);
        p.AddArc(b.X, b.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    private static void FillRoundedRect(this Graphics g, Brush b, float x, float y, float width, float height, float r)
    {
        using var path = RoundedRect(new RectangleF(x, y, width, height), r);
        g.FillPath(b, path);
    }

    private static void DrawRoundedRect(this Graphics g, Pen p, float x, float y, float width, float height, float r)
    {
        using var path = RoundedRect(new RectangleF(x, y, width, height), r);
        g.DrawPath(p, path);
    }
}
