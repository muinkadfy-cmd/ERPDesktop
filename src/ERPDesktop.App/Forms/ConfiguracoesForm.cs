using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class ConfiguracoesForm : Form
{
    private readonly ConfiguracoesAppService _cfg;

    public ConfiguracoesForm(ConfiguracoesAppService cfg)
    {
        _cfg = cfg;
        InitializeComponent();
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        _st.BackColor = ErpTheme.StatusBack;
        _btnSalvarEmp.Click += (_, _) => Salvar();
        Shown += (_, _) =>
        {
            _txtRazao.Text = _cfg.Obter("empresa.razao_social");
            _txtCidade.Text = _cfg.Obter("empresa.cidade");
            _txtFone.Text = _cfg.Obter("empresa.telefone");
            _stMsg.Text = "Pronto.";
        };
    }

    private void Salvar()
    {
        _cfg.Salvar("empresa.razao_social", _txtRazao.Text.Trim());
        _cfg.Salvar("empresa.cidade", _txtCidade.Text.Trim());
        _cfg.Salvar("empresa.telefone", _txtFone.Text.Trim());
        _stMsg.Text = "Salvo em " + DateTime.Now.ToString("HH:mm:ss");
    }
}
