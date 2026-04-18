#nullable enable
using ERPDesktop.Application.DTOs;
using ERPDesktop.App.Ui;
using ERPDesktop.Shared.Ui;

namespace ERPDesktop.App.Forms;

partial class ProdutosPesquisaForm
{
    private ToolStrip _ts = null!;
    private DataGridView _grid = null!;
    private TableLayoutPanel _filters = null!;
    private Label _lblRodape = null!;
    private ComboBox _cbOrdem = null!;
    private TextBox _txtCodigo = null!;
    private TextBox _txtDesc = null!;
    private TextBox _txtMarca = null!;
    private TextBox _txtCat = null!;
    private CheckBox _chkAtivos = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterParent;

        _ts = new ToolStrip
        {
            Dock = DockStyle.Top,
            GripStyle = ToolStripGripStyle.Hidden,
            ImageScalingSize = new Size(22, 22),
            Padding = new Padding(8, 4, 8, 6)
        };
        _ts.Items.Add(MkTs("Novo", ToolbarIcons.Create(22, ErpTheme.ToolbarVendas, Color.White, "box"), BtnNovo_Click));
        _ts.Items.Add(MkTs("Editar", ToolbarIcons.Create(22, ErpTheme.ToolbarClientes, Color.White, "box"), BtnEditar_Click));
        _ts.Items.Add(MkTs("Excluir", ToolbarIcons.Create(22, ErpTheme.ToolbarSair, Color.White, "box"), BtnExcluir_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(MkTs("Atualizar", ToolbarIcons.Create(22, Color.FromArgb(95, 105, 120), Color.White, "chart"), BtnAtualizar_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(MkTs("Sair", ToolbarIcons.Create(22, Color.FromArgb(95, 105, 120), Color.White, "door"), BtnSair_Click));

        _filters = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(4, 6, 4, 8),
            ColumnCount = 4,
            RowCount = 3
        };
        _filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F));
        _filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
        _filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        _filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
        _filters.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        _filters.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
        _filters.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

        _cbOrdem = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 2, 0, 0),
            Font = ErpTheme.UiFont(9f),
            MinimumSize = new Size(0, 28)
        };
        _cbOrdem.Items.AddRange(new object[] { "Descricao", "Codigo", "Marca", "Estoque" });
        _cbOrdem.SelectedIndex = 0;
        _txtCodigo = MkTxt();
        _txtDesc = MkTxt();
        _txtMarca = MkTxt();
        _txtCat = MkTxt();
        _chkAtivos = new CheckBox { Text = "Somente produtos ativos", AutoSize = true, Checked = true };

        void L(int col, int row, string t, Control c)
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 4, 10, 8) };
            var lab = new Label
            {
                Text = t,
                Dock = DockStyle.Top,
                AutoSize = true,
                ForeColor = ErpTheme.TextMuted,
                Font = ErpTheme.UiFont(8.25f),
                Margin = new Padding(0, 0, 0, 4)
            };
            c.Dock = DockStyle.Fill;
            p.Controls.Add(lab);
            p.Controls.Add(c);
            _filters.Controls.Add(p, col, row);
        }

        L(0, 0, "Ordem", _cbOrdem);
        L(1, 0, "Código", _txtCodigo);
        L(2, 0, "Descrição", _txtDesc);
        L(3, 0, "Marca", _txtMarca);
        L(0, 1, "Categoria", _txtCat);
        var pChk = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 12, 10, 6) };
        _chkAtivos.Dock = DockStyle.Left;
        pChk.Controls.Add(_chkAtivos);
        _filters.Controls.Add(pChk, 1, 1);
        _filters.SetColumnSpan(pChk, 3);
        var btn = new Button
        {
            Text = "Aplicar filtros",
            Dock = DockStyle.Fill,
            Margin = new Padding(10, 4, 10, 4),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(245, 248, 255),
            ForeColor = Color.FromArgb(25, 55, 95),
            Font = ErpTheme.UiFont(9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderColor = Color.FromArgb(25, 118, 210);
        btn.Click += BtnFiltrar_Click;
        var pBtn = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 2, 6, 2) };
        pBtn.Controls.Add(btn);
        _filters.Controls.Add(pBtn, 0, 2);
        _filters.SetColumnSpan(pBtn, 4);

        _lblRodape = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 34,
            Padding = new Padding(14, 8, 14, 8),
            ForeColor = ErpTheme.TextMuted,
            BackColor = ErpTheme.StatusBack,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = ErpTheme.UiFont(8.25f),
            Text = "Carregando…"
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoGenerateColumns = false
        };
        _grid.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.Codigo), HeaderText = "Código", FillWeight = 12, MinimumWidth = 90 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.Descricao), HeaderText = "Descrição", FillWeight = 35, MinimumWidth = 200 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.Unidade), HeaderText = "UN", FillWeight = 6, MinimumWidth = 50 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.PrecoAvista), HeaderText = "À vista", FillWeight = 10, MinimumWidth = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.PrecoAprazo), HeaderText = "A prazo", FillWeight = 10, MinimumWidth = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.EstoqueAtual), HeaderText = "Estoque", FillWeight = 10, MinimumWidth = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.EstoqueMinimo), HeaderText = "Mínimo", FillWeight = 8, MinimumWidth = 70, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.Categoria), HeaderText = "Categoria", FillWeight = 12, MinimumWidth = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.Marca), HeaderText = "Marca", FillWeight = 12, MinimumWidth = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.ReferenciaFabrica), HeaderText = "Ref. fábrica", FillWeight = 12, MinimumWidth = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProdutoGridRow.AtivoTexto), HeaderText = "Ativo", FillWeight = 8, MinimumWidth = 60 },
        });
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        var gb = new GroupBox
        {
            Text = "Critérios de pesquisa",
            Dock = DockStyle.Top,
            Height = 198,
            Padding = new Padding(12, 10, 12, 10),
            Font = ErpTheme.UiFont(9f, FontStyle.Bold),
            ForeColor = ErpTheme.TextMuted
        };
        _filters.Dock = DockStyle.Fill;
        gb.Controls.Add(_filters);

        Controls.Add(_grid);
        Controls.Add(_lblRodape);
        Controls.Add(gb);
        Controls.Add(_ts);

        ResumeLayout(false);
    }

    private static TextBox MkTxt() => new()
    {
        BorderStyle = BorderStyle.FixedSingle,
        Font = ErpTheme.UiFont(9f),
        MinimumSize = new Size(40, 28)
    };

    private static ToolStripButton MkTs(string text, Image img, EventHandler onClick) =>
        new(text, img, onClick)
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            AutoSize = true,
            Margin = new Padding(6, 2, 8, 2)
        };
}
