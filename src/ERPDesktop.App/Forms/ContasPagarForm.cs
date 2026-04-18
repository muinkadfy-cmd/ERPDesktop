using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Domain.Entities;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class ContasPagarForm : Form
{
    private readonly ITituloPagarRepository _repo;
    private readonly IFornecedorRepository _forn;

    public ContasPagarForm(ITituloPagarRepository repo, IFornecedorRepository forn)
    {
        _repo = repo;
        _forn = forn;
        InitializeComponent();
        ErpGridStyle.Aplicar(_grid);
        ErpChrome.AplicarBarraFilha(_ts, 22);
        _st.BackColor = ErpTheme.StatusBack;
        _ts.Items.Add(Mk("Novo", BtnNovo_Click));
        _ts.Items.Add(Mk("Editar", BtnEditar_Click));
        _ts.Items.Add(Mk("Pagar", BtnPagar_Click));
        _ts.Items.Add(Mk("Excluir", BtnExcluir_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(Mk("Atualizar", (_, _) => Carregar()));
        _ts.Items.Add(Mk("Sair", (_, _) => Close()));
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.RowHeadersVisible = false;
        _grid.AutoGenerateColumns = false;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fornecedor", DataPropertyName = nameof(TituloPagarGridRow.FornecedorNome), FillWeight = 28 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descrição", DataPropertyName = nameof(TituloPagarGridRow.Descricao), FillWeight = 32 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Vencimento", DataPropertyName = nameof(TituloPagarGridRow.Vencimento), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Valor", DataPropertyName = nameof(TituloPagarGridRow.Valor), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Pago", DataPropertyName = nameof(TituloPagarGridRow.ValorPago), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Situação", DataPropertyName = nameof(TituloPagarGridRow.Situacao), FillWeight = 12 });
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
        _stMsg.Text = $"{_grid.Rows.Count} título(s).";
    }

    private long? IdSel() =>
        _grid.CurrentRow?.DataBoundItem is TituloPagarGridRow r ? r.Id : null;

    private void BtnNovo_Click(object? s, EventArgs e)
    {
        using var dlg = new Form
        {
            Text = "Novo título a pagar",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(420, 260),
            MaximizeBox = false,
            MinimizeBox = false
        };
        var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 40), Width = 360 };
        cb.DisplayMember = nameof(FornecedorGridRow.RazaoSocial);
        cb.ValueMember = nameof(FornecedorGridRow.Id);
        cb.DataSource = _forn.ListarParaGrid().ToList();
        var txtDesc = new TextBox { Location = new Point(20, 100), Width = 360 };
        var nudVal = new NumericUpDown { Location = new Point(20, 140), Width = 120, DecimalPlaces = 2, Maximum = 9999999, Minimum = 0.01m, Value = 100 };
        var dtp = new DateTimePicker { Location = new Point(160, 138), Width = 220, Value = DateTime.Today.AddDays(10) };
        dlg.Controls.Add(new Label { Text = "Fornecedor", Location = new Point(20, 18), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "Descrição", Location = new Point(20, 80), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "Valor / vencimento", Location = new Point(20, 120), AutoSize = true });
        dlg.Controls.Add(cb);
        dlg.Controls.Add(txtDesc);
        dlg.Controls.Add(nudVal);
        dlg.Controls.Add(dtp);
        var ok = new Button { Text = "Salvar", DialogResult = DialogResult.OK, Location = new Point(200, 190), Width = 90 };
        var cx = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(300, 190), Width = 90 };
        dlg.Controls.Add(ok);
        dlg.Controls.Add(cx);
        dlg.AcceptButton = ok;
        dlg.CancelButton = cx;
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        var agora = DateTime.UtcNow;
        var t = new TituloPagar
        {
            FornecedorId = cb.SelectedValue is long id ? id : null,
            Descricao = txtDesc.Text.Trim(),
            DataEmissao = DateTime.Today,
            DataVencimento = dtp.Value.Date,
            Valor = nudVal.Value,
            ValorPago = 0,
            Situacao = "Aberto",
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        if (string.IsNullOrWhiteSpace(t.Descricao))
        {
            MessageBox.Show("Informe a descrição.");
            return;
        }

        _repo.Inserir(t);
        Carregar();
    }

    private void BtnEditar_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null)
        {
            MessageBox.Show("Selecione um título.");
            return;
        }

        var t = _repo.ObterPorId(id.Value);
        if (t is null)
            return;

        using var dlg = new Form
        {
            Text = "Editar título a pagar",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(440, 320),
            MaximizeBox = false,
            MinimizeBox = false
        };
        var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 40), Width = 380 };
        cb.DisplayMember = nameof(FornecedorGridRow.RazaoSocial);
        cb.ValueMember = nameof(FornecedorGridRow.Id);
        cb.DataSource = _forn.ListarParaGrid().ToList();
        if (t.FornecedorId.HasValue)
            cb.SelectedValue = t.FornecedorId.Value;
        else
            cb.SelectedIndex = -1;

        var txtDesc = new TextBox { Location = new Point(20, 100), Width = 380, Text = t.Descricao };
        var nudVal = new NumericUpDown { Location = new Point(20, 140), Width = 120, DecimalPlaces = 2, Maximum = 9999999, Minimum = 0.01m, Value = t.Valor < 0.01m ? 0.01m : t.Valor };
        var dtp = new DateTimePicker { Location = new Point(160, 138), Width = 240, Value = t.DataVencimento };
        var txtObs = new TextBox { Location = new Point(20, 200), Width = 380, Height = 64, Multiline = true, ScrollBars = ScrollBars.Vertical, Text = t.Observacoes };
        var lblPag = new Label { Location = new Point(20, 172), AutoSize = true, Text = $"Já pago: {t.ValorPago:C} (use Pagar para lançar pagamentos)" };

        dlg.Controls.Add(new Label { Text = "Fornecedor", Location = new Point(20, 18), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "Descrição", Location = new Point(20, 80), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "Valor / vencimento", Location = new Point(20, 120), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "Observações", Location = new Point(20, 180), AutoSize = true });
        dlg.Controls.Add(cb);
        dlg.Controls.Add(txtDesc);
        dlg.Controls.Add(nudVal);
        dlg.Controls.Add(dtp);
        dlg.Controls.Add(lblPag);
        dlg.Controls.Add(txtObs);
        var ok = new Button { Text = "Salvar", DialogResult = DialogResult.OK, Location = new Point(220, 270), Width = 90 };
        var cx = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(320, 270), Width = 90 };
        dlg.Controls.Add(ok);
        dlg.Controls.Add(cx);
        dlg.AcceptButton = ok;
        dlg.CancelButton = cx;
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        t.FornecedorId = cb.SelectedValue is long fid ? fid : null;
        t.Descricao = txtDesc.Text.Trim();
        t.DataVencimento = dtp.Value.Date;
        t.Valor = nudVal.Value;
        t.Observacoes = txtObs.Text.Trim();
        t.Situacao = t.ValorPago >= t.Valor ? "Quitado" : "Aberto";
        t.AtualizadoEm = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(t.Descricao))
        {
            MessageBox.Show("Informe a descrição.");
            return;
        }

        _repo.Atualizar(t);
        Carregar();
    }

    private void BtnPagar_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null)
        {
            MessageBox.Show("Selecione um título.");
            return;
        }

        using var dlg = new Form
        {
            Text = "Valor a pagar",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = new Size(280, 120),
            MaximizeBox = false,
            MinimizeBox = false
        };
        var n = new NumericUpDown { Location = new Point(20, 24), Width = 220, DecimalPlaces = 2, Maximum = 9999999, Minimum = 0.01m, Value = 50 };
        var bOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(60, 60), Width = 80 };
        var bCx = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(150, 60), Width = 80 };
        dlg.Controls.AddRange(new Control[] { new Label { Text = "Valor (R$)", Location = new Point(20, 6), AutoSize = true }, n, bOk, bCx });
        dlg.AcceptButton = bOk;
        dlg.CancelButton = bCx;
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;
        _repo.RegistrarPagamento(id.Value, n.Value);
        Carregar();
    }

    private void BtnExcluir_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null)
            return;
        if (_grid.CurrentRow?.DataBoundItem is TituloPagarGridRow row &&
            string.Equals(row.Situacao, "Quitado", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Não é possível excluir um título quitado.", "Contas a pagar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("Excluir título selecionado?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        _repo.MarcarExcluido(id.Value);
        Carregar();
    }
}
