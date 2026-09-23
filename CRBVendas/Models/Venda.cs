using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRBVendas.Models;

public class Venda
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public int FornecedorId { get; set; }

    public decimal Valor { get; set; }

    public DateTime DataVenda { get; set; }
    public DateTime PrevisaoFaturamento { get; set; }

    public bool Faturado { get; set; }

    public int DescontoOfertado { get; set; }

    [StringLength(100)]
    public string? Obs { get; set; }

    public Cliente? Cliente { get; set; }
    public Fornecedor? Fornecedor { get; set; }
}