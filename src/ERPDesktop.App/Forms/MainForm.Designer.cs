#nullable enable
using ERPDesktop.App.Ui;
using ERPDesktop.Shared.Ui;

namespace ERPDesktop.App.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private Panel _header = null!;
    private Label _lblCabTitulo = null!;
    private Label _lblCabSub = null!;
    private MenuStrip _menu = null!;
    private ToolStrip _tool = null!;
    private Panel _sidebar = null!;
    private TreeView _tree = null!;
    private StatusStrip _status = null!;
    private Panel _footerAtalhos = null!;
    private FlowLayoutPanel _flowFkeys = null!;
    private ToolStripStatusLabel _statusMensagem = null!;
    private ToolStripStatusLabel _statusCaps = null!;
    private ToolStripStatusLabel _statusNum = null!;
    private ToolStripStatusLabel _statusIns = null!;
    private ToolStripStatusLabel _statusHora = null!;
    private ToolStripStatusLabel _statusUsuario = null!;
    private ToolStripStatusLabel _statusVersao = null!;
    private ToolStripStatusLabel _statusData = null!;
    private ToolStripStatusLabel _statusCaminhoDb = null!;
    private ToolStripMenuItem _miExibirSidebar = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _header = new Panel();
        _lblCabTitulo = new Label();
        _lblCabSub = new Label();
        _menu = new MenuStrip();
        _tool = new ToolStrip();
        _sidebar = new Panel();
        _tree = new TreeView();
        _status = new StatusStrip();
        _footerAtalhos = new Panel();
        _flowFkeys = new FlowLayoutPanel();
        _statusMensagem = new ToolStripStatusLabel();
        _statusCaps = new ToolStripStatusLabel();
        _statusNum = new ToolStripStatusLabel();
        _statusIns = new ToolStripStatusLabel();
        _statusHora = new ToolStripStatusLabel();
        _statusUsuario = new ToolStripStatusLabel();
        _statusVersao = new ToolStripStatusLabel();
        _statusData = new ToolStripStatusLabel();
        _statusCaminhoDb = new ToolStripStatusLabel();

        SuspendLayout();

        _header.Dock = DockStyle.Top;
        _header.Height = 46;
        _header.BackColor = Color.FromArgb(27, 43, 75);
        _lblCabTitulo.ForeColor = Color.White;
        _lblCabTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        _lblCabTitulo.Text = "ERP Loja de Calçados — Santiago";
        _lblCabTitulo.AutoSize = true;
        _lblCabTitulo.Location = new Point(16, 10);
        _lblCabSub.ForeColor = Color.FromArgb(200, 210, 230);
        _lblCabSub.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        _lblCabSub.Text = "Desktop · SQLite local · Modo offline";
        _lblCabSub.AutoSize = true;
        _lblCabSub.Location = new Point(420, 16);
        _header.Controls.Add(_lblCabSub);
        _header.Controls.Add(_lblCabTitulo);

        var mIni = new ToolStripMenuItem("Início", null, menuInicio_Click);
        var mCad = new ToolStripMenuItem("Cadastros");
        mCad.DropDownItems.Add(new ToolStripMenuItem("Clientes…", null, menuCadastrosClientes_Click) { ShortcutKeys = Keys.Control | Keys.Shift | Keys.C });
        mCad.DropDownItems.Add(new ToolStripMenuItem("Produtos / Calçados…", null, menuCadastrosProdutos_Click) { ShortcutKeys = Keys.Control | Keys.Shift | Keys.P });
        mCad.DropDownItems.Add(new ToolStripMenuItem("Fornecedores…", null, menuCadastrosFornecedores_Click));
        var mVen = new ToolStripMenuItem("Vendas");
        mVen.DropDownItems.Add(new ToolStripMenuItem("PDV / Venda balcão…", null, menuVendasPdv_Click));
        mVen.DropDownItems.Add(new ToolStripMenuItem("Orçamentos / Pedidos…", null, menuVendasOrcamentos_Click));
        mVen.DropDownItems.Add(new ToolStripMenuItem("Menu de Vendas (quadro)…", null, menuVendasMenu_Click));
        var mEst = new ToolStripMenuItem("Estoque");
        mEst.DropDownItems.Add(new ToolStripMenuItem("Consulta e ajuste…", null, menuEstoque_Click));
        var mCmp = new ToolStripMenuItem("Compras");
        mCmp.DropDownItems.Add(new ToolStripMenuItem("Pedidos de compra…", null, menuCompras_Click));
        var mFin = new ToolStripMenuItem("Financeiro");
        mFin.DropDownItems.Add(new ToolStripMenuItem("Menu financeiro (quadro)…", null, menuFinancasMenu_Click));
        mFin.DropDownItems.Add(new ToolStripMenuItem("Contas a receber…", null, menuFinCr_Click));
        mFin.DropDownItems.Add(new ToolStripMenuItem("Contas a pagar…", null, menuFinCp_Click));
        mFin.DropDownItems.Add(new ToolStripMenuItem("Caixa / movimentos…", null, menuFinCaixa_Click));
        var mRel = new ToolStripMenuItem("Relatórios");
        mRel.DropDownItems.Add(new ToolStripMenuItem("Central de relatórios…", null, menuRelatorios_Click));
        var mEti = new ToolStripMenuItem("Etiquetas");
        mEti.DropDownItems.Add(new ToolStripMenuItem("Etiquetas / código de barras…", null, menuEtiquetas_Click));
        var mPro = new ToolStripMenuItem("Promoções");
        mPro.DropDownItems.Add(new ToolStripMenuItem("Cadastro de promoções…", null, menuPromocoes_Click));
        var mCfg = new ToolStripMenuItem("Configurações");
        mCfg.DropDownItems.Add(new ToolStripMenuItem("Parâmetros do sistema…", null, menuConfig_Click));
        var mAju = new ToolStripMenuItem("Ajuda");
        mAju.DropDownItems.Add(new ToolStripMenuItem("Sobre…", null, menuAjudaSobre_Click));

        var mExi = new ToolStripMenuItem("Exibir");
        _miExibirSidebar = new ToolStripMenuItem("Menu lateral (árvore)  ·  F8", null, menuExibirSidebar_Click)
        {
            CheckOnClick = true,
            Checked = false
        };
        mExi.DropDownItems.Add(_miExibirSidebar);

        _menu.Items.AddRange(new ToolStripItem[] { mIni, mExi, mCad, mVen, mEst, mCmp, mFin, mRel, mEti, mPro, mCfg, mAju });
        _menu.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        var sz = new Size(40, 40);
        _tool.ImageScalingSize = sz;
        _tool.GripStyle = ToolStripGripStyle.Hidden;
        _tool.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        _tool.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        _tool.LayoutStyle = ToolStripLayoutStyle.Flow;
        _tool.AutoSize = false;
        _tool.Height = 88;
        _tool.Items.Add(MkBtn("Início", ToolbarIcons.Create(40, Color.FromArgb(25, 118, 210), Color.White, "chart"), tsInicio_Click));
        _tool.Items.Add(MkBtn("Clientes", ToolbarIcons.Create(40, ErpTheme.ToolbarClientes, Color.White, "users"), tsClientes_Click));
        _tool.Items.Add(MkBtn("Calçados", ToolbarIcons.Create(40, ErpTheme.ToolbarProdutos, Color.White, "box"), tsProdutos_Click));
        _tool.Items.Add(MkBtn("PDV", ToolbarIcons.Create(40, ErpTheme.ToolbarVendas, Color.White, "cart"), tsPdv_Click));
        _tool.Items.Add(MkBtn("Orçamentos", ToolbarIcons.Create(40, Color.FromArgb(124, 88, 42), Color.White, "orc"), tsOrcamentos_Click));
        _tool.Items.Add(MkBtn("Estoque", ToolbarIcons.Create(40, Color.FromArgb(0, 131, 143), Color.White, "est"), tsEstoque_Click));
        _tool.Items.Add(MkBtn("Compras", ToolbarIcons.Create(40, Color.FromArgb(93, 64, 55), Color.White, "cmp"), tsCompras_Click));
        _tool.Items.Add(MkBtn("Fornecedores", ToolbarIcons.Create(40, Color.FromArgb(81, 45, 168), Color.White, "for"), tsFornecedores_Click));
        _tool.Items.Add(MkBtn("Finanças", ToolbarIcons.Create(40, ErpTheme.ToolbarFinancas, Color.White, "money"), tsFinanceiro_Click));
        _tool.Items.Add(MkBtn("À receber", ToolbarIcons.Create(40, Color.FromArgb(106, 27, 154), Color.White, "CR"), tsCr_Click));
        _tool.Items.Add(MkBtn("À pagar", ToolbarIcons.Create(40, Color.FromArgb(183, 28, 28), Color.White, "CP"), tsCp_Click));
        _tool.Items.Add(MkBtn("Relatórios", ToolbarIcons.Create(40, Color.FromArgb(69, 90, 100), Color.White, "rel"), tsRelatorios_Click));
        _tool.Items.Add(MkBtn("Etiquetas", ToolbarIcons.Create(40, Color.FromArgb(0, 121, 107), Color.White, "etq"), tsEtiquetas_Click));
        _tool.Items.Add(MkBtn("Promoções", ToolbarIcons.Create(40, Color.FromArgb(245, 124, 0), Color.White, "%"), tsPromocoes_Click));
        _tool.Items.Add(MkBtn("Config", ToolbarIcons.Create(40, Color.FromArgb(69, 90, 100), Color.White, "cfg"), tsConfig_Click));
        _tool.Items.Add(new ToolStripSeparator());
        _tool.Items.Add(MkBtn("Sair", ToolbarIcons.Create(40, ErpTheme.ToolbarSair, Color.White, "door"), tsSair_Click));

        _sidebar.Dock = DockStyle.Left;
        _sidebar.Width = 228;
        _sidebar.BackColor = Color.FromArgb(236, 239, 245);
        _sidebar.Padding = new Padding(0, 6, 0, 0);
        _tree.Dock = DockStyle.Fill;
        _tree.BorderStyle = BorderStyle.None;
        _tree.Font = new Font("Segoe UI", 9F);
        _tree.FullRowSelect = true;
        _tree.HideSelection = false;
        _tree.ShowLines = true;
        _tree.ShowPlusMinus = true;
        _tree.ItemHeight = 22;
        _tree.Nodes.Clear();
        var n0 = new TreeNode("Início / Dashboard") { Tag = "INICIO" };
        var n1 = new TreeNode("Cadastros");
        n1.Nodes.Add(new TreeNode("Clientes") { Tag = "CLI" });
        n1.Nodes.Add(new TreeNode("Produtos / Calçados") { Tag = "PROD" });
        n1.Nodes.Add(new TreeNode("Fornecedores") { Tag = "FORN" });
        var n2 = new TreeNode("Vendas");
        n2.Nodes.Add(new TreeNode("PDV / Balcão") { Tag = "PDV" });
        n2.Nodes.Add(new TreeNode("Orçamentos e pedidos") { Tag = "ORC" });
        n2.Nodes.Add(new TreeNode("Menu de vendas (quadro)") { Tag = "MENUVEN" });
        var n3 = new TreeNode("Estoque");
        n3.Nodes.Add(new TreeNode("Consulta e ajuste") { Tag = "EST" });
        var n4 = new TreeNode("Compras");
        n4.Nodes.Add(new TreeNode("Pedidos") { Tag = "COMP" });
        var n5 = new TreeNode("Financeiro");
        n5.Nodes.Add(new TreeNode("Menu financeiro (quadro)") { Tag = "MENUFIN" });
        n5.Nodes.Add(new TreeNode("Contas a receber") { Tag = "CR" });
        n5.Nodes.Add(new TreeNode("Contas a pagar") { Tag = "CP" });
        n5.Nodes.Add(new TreeNode("Caixa") { Tag = "CAIXA" });
        var n6 = new TreeNode("Relatórios") { Tag = "REL" };
        var n7 = new TreeNode("Etiquetas / código de barras") { Tag = "ETI" };
        var n8 = new TreeNode("Promoções") { Tag = "PROMO" };
        var n9 = new TreeNode("Configurações") { Tag = "CFG" };
        _tree.Nodes.AddRange(new[] { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9 });
        _tree.ExpandAll();
        _sidebar.Controls.Add(_tree);
        _sidebar.Visible = false;

        _footerAtalhos.Dock = DockStyle.Bottom;
        _footerAtalhos.Height = 34;
        _footerAtalhos.BackColor = Color.FromArgb(27, 43, 75);
        _flowFkeys.Dock = DockStyle.Fill;
        _flowFkeys.FlowDirection = FlowDirection.LeftToRight;
        _flowFkeys.WrapContents = false;
        _flowFkeys.Padding = new Padding(8, 6, 8, 4);
        _footerAtalhos.Controls.Add(_flowFkeys);
        MkFkey("F2", "Nova venda / PDV", Color.FromArgb(25, 118, 210));
        MkFkey("F3", "PDV rápido", Color.FromArgb(0, 137, 123));
        MkFkey("F8", "Menu lateral", Color.FromArgb(69, 90, 100));
        MkFkey("F4", "Produtos", Color.FromArgb(86, 72, 132));
        MkFkey("F5", "Clientes", Color.FromArgb(52, 88, 138));
        MkFkey("F6", "Estoque", Color.FromArgb(0, 151, 167));
        MkFkey("F7", "Caixa", Color.FromArgb(198, 40, 40));
        MkFkey("F12", "Finalizar (no PDV)", Color.FromArgb(46, 125, 50));

        _statusMensagem = new ToolStripStatusLabel("Pronto") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
        _statusData = new ToolStripStatusLabel("") { BorderSides = ToolStripStatusLabelBorderSides.Left };
        _statusCaminhoDb = new ToolStripStatusLabel("") { BorderSides = ToolStripStatusLabelBorderSides.Left };
        _statusNum = new ToolStripStatusLabel("Num") { BorderSides = ToolStripStatusLabelBorderSides.Left };
        _statusCaps = new ToolStripStatusLabel("Caps") { BorderSides = ToolStripStatusLabelBorderSides.Left };
        _statusIns = new ToolStripStatusLabel("Ins") { BorderSides = ToolStripStatusLabelBorderSides.Left };
        _statusHora = new ToolStripStatusLabel("") { BorderSides = ToolStripStatusLabelBorderSides.Left };
        _statusUsuario = new ToolStripStatusLabel("MASTER") { ForeColor = Color.FromArgb(110, 48, 48), BorderSides = ToolStripStatusLabelBorderSides.Left };
        _statusVersao = new ToolStripStatusLabel("ERP Desktop 1.0") { BorderSides = ToolStripStatusLabelBorderSides.Left };
        _status.Items.AddRange(new ToolStripItem[]
        {
            _statusMensagem, _statusData, _statusCaminhoDb, _statusNum, _statusCaps, _statusIns, _statusHora, _statusUsuario, _statusVersao
        });
        _status.Font = new Font("Segoe UI", 8.25F);
        _menu.Dock = DockStyle.Top;
        _tool.Dock = DockStyle.Top;
        _status.Dock = DockStyle.Bottom;

        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(1280, 800);
        Controls.Add(_sidebar);
        Controls.Add(_status);
        Controls.Add(_footerAtalhos);
        Controls.Add(_tool);
        Controls.Add(_menu);
        Controls.Add(_header);
        MainMenuStrip = _menu;
        Text = "ERP Loja de Calçados — Desktop";

        ResumeLayout(false);
        PerformLayout();

        System.Windows.Forms.Application.Idle += Application_Idle;
    }

    private void MkFkey(string tecla, string texto, Color fundo)
    {
        var p = new Panel
        {
            AutoSize = true,
            Margin = new Padding(0, 0, 10, 0),
            BackColor = fundo,
            Padding = new Padding(8, 2, 8, 2)
        };
        var l = new Label
        {
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.25F, FontStyle.Bold),
            Text = $"{tecla}  {texto}"
        };
        p.Controls.Add(l);
        _flowFkeys.Controls.Add(p);
    }

    private void Application_Idle(object? sender, EventArgs e)
    {
        _statusNum.Text = Control.IsKeyLocked(Keys.NumLock) ? "Num" : "Num ·";
        _statusCaps.Text = Control.IsKeyLocked(Keys.CapsLock) ? "CAPS" : "caps";
        _statusIns.Text = Control.IsKeyLocked(Keys.Insert) ? "INS" : "Ovr";
    }

    private static ToolStripButton MkBtn(string text, Image img, EventHandler onClick)
    {
        var b = new ToolStripButton(text, img, onClick)
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            TextImageRelation = TextImageRelation.ImageAboveText,
            AutoSize = true,
            Margin = new Padding(4, 2, 4, 2)
        };
        return b;
    }
}
