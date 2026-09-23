using CRBVendas.Models;

namespace CRBVendas.Services.Interfaces;

public interface IClienteService
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<List<Cliente>> GetByNomeAsync(string nome);
    Task<List<Cliente>> GetByNomeFantasiaAsync(string nome);
    Task<List<Cliente>> GetByCidadeAsync(string cidade);
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
}