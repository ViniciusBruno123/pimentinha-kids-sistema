using CRBVendas.Data;
using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRBVendas.Repositories;

public class FornecedorRepository : IFornecedorRepository
{
    private readonly AppDbContext _context;

    public FornecedorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Fornecedor>> GetAllAsync()
    {
        return await _context.Fornecedores
            .Include(f => f.Telefones)
            .Include(f => f.Enderecos)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<Fornecedor?> GetByIdAsync(int id)
    {
        return await _context.Fornecedores
            .Include(f => f.Telefones)
            .Include(f => f.Enderecos)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<List<Fornecedor>> GetByNomeAsync(string nome)
    {
        nome = nome.Trim();

        return await _context.Fornecedores
            .Include(f => f.Telefones)
            .Include(f => f.Enderecos)
            .Where(f => f.Nome.Contains(nome))
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task AddAsync(Fornecedor fornecedor)
    {
        _context.Fornecedores.Add(fornecedor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Fornecedor fornecedor)
    {
        _context.Fornecedores.Update(fornecedor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);

        if (fornecedor == null)
            throw new Exception("Fornecedor não encontrado.");

        _context.Fornecedores.Remove(fornecedor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Fornecedores.AnyAsync(f => f.Id == id);
    }
}