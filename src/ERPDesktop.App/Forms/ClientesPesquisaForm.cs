using System.Diagnostics;
using ERPDesktop.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using ERPDesktop.Application.DTOs;
using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;
using ERPDesktop.Shared.Ui;

namespace ERPDesktop.App.Forms;

public partial class ClientesPesquisaForm : Form
{
    private readonly ClienteAppService _clientes;
    private readonly IServiceProvider _provider;

    public ClientesPesquisaForm(ClienteAppService clientes, IServiceProvider provider)
    {
        _clientes = clientes;
        _provider = provider;
        InitializeComponent();
        ErpGridStyle.Aplicar(_grid);
        ErpChrome.AplicarBarraFilha(_ts, 22);
        BackColor = ErpTheme.FormBack;
        Text = "Pesquisa — Cadastro de Clientes";
        KeyPreview = true;
        KeyDown += ClientesPesquisaForm_KeyDown;
        Shown += (_, _) => CarregarGrid();
        _grid.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
                AbrirEdicaoSelecionado();
        };
    }

    private void ClientesPesquisaForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
            e.Handled = true;
            return;
        }

        if (e.KeyCode == Keys.F2)
        {
            AbrirNovo();
            e.Handled = true;
            return;
        }

        if (e.KeyCode == Keys.F3)
        {
            AbrirEdicaoSelecionado();
            e.Handled = true;
        }
    }

    private ClienteFiltro MontarFiltro() => new()
    {
        Ordenacao = _cbOrdem.SelectedItem?.ToString() ?? "Nome",
        Nome = _txtNome.Text,
        Fantasia = _txtFantasia.Text,
        RastrearNome = _txtRastNome.Text,
        RastrearEndereco = _txtRastEnd.Text,
        RastrearTelefone = _txtRastFone.Text,
        RastrearCpf = _txtRastCpf.Text,
        RastrearCnpj = _txtRastCnpj.Text,
        TipoCadastro = _cbTipo.SelectedIndex <= 0 ? string.Empty : _cbTipo.SelectedItem!.ToString()!,
        OrigemMarketing = _cbOrigem.SelectedIndex <= 0 ? string.Empty : _cbOrigem.SelectedItem!.ToString()!
    };

    private void CarregarGrid()
    {
        try
        {
            var rows = _clientes.PesquisarParaGrid(MontarFiltro());
            _grid.DataSource = rows;
            _statusRodape.Text = $"{rows.Count} registro(s) listado(s).";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar clientes.\r\n\r\n{ex.Message}", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private long? ObterIdSelecionado()
    {
        if (_grid.CurrentRow?.DataBoundItem is not ClienteGridRow r)
            return null;
        return r.Id;
    }

    private void AbrirNovo()
    {
        var f = _provider.GetRequiredService<ClienteCadastroForm>();
        f.Inicializar(null);
        f.ShowDialog(this);
        CarregarGrid();
    }

    private void AbrirEdicaoSelecionado()
    {
        var id = ObterIdSelecionado();
        if (id is null)
        {
            MessageBox.Show("Selecione um cliente na lista.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var f = _provider.GetRequiredService<ClienteCadastroForm>();
        f.Inicializar(id);
        f.ShowDialog(this);
        CarregarGrid();
    }

    private void ExcluirSelecionado()
    {
        var id = ObterIdSelecionado();
        if (id is null)
        {
            MessageBox.Show("Selecione um cliente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("Confirma a exclusão (lógica) do cliente selecionado?", "Clientes", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var r = _clientes.ExcluirSeguro(id.Value);
        if (!r.Ok)
        {
            MessageBox.Show(r.Mensagem, "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        CarregarGrid();
    }

    private void BtnAtualizar_Click(object? sender, EventArgs e) => CarregarGrid();
    private void BtnNovo_Click(object? sender, EventArgs e) => AbrirNovo();
    private void BtnEditar_Click(object? sender, EventArgs e) => AbrirEdicaoSelecionado();
    private void BtnExcluir_Click(object? sender, EventArgs e) => ExcluirSelecionado();
    private void BtnSair_Click(object? sender, EventArgs e) => Close();

    private void BtnImprimir_Click(object? sender, EventArgs e)
    {
        if (_grid.Rows.Count <= 0)
        {
            MessageBox.Show("Nada para imprimir.", "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        MessageBox.Show(
            "Impressão do grid será conectada ao motor de relatórios.\r\n\r\nPor ora, exporte/consulte diretamente no SQLite para auditoria.",
            "Imprimir",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void BtnEmail_Click(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not ClienteGridRow r || string.IsNullOrWhiteSpace(r.Email))
        {
            MessageBox.Show("Selecione um cliente com e-mail preenchido.", "E-mail", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo($"mailto:{Uri.EscapeDataString(r.Email)}") { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Não foi possível abrir o cliente de e-mail.\r\n\r\n{ex.Message}", "E-mail", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnWhats_Click(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not ClienteGridRow r || string.IsNullOrWhiteSpace(r.Whatsapp))
        {
            MessageBox.Show("Selecione um cliente com WhatsApp preenchido.", "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var digits = new string(r.Whatsapp.Where(char.IsDigit).ToArray());
        if (digits.Length < 10)
        {
            MessageBox.Show("Número de WhatsApp inválido para abrir o link.", "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo($"https://wa.me/{digits}") { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Não foi possível abrir o WhatsApp Web.\r\n\r\n{ex.Message}", "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnFiltrar_Click(object? sender, EventArgs e) => CarregarGrid();
}
