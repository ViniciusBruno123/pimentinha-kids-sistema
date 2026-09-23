using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class ClienteEndereco
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    [StringLength(150)]
    public string Logradouro { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Numero { get; set; }

    [Required]
    [StringLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [StringLength(15)]
    public string? CEP { get; set; }

    public Cliente? Cliente { get; set; }
}