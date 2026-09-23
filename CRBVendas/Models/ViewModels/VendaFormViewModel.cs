using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRBVendas.Models.ViewModels;

public class VendaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Selecione um cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Selecione um fornecedor.")]
    [Display(Name = "Fornecedor")]
    public int FornecedorId { get; set; }

    [Required(ErrorMessage = "Informe o valor.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Informe a data da venda.")]
    [Display(Name = "Data da venda")]
    [DataType(DataType.Date)]
    public DateTime DataVenda { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Informe a previsão de faturamento.")]
    [Display(Name = "Previsão de faturamento")]
    [DataType(DataType.Date)]
    public DateTime PrevisaoFaturamento { get; set; } = DateTime.Today;

    [Display(Name = "Faturado")]
    public bool Faturado { get; set; }

    [Required(ErrorMessage = "Informe o desconto ofertado.")]
    [Display(Name = "Desconto ofertado (%)")]
    public int DescontoOfertado { get; set; }

    [StringLength(100, ErrorMessage = "A observação deve ter no máximo 100 caracteres.")]
    [Display(Name = "Observação")]
    public string? Obs { get; set; }

    public List<SelectListItem> Clientes { get; set; } = new();
    public List<SelectListItem> Fornecedores { get; set; } = new();
    public List<SelectListItem> Descontos { get; set; } = new();
}