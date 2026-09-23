using System.ComponentModel.DataAnnotations;

namespace CRBVendas.Models;

public class ContatoEndereco
{
    public int Id { get; set; }

    public int ContatoId { get; set; }

    public string Logradouro { get; set; } = string.Empty;

    public string? Numero { get; set; }

    public string Cidade { get; set; } = string.Empty;

    public string? CEP { get; set; }

    public Contato? Contato { get; set; }
}