namespace ERPDesktop.App.Forms;

partial class CaixaForm
{
    private System.ComponentModel.IContainer components = null!;
    private DataGridView _grid = null!;
    private StatusStrip _st = null!;
    private ToolStripStatusLabel _stMsg = null!;
    private Label _lblTit = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _grid = new DataGridView();
        _st = new StatusStrip();
        _stMsg = new ToolStripStatusLabel();
        _lblTit = new Label();
        SuspendLayout();
        Text = ">>> CAIXA — MOVIMENTOS <<<";
        MinimumSize = new Size(880, 480);
        _lblTit.Dock = DockStyle.Top;
        _lblTit.Height = 36;
        _lblTit.Padding = new Padding(12, 10, 0, 0);
        _lblTit.Text = "Últimas movimentações financeiras registradas";
        _lblTit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _grid.Dock = DockStyle.Fill;
        _st.Items.Add(_stMsg);
        _stMsg.Spring = true;
        _st.Dock = DockStyle.Bottom;
        Controls.Add(_grid);
        Controls.Add(_lblTit);
        Controls.Add(_st);
        ResumeLayout(false);
    }
}
