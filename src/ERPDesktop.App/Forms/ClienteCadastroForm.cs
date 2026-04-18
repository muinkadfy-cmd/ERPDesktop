using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.Services;
using ERPDesktop.Domain.Entities;
using ERPDesktop.Domain.Enums;
namespace ERPDesktop.App.Forms;

public partial class ClienteCadastroForm : Form
{
    private readonly ClienteAppService _clientes;
    private readonly IVendedorRepository _vendedores;

    private Cliente _model = new();
    private bool _carregando;

    public ClienteCadastroForm(ClienteAppService clientes, IVendedorRepository vendedores)
    {
        _clientes = clientes;
        _vendedores = vendedores;
        InitializeComponent();
        Text = "Cadastro de Clientes";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(980, 720);
        Font = new Font("Segoe UI", 9F);
        BackColor = Color.FromArgb(245, 245, 245);
    }

    public void Inicializar(long? id)
    {
        _carregando = true;
        try
        {
            CarregarVendedores();

            if (id is null)
            {
                _model = _clientes.CriarNovoVazio();
            }
            else
            {
                _model = _clientes.Obter(id.Value) ?? _clientes.CriarNovoVazio();
            }

            VincularModeloParaTela();
        }
        finally
        {
            _carregando = false;
        }
    }

    private void CarregarVendedores()
    {
        _cbVendedor.Items.Clear();
        _cbVendedor.DisplayMember = "Nome";
        _cbVendedor.ValueMember = "Id";
        foreach (var v in _vendedores.ListarAtivos())
            _cbVendedor.Items.Add(v);
    }

    private void VincularModeloParaTela()
    {
        _lblRegistro.Text = _model.Id > 0 ? _model.Id.ToString() : "(novo)";
        _dtCadastro.Value = _model.DataCadastro;

        _rbLiberado.Checked = _model.Status == ClienteStatusCadastro.Liberado;
        _rbRestrito.Checked = _model.Status == ClienteStatusCadastro.Restrito;
        _rbBloqueado.Checked = _model.Status == ClienteStatusCadastro.Bloqueado;

        _txtCodigo.Text = _model.Codigo;
        _txtNome.Text = _model.NomeRazaoSocial;
        _txtFantasia.Text = _model.NomeFantasia;

        _cbVendedor.SelectedItem = _cbVendedor.Items.Cast<Vendedor>().FirstOrDefault(v => v.Id == _model.VendedorId);

        _txtEndereco.Text = _model.Endereco;
        _txtNumero.Text = _model.Numero;
        _txtComplemento.Text = _model.Complemento;
        _txtBairro.Text = _model.Bairro;
        _txtCidade.Text = _model.Cidade;
        _cbUf.Text = _model.Uf;
        _txtCep.Text = _model.Cep;

        _txtTel1.Text = _model.Telefone1;
        _txtTel2.Text = _model.Telefone2;
        _txtCel.Text = _model.Celular;
        _txtWa.Text = _model.Whatsapp;
        _txtWaAbordagem.Text = _model.WhatsappAbordagem;
        _txtEmail.Text = _model.Email;
        _txtContato.Text = _model.Contato;
        _txtRede.Text = _model.RedeSocial;

        _cbTipoCadastro.Text = string.IsNullOrWhiteSpace(_model.TipoCadastro) ? "Pessoa Física" : _model.TipoCadastro;
        _txtOrigem.Text = _model.OrigemMarketing;
        _numLimite.Value = _model.LimiteCredito < 0 ? 0 : _model.LimiteCredito;
        _txtStatusFin.Text = _model.StatusFinanceiro;
        _chkRestrito.Checked = _model.Restrito;
        _chkBloqueado.Checked = _model.Bloqueado;
        _txtObsFin.Text = _model.ObservacoesFinanceiras;

        _txtCpf.Text = _model.Cpf;
        _txtRg.Text = _model.Rg;
        _txtOrgao.Text = _model.OrgaoEmissor;
        _txtCnpj.Text = _model.Cnpj;
        _txtIe.Text = _model.Ie;
        _txtIm.Text = _model.Im;
        _txtHistorico.Text = _model.HistoricoTexto;

        _txtObs.Text = _model.Observacoes;

        CarregarFoto();
    }

