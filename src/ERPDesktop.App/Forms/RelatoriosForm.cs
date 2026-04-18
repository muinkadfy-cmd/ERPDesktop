using ERPDesktop.Application.Services;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class RelatoriosForm : Form
{
    private readonly DashboardAppService _dash;
    private readonly ProdutoAppService _produtos;

    public RelatoriosForm(DashboardAppService dash, ProdutoAppService produtos)
    {
        _dash = dash;
        _produtos = produtos;
        InitializeComponent();
        Font = ErpTheme.UiFont();
        BackColor = ErpTheme.FormBack;
        _lista.Items.AddRange(new object[]
        {
            "Resumo do dia (vendas / estoque / CR)",
            "Vendas últimos 7 dias (totais por dia)",
            "Produtos com estoque baixo (lista)"
        });
        _lista.SelectedIndexChanged += (_, _) => AtualizarTexto();
        void Btn(string t, Action a)
        {
            var b = new Button { Text = t, AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            b.Click += (_, _) => a();
            _acoes.Controls.Add(b);
        }
        Btn("Atualizar visão", () => AtualizarTexto());
        Btn("Fechar", Close);
        Shown += (_, _) => { _lista.SelectedIndex = 0; };
    }

    private void AtualizarTexto()
    {
        if (_lista.SelectedIndex < 0)
            return;

        try
        {
            if (_lista.SelectedIndex == 0)
            {
                var r = _dash.ResumoHoje();
                _txt.Text =
                    $"RESUMO DO DIA ({DateTime.Today:dd/MM/yyyy})\r\n" +
                    $"Vendas (finalizadas): {r.VendasHoje:C}\r\n" +
                    $"Volume (unid.): {r.ParesVendidosHoje:N2}\r\n" +
                    $"Ticket médio: {r.TicketMedioHoje:C}\r\n" +
                    $"SKUs estoque baixo: {r.SkuEstoqueBaixo}\r\n" +
                    $"Contas a receber em aberto: {r.ContasReceberAberto:C}\r\n";
            }
            else if (_lista.SelectedIndex == 1)
            {
                var linhas = _dash.Vendas7Dias();
                _txt.Text = string.Join("\r\n", linhas.Select(l => $"{l.Dia:dd/MM/yyyy}\t{l.Total:C}"));
            }
            else
            {
                var rows = _dash.EstoqueBaixo(500);
                _txt.Text = string.Join("\r\n", rows.Select(r => $"{r.Codigo}\t{r.Descricao}\t{r.EstoqueAtual:N2}\t{r.EstoqueMinimo:N2}"));
            }
        }
        catch (Exception ex)
        {
            _txt.Text = ex.Message;
        }
    }
}
