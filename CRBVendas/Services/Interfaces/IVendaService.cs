using CRBVendas.Models;

namespace CRBVendas.Services.Interfaces;

public interface IVendaService
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

    Task MarcarComoFaturadaAsync(int id);
    Task MarcarComoNaoFaturadaAsync(int id);

    Task<decimal> ObterTotalVendidoPorClienteAsync(int clienteId);
    Task<decimal> ObterTotalVendidoPorFornecedorAsync(int fornecedorId);

    Task<decimal> ObterComissaoTotalPorClienteAsync(int clienteId);
    Task<decimal> ObterComissaoTotalPorFornecedorAsync(int fornecedorId);

    Task<decimal> ObterComissaoFaturadaNoMesAsync(int ano, int mes);
    Task<decimal> ObterComissaoPendenteNoMesAsync(int ano, int mes);

    Task<List<Venda>> ObterVendasFaturadasNoMesAsync(int ano, int mes);
    Task<List<Venda>> ObterVendasPendentesNoMesAsync(int ano, int mes);

    decimal ObterPercentualComissao(int descontoOfertado);
    decimal CalcularComissao(Venda venda);

    Task<List<Venda>> GetFaturadasAsync();
    Task<List<Venda>> GetNaoFaturadasAsync();
}