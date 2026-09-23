using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class FornecedorTelefone
{
    public int Id { get; set; }

    [Required]
    public int FornecedorId { get; set; }

    [Required]
    [StringLength(30)]
    public string Telefone { get; set; } = string.Empty;

    public Fornecedor? Fornecedor { get; set; }
}