using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Razão Social")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Display(Name = "Nome Fantasia")]
    public string NomeFantasia { get; set; } = string.Empty;

    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Representante { get; set; }

    public List<ClienteTelefone> Telefones { get; set; } = new();
    public List<ClienteEndereco> Enderecos { get; set; } = new();
    public List<Venda> Vendas { get; set; } = new();

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [StringLength(18)]
    public string CNPJ { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Inscrição Estadual")]
    public string? InscricaoEstadual { get; set; }
}