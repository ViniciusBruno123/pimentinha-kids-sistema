using CRBVendas.Models;

namespace CRBVendas.Repositories.Interfaces;

public interface IContatoRepository
{
    Task<List<Contato>> GetAllAsync();

    Task<Contato?> GetByIdAsync(int id);

    Task<List<Contato>> GetByNomeAsync(string nome);

    Task<int> GetTotalRegistrosAsync(string? cidade = null);

    Task AddAsync(Contato contato);

    Task<List<Contato>> GetAgendaAsync(
    string? cidade = null,
    int pagina = 1,
    int tamanhoPagina = 20);

    Task UpdateAsync(Contato contato);

    Task DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
}