using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ERPDesktop.Shared.Ui;

/// <summary>Folha de ícones em grelha (ex.: 4×6). Recorta e escala para o tamanho da ToolStrip.</summary>
public static class ToolbarSpriteSheet
{
    /// <param name="cellIndex">Índice em ordem de leitura: linha a linha, esquerda → direita (0 = canto superior esquerdo).</param>
    public static Image? CropScale(Bitmap sprite, int cellIndex, int columns, int rows, int outputSize)
    {
        if (columns <= 0 || rows <= 0 || cellIndex < 0 || cellIndex >= columns * rows)
            return null;

        var col = cellIndex % columns;
        var row = cellIndex / columns;
        var cw = sprite.Width / columns;
        var ch = sprite.Height / rows;
        if (cw <= 0 || ch <= 0)
            return null;

        var srcRect = new Rectangle(col * cw, row * ch, cw, ch);
        using var cell = sprite.Clone(srcRect, sprite.PixelFormat);
        var dest = new Bitmap(outputSize, outputSize, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(dest))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(cell, new Rectangle(0, 0, outputSize, outputSize));
        }

        return dest;
    }

    public static Bitmap? LoadFromStream(Stream stream)
    {
        try
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return (Bitmap)Image.FromStream(ms);
        }
        catch
        {
            return null;
        }
    }
}
