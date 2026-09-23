using CRBVendas.Data;
using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRBVendas.Repositories;

public class ContatoRepository : IContatoRepository
{
    private readonly AppDbContext _context;

    public ContatoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Contato>> GetAllAsync()
    {
        return await _context.Contatos
            .Include(c => c.Telefones)
            .Include(c => c.Endereco)
            .OrderBy(c => c.NomeFantasia)
            .ToListAsync();
    }

    public async Task<Contato?> GetByIdAsync(int id)
    {
        return await _context.Contatos
            .Include(c => c.Telefones)
            .Include(c => c.Endereco)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Contato>> GetByNomeAsync(string nome)
    {
        nome = nome.Trim();

        return await _context.Contatos
            .Include(c => c.Telefones)
            .Include(c => c.Endereco)
            .Where(c => c.NomeFantasia.Contains(nome))
            .OrderBy(c => c.NomeFantasia)
            .ToListAsync();
    }

    public async Task<int> GetTotalRegistrosAsync(string? cidade = null)
    {
        var query = _context.Contatos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(c =>
                c.Endereco != null &&
                c.Endereco.Cidade.Contains(cidade));
        }

        return await query.CountAsync();
    }

    public async Task AddAsync(Contato contato)
    {
        _context.Contatos.Add(contato);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Contato contato)
    {
        _context.Contatos.Update(contato);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var contato = await _context.Contatos.FindAsync(id);

        if (contato == null)
            throw new Exception("Contato não encontrado.");

        _context.Contatos.Remove(contato);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Contatos.AnyAsync(c => c.Id == id);
    }

    public async Task<List<Contato>> GetAgendaAsync(
        string? cidade = null,
        int pagina = 1,
        int tamanhoPagina = 20)
    {
        var hoje = DateTime.Today;

        var query = _context.Contatos
            .Include(c => c.Telefones)
            .Include(c => c.Endereco)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(c =>
                c.Endereco != null &&
                c.Endereco.Cidade.Contains(cidade));
        }

        return await query
            .OrderBy(c =>
                c.AgendaProxima.HasValue &&
                c.AgendaProxima.Value < hoje
                    ? 1
                    : 0)
            .ThenBy(c =>
                c.AgendaProxima ?? DateTime.MaxValue)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
    }
}