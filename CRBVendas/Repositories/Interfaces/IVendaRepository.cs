using CRBVendas.Models;

namespace CRBVendas.Repositories.Interfaces;

public interface IVendaRepository
{
    Task<List<Venda>> GetAllAsync();
    Task<Venda?> GetByIdAsync(int id);

    Task<List<Venda>> GetByClienteAsync(int clienteId);
    Task<List<Venda>> GetByFornecedorAsync(int fornecedorId);
    Task<List<Venda>> GetByPeriodoVendaAsync(DateTime dataInicial, DateTime dataFinal);
    Task<List<Venda>> GetByPeriodoFaturamentoAsync(DateTime dataInicial, DateTime dataFinal);

    Task AddAsync(Venda venda);
    Task UpdateAsync(Venda venda);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<List<Venda>> GetFaturadasAsync();
    Task<List<Venda>> GetNaoFaturadasAsync();
}