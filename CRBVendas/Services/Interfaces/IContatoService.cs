using CRBVendas.Models;

namespace CRBVendas.Services.Interfaces;

public interface IContatoService
{
    Task<List<Contato>> GetAllAsync();

    Task<Contato?> GetByIdAsync(int id);

    Task<List<Contato>> GetByNomeAsync(string nome);

    Task<int> GetTotalRegistrosAsync(string? cidade = null);

    Task AddAsync(Contato contato);

    Task UpdateAsync(Contato contato);

    Task DeleteAsync(int id);

    Task<List<Contato>> GetAgendaAsync(
    string? cidade,
    int pagina = 1,
    int tamanhoPagina = 20);
}