using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class ContatoTelefone
{
    public int Id { get; set; }

    public int ContatoId { get; set; }

    public string Telefone { get; set; } = string.Empty;

    public Contato? Contato { get; set; }
}