    private void CarregarFoto()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(_model.FotoPath) && File.Exists(_model.FotoPath))
                _pic.Image = Image.FromFile(_model.FotoPath);
            else
                _pic.Image = null;
        }
        catch
        {
            _pic.Image = null;
        }
    }

    private void VincularTelaParaModelo()
    {
        _model.DataCadastro = _dtCadastro.Value.Date;
        _model.Status = _rbBloqueado.Checked
            ? ClienteStatusCadastro.Bloqueado
            : _rbRestrito.Checked
                ? ClienteStatusCadastro.Restrito
                : ClienteStatusCadastro.Liberado;

        _model.Codigo = _txtCodigo.Text.Trim();
        _model.NomeRazaoSocial = _txtNome.Text.Trim();
        _model.NomeFantasia = _txtFantasia.Text.Trim();

        if (_cbVendedor.SelectedItem is Vendedor vv)
            _model.VendedorId = vv.Id;
        else
            _model.VendedorId = null;

        _model.Endereco = _txtEndereco.Text.Trim();
        _model.Numero = _txtNumero.Text.Trim();
        _model.Complemento = _txtComplemento.Text.Trim();
        _model.Bairro = _txtBairro.Text.Trim();
        _model.Cidade = _txtCidade.Text.Trim();
        _model.Uf = _cbUf.Text.Trim();
        _model.Cep = _txtCep.Text.Trim();

        _model.Telefone1 = _txtTel1.Text.Trim();
        _model.Telefone2 = _txtTel2.Text.Trim();
        _model.Celular = _txtCel.Text.Trim();
        _model.Whatsapp = _txtWa.Text.Trim();
        _model.WhatsappAbordagem = _txtWaAbordagem.Text.Trim();
        _model.Email = _txtEmail.Text.Trim();
        _model.Contato = _txtContato.Text.Trim();
        _model.RedeSocial = _txtRede.Text.Trim();

        _model.TipoCadastro = _cbTipoCadastro.Text.Trim();
        _model.OrigemMarketing = _txtOrigem.Text.Trim();
        _model.LimiteCredito = _numLimite.Value;
        _model.StatusFinanceiro = _txtStatusFin.Text.Trim();
        _model.Restrito = _chkRestrito.Checked;
        _model.Bloqueado = _chkBloqueado.Checked;
        _model.ObservacoesFinanceiras = _txtObsFin.Text.Trim();

        _model.Cpf = _txtCpf.Text.Trim();
        _model.Rg = _txtRg.Text.Trim();
        _model.OrgaoEmissor = _txtOrgao.Text.Trim();
        _model.Cnpj = _txtCnpj.Text.Trim();
        _model.Ie = _txtIe.Text.Trim();
        _model.Im = _txtIm.Text.Trim();
        _model.HistoricoTexto = _txtHistorico.Text.Trim();

        _model.Observacoes = _txtObs.Text.Trim();
    }

    private void BtnSalvar_Click(object? sender, EventArgs e)
    {
        try
        {
            VincularTelaParaModelo();
            var r = _clientes.Salvar(_model);
            if (!r.Ok)
            {
                MessageBox.Show(r.Mensagem, "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Cadastro salvo com sucesso.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Não foi possível salvar.\r\n\r\n{ex.Message}", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSair_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void BtnImprimir_Click(object? sender, EventArgs e) =>
        MessageBox.Show("Impressão da ficha será ligada ao relatório padrão na próxima etapa.", "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void BtnFoto_Click(object? sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "Imagens|*.png;*.jpg;*.jpeg;*.bmp|Todos|*.*",
            Title = "Selecionar foto do cliente"
        };
        if (ofd.ShowDialog(this) != DialogResult.OK)
            return;

        _model.FotoPath = ofd.FileName;
        CarregarFoto();
    }

    private void BtnLimparFoto_Click(object? sender, EventArgs e)
    {
        _model.FotoPath = string.Empty;
        _pic.Image = null;
    }

    private void Status_Changed(object? sender, EventArgs e)
    {
        if (_carregando)
            return;

        if (_rbBloqueado.Checked)
        {
            _chkBloqueado.Checked = true;
            _chkRestrito.Checked = false;
        }
        else if (_rbRestrito.Checked)
        {
            _chkRestrito.Checked = true;
            _chkBloqueado.Checked = false;
        }
        else
        {
            _chkRestrito.Checked = false;
            _chkBloqueado.Checked = false;
        }
    }
}
