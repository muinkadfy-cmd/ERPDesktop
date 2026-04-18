using System.Drawing;
using System.Windows.Forms;

namespace ERPDesktop.App.Ui;

/// <summary>Layout e responsividade comuns a formulários filhos (lista + detalhe).</summary>
internal static class ErpFormLayout
{
    /// <summary>Fundo do form alinhado ao tema e escala de fonte consistente com o shell.</summary>
    public static void AplicarShellFilho(Form form)
    {
        form.AutoScaleMode = AutoScaleMode.Font;
        form.BackColor = ErpTheme.FormBack;
    }

    /// <summary>
    /// Painel direito com largura mínima estável; painel esquerdo (itens) absorve o redimensionamento horizontal.
    /// </summary>
    public static void ConfigurarDetalheDuasColunas(TableLayoutPanel pDet, int larguraMinPainelDireitoPx)
    {
        pDet.ColumnStyles.Clear();
        pDet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pDet.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, larguraMinPainelDireitoPx));
    }

    /// <summary>Split horizontal: limites mínimos e correção ao redimensionar para não “esmagar” painéis.</summary>
    public static void AnexarSplitListaDetalhe(SplitContainer split, int panel1Min = 96, int panel2Min = 220, int splitterPx = 6)
    {
        split.Panel1MinSize = panel1Min;
        split.Panel2MinSize = panel2Min;
        split.SplitterWidth = Math.Max(4, splitterPx);
        split.Resize += (_, _) => GarantirSplitDentroDosLimites(split);
    }

    /// <summary>Define a altura do painel superior uma vez (ex.: ao abrir), em fracção da altura útil.</summary>
    public static void AplicarDistanciaSplitInicial(SplitContainer split, double fracAlturaPanel1)
    {
        if (split.Height < split.Panel1MinSize + split.Panel2MinSize + split.SplitterWidth)
            return;
        var ideal = (int)Math.Round(split.Height * fracAlturaPanel1);
        var max = split.Height - split.Panel2MinSize - split.SplitterWidth;
        var min = split.Panel1MinSize;
        split.SplitterDistance = Math.Clamp(ideal, min, max);
    }

    private static void GarantirSplitDentroDosLimites(SplitContainer split)
    {
        if (!split.IsHandleCreated || split.Height < 1)
            return;
        var max = split.Height - split.Panel2MinSize - split.SplitterWidth;
        var min = split.Panel1MinSize;
        if (max < min)
            return;
        if (split.SplitterDistance > max)
            split.SplitterDistance = max;
        else if (split.SplitterDistance < min)
            split.SplitterDistance = min;
    }
}
