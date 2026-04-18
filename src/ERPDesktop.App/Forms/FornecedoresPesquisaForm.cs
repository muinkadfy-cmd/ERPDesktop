using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;
using Microsoft.Extensions.DependencyInjection;

namespace ERPDesktop.App.Forms;

public partial class FornecedoresPesquisaForm : Form
{
    private readonly FornecedorAppService _svc;
    private readonly IServiceProvider _provider;

    public FornecedoresPesquisaForm(FornecedorAppService svc, IServiceProvider provider)
    {
        _svc = svc;
        _provider = provider;
        InitializeComponent();
        ErpGridStyle.Aplicar(_grid);
        ErpChrome.AplicarBarraFilha(_ts, 22);
        _st.BackColor = ErpTheme.StatusBack;
        _ts.Items.Add(Mk("Novo", BtnNovo_Click));
        _ts.Items.Add(Mk("Editar", BtnEditar_Click));
        _ts.Items.Add(Mk("Excluir", BtnExcluir_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(Mk("Atualizar", (_, _) => Carregar()));
        _ts.Items.Add(Mk("Sair", (_, _) => Close()));
        _grid.AutoGenerateColumns = false;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(FornecedorGridRow.Codigo), FillWeight = 12 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Razão social", DataPropertyName = nameof(FornecedorGridRow.RazaoSocial), FillWeight = 35 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fantasia", DataPropertyName = nameof(FornecedorGridRow.NomeFantasia), FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cidade", DataPropertyName = nameof(FornecedorGridRow.Cidade), FillWeight = 15 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "UF", DataPropertyName = nameof(FornecedorGridRow.Uf), FillWeight = 8 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Telefone", DataPropertyName = nameof(FornecedorGridRow.Telefone), FillWeight = 20 });
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) BtnEditar_Click(null!, EventArgs.Empty); };
        Shown += (_, _) => Carregar();
        KeyPreview = true;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape) Close();
            else if (e.KeyCode == Keys.F2) BtnNovo_Click(null!, EventArgs.Empty);
        };
    }

    private static ToolStripButton Mk(string t, EventHandler c) =>
        new(t, null, c) { DisplayStyle = ToolStripItemDisplayStyle.Text };

    private void Carregar()
    {
        _grid.DataSource = _svc.PesquisarGrid().ToList();
        _stMsg.Text = $"{_grid.Rows.Count} fornecedor(es).";
    }

    private long? IdSel() =>
        _grid.CurrentRow?.DataBoundItem is FornecedorGridRow r ? r.Id : null;

    private void BtnNovo_Click(object? s, EventArgs e)
    {
        var f = _provider.GetRequiredService<FornecedorCadastroForm>();
        f.Inicializar(null);
        f.ShowDialog(this);
        Carregar();
    }

    private void BtnEditar_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null)
        {
            MessageBox.Show("Selecione um fornecedor.");
            return;
        }

        var f = _provider.GetRequiredService<FornecedorCadastroForm>();
        f.Inicializar(id);
        f.ShowDialog(this);
        Carregar();
    }

    private void BtnExcluir_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null || MessageBox.Show("Excluir fornecedor?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        _svc.Excluir(id.Value);
        Carregar();
    }
}
