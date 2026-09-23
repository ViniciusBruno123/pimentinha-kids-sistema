using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using CRBVendas.Services.Interfaces;

namespace CRBVendas.Services;

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IFornecedorRepository _fornecedorRepository;

    public VendaService(
        IVendaRepository vendaRepository,
        IClienteRepository clienteRepository,
        IFornecedorRepository fornecedorRepository)
    {
        _vendaRepository = vendaRepository;
        _clienteRepository = clienteRepository;
        _fornecedorRepository = fornecedorRepository;
    }

    public async Task<List<Venda>> GetAllAsync()
    {
        return await _vendaRepository.GetAllAsync();
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new Exception("Id inválido.");

        return await _vendaRepository.GetByIdAsync(id);
    }

    public async Task<List<Venda>> GetByClienteAsync(int clienteId)
    {
        if (clienteId <= 0)
            throw new Exception("Cliente inválido.");

        return await _vendaRepository.GetByClienteAsync(clienteId);
    }

    public async Task<List<Venda>> GetByFornecedorAsync(int fornecedorId)
    {
        if (fornecedorId <= 0)
            throw new Exception("Fornecedor inválido.");

        return await _vendaRepository.GetByFornecedorAsync(fornecedorId);
    }

    public async Task<List<Venda>> GetByPeriodoVendaAsync(DateTime dataInicial, DateTime dataFinal)
    {
        ValidarPeriodo(dataInicial, dataFinal);
        return await _vendaRepository.GetByPeriodoVendaAsync(dataInicial, dataFinal);
    }

    public async Task<List<Venda>> GetByPeriodoFaturamentoAsync(DateTime dataInicial, DateTime dataFinal)
    {
        ValidarPeriodo(dataInicial, dataFinal);
        return await _vendaRepository.GetByPeriodoFaturamentoAsync(dataInicial, dataFinal);
    }

    public async Task AddAsync(Venda venda)
    {
        await ValidarVendaAsync(venda);
        NormalizarVenda(venda);

        await _vendaRepository.AddAsync(venda);
    }

    public async Task UpdateAsync(Venda venda)
    {
        if (venda.Id <= 0)
            throw new Exception("Id inválido para atualização.");

        var vendaBanco = await _vendaRepository.GetByIdAsync(venda.Id);

        if (vendaBanco == null)
            throw new Exception("Venda não encontrada.");

        await ValidarVendaAsync(venda);
        NormalizarVenda(venda);

        vendaBanco.ClienteId = venda.ClienteId;
        vendaBanco.FornecedorId = venda.FornecedorId;
        vendaBanco.Valor = venda.Valor;
        vendaBanco.DataVenda = venda.DataVenda;
        vendaBanco.PrevisaoFaturamento = venda.PrevisaoFaturamento;
        vendaBanco.Faturado = venda.Faturado;
        vendaBanco.DescontoOfertado = venda.DescontoOfertado;
        vendaBanco.Obs = venda.Obs;

        await _vendaRepository.UpdateAsync(vendaBanco);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new Exception("Id inválido.");

        await _vendaRepository.DeleteAsync(id);
    }

    public async Task MarcarComoFaturadaAsync(int id)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);

        if (venda == null)
            throw new Exception("Venda não encontrada.");

        venda.Faturado = true;
        await _vendaRepository.UpdateAsync(venda);
    }

    public async Task MarcarComoNaoFaturadaAsync(int id)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);

        if (venda == null)
            throw new Exception("Venda não encontrada.");

        venda.Faturado = false;
        await _vendaRepository.UpdateAsync(venda);
    }

    public async Task<decimal> ObterTotalVendidoPorClienteAsync(int clienteId)
    {
        if (clienteId <= 0)
            throw new Exception("Cliente inválido.");

        var vendas = await _vendaRepository.GetByClienteAsync(clienteId);
        return vendas.Sum(v => v.Valor);
    }

    public async Task<decimal> ObterTotalVendidoPorFornecedorAsync(int fornecedorId)
    {
        if (fornecedorId <= 0)
            throw new Exception("Fornecedor inválido.");

        var vendas = await _vendaRepository.GetByFornecedorAsync(fornecedorId);
        return vendas.Sum(v => v.Valor);
    }

    public async Task<decimal> ObterComissaoTotalPorClienteAsync(int clienteId)
    {
        if (clienteId <= 0)
            throw new Exception("Cliente inválido.");

        var vendas = await _vendaRepository.GetByClienteAsync(clienteId);
        return vendas.Sum(CalcularComissao);
    }

    public async Task<decimal> ObterComissaoTotalPorFornecedorAsync(int fornecedorId)
    {
        if (fornecedorId <= 0)
            throw new Exception("Fornecedor inválido.");

        var vendas = await _vendaRepository.GetByFornecedorAsync(fornecedorId);
        return vendas.Sum(CalcularComissao);
    }

    public async Task<decimal> ObterComissaoFaturadaNoMesAsync(int ano, int mes)
    {
        ValidarAnoMes(ano, mes);

        var (inicio, fim) = ObterIntervaloMes(ano, mes);
        var vendas = await _vendaRepository.GetByPeriodoFaturamentoAsync(inicio, fim);

        return vendas
            .Where(v => v.Faturado)
            .Sum(CalcularComissao);
    }

    public async Task<decimal> ObterComissaoPendenteNoMesAsync(int ano, int mes)
    {
        ValidarAnoMes(ano, mes);

        var (inicio, fim) = ObterIntervaloMes(ano, mes);
        var vendas = await _vendaRepository.GetByPeriodoFaturamentoAsync(inicio, fim);

        return vendas
            .Where(v => !v.Faturado)
            .Sum(CalcularComissao);
    }

    public async Task<List<Venda>> ObterVendasFaturadasNoMesAsync(int ano, int mes)
    {
        ValidarAnoMes(ano, mes);

        var (inicio, fim) = ObterIntervaloMes(ano, mes);
        var vendas = await _vendaRepository.GetByPeriodoFaturamentoAsync(inicio, fim);

        return vendas
            .Where(v => v.Faturado)
            .OrderBy(v => v.PrevisaoFaturamento)
            .ToList();
    }

    public async Task<List<Venda>> ObterVendasPendentesNoMesAsync(int ano, int mes)
    {
        ValidarAnoMes(ano, mes);

        var (inicio, fim) = ObterIntervaloMes(ano, mes);
        var vendas = await _vendaRepository.GetByPeriodoFaturamentoAsync(inicio, fim);

        return vendas
            .Where(v => !v.Faturado)
            .OrderBy(v => v.PrevisaoFaturamento)
            .ToList();
    }

    public decimal ObterPercentualComissao(int descontoOfertado)
    {
        return descontoOfertado switch
        {
            0 => 15m,
            10 => 15m,
            20 => 10m,
            25 => 7m,
            30 => 5m,
            _ => throw new Exception("Desconto ofertado inválido para cálculo de comissão.")
        };
    }

    public decimal CalcularComissao(Venda venda)
    {
        var percentual = ObterPercentualComissao(venda.DescontoOfertado);
        return venda.Valor * percentual / 100m;
    }

    private async Task ValidarVendaAsync(Venda venda)
    {
        if (venda == null)
            throw new Exception("Venda inválida.");

        if (venda.ClienteId <= 0)
            throw new Exception("Cliente inválido.");

        if (venda.FornecedorId <= 0)
            throw new Exception("Fornecedor inválido.");

        if (!await _clienteRepository.ExistsAsync(venda.ClienteId))
            throw new Exception("Cliente não encontrado.");

        if (!await _fornecedorRepository.ExistsAsync(venda.FornecedorId))
            throw new Exception("Fornecedor não encontrado.");

        if (!string.IsNullOrWhiteSpace(venda.Obs) && venda.Obs.Length > 100)
            throw new Exception("A observação deve ter no máximo 100 caracteres.");

        if (venda.Valor <= 0)
            throw new Exception("O valor da venda deve ser maior que zero.");

        if (!DescontoEhValido(venda.DescontoOfertado))
            throw new Exception("O desconto ofertado deve ser um dos valores permitidos: 0, 10, 20, 25 ou 30.");

        if (venda.PrevisaoFaturamento < venda.DataVenda.Date)
            throw new Exception("A previsão de faturamento não pode ser anterior à data da venda.");
    }

    private bool DescontoEhValido(int descontoOfertado)
    {
        return descontoOfertado == 0
            || descontoOfertado == 10
            || descontoOfertado == 20
            || descontoOfertado == 25
            || descontoOfertado == 30;
    }

    private void NormalizarVenda(Venda venda)
    {
        venda.Cliente = null;
        venda.Fornecedor = null;

        venda.Obs = string.IsNullOrWhiteSpace(venda.Obs)
            ? null
            : venda.Obs.Trim();
    }

    private void ValidarPeriodo(DateTime dataInicial, DateTime dataFinal)
    {
        if (dataFinal < dataInicial)
            throw new Exception("A data final não pode ser anterior à data inicial.");
    }

    private void ValidarAnoMes(int ano, int mes)
    {
        if (ano < 2000 || ano > 3000)
            throw new Exception("Ano inválido.");

        if (mes < 1 || mes > 12)
            throw new Exception("Mês inválido.");
    }

    private (DateTime inicio, DateTime fim) ObterIntervaloMes(int ano, int mes)
    {
        var inicio = new DateTime(ano, mes, 1, 0, 0, 0);
        var fim = inicio.AddMonths(1).AddTicks(-1);
        return (inicio, fim);
    }

    public async Task<List<Venda>> GetFaturadasAsync()
    {
        return await _vendaRepository.GetFaturadasAsync();
    }

    public async Task<List<Venda>> GetNaoFaturadasAsync()
    {
        return await _vendaRepository.GetNaoFaturadasAsync();
    }
}