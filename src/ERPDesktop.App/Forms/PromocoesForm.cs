using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class PromocoesForm : Form
{
    private readonly IPromocaoRepository _repo;

    public PromocoesForm(IPromocaoRepository repo)
    {
        _repo = repo;
        InitializeComponent();
        ErpGridStyle.Aplicar(_grid);
        ErpChrome.AplicarBarraFilha(_ts, 22);
        _st.BackColor = ErpTheme.StatusBack;
        _ts.Items.Add(Mk("Nova", BtnNovo_Click));
        _ts.Items.Add(Mk("Excluir", BtnExc_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(Mk("Atualizar", (_, _) => Carregar()));
        _ts.Items.Add(Mk("Sair", (_, _) => Close()));
        _grid.AutoGenerateColumns = false;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nome", DataPropertyName = nameof(PromocaoGridRow.Nome), FillWeight = 40 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "% Desc.", DataPropertyName = nameof(PromocaoGridRow.Percentual), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Início", DataPropertyName = nameof(PromocaoGridRow.Inicio), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fim", DataPropertyName = nameof(PromocaoGridRow.Fim), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ativo", DataPropertyName = nameof(PromocaoGridRow.AtivoTexto), FillWeight = 12 });
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Shown += (_, _) => Carregar();
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
    }

    private static ToolStripButton Mk(string t, EventHandler c) =>
        new(t, null, c) { DisplayStyle = ToolStripItemDisplayStyle.Text };

    private void Carregar()
    {
        _grid.DataSource = _repo.ListarParaGrid().ToList();
        _stMsg.Text = $"{_grid.Rows.Count} promoção(ões).";
    }

    private long? IdSel() =>
        _grid.CurrentRow?.DataBoundItem is PromocaoGridRow r ? r.Id : null;

    private void BtnNovo_Click(object? s, EventArgs e)
    {
        using var dlg = new Form
        {
            Text = "Nova promoção",
            ClientSize = new Size(400, 220),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var txt = new TextBox { Location = new Point(20, 32), Width = 340 };
        var nud = new NumericUpDown { Location = new Point(20, 80), Width = 120, DecimalPlaces = 2, Maximum = 100, Minimum = 0.01m, Value = 10 };
        var d1 = new DateTimePicker { Location = new Point(20, 120), Width = 160 };
        var d2 = new DateTimePicker { Location = new Point(200, 120), Width = 160, Value = DateTime.Today.AddMonths(1) };
        dlg.Controls.Add(new Label { Text = "Nome", Location = new Point(20, 10), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "% desconto", Location = new Point(20, 58), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "Vigência", Location = new Point(20, 100), AutoSize = true });
        dlg.Controls.Add(txt);
        dlg.Controls.Add(nud);
        dlg.Controls.Add(d1);
        dlg.Controls.Add(d2);
        var ok = new Button { Text = "Salvar", DialogResult = DialogResult.OK, Location = new Point(180, 170), Width = 90 };
        var cx = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(280, 170), Width = 90 };
        dlg.Controls.Add(ok);
        dlg.Controls.Add(cx);
        dlg.AcceptButton = ok;
        dlg.CancelButton = cx;
        if (dlg.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(txt.Text))
            return;

        var agora = DateTime.UtcNow;
        var p = new Promocao
        {
            Nome = txt.Text.Trim(),
            PercentualDesconto = nud.Value,
            DataInicio = d1.Value.Date,
            DataFim = d2.Value.Date,
            Ativo = true,
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        _repo.Inserir(p);
        Carregar();
    }

    private void BtnExc_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null || MessageBox.Show("Excluir promoção?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        _repo.MarcarExcluido(id.Value);
        Carregar();
    }
}
