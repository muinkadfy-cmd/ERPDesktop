using System.Drawing;
using System.Windows.Forms;

namespace ERPDesktop.App.Ui;

internal static class ErpGridStyle
{
    public static void Aplicar(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = ErpTheme.GridLine;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        grid.ColumnHeadersHeight = 40;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersDefaultCellStyle.BackColor = ErpTheme.GridHeaderBack;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = ErpTheme.GridHeaderFore;
        grid.ColumnHeadersDefaultCellStyle.Font = ErpTheme.UiFont(9f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 6, 10, 6);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = ErpTheme.GridHeaderBack;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = ErpTheme.GridHeaderFore;
        grid.DefaultCellStyle.BackColor = Color.White;
        grid.DefaultCellStyle.ForeColor = Color.FromArgb(30, 34, 42);
        grid.DefaultCellStyle.Padding = new Padding(10, 6, 10, 6);
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 248);
        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        grid.AlternatingRowsDefaultCellStyle.BackColor = ErpTheme.GridAltRow;
        grid.AlternatingRowsDefaultCellStyle.Padding = new Padding(10, 6, 10, 6);
        grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 248);
        grid.RowTemplate.Height = 30;
        grid.Font = ErpTheme.UiFont();
    }
}
