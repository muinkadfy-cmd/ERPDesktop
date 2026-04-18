namespace ERPDesktop.App.Forms;

partial class ContasReceberForm
{
    private System.ComponentModel.IContainer components = null!;
    private ToolStrip _ts = null!;
    private DataGridView _grid = null!;
    private StatusStrip _st = null!;
    private ToolStripStatusLabel _stMsg = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _ts = new ToolStrip();
        _grid = new DataGridView();
        _st = new StatusStrip();
        _stMsg = new ToolStripStatusLabel();
        SuspendLayout();
        Text = ">>> CONTAS A RECEBER <<<";
        MinimumSize = new Size(900, 480);
        _ts.Dock = DockStyle.Top;
        _ts.GripStyle = ToolStripGripStyle.Hidden;
        _grid.Dock = DockStyle.Fill;
        _st.Items.Add(_stMsg);
        _stMsg.Spring = true;
        _st.Dock = DockStyle.Bottom;
        Controls.Add(_grid);
        Controls.Add(_ts);
        Controls.Add(_st);
        ResumeLayout(false);
    }
}
