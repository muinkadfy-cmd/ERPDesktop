using System.ComponentModel;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class PdvVendaBalcaoForm : Form
{
    private readonly VendaPdvAppService _pdv;
    private readonly ClienteAppService _clientes;
    private readonly BindingList<PdvLinhaUi> _linhas = new();

    public PdvVendaBalcaoForm(VendaPdvAppService pdv, ClienteAppService clientes)
    {
        _pdv = pdv;
        _clientes = clientes;
        InitializeComponent();
        ErpFormLayout.AplicarShellFilho(this);
        ErpGridStyle.Aplicar(_grid);
        _grid.AutoGenerateColumns = false;
        _grid.AllowUserToAddRows = false;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descrição", DataPropertyName = nameof(PdvLinhaUi.Descricao), FillWeight = 50, ReadOnly = true });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qtd", DataPropertyName = nameof(PdvLinhaUi.Quantidade), FillWeight = 15, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Unitário", DataPropertyName = nameof(PdvLinhaUi.PrecoUnitario), FillWeight = 17, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = nameof(PdvLinhaUi.TotalLinha), FillWeight = 18, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.DataSource = _linhas;
        _btnAdd.Click += (_, _) => AdicionarProduto();
        _btnFinalizar.Click += (_, _) => Finalizar();
        _txtCodigo.KeyDown += TxtCodigo_KeyDown;
        _linhas.ListChanged += (_, _) => Recalcular();
        Shown += (_, _) => CarregarClientes();
        KeyPreview = true;
        KeyDown += PdvVendaBalcaoForm_KeyDown;
        _stMsg.Text = "Pronto.";
    }

    private void PdvVendaBalcaoForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F12)
        {
            Finalizar();
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Escape)
        {
            Close();
            e.Handled = true;
        }
    }

    private void TxtCodigo_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            AdicionarProduto();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void CarregarClientes()
    {
        var rows = _clientes.PesquisarParaGrid(new ClienteFiltro { Ordenacao = "Nome" });
        _cbCliente.DisplayMember = nameof(ClienteGridRow.NomeRazaoSocial);
        _cbCliente.ValueMember = nameof(ClienteGridRow.Id);
        _cbCliente.DataSource = rows.ToList();
        _cbCliente.SelectedIndex = -1;
    }

    private void AdicionarProduto()
    {
        var cod = _txtCodigo.Text.Trim();
        if (string.IsNullOrEmpty(cod))
            return;

        var p = _pdv.BuscarProdutoPorCodigo(cod);
        if (p is null)
        {
            MessageBox.Show("Produto não encontrado ou inativo.", "PDV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var qtd = _numQtd.Value;
        if (qtd <= 0)
        {
            MessageBox.Show("Quantidade inválida.", "PDV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var existente = _linhas.FirstOrDefault(l => l.ProdutoId == p.Id);
        if (existente is not null)
            existente.Quantidade += qtd;
        else
            _linhas.Add(new PdvLinhaUi
            {
                ProdutoId = p.Id,
                Descricao = p.Descricao,
                Quantidade = qtd,
                PrecoUnitario = p.PrecoAvista
            });

        _txtCodigo.Clear();
        _txtCodigo.Focus();
        _numQtd.Value = 1;
    }

    private void Recalcular()
    {
        var sub = _linhas.Sum(l => l.TotalLinha);
        var cul = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
        _lblSub.Text = sub.ToString("C", cul);
        _lblTotal.Text = sub.ToString("C", cul);
    }

    private void Finalizar()
    {
        if (_linhas.Count == 0)
        {
            MessageBox.Show("Adicione itens ao cupom.", "PDV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var cul = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
        var total = _linhas.Sum(l => l.TotalLinha);
        if (MessageBox.Show(
                $"Confirmar finalização da venda em {total.ToString("C", cul)}?\r\nO estoque será atualizado.",
                "PDV",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        long? clienteId = _cbCliente.SelectedValue is long id ? id : null;
        var itens = _linhas.Select(l => new PdvItemLinha
        {
            ProdutoId = l.ProdutoId,
            Descricao = l.Descricao,
            Quantidade = l.Quantidade,
            PrecoUnitario = l.PrecoUnitario
        }).ToList();

        var r = _pdv.FinalizarBalcao(clienteId, null, itens);
        if (!r.Ok)
        {
            MessageBox.Show(r.Mensagem, "PDV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show("Venda finalizada e estoque atualizado.", "PDV", MessageBoxButtons.OK, MessageBoxIcon.Information);
        _linhas.Clear();
        Recalcular();
    }
}

public sealed class PdvLinhaUi : INotifyPropertyChanged
{
    private decimal _quantidade;

    public long ProdutoId { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public decimal PrecoUnitario { get; init; }

    public decimal Quantidade
    {
        get => _quantidade;
        set
        {
            _quantidade = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantidade)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalLinha)));
        }
    }

    public decimal TotalLinha => Quantidade * PrecoUnitario;

    public event PropertyChangedEventHandler? PropertyChanged;
}
