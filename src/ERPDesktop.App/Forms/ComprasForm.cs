using System.ComponentModel;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.Domain.Entities;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class ComprasForm : Form
{
    private readonly ICompraRepository _repo;
    private readonly IFornecedorRepository _forn;
    private readonly ProdutoAppService _produtos;
    private readonly BindingList<MvCompraItem> _itens = new();
    private bool _layoutResponsivoInicializado;

    public ComprasForm(ICompraRepository repo, IFornecedorRepository forn, ProdutoAppService produtos)
    {
        _repo = repo;
        _forn = forn;
        _produtos = produtos;
        InitializeComponent();
        ErpFormLayout.AplicarShellFilho(this);
        ErpFormLayout.AnexarSplitListaDetalhe(_splitMain);
        ErpGridStyle.Aplicar(_gridCompra);
        ErpGridStyle.Aplicar(_gridItens);
        ErpChrome.AplicarBarraFilha(_ts, 22);
        _st.BackColor = ErpTheme.StatusBack;

        _ts.Items.Add(Mk("Novo pedido", BtnNovo_Click));
        _ts.Items.Add(Mk("Alterar situação", BtnSit_Click));
        _ts.Items.Add(Mk("Dar entrada no estoque", BtnEntradaEstoque_Click));
        _ts.Items.Add(Mk("Excluir", BtnExc_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(Mk("Atualizar", (_, _) => Carregar()));
        _ts.Items.Add(Mk("Sair", (_, _) => Close()));

        _gridCompra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _gridCompra.RowHeadersVisible = false;
        _gridCompra.AutoGenerateColumns = false;
        _gridCompra.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Número", DataPropertyName = nameof(CompraGridRow.Numero), FillWeight = 15 });
        _gridCompra.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Data", DataPropertyName = nameof(CompraGridRow.DataEmissao), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _gridCompra.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fornecedor", DataPropertyName = nameof(CompraGridRow.FornecedorNome), FillWeight = 35 });
        _gridCompra.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Valor", DataPropertyName = nameof(CompraGridRow.ValorTotal), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridCompra.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Situação", DataPropertyName = nameof(CompraGridRow.Situacao), FillWeight = 22 });
        _gridCompra.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _gridCompra.SelectionChanged += (_, _) => AoTrocarCompra();

        _gridItens.AutoGenerateColumns = false;
        _gridItens.RowHeadersVisible = false;
        _gridItens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cód.", DataPropertyName = nameof(MvCompraItem.CodigoProduto), FillWeight = 14, ReadOnly = true });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descrição", DataPropertyName = nameof(MvCompraItem.Descricao), FillWeight = 40, ReadOnly = true });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qtd", DataPropertyName = nameof(MvCompraItem.Quantidade), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Unit.", DataPropertyName = nameof(MvCompraItem.ValorUnitario), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = nameof(MvCompraItem.ValorTotal), FillWeight = 16, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _gridItens.DataSource = _itens;
        _itens.ListChanged += (_, _) =>
        {
            AtualizarLblTotal();
            ReaplicarToolbarCompras();
        };

        _btnAddItem.Click += (_, _) => AdicionarItem();
        _btnRemItem.Click += (_, _) => RemoverItem();
        _btnSalvarItens.Click += (_, _) => GravarItens();
        _txtCodItem.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                AdicionarItem();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        };

        Shown += AoMostrarPrimeiraVez;
        DpiChangedAfterParent += (_, _) => AjustarColunasDetalheDpi();
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
    }

    private void AoMostrarPrimeiraVez(object? sender, EventArgs e)
    {
        if (!_layoutResponsivoInicializado)
        {
            _layoutResponsivoInicializado = true;
            AjustarColunasDetalheDpi();
            ErpFormLayout.AplicarDistanciaSplitInicial(_splitMain, 0.34);
        }

        Carregar();
    }

    private void AjustarColunasDetalheDpi()
    {
        var dpi = DeviceDpi <= 0 ? 96 : DeviceDpi;
        var larguraDireita = Math.Max(260, (int)Math.Round(288 * dpi / 96.0));
        ErpFormLayout.ConfigurarDetalheDuasColunas(_pDetItens, larguraDireita);
    }

    private void ReaplicarToolbarCompras()
    {
        var id = IdSel();
        if (id is null)
        {
            AplicarEstadoControlesPedido(null, false);
            return;
        }

        AplicarEstadoControlesPedido(_repo.ObterPorId(id.Value), true);
    }

    private static ToolStripButton Mk(string t, EventHandler c) =>
        new(t, null, c) { DisplayStyle = ToolStripItemDisplayStyle.Text };

    private long? IdSel() =>
        _gridCompra.CurrentRow?.DataBoundItem is CompraGridRow r ? r.Id : null;

    private void Carregar()
    {
        _gridCompra.DataSource = _repo.ListarParaGrid().ToList();
        _stMsg.Text = $"{_gridCompra.Rows.Count} compra(s).";
        AoTrocarCompra();
    }

    private void AoTrocarCompra()
    {
        _itens.Clear();
        var id = IdSel();
        if (id is null)
        {
            _lblCompraSel.Text = "Selecione um pedido na lista superior.";
            AplicarEstadoControlesPedido(null, false);
            AtualizarLblTotal();
            return;
        }

        var c = _repo.ObterPorId(id.Value);
        _lblCompraSel.Text = c is null ? "—" : $"{c.Numero} · {c.Situacao}";
        foreach (var r in _repo.ListarItensParaGrid(id.Value))
        {
            _itens.Add(new MvCompraItem
            {
                ProdutoId = r.ProdutoId,
                CodigoProduto = r.CodigoProduto,
                Descricao = r.Descricao,
                Quantidade = r.Quantidade,
                ValorUnitario = r.ValorUnitario
            });
        }

        AplicarEstadoControlesPedido(c, true);
        AtualizarLblTotal();
    }

    private void AplicarEstadoControlesPedido(Compra? c, bool temSelecao)
    {
        var bloqItens = !temSelecao || BloqueiaEdicaoItens(c);
        _btnAddItem.Enabled = !bloqItens;
        _btnRemItem.Enabled = !bloqItens;
        _btnSalvarItens.Enabled = !bloqItens;
        _txtCodItem.Enabled = !bloqItens;
        _numQtdItem.Enabled = !bloqItens;

        var recebOuCanc = c is not null && (
            string.Equals(c.Situacao, "Recebido", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.Situacao, "Cancelado", StringComparison.OrdinalIgnoreCase));
        // "Alterar situação" não altera pedido já recebido (evita inconsistência com estoque).
        foreach (ToolStripItem it in _ts.Items)
        {
            if (it is ToolStripButton b)
            {
                if (b.Text == "Alterar situação")
                    b.Enabled = temSelecao && !string.Equals(c?.Situacao, "Recebido", StringComparison.OrdinalIgnoreCase);
                if (b.Text == "Dar entrada no estoque")
                    b.Enabled = temSelecao && !recebOuCanc && _itens.Count > 0;
            }
        }
    }

    private static bool BloqueiaEdicaoItens(Compra? c) =>
        c is not null && (
            string.Equals(c.Situacao, "Recebido", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.Situacao, "Cancelado", StringComparison.OrdinalIgnoreCase));

    private void AtualizarLblTotal() =>
        _lblTotItens.Text = $"Total itens: {_itens.Sum(i => i.ValorTotal):C}";

    private void AdicionarItem()
    {
        if (IdSel() is null)
        {
            MessageBox.Show("Selecione um pedido de compra.", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var cod = _txtCodItem.Text.Trim();
        if (string.IsNullOrEmpty(cod))
            return;

        var p = _produtos.ObterPorCodigoOuNull(cod);
        if (p is null)
        {
            MessageBox.Show("Produto não encontrado.", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var q = _numQtdItem.Value;
        if (q <= 0)
            return;

        var existente = _itens.FirstOrDefault(i => i.ProdutoId == p.Id);
        if (existente is not null)
            existente.Quantidade += q;
        else
            _itens.Add(new MvCompraItem
            {
                ProdutoId = p.Id,
                CodigoProduto = p.Codigo,
                Descricao = p.Descricao,
                Quantidade = q,
                ValorUnitario = p.PrecoAvista
            });

        _txtCodItem.Clear();
        _txtCodItem.Focus();
        _numQtdItem.Value = 1;
    }

    private void RemoverItem()
    {
        if (_gridItens.CurrentRow?.DataBoundItem is not MvCompraItem m)
            return;
        _itens.Remove(m);
    }

    private void GravarItens()
    {
        var id = IdSel();
        if (id is null)
            return;

        var cab = _repo.ObterPorId(id.Value);
        if (BloqueiaEdicaoItens(cab))
        {
            MessageBox.Show("Não é possível alterar itens de pedido recebido ou cancelado.", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var linhas = _itens.Select(i => new ItemPedidoLinha
        {
            ProdutoId = i.ProdutoId,
            Descricao = i.Descricao,
            Quantidade = i.Quantidade,
            ValorUnitario = i.ValorUnitario
        }).ToList();

        _repo.SalvarItens(id.Value, linhas);
        Carregar();
        MessageBox.Show("Itens gravados e total do pedido atualizado.", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnNovo_Click(object? s, EventArgs e)
    {
        using var dlg = new Form
        {
            Text = "Novo pedido de compra",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            ClientSize = new Size(420, 220),
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 40), Width = 360 };
        var opts = new List<FornecedorOpt> { new(null, "(Sem fornecedor)") };
        opts.AddRange(_forn.ListarParaGrid().Select(f => new FornecedorOpt(f.Id, f.RazaoSocial)));
        cb.DataSource = opts;
        cb.DisplayMember = nameof(FornecedorOpt.Nome);
        var txtObs = new TextBox { Location = new Point(20, 110), Width = 360, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };
        dlg.Controls.Add(new Label { Text = "Fornecedor", Location = new Point(20, 14), AutoSize = true });
        dlg.Controls.Add(new Label { Text = "Observações", Location = new Point(20, 88), AutoSize = true });
        dlg.Controls.Add(cb);
        dlg.Controls.Add(txtObs);
        var ok = new Button { Text = "Criar", DialogResult = DialogResult.OK, Location = new Point(200, 170), Width = 90 };
        var cx = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(300, 170), Width = 90 };
        dlg.Controls.Add(ok);
        dlg.Controls.Add(cx);
        dlg.AcceptButton = ok;
        dlg.CancelButton = cx;
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        var agora = DateTime.UtcNow;
        var sel = (FornecedorOpt?)cb.SelectedItem;
        var c = new Compra
        {
            Numero = _repo.GerarProximoNumero(),
            FornecedorId = sel?.Id,
            DataEmissao = DateTime.Today,
            Situacao = "Pedido",
            ValorTotal = 0,
            Observacoes = txtObs.Text.Trim(),
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        var nid = _repo.Inserir(c);
        Carregar();
        foreach (DataGridViewRow row in _gridCompra.Rows)
        {
            if (row.DataBoundItem is CompraGridRow gr && gr.Id == nid)
            {
                _gridCompra.ClearSelection();
                row.Selected = true;
                break;
            }
        }
    }

    private sealed record FornecedorOpt(long? Id, string Nome);

    private void BtnSit_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null)
            return;
        var c = _repo.ObterPorId(id.Value);
        if (c is null)
            return;
        if (string.Equals(c.Situacao, "Recebido", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Pedido já recebido. A situação não pode ser alterada.", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new Form
        {
            Text = "Situação do pedido",
            ClientSize = new Size(320, 120),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 24), Width = 260 };
        cb.Items.AddRange(new object[] { "Pedido", "Parcial", "Cancelado" });
        cb.SelectedIndex = 0;
        for (var i = 0; i < cb.Items.Count; i++)
        {
            if (string.Equals(cb.Items[i]?.ToString(), c.Situacao, StringComparison.OrdinalIgnoreCase))
            {
                cb.SelectedIndex = i;
                break;
            }
        }
        dlg.Controls.Add(cb);
        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(100, 60), Width = 80 };
        var cx = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(190, 60), Width = 80 };
        dlg.Controls.Add(ok);
        dlg.Controls.Add(cx);
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        c.Situacao = cb.SelectedItem?.ToString() ?? c.Situacao;
        c.AtualizadoEm = DateTime.UtcNow;
        _repo.Atualizar(c);
        Carregar();
    }

    private void BtnEntradaEstoque_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null)
            return;

        if (MessageBox.Show(
                "Será incrementado o estoque de cada produto conforme os itens deste pedido já gravados no banco, e o pedido passará para situação Recebido. Continuar?",
                "Entrada de estoque",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var err = _repo.DarEntradaEstoque(id.Value);
        if (err is not null)
        {
            MessageBox.Show(err, "Compras", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show("Estoque atualizado e pedido marcado como Recebido.", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Carregar();
    }

    private void BtnExc_Click(object? s, EventArgs e)
    {
        var id = IdSel();
        if (id is null)
            return;
        if (_gridCompra.CurrentRow?.DataBoundItem is CompraGridRow row &&
            string.Equals(row.Situacao, "Recebido", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Não é possível excluir pedido que já teve entrada de estoque.", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("Excluir registro?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        _repo.MarcarExcluido(id.Value);
        Carregar();
    }
}

public sealed class MvCompraItem : INotifyPropertyChanged
{
    private decimal _quantidade;

    public long? ProdutoId { get; init; }
    public string CodigoProduto { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;

    public decimal ValorUnitario { get; init; }

    public decimal Quantidade
    {
        get => _quantidade;
        set
        {
            _quantidade = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantidade)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorTotal)));
        }
    }

    public decimal ValorTotal => Quantidade * ValorUnitario;

    public event PropertyChangedEventHandler? PropertyChanged;
}
