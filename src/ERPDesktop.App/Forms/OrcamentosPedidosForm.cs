using System.ComponentModel;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.Domain.Entities;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class OrcamentosPedidosForm : Form
{
    private readonly IOrcamentoRepository _repo;
    private readonly ClienteAppService _clientes;
    private readonly ProdutoAppService _produtos;
    private readonly VendaPdvAppService _pdv;
    private readonly BindingList<MvOrcItem> _itens = new();
    private bool _layoutResponsivoInicializado;

    public OrcamentosPedidosForm(IOrcamentoRepository repo, ClienteAppService clientes, ProdutoAppService produtos, VendaPdvAppService pdv)
    {
        _repo = repo;
        _clientes = clientes;
        _produtos = produtos;
        _pdv = pdv;
        InitializeComponent();
        ErpFormLayout.AplicarShellFilho(this);
        ErpFormLayout.AnexarSplitListaDetalhe(_splitMain);
        ErpGridStyle.Aplicar(_gridOrc);
        ErpGridStyle.Aplicar(_gridItens);
        ErpChrome.AplicarBarraFilha(_ts, 22);
        _st.BackColor = ErpTheme.StatusBack;

        _ts.Items.Add(Mk("Novo", BtnNovo_Click));
        _ts.Items.Add(Mk("Alterar situação", BtnSit_Click));
        _ts.Items.Add(Mk("Faturar (venda)", BtnFaturar_Click));
        _ts.Items.Add(Mk("Excluir", BtnExc_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(Mk("Atualizar", (_, _) => CarregarOrcamentos()));
        _ts.Items.Add(Mk("Sair", (_, _) => Close()));

        _gridOrc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _gridOrc.RowHeadersVisible = false;
        _gridOrc.AutoGenerateColumns = false;
        _gridOrc.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Número", DataPropertyName = nameof(OrcamentoGridRow.Numero), FillWeight = 15 });
        _gridOrc.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Data", DataPropertyName = nameof(OrcamentoGridRow.DataEmissao), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _gridOrc.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", DataPropertyName = nameof(OrcamentoGridRow.ClienteNome), FillWeight = 35 });
        _gridOrc.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = nameof(OrcamentoGridRow.ValorTotal), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridOrc.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Situação", DataPropertyName = nameof(OrcamentoGridRow.Situacao), FillWeight = 22 });
        _gridOrc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _gridOrc.SelectionChanged += (_, _) => AoTrocarOrcamento();

        _gridItens.AutoGenerateColumns = false;
        _gridItens.RowHeadersVisible = false;
        _gridItens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cód.", DataPropertyName = nameof(MvOrcItem.CodigoProduto), FillWeight = 14, ReadOnly = true });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descrição", DataPropertyName = nameof(MvOrcItem.Descricao), FillWeight = 40, ReadOnly = true });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qtd", DataPropertyName = nameof(MvOrcItem.Quantidade), FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Unit.", DataPropertyName = nameof(MvOrcItem.ValorUnitario), FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridItens.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = nameof(MvOrcItem.ValorTotal), FillWeight = 16, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _gridItens.DataSource = _itens;
        _itens.ListChanged += (_, _) =>
        {
            AtualizarLblTotal();
            ReaplicarToolbarOrcamento();
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

        CarregarOrcamentos();
    }

    private void AjustarColunasDetalheDpi()
    {
        var dpi = DeviceDpi <= 0 ? 96 : DeviceDpi;
        var larguraDireita = Math.Max(260, (int)Math.Round(288 * dpi / 96.0));
        ErpFormLayout.ConfigurarDetalheDuasColunas(_pDetItens, larguraDireita);
    }

    private static ToolStripButton Mk(string t, EventHandler c) =>
        new(t, null, c) { DisplayStyle = ToolStripItemDisplayStyle.Text };

    private long? IdOrcSel() =>
        _gridOrc.CurrentRow?.DataBoundItem is OrcamentoGridRow r ? r.Id : null;

    private void CarregarOrcamentos()
    {
        _gridOrc.DataSource = _repo.ListarParaGrid().ToList();
        _stMsg.Text = $"{_gridOrc.Rows.Count} orçamento(s).";
        AoTrocarOrcamento();
    }

    private void AoTrocarOrcamento()
    {
        _itens.Clear();
        var id = IdOrcSel();
        if (id is null)
        {
            _lblOrcSel.Text = "Selecione um orçamento na lista superior.";
            AplicarEstadoControlesOrcamento(null, false);
            AtualizarLblTotal();
            return;
        }

        var o = _repo.ObterPorId(id.Value);
        _lblOrcSel.Text = o is null ? "—" : $"{o.Numero} · {o.Situacao}";
        foreach (var r in _repo.ListarItensParaGrid(id.Value))
        {
            _itens.Add(new MvOrcItem
            {
                ProdutoId = r.ProdutoId,
                CodigoProduto = r.CodigoProduto,
                Descricao = r.Descricao,
                Quantidade = r.Quantidade,
                ValorUnitario = r.ValorUnitario
            });
        }

        AplicarEstadoControlesOrcamento(o, true);
        AtualizarLblTotal();
    }

    private void AplicarEstadoControlesOrcamento(Orcamento? o, bool temSelecao)
    {
        var bloqItens = !temSelecao || BloqueiaEdicaoItens(o);
        _btnAddItem.Enabled = !bloqItens;
        _btnRemItem.Enabled = !bloqItens;
        _btnSalvarItens.Enabled = !bloqItens;
        _txtCodItem.Enabled = !bloqItens;
        _numQtdItem.Enabled = !bloqItens;

        var podeFaturar = temSelecao && o is not null &&
            !string.Equals(o.Situacao, "Faturado", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(o.Situacao, "Cancelado", StringComparison.OrdinalIgnoreCase) &&
            _itens.Count > 0;

        foreach (ToolStripItem it in _ts.Items)
        {
            if (it is not ToolStripButton b)
                continue;
            if (b.Text == "Alterar situação")
                b.Enabled = temSelecao && o is not null &&
                    !string.Equals(o.Situacao, "Faturado", StringComparison.OrdinalIgnoreCase);
            if (b.Text == "Faturar (venda)")
                b.Enabled = podeFaturar;
        }
    }

    private static bool BloqueiaEdicaoItens(Orcamento? o) =>
        o is not null && (
            string.Equals(o.Situacao, "Faturado", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(o.Situacao, "Cancelado", StringComparison.OrdinalIgnoreCase));

    private void ReaplicarToolbarOrcamento()
    {
        var id = IdOrcSel();
        if (id is null)
        {
            AplicarEstadoControlesOrcamento(null, false);
            return;
        }

        AplicarEstadoControlesOrcamento(_repo.ObterPorId(id.Value), true);
    }

    private void AtualizarLblTotal()
    {
        var cul = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
        _lblTotItens.Text = $"Total itens: {_itens.Sum(i => i.ValorTotal):C}";
    }

    private void AdicionarItem()
    {
        if (IdOrcSel() is null)
        {
            MessageBox.Show("Selecione um orçamento.", "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var cod = _txtCodItem.Text.Trim();
        if (string.IsNullOrEmpty(cod))
            return;

        var p = _produtos.ObterPorCodigoOuNull(cod);
        if (p is null)
        {
            MessageBox.Show("Produto não encontrado.", "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var q = _numQtdItem.Value;
        if (q <= 0)
            return;

        var existente = _itens.FirstOrDefault(i => i.ProdutoId == p.Id);
        if (existente is not null)
            existente.Quantidade += q;
        else
            _itens.Add(new MvOrcItem
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
        if (_gridItens.CurrentRow?.DataBoundItem is not MvOrcItem m)
            return;
        _itens.Remove(m);
    }

    private void GravarItens()
    {
        var id = IdOrcSel();
        if (id is null)
            return;

        var cab = _repo.ObterPorId(id.Value);
        if (BloqueiaEdicaoItens(cab))
        {
            MessageBox.Show("Não é possível alterar itens de orçamento faturado ou cancelado.", "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        CarregarOrcamentos();
        MessageBox.Show("Itens gravados e total do orçamento atualizado.", "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnNovo_Click(object? s, EventArgs e)
    {
        using var dlg = new Form
        {
            Text = "Novo orçamento",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            ClientSize = new Size(420, 220),
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 40), Width = 360 };
        var opts = new List<ClienteOpt> { new(null, "(Sem cliente)") };
        opts.AddRange(_clientes.PesquisarParaGrid(new ClienteFiltro { Ordenacao = "Nome" })
            .Select(r => new ClienteOpt(r.Id, r.NomeRazaoSocial)));
        cb.DataSource = opts;
        cb.DisplayMember = nameof(ClienteOpt.Nome);
        var txtObs = new TextBox { Location = new Point(20, 110), Width = 360, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };
        dlg.Controls.Add(new Label { Text = "Cliente", Location = new Point(20, 14), AutoSize = true });
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
        var sel = (ClienteOpt?)cb.SelectedItem;
        var o = new Orcamento
        {
            Numero = _repo.GerarProximoNumero(),
            ClienteId = sel?.Id,
            DataEmissao = DateTime.Today,
            Situacao = "Aberto",
            ValorTotal = 0,
            Observacoes = txtObs.Text.Trim(),
            CriadoEm = agora,
            AtualizadoEm = agora
        };
        var nid = _repo.Inserir(o);
        CarregarOrcamentos();
        foreach (DataGridViewRow row in _gridOrc.Rows)
        {
            if (row.DataBoundItem is OrcamentoGridRow gr && gr.Id == nid)
            {
                _gridOrc.ClearSelection();
                row.Selected = true;
                break;
            }
        }
    }

    private sealed record ClienteOpt(long? Id, string Nome);

    private void BtnFaturar_Click(object? s, EventArgs e)
    {
        var id = IdOrcSel();
        if (id is null)
            return;

        if (MessageBox.Show(
                "Será gerada uma venda finalizada com os itens deste orçamento já gravados no banco, com baixa de estoque dos produtos vinculados. O orçamento passará para situação Faturado. Continuar?",
                "Faturar orçamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var r = _pdv.FaturarOrcamento(id.Value);
        if (!r.Ok)
        {
            MessageBox.Show(r.Mensagem, "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show("Venda gerada e orçamento marcado como Faturado.", "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        CarregarOrcamentos();
    }

    private void BtnSit_Click(object? s, EventArgs e)
    {
        var id = IdOrcSel();
        if (id is null)
            return;
        var o = _repo.ObterPorId(id.Value);
        if (o is null)
            return;
        if (string.Equals(o.Situacao, "Faturado", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Orçamento já faturado. A situação não pode ser alterada manualmente.", "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new Form
        {
            Text = "Situação",
            ClientSize = new Size(320, 120),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 24), Width = 260 };
        cb.Items.AddRange(new object[] { "Aberto", "Aprovado", "Cancelado" });
        cb.SelectedIndex = 0;
        for (var i = 0; i < cb.Items.Count; i++)
        {
            if (string.Equals(cb.Items[i]?.ToString(), o.Situacao, StringComparison.OrdinalIgnoreCase))
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
        o.Situacao = cb.SelectedItem?.ToString() ?? o.Situacao;
        o.AtualizadoEm = DateTime.UtcNow;
        _repo.Atualizar(o);
        CarregarOrcamentos();
    }

    private void BtnExc_Click(object? s, EventArgs e)
    {
        var id = IdOrcSel();
        if (id is null)
            return;
        if (_gridOrc.CurrentRow?.DataBoundItem is OrcamentoGridRow row &&
            string.Equals(row.Situacao, "Faturado", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Não é possível excluir orçamento já faturado (venda já gerada).", "Orçamentos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("Excluir orçamento?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        _repo.MarcarExcluido(id.Value);
        CarregarOrcamentos();
    }
}

public sealed class MvOrcItem : INotifyPropertyChanged
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
