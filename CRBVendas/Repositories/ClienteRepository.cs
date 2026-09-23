using CRBVendas.Data;
using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRBVendas.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
            .Include(c => c.Telefones)
            .Include(c => c.Enderecos)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.Telefones)
            .Include(c => c.Enderecos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Cliente>> GetByNomeAsync(string nome)
    {
        nome = nome.Trim();

        return await _context.Clientes
            .Include(c => c.Telefones)
            .Include(c => c.Enderecos)
            .Where(c => c.Nome.Contains(nome))
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<Cliente>> GetByNomeFantasiaAsync(string nomefantasia)
    {
        nomefantasia = nomefantasia.Trim();

        return await _context.Clientes
            .Include(c => c.Telefones)
            .Include(c => c.Enderecos)
            .Where(c => c.NomeFantasia.Contains(nomefantasia))
            .OrderBy(c => c.NomeFantasia)
            .ToListAsync();
    }

    public async Task<List<Cliente>> GetByCidadeAsync(string cidade)
    {
        cidade = cidade.Trim();

        return await _context.Clientes
            .Include(c => c.Telefones)
            .Include(c => c.Enderecos)
            .Where(c => c.Enderecos.Any(e => e.Cidade.Contains(cidade)))
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            throw new Exception("Cliente não encontrado.");

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Clientes.AnyAsync(c => c.Id == id);
    }
}