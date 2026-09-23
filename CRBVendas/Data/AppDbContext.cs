using CRBVendas.Models;
using Microsoft.EntityFrameworkCore;

namespace CRBVendas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ClienteTelefone> ClienteTelefones => Set<ClienteTelefone>();
    public DbSet<ClienteEndereco> ClienteEnderecos => Set<ClienteEndereco>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<FornecedorTelefone> FornecedorTelefones => Set<FornecedorTelefone>();
    public DbSet<FornecedorEndereco> FornecedorEnderecos => Set<FornecedorEndereco>();
    public DbSet<Venda> Vendas => Set<Venda>();

    public DbSet<Contato> Contatos => Set<Contato>();
    public DbSet<ContatoTelefone> ContatoTelefones => Set<ContatoTelefone>();
    public DbSet<ContatoEndereco> ContatoEnderecos => Set<ContatoEndereco>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClienteTelefone>()
            .HasOne(x => x.Cliente)
            .WithMany(x => x.Telefones)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClienteEndereco>()
            .HasOne(x => x.Cliente)
            .WithMany(x => x.Enderecos)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FornecedorTelefone>()
            .HasOne(x => x.Fornecedor)
            .WithMany(x => x.Telefones)
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FornecedorEndereco>()
            .HasOne(x => x.Fornecedor)
            .WithMany(x => x.Enderecos)
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Venda>()
            .HasOne(x => x.Cliente)
            .WithMany(x => x.Vendas)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venda>()
            .HasOne(x => x.Fornecedor)
            .WithMany(x => x.Vendas)
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venda>()
            .Property(x => x.Valor)
            .HasPrecision(12, 2);

            modelBuilder.Entity<ContatoTelefone>()
                .HasOne(x => x.Contato)
                .WithMany(x => x.Telefones)
                .HasForeignKey(x => x.ContatoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContatoEndereco>()
                .HasOne(x => x.Contato)
                .WithOne(x => x.Endereco)
                .HasForeignKey<ContatoEndereco>(x => x.ContatoId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}