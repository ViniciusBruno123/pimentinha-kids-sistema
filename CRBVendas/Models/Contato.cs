using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class Contato
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Nome Fantasia")]
    public string NomeFantasia { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Contato")]
    public string? NomeContato { get; set; }

    [Display(Name = "Última Visita")]
    public DateTime? AgendaAtual { get; set; }

    [Display(Name = "Próxima Visita")]
    public DateTime? AgendaProxima { get; set; }

    [StringLength(500)]
    [Display(Name = "Observações")]
    public string? Observacao { get; set; }

    public List<ContatoTelefone> Telefones { get; set; } = new();

    public ContatoEndereco? Endereco { get; set; }
}