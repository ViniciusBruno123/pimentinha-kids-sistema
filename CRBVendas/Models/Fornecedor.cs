using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class Fornecedor
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Representante { get; set; }

    public List<FornecedorTelefone> Telefones { get; set; } = new();
    public List<FornecedorEndereco> Enderecos { get; set; } = new();
    public List<Venda> Vendas { get; set; } = new();
}