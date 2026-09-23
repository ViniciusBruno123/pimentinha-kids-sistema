using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class ClienteTelefone
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    [StringLength(30)]
    public string Telefone { get; set; } = string.Empty;

    public Cliente? Cliente { get; set; }
}