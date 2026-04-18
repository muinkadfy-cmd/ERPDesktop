namespace ERPDesktop.App.Forms;

partial class RelatoriosForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel _root = null!;
    private ListBox _lista = null!;
    private RichTextBox _txt = null!;
    private FlowLayoutPanel _acoes = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _root = new TableLayoutPanel();
        _lista = new ListBox();
        _txt = new RichTextBox();
        _acoes = new FlowLayoutPanel();
        SuspendLayout();
        Text = ">>> RELATÓRIOS <<<";
        MinimumSize = new Size(920, 520);
        _root.Dock = DockStyle.Fill;
        _root.ColumnCount = 2;
        _root.RowCount = 2;
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        _lista.Dock = DockStyle.Fill;
        _lista.IntegralHeight = false;
        _txt.Dock = DockStyle.Fill;
        _txt.ReadOnly = true;
        _txt.BorderStyle = BorderStyle.FixedSingle;
        _txt.Font = new Font("Consolas", 9.5f);
        _acoes.Dock = DockStyle.Fill;
        _acoes.FlowDirection = FlowDirection.LeftToRight;
        _acoes.Padding = new Padding(8, 8, 8, 0);
        _root.Controls.Add(_lista, 0, 0);
        _root.Controls.Add(_txt, 1, 0);
        _root.Controls.Add(_acoes, 0, 1);
        _root.SetColumnSpan(_acoes, 2);
        Controls.Add(_root);
        ResumeLayout(false);
    }
}
