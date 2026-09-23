using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using CRBVendas.Services.Interfaces;

namespace CRBVendas.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _clienteRepository.GetAllAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new Exception("Id inválido.");

        return await _clienteRepository.GetByIdAsync(id);
    }

    public async Task<List<Cliente>> GetByNomeAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return new List<Cliente>();

        return await _clienteRepository.GetByNomeAsync(nome.Trim());
    }

    public async Task<List<Cliente>> GetByNomeFantasiaAsync(string nomefantasia)
    {
        if (string.IsNullOrWhiteSpace(nomefantasia))
            return new List<Cliente>();

        return await _clienteRepository.GetByNomeFantasiaAsync(nomefantasia.Trim());
    }

    public async Task<List<Cliente>> GetByCidadeAsync(string cidade)
    {
        if (string.IsNullOrWhiteSpace(cidade))
            return new List<Cliente>();

        return await _clienteRepository.GetByCidadeAsync(cidade.Trim());
    }

    public async Task AddAsync(Cliente cliente)
    {
        ValidarCliente(cliente);
        NormalizarCliente(cliente);
        PrepararRelacionamentos(cliente);

        await _clienteRepository.AddAsync(cliente);
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        if (cliente.Id <= 0)
            throw new Exception("Id inválido para atualização.");

        var clienteBanco = await _clienteRepository.GetByIdAsync(cliente.Id);

        if (clienteBanco == null)
            throw new Exception("Cliente não encontrado.");

        ValidarCliente(cliente);
        NormalizarCliente(cliente);

        clienteBanco.Nome = cliente.Nome;
        clienteBanco.NomeFantasia = cliente.NomeFantasia;
        clienteBanco.Email = cliente.Email;
        clienteBanco.Representante = cliente.Representante;

        AtualizarTelefones(clienteBanco, cliente.Telefones);
        AtualizarEnderecos(clienteBanco, cliente.Enderecos);

        await _clienteRepository.UpdateAsync(clienteBanco);
    }

    private void ValidarCliente(Cliente cliente)
    {
        if (cliente == null)
            throw new Exception("Cliente inválido.");

        if (string.IsNullOrWhiteSpace(cliente.Nome))
            throw new Exception("O nome do cliente é obrigatório.");

        if (string.IsNullOrWhiteSpace(cliente.NomeFantasia))
            throw new Exception("O nome fantasia do cliente é obrigatório.");

        if (cliente.Nome.Trim().Length > 150)
            throw new Exception("O nome do cliente deve ter no máximo 150 caracteres.");

        if (cliente.NomeFantasia.Trim().Length > 150)
            throw new Exception("O nome fantasia do cliente deve ter no máximo 150 caracteres.");

        if (cliente.Email.Trim().Length > 150)
            throw new Exception("O email do cliente deve ter no máximo 150 caracteres.");

        if (!string.IsNullOrWhiteSpace(cliente.Representante) && cliente.Representante.Trim().Length > 150)
            throw new Exception("O representante deve ter no máximo 150 caracteres.");

        if (string.IsNullOrWhiteSpace(cliente.CNPJ))
            throw new Exception("CNPJ é obrigatório.");

        if (!ValidarCnpj(cliente.CNPJ))
            throw new Exception("CNPJ inválido.");

        if (!string.IsNullOrWhiteSpace(cliente.InscricaoEstadual))
        {
            if (!ValidarIE_SP(cliente.InscricaoEstadual))
                throw new Exception("Inscrição Estadual inválida (SP).");
        }
    }

    private void NormalizarCliente(Cliente cliente)
    {
        cliente.Nome = cliente.Nome.Trim();
        cliente.NomeFantasia = cliente.NomeFantasia.Trim();
        cliente.Email = cliente.Email.Trim();

        cliente.Representante = string.IsNullOrWhiteSpace(cliente.Representante)
            ? null
            : cliente.Representante.Trim();

        cliente.CNPJ = SomenteNumeros(cliente.CNPJ);

        if (!string.IsNullOrWhiteSpace(cliente.InscricaoEstadual))
            cliente.InscricaoEstadual = SomenteNumeros(cliente.InscricaoEstadual);

        cliente.Telefones ??= new List<ClienteTelefone>();
        cliente.Enderecos ??= new List<ClienteEndereco>();

        cliente.Telefones = cliente.Telefones
            .Where(t => t != null && !string.IsNullOrWhiteSpace(t.Telefone))
            .Select(t => new ClienteTelefone
            {
                Id = t.Id,
                ClienteId = t.ClienteId,
                Telefone = SomenteNumeros(t.Telefone)
            })
            .DistinctBy(t => t.Telefone)
            .ToList();

        cliente.Enderecos = cliente.Enderecos
            .Where(e =>
                e != null &&
                !string.IsNullOrWhiteSpace(e.Logradouro) &&
                !string.IsNullOrWhiteSpace(e.Cidade))
            .Select(e => new ClienteEndereco
            {
                Id = e.Id,
                ClienteId = e.ClienteId,
                Logradouro = e.Logradouro.Trim(),
                Numero = string.IsNullOrWhiteSpace(e.Numero) ? null : e.Numero.Trim(),
                Cidade = e.Cidade.Trim(),
                CEP = string.IsNullOrWhiteSpace(e.CEP)
                    ? null
                    : SomenteNumeros(e.CEP)
            })
            .ToList();
    }

    private void PrepararRelacionamentos(Cliente cliente)
    {
        foreach (var telefone in cliente.Telefones)
        {
            telefone.Cliente = null;
        }

        foreach (var endereco in cliente.Enderecos)
        {
            endereco.Cliente = null;
        }
    }

    private void AtualizarTelefones(Cliente clienteBanco, List<ClienteTelefone>? telefonesNovos)
    {
        telefonesNovos ??= new List<ClienteTelefone>();

        clienteBanco.Telefones.Clear();

        foreach (var telefone in telefonesNovos)
        {
            clienteBanco.Telefones.Add(new ClienteTelefone
            {
                Id = 0,
                ClienteId = clienteBanco.Id,
                Telefone = telefone.Telefone
            });
        }
    }

    private void AtualizarEnderecos(Cliente clienteBanco, List<ClienteEndereco>? enderecosNovos)
    {
        enderecosNovos ??= new List<ClienteEndereco>();

        clienteBanco.Enderecos.Clear();

        foreach (var endereco in enderecosNovos)
        {
            clienteBanco.Enderecos.Add(new ClienteEndereco
            {
                Id = 0,
                ClienteId = clienteBanco.Id,
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

        await _clienteRepository.DeleteAsync(id);
    }

    private string SomenteNumeros(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }

    private bool ValidarCnpj(string cnpj)
    {
        cnpj = SomenteNumeros(cnpj);

        if (cnpj.Length != 14)
            return false;

        if (new string(cnpj[0], 14) == cnpj)
            return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string temp = cnpj.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(temp[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        temp += digito;

        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(temp[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        return cnpj.EndsWith(digito);
    }

    private bool ValidarIE_SP(string ie)
    {
        ie = SomenteNumeros(ie);

        if (ie.Length != 12)
            return false;

        return true; // podemos melhorar depois com DV real
    }
}