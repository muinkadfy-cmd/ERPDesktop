using ERPDesktop.Application.Services;
using ERPDesktop.Domain.Entities;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class ProdutoCadastroForm : Form
{
    private readonly ProdutoAppService _svc;
    private Produto _model = new();
    public ProdutoCadastroForm(ProdutoAppService svc)
    {
        _svc = svc;
        InitializeComponent();
        Text = "Cadastro de Produto";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(720, 520);
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
    }

    public void Inicializar(long? id)
    {
        _model = id is null ? _svc.CriarNovoVazio() : _svc.Obter(id.Value) ?? _svc.CriarNovoVazio();
        VincularTela();
    }

    private void VincularTela()
    {
        _lblId.Text = _model.Id > 0 ? _model.Id.ToString() : "(novo)";
        _txtCodigo.Text = _model.Codigo;
        _txtDesc.Text = _model.Descricao;
        _cbUnidade.Text = string.IsNullOrWhiteSpace(_model.Unidade) ? "UN" : _model.Unidade;
        _numAvista.Value = _model.PrecoAvista < 0 ? 0 : _model.PrecoAvista;
        _numAprazo.Value = _model.PrecoAprazo < 0 ? 0 : _model.PrecoAprazo;
        _numEst.Value = _model.EstoqueAtual < 0 ? 0 : _model.EstoqueAtual;
        _numEstMin.Value = _model.EstoqueMinimo < 0 ? 0 : _model.EstoqueMinimo;
        _txtCat.Text = _model.Categoria;
        _txtMarca.Text = _model.Marca;
        _txtRef.Text = _model.ReferenciaFabrica;
        _chkAtivo.Checked = _model.Ativo;
    }

    private void VincularModelo()
    {
        _model.Codigo = _txtCodigo.Text.Trim();
        _model.Descricao = _txtDesc.Text.Trim();
        _model.Unidade = _cbUnidade.Text.Trim();
        if (string.IsNullOrWhiteSpace(_model.Unidade))
            _model.Unidade = "UN";
        _model.PrecoAvista = _numAvista.Value;
        _model.PrecoAprazo = _numAprazo.Value;
        _model.EstoqueAtual = _numEst.Value;
        _model.EstoqueMinimo = _numEstMin.Value;
        _model.Categoria = _txtCat.Text.Trim();
        _model.Marca = _txtMarca.Text.Trim();
        _model.ReferenciaFabrica = _txtRef.Text.Trim();
        _model.Ativo = _chkAtivo.Checked;
    }

    private void BtnSalvar_Click(object? sender, EventArgs e)
    {
        try
        {
            VincularModelo();
            var r = _svc.Salvar(_model);
            if (!r.Ok)
            {
                MessageBox.Show(r.Mensagem, "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Produto salvo.", "Produtos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Produtos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSair_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
