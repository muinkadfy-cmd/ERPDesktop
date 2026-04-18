using ERPDesktop.Application.Abstractions;
using ERPDesktop.Application.DTOs;
using ERPDesktop.App.Ui;

namespace ERPDesktop.App.Forms;

public partial class CaixaForm : Form
{
    private readonly IMovimentacaoFinanceiraRepository _repo;

    public CaixaForm(IMovimentacaoFinanceiraRepository repo)
    {
        _repo = repo;
        InitializeComponent();
        ErpGridStyle.Aplicar(_grid);
        _st.BackColor = ErpTheme.StatusBack;
        _grid.AutoGenerateColumns = false;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Data", DataPropertyName = nameof(MovimentacaoFinanceiraGridRow.DataMovimento), FillWeight = 18, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipo", DataPropertyName = nameof(MovimentacaoFinanceiraGridRow.Tipo), FillWeight = 14 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Histórico", DataPropertyName = nameof(MovimentacaoFinanceiraGridRow.Historico), FillWeight = 48 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Valor", DataPropertyName = nameof(MovimentacaoFinanceiraGridRow.Valor), FillWeight = 20, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Shown += (_, _) =>
        {
            _grid.DataSource = _repo.ListarUltimas(200).ToList();
            _stMsg.Text = $"{_grid.Rows.Count} lançamento(s).";
        };
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
    }
}
