using ERPDesktop.Application.Services;
using ERPDesktop.Domain.Entities;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class FornecedorCadastroForm : Form
{
    private readonly FornecedorAppService _svc;
    private Fornecedor _model = new();

    public FornecedorCadastroForm(FornecedorAppService svc)
    {
        _svc = svc;
        InitializeComponent();
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        _txtCodigo.ReadOnly = true;
        _btnSalvar.Click += (_, _) => Salvar();
        _btnSair.Click += (_, _) => Close();
    }

    public void Inicializar(long? id)
    {
        if (id is null)
        {
            _model = _svc.CriarNovoVazio();
        }
        else
        {
            _model = _svc.Obter(id.Value) ?? _svc.CriarNovoVazio();
        }

        Bind();
    }

    private void Bind()
    {
        _txtCodigo.Text = _model.Codigo;
        _txtRazao.Text = _model.RazaoSocial;
        _txtFantasia.Text = _model.NomeFantasia;
        _txtCnpj.Text = _model.Cnpj;
        _txtTel.Text = _model.Telefone;
        _txtCidade.Text = _model.Cidade;
        _txtUf.Text = _model.Uf;
        _txtEmail.Text = _model.Email;
        _txtObs.Text = _model.Observacoes;
    }

    private void Salvar()
    {
        _model.RazaoSocial = _txtRazao.Text.Trim();
        _model.NomeFantasia = _txtFantasia.Text.Trim();
        _model.Cnpj = _txtCnpj.Text.Trim();
        _model.Telefone = _txtTel.Text.Trim();
        _model.Cidade = _txtCidade.Text.Trim();
        _model.Uf = _txtUf.Text.Trim();
        _model.Email = _txtEmail.Text.Trim();
        _model.Observacoes = _txtObs.Text.Trim();
        var r = _svc.Salvar(_model);
        if (!r.Ok)
        {
            MessageBox.Show(r.Mensagem);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
