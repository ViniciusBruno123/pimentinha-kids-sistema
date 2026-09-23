using CRBVendas.Models;

namespace CRBVendas.Models.ViewModels;

public class HomeIndexViewModel
{
    public int AnoReferencia { get; set; }
    public int MesReferencia { get; set; }

    public decimal ComissaoFaturadaNoMes { get; set; }
    public decimal ComissaoPendenteNoMes { get; set; }

    public int QuantidadeVendasFaturadasNoMes { get; set; }
    public int QuantidadeVendasPendentesNoMes { get; set; }

    public List<Venda> VendasFaturadasNoMes { get; set; } = new();
    public List<Venda> VendasPendentesNoMes { get; set; } = new();
}