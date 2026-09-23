using CRBVendas.Models;

namespace CRBVendas.Repositories.Interfaces;

public interface IFornecedorRepository
{
    Task<List<Fornecedor>> GetAllAsync();
    Task<Fornecedor?> GetByIdAsync(int id);
    Task<List<Fornecedor>> GetByNomeAsync(string nome);
    Task AddAsync(Fornecedor fornecedor);
    Task UpdateAsync(Fornecedor fornecedor);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}