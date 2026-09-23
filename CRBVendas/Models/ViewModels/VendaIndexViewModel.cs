using CRBVendas.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRBVendas.Models.ViewModels;

public class VendaIndexViewModel
{
    public List<Venda> Vendas { get; set; } = new();

    public int? ClienteId { get; set; }
    public int? FornecedorId { get; set; }
    public DateTime? DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }

    public List<SelectListItem> Clientes { get; set; } = new();
    public List<SelectListItem> Fornecedores { get; set; } = new();
}