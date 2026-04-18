using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;
using Microsoft.Extensions.DependencyInjection;

namespace ERPDesktop.App.Forms;

public partial class ProdutosPesquisaForm : Form
{
    private const string RodapeDica = "ESC fecha · F2 novo · F3 editar · duplo clique abre o cadastro.";

    private readonly ProdutoAppService _svc;
    private readonly IServiceProvider _provider;

    public ProdutosPesquisaForm(ProdutoAppService svc, IServiceProvider provider)
    {
        _svc = svc;
        _provider = provider;
        InitializeComponent();
        ErpGridStyle.Aplicar(_grid);
        ErpChrome.AplicarBarraFilha(_ts, 22);
        Text = "Pesquisa — Cadastro de Produtos";
        KeyPreview = true;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape) Close();
            else if (e.KeyCode == Keys.F2) AbrirNovo();
            else if (e.KeyCode == Keys.F3) AbrirEdicao();
        };
        Shown += (_, _) => Carregar();
        _grid.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
                AbrirEdicao();
        };
    }

    private ProdutoFiltro MontarFiltro() => new()
    {
        Ordenacao = _cbOrdem.SelectedItem?.ToString() ?? "Descricao",
        Codigo = _txtCodigo.Text,
        Descricao = _txtDesc.Text,
        Marca = _txtMarca.Text,
        Categoria = _txtCat.Text,
        SomenteAtivos = _chkAtivos.Checked ? true : null
    };

    private void Carregar()
    {
        try
        {
            var rows = _svc.PesquisarParaGrid(MontarFiltro());
            _grid.DataSource = rows;
            _lblRodape.Text = $"{rows.Count} produto(s) listado(s).  ·  {RodapeDica}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar produtos.\r\n\r\n{ex.Message}", "Produtos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private long? IdSelecionado() =>
        _grid.CurrentRow?.DataBoundItem is ProdutoGridRow r ? r.Id : null;

    private void AbrirNovo()
    {
        var f = _provider.GetRequiredService<ProdutoCadastroForm>();
        f.Inicializar(null);
        f.ShowDialog(this);
        Carregar();
    }

    private void AbrirEdicao()
    {
        var id = IdSelecionado();
        if (id is null)
        {
            MessageBox.Show("Selecione um produto.", "Produtos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var f = _provider.GetRequiredService<ProdutoCadastroForm>();
        f.Inicializar(id);
        f.ShowDialog(this);
        Carregar();
    }

    private void Excluir()
    {
        var id = IdSelecionado();
        if (id is null)
        {
            MessageBox.Show("Selecione um produto.", "Produtos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("Confirma exclusão (lógica) do produto?", "Produtos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var r = _svc.ExcluirSeguro(id.Value);
        if (!r.Ok)
            MessageBox.Show(r.Mensagem, "Produtos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        Carregar();
    }

    private void BtnNovo_Click(object? sender, EventArgs e) => AbrirNovo();
    private void BtnEditar_Click(object? sender, EventArgs e) => AbrirEdicao();
    private void BtnExcluir_Click(object? sender, EventArgs e) => Excluir();
    private void BtnAtualizar_Click(object? sender, EventArgs e) => Carregar();
    private void BtnSair_Click(object? sender, EventArgs e) => Close();
    private void BtnFiltrar_Click(object? sender, EventArgs e) => Carregar();
}
