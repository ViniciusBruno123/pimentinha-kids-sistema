using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using CRBVendas.Services.Interfaces;

namespace CRBVendas.Services;

public class FornecedorService : IFornecedorService
{
    private readonly IFornecedorRepository _fornecedorRepository;

    public FornecedorService(IFornecedorRepository fornecedorRepository)
    {
        _fornecedorRepository = fornecedorRepository;
    }

    public async Task<List<Fornecedor>> GetAllAsync()
    {
        return await _fornecedorRepository.GetAllAsync();
    }

    public async Task<Fornecedor?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new Exception("Id inválido.");

        return await _fornecedorRepository.GetByIdAsync(id);
    }

    public async Task<List<Fornecedor>> GetByNomeAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return new List<Fornecedor>();

        return await _fornecedorRepository.GetByNomeAsync(nome.Trim());
    }

    public async Task AddAsync(Fornecedor fornecedor)
    {
        ValidarFornecedor(fornecedor);
        NormalizarFornecedor(fornecedor);
        PrepararRelacionamentos(fornecedor);

        await _fornecedorRepository.AddAsync(fornecedor);
    }

    public async Task UpdateAsync(Fornecedor fornecedor)
    {
        if (fornecedor.Id <= 0)
            throw new Exception("Id inválido para atualização.");

        var fornecedorBanco = await _fornecedorRepository.GetByIdAsync(fornecedor.Id);

        if (fornecedorBanco == null)
            throw new Exception("Fornecedor não encontrado.");

        ValidarFornecedor(fornecedor);
        NormalizarFornecedor(fornecedor);

        fornecedorBanco.Nome = fornecedor.Nome;
        fornecedorBanco.Representante = fornecedor.Representante;

        AtualizarTelefones(fornecedorBanco, fornecedor.Telefones);
        AtualizarEnderecos(fornecedorBanco, fornecedor.Enderecos);

        await _fornecedorRepository.UpdateAsync(fornecedorBanco);
    }

    private void ValidarFornecedor(Fornecedor fornecedor)
    {
        if (fornecedor == null)
            throw new Exception("Fornecedor inválido.");

        if (string.IsNullOrWhiteSpace(fornecedor.Nome))
            throw new Exception("O nome do fornecedor é obrigatório.");

        if (fornecedor.Nome.Trim().Length > 150)
            throw new Exception("O nome do fornecedor deve ter no máximo 150 caracteres.");

        if (!string.IsNullOrWhiteSpace(fornecedor.Representante) && fornecedor.Representante.Trim().Length > 150)
            throw new Exception("O representante deve ter no máximo 150 caracteres.");
    }

    private void NormalizarFornecedor(Fornecedor fornecedor)
    {
        fornecedor.Nome = fornecedor.Nome.Trim();
        fornecedor.Representante = string.IsNullOrWhiteSpace(fornecedor.Representante)
            ? null
            : fornecedor.Representante.Trim();

        fornecedor.Telefones ??= new List<FornecedorTelefone>();
        fornecedor.Enderecos ??= new List<FornecedorEndereco>();

        fornecedor.Telefones = fornecedor.Telefones
            .Where(t => t != null && !string.IsNullOrWhiteSpace(t.Telefone))
            .Select(t => new FornecedorTelefone
            {
                Id = t.Id,
                FornecedorId = t.FornecedorId,
                Telefone = SomenteNumeros(t.Telefone)
            })
            .DistinctBy(t => t.Telefone)
            .ToList();

        fornecedor.Enderecos = fornecedor.Enderecos
            .Where(e =>
                e != null &&
                !string.IsNullOrWhiteSpace(e.Logradouro) &&
                !string.IsNullOrWhiteSpace(e.Cidade))
            .Select(e => new FornecedorEndereco
            {
                Id = e.Id,
                FornecedorId = e.FornecedorId,
                Logradouro = e.Logradouro.Trim(),
                Numero = string.IsNullOrWhiteSpace(e.Numero) ? null : e.Numero.Trim(),
                Cidade = e.Cidade.Trim(),
                CEP = string.IsNullOrWhiteSpace(e.CEP) ? null : SomenteNumeros(e.CEP)
            })
            .ToList();
    }

    private static string SomenteNumeros(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }

    private void PrepararRelacionamentos(Fornecedor fornecedor)
    {
        foreach (var telefone in fornecedor.Telefones)
        {
            telefone.Fornecedor = null;
        }

        foreach (var endereco in fornecedor.Enderecos)
        {
            endereco.Fornecedor = null;
        }
    }

    private void AtualizarTelefones(Fornecedor fornecedorBanco, List<FornecedorTelefone>? telefonesNovos)
    {
        telefonesNovos ??= new List<FornecedorTelefone>();

        fornecedorBanco.Telefones.Clear();

        foreach (var telefone in telefonesNovos)
        {
            fornecedorBanco.Telefones.Add(new FornecedorTelefone
            {
                Id = 0,
                FornecedorId = fornecedorBanco.Id,
                Telefone = telefone.Telefone
            });
        }
    }

    private void AtualizarEnderecos(Fornecedor fornecedorBanco, List<FornecedorEndereco>? enderecosNovos)
    {
        enderecosNovos ??= new List<FornecedorEndereco>();

        fornecedorBanco.Enderecos.Clear();

        foreach (var endereco in enderecosNovos)
        {
            fornecedorBanco.Enderecos.Add(new FornecedorEndereco
            {
                Id = 0,
                FornecedorId = fornecedorBanco.Id,
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Cidade = endereco.Cidade,
                CEP = endereco.CEP
            });
        }
    }
    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new Exception("Id inválido.");

        await _fornecedorRepository.DeleteAsync(id);
    }
}