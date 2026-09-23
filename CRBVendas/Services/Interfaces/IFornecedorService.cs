using CRBVendas.Models;

namespace CRBVendas.Services.Interfaces;

public interface IFornecedorService
{
    Task<List<Fornecedor>> GetAllAsync();
    Task<Fornecedor?> GetByIdAsync(int id);
    Task<List<Fornecedor>> GetByNomeAsync(string nome);
    Task AddAsync(Fornecedor fornecedor);
    Task UpdateAsync(Fornecedor fornecedor);
    Task DeleteAsync(int id);
}