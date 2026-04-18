using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class EstoqueConsultaForm : Form
{
    private readonly ProdutoAppService _svc;

    public EstoqueConsultaForm(ProdutoAppService svc)
    {
        _svc = svc;
        InitializeComponent();
        ErpGridStyle.Aplicar(_grid);
        _st.BackColor = ErpTheme.StatusBack;
        _grid.AutoGenerateColumns = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.RowHeadersVisible = false;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(ProdutoGridRow.Codigo), FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descrição", DataPropertyName = nameof(ProdutoGridRow.Descricao), FillWeight = 40 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Marca", DataPropertyName = nameof(ProdutoGridRow.Marca), FillWeight = 16 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Estoque", DataPropertyName = nameof(ProdutoGridRow.EstoqueAtual), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mín.", DataPropertyName = nameof(ProdutoGridRow.EstoqueMinimo), FillWeight = 10, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionChanged += (_, _) => SincSel();
        _btnFiltrar.Click += (_, _) => Carregar();
        _btnAplicar.Click += (_, _) => Aplicar();
        Shown += (_, _) => Carregar();
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
    }

    private ProdutoFiltro Filtro() => new()
    {
        Codigo = _txtCod.Text,
        Descricao = _txtDesc.Text,
        Ordenacao = "Descricao",
        SomenteAtivos = null
    };

    private void Carregar()
    {
        _grid.DataSource = _svc.PesquisarParaGrid(Filtro()).ToList();
        _stMsg.Text = $"{_grid.Rows.Count} produto(s).";
        SincSel();
    }

    private void SincSel()
    {
        if (_grid.CurrentRow?.DataBoundItem is not ProdutoGridRow r)
        {
            _lblProd.Text = "Nenhum produto selecionado.";
            return;
        }

        var p = _svc.Obter(r.Id);
        if (p is null)
            return;
        _lblProd.Text = $"{p.Codigo} — {p.Descricao} (atual: {p.EstoqueAtual:N2})";
        var v = p.EstoqueAtual;
        if (v > _numNovo.Maximum) v = _numNovo.Maximum;
        if (v < _numNovo.Minimum) v = _numNovo.Minimum;
        _numNovo.Value = v;
    }

    private void Aplicar()
    {
        if (_grid.CurrentRow?.DataBoundItem is not ProdutoGridRow r)
        {
            MessageBox.Show("Selecione um produto.");
            return;
        }

        var p = _svc.Obter(r.Id);
        if (p is null)
            return;
        p.EstoqueAtual = _numNovo.Value;
        var res = _svc.Salvar(p);
        if (!res.Ok)
        {
            MessageBox.Show(res.Mensagem);
            return;
        }

        Carregar();
    }
}
