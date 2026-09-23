using CRBVendas.Models;

namespace CRBVendas.Models.ViewModels;

public class VendaMesViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }

    public decimal ComissaoFaturada { get; set; }
    public decimal ComissaoPendente { get; set; }

    public List<Venda> VendasFaturadas { get; set; } = new();
    public List<Venda> VendasPendentes { get; set; } = new();
}