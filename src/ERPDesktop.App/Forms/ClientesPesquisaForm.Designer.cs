using ERPDesktop.Application.DTOs;
using ERPDesktop.App.Ui;
using ERPDesktop.Shared.Ui;

namespace ERPDesktop.App.Forms;

partial class ClientesPesquisaForm
{
    private ToolStrip _ts = null!;
    private DataGridView _grid = null!;
    private TableLayoutPanel _topFilters = null!;
    private TableLayoutPanel _bottomFilters = null!;
    private Label _statusRodape = null!;

    private ComboBox _cbOrdem = null!;
    private TextBox _txtNome = null!;
    private TextBox _txtFantasia = null!;
    private TextBox _txtRastNome = null!;
    private TextBox _txtRastEnd = null!;
    private TextBox _txtRastFone = null!;
    private TextBox _txtRastCpf = null!;
    private TextBox _txtRastCnpj = null!;
    private ComboBox _cbTipo = null!;
    private ComboBox _cbOrigem = null!;

    private void InitializeComponent()
    {
        SuspendLayout();

        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        MinimumSize = new Size(1100, 640);
        StartPosition = FormStartPosition.CenterParent;

        _ts = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden, ImageScalingSize = new Size(22, 22), Dock = DockStyle.Top };
        _ts.Items.Add(MkTsBtn("Novo", ToolbarIcons.Create(22, ErpTheme.ToolbarVendas, Color.White, "users"), BtnNovo_Click));
        _ts.Items.Add(MkTsBtn("Editar", ToolbarIcons.Create(22, ErpTheme.ToolbarClientes, Color.White, "users"), BtnEditar_Click));
        _ts.Items.Add(MkTsBtn("Excluir", ToolbarIcons.Create(22, ErpTheme.ToolbarSair, Color.White, "users"), BtnExcluir_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(MkTsBtn("Imprimir", ToolbarIcons.Create(22, Color.FromArgb(95, 105, 120), Color.White, "chart"), BtnImprimir_Click));
        _ts.Items.Add(MkTsBtn("Atualizar", ToolbarIcons.Create(22, Color.FromArgb(95, 105, 120), Color.White, "chart"), BtnAtualizar_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(MkTsBtn("E-mail", ToolbarIcons.Create(22, ErpTheme.ToolbarClientes, Color.White, "users"), BtnEmail_Click));
        _ts.Items.Add(MkTsBtn("WhatsApp", ToolbarIcons.Create(22, ErpTheme.ToolbarVendas, Color.White, "users"), BtnWhats_Click));
        _ts.Items.Add(new ToolStripSeparator());
        _ts.Items.Add(MkTsBtn("Sair", ToolbarIcons.Create(22, Color.FromArgb(95, 105, 120), Color.White, "door"), BtnSair_Click));

        _topFilters = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(12, 8, 12, 8),
            ColumnCount = 3,
            RowCount = 3
        };
        _topFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        _topFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _topFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _topFilters.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        _topFilters.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        _topFilters.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        _cbOrdem = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(6, 2, 6, 2) };
        _cbOrdem.Items.AddRange(new object[] { "Nome", "Codigo", "Fantasia" });
        _cbOrdem.SelectedIndex = 0;

        _txtNome = MkTxt();
        _txtFantasia = MkTxt();
        _txtRastNome = MkTxt();
        _txtRastEnd = MkTxt();
        _txtRastFone = MkTxt();

        void AddFilter(int row, int col, string titulo, Control c)
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var l = new Label { Text = titulo, Dock = DockStyle.Top, AutoSize = true, ForeColor = ErpTheme.TextMuted };
            c.Dock = DockStyle.Fill;
            p.Controls.Add(c);
            p.Controls.Add(l);
            _topFilters.Controls.Add(p, col, row);
        }

        AddFilter(0, 0, "Ordem / filtro", _cbOrdem);
        AddFilter(0, 1, "Pesquisar por Nome", _txtNome);
        AddFilter(0, 2, "Pesquisar por Fantasia", _txtFantasia);
        AddFilter(1, 0, "Rastrear Nome", _txtRastNome);
        AddFilter(1, 1, "Rastrear Endereço", _txtRastEnd);
        AddFilter(1, 2, "Rastrear Telefone", _txtRastFone);

        var btnFiltrar = new Button
        {
            Text = "Aplicar filtros",
            Dock = DockStyle.Fill,
            Margin = new Padding(6, 2, 6, 2),
            AutoSize = true,
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnFiltrar.FlatAppearance.BorderColor = ErpTheme.BorderSubtle;
        btnFiltrar.Click += BtnFiltrar_Click;
        var pFiltrar = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
        pFiltrar.Controls.Add(btnFiltrar);
        _topFilters.Controls.Add(pFiltrar, 0, 2);
        _topFilters.SetColumnSpan(pFiltrar, 3);

        _bottomFilters = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 92,
            Padding = new Padding(12, 6, 12, 6),
            ColumnCount = 4,
            RowCount = 2
        };
        _bottomFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _bottomFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _bottomFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _bottomFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

        _txtRastCpf = MkTxt();
        _txtRastCnpj = MkTxt();
        _cbTipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(6, 2, 6, 2) };
        _cbTipo.Items.AddRange(new object[] { "(Todos)", "Pessoa Física", "Pessoa Jurídica" });
        _cbTipo.SelectedIndex = 0;

        _cbOrigem = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(6, 2, 6, 2) };
        _cbOrigem.Items.AddRange(new object[] { "(Todos)", "Indicação", "Instagram", "Facebook", "Google", "Passagem", "Outro" });
        _cbOrigem.SelectedIndex = 0;

        void AddBottom(int row, int col, string titulo, Control c)
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 6, 0) };
            var l = new Label { Text = titulo, Dock = DockStyle.Top, AutoSize = true, ForeColor = ErpTheme.TextMuted };
            c.Dock = DockStyle.Fill;
            p.Controls.Add(c);
            p.Controls.Add(l);
            _bottomFilters.Controls.Add(p, col, row);
        }

        AddBottom(0, 0, "Rastrear CPF", _txtRastCpf);
        AddBottom(0, 1, "Rastrear CNPJ", _txtRastCnpj);
        AddBottom(0, 2, "Filtrar tipo cadastro", _cbTipo);
        AddBottom(0, 3, "Filtrar origem", _cbOrigem);

        _statusRodape = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 28,
            Padding = new Padding(12, 4, 12, 4),
            Text = "Dica: ESC fecha • F2 novo • F3 editar • duplo clique abre o cadastro.",
            ForeColor = ErpTheme.TextMuted,
            BackColor = ErpTheme.StatusBack
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
            AutoGenerateColumns = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical,
            GridColor = Color.FromArgb(230, 230, 230),
            ColumnHeadersHeight = 32,
            EnableHeadersVisualStyles = true,
            Font = new Font("Segoe UI", 9F)
        };
        _grid.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.Codigo), HeaderText = "Nº", FillWeight = 10, MinimumWidth = 70 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.NomeRazaoSocial), HeaderText = "Nome / Razão Social", FillWeight = 35, MinimumWidth = 180 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.NomeFantasia), HeaderText = "Fantasia", FillWeight = 20, MinimumWidth = 120 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.TipoCadastro), HeaderText = "Tipo", FillWeight = 12, MinimumWidth = 90 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.OrigemMarketing), HeaderText = "Origem / Marketing", FillWeight = 15, MinimumWidth = 110 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.Whatsapp), HeaderText = "WhatsApp", FillWeight = 12, MinimumWidth = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.WhatsappAbordagem), HeaderText = "Abordagem", FillWeight = 12, MinimumWidth = 90 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.Telefone1), HeaderText = "Telefone", FillWeight = 12, MinimumWidth = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.Celular), HeaderText = "Celular", FillWeight = 12, MinimumWidth = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.Email), HeaderText = "E-mail", FillWeight = 18, MinimumWidth = 140 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.VendedorNome), HeaderText = "Vendedor", FillWeight = 14, MinimumWidth = 110 },
            new DataGridViewTextBoxColumn { DataPropertyName = nameof(ClienteGridRow.StatusTexto), HeaderText = "Status", FillWeight = 10, MinimumWidth = 80 },
        });
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        Controls.Add(_grid);
        Controls.Add(_bottomFilters);
        Controls.Add(_statusRodape);
        Controls.Add(_topFilters);
        Controls.Add(_ts);

        ResumeLayout(false);
    }

    private static TextBox MkTxt() => new() { Margin = new Padding(6, 2, 6, 2) };

    private static ToolStripButton MkTsBtn(string text, Image img, EventHandler onClick)
    {
        var b = new ToolStripButton(text, img, onClick)
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            AutoSize = true,
            Margin = new Padding(4, 2, 4, 2)
        };
        return b;
    }
}
