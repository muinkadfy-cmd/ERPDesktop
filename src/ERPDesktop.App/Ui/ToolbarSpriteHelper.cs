using System.Drawing;
using ERPDesktop.Shared.Ui;

namespace ERPDesktop.App.Ui;

/// <summary>Aplica ícones da folha 4×6 à ToolStrip principal (alinhados a Início, Clientes, PDV, etc.).</summary>
internal static class ToolbarSpriteHelper
{
    private const int Columns = 4;
    private const int Rows = 6;

    /// <summary>Índices na grelha (ver descrição do asset) por ordem dos botões da barra.</summary>
    private static readonly int[] MainToolbarCells =
    {
        0,  // Início — dashboard
        1,  // Clientes
        8,  // Calçados / produtos — etiquetas de preço
        2,  // PDV — terminal
        3,  // Orçamentos — fatura/checklist
        18, // Estoque — código de barras
        4,  // Compras — carrinho
        5,  // Fornecedores — parceria / loja
        6,  // Finanças — moedas / notas
        7,  // Contas a receber — fluxo documento
        16, // Contas a pagar — gaveta / saída
        12, // Relatórios — registo / resumo
        14, // Etiquetas — impressão
        19, // Promoções — loja / retalho
        21, // Config — pasta / registos
        11, // Sair — porta
    };

    public static void Apply(ToolStrip toolStrip, Form form)
    {
        var px = ScaledIconSize(form);
        toolStrip.ImageScalingSize = new Size(px, px);

        using var sprite = LoadSpriteBitmap();
        if (sprite is null)
            return;

        var buttons = toolStrip.Items.OfType<ToolStripButton>().ToList();
        if (buttons.Count < MainToolbarCells.Length)
            return;

        for (var i = 0; i < MainToolbarCells.Length; i++)
        {
            var img = ToolbarSpriteSheet.CropScale(sprite, MainToolbarCells[i], Columns, Rows, px);
            if (img is null)
                continue;
            var old = buttons[i].Image;
            buttons[i].Image = img;
            old?.Dispose();
        }
    }

    private static int ScaledIconSize(Form form)
    {
        var dpi = form.IsHandleCreated ? form.DeviceDpi : 96;
        if (dpi <= 0)
            dpi = 96;
        return Math.Clamp((int)Math.Round(40 * dpi / 96.0), 32, 72);
    }

    private static Bitmap? LoadSpriteBitmap()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "erp_toolbar_sprite.png");
        if (!File.Exists(path))
            return null;
        try
        {
            using var fs = File.OpenRead(path);
            return ToolbarSpriteSheet.LoadFromStream(fs);
        }
        catch
        {
            return null;
        }
    }
}
