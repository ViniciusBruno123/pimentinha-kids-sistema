using CRBVendas.Data;
using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRBVendas.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly AppDbContext _context;

    public VendaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Venda>> GetAllAsync()
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<Venda>> GetByClienteAsync(int clienteId)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<List<Venda>> GetByFornecedorAsync(int fornecedorId)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .Where(v => v.FornecedorId == fornecedorId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<List<Venda>> GetByPeriodoVendaAsync(DateTime dataInicial, DateTime dataFinal)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .Where(v => v.DataVenda >= dataInicial && v.DataVenda <= dataFinal)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<List<Venda>> GetByPeriodoFaturamentoAsync(DateTime dataInicial, DateTime dataFinal)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .Where(v => v.PrevisaoFaturamento >= dataInicial && v.PrevisaoFaturamento <= dataFinal)
            .OrderBy(v => v.PrevisaoFaturamento)
            .ToListAsync();
    }

    public async Task AddAsync(Venda venda)
    {
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Venda venda)
    {
        _context.Vendas.Update(venda);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var venda = await _context.Vendas.FindAsync(id);

        if (venda == null)
            throw new Exception("Venda não encontrada.");

        _context.Vendas.Remove(venda);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Vendas.AnyAsync(v => v.Id == id);
    
    }

    public async Task<List<Venda>> GetFaturadasAsync()
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .Where(v => v.Faturado)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<List<Venda>> GetNaoFaturadasAsync()
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Fornecedor)
            .Where(v => !v.Faturado)
            .OrderBy(v => v.PrevisaoFaturamento)
            .ToListAsync();
    }
}