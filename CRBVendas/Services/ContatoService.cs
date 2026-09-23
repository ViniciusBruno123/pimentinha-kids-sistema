using CRBVendas.Models;
using CRBVendas.Repositories.Interfaces;
using CRBVendas.Services.Interfaces;

namespace CRBVendas.Services;

public class ContatoService : IContatoService
{
    private readonly IContatoRepository _repository;

    public ContatoService(IContatoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Contato>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Contato?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new Exception("Id inválido.");

        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Contato>> GetByNomeAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return new List<Contato>();

        return await _repository.GetByNomeAsync(nome.Trim());
    }

    public async Task<int> GetTotalRegistrosAsync(string? cidade = null)
    {
        return await _repository.GetTotalRegistrosAsync(cidade);
    }

    public async Task AddAsync(Contato contato)
    {
        Validar(contato);
        Normalizar(contato);
        PrepararRelacionamentos(contato);

        await _repository.AddAsync(contato);
    }

    public async Task UpdateAsync(Contato contato)
    {
        if (contato.Id <= 0)
            throw new Exception("Contato inválido.");

        var contatoBanco = await _repository.GetByIdAsync(contato.Id);

        if (contatoBanco == null)
            throw new Exception("Contato não encontrado.");

        Validar(contato);
        Normalizar(contato);

        contatoBanco.NomeFantasia = contato.NomeFantasia;
        contatoBanco.NomeContato = contato.NomeContato;
        contatoBanco.AgendaAtual = contato.AgendaAtual;
        contatoBanco.AgendaProxima = contato.AgendaProxima;
        contatoBanco.Observacao = contato.Observacao;

        AtualizarTelefones(contatoBanco, contato.Telefones);

        contatoBanco.Endereco = contato.Endereco;

        await _repository.UpdateAsync(contatoBanco);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new Exception("Id inválido.");

        await _repository.DeleteAsync(id);
    }

    private void Validar(Contato contato)
    {
        if (contato == null)
            throw new Exception("Contato inválido.");

        if (string.IsNullOrWhiteSpace(contato.NomeFantasia))
            throw new Exception("Nome fantasia é obrigatório.");

        if (contato.NomeFantasia.Length > 150)
            throw new Exception("Nome fantasia muito grande.");

        if (!string.IsNullOrWhiteSpace(contato.NomeContato) &&
            contato.NomeContato.Length > 150)
            throw new Exception("Nome do contato muito grande.");

        if (!string.IsNullOrWhiteSpace(contato.Observacao) &&
            contato.Observacao.Length > 500)
            throw new Exception("Observação muito grande.");
    }

    private void Normalizar(Contato contato)
    {
        contato.NomeFantasia = contato.NomeFantasia.Trim();

        contato.NomeContato = string.IsNullOrWhiteSpace(contato.NomeContato)
            ? null
            : contato.NomeContato.Trim();

        contato.Observacao = string.IsNullOrWhiteSpace(contato.Observacao)
            ? null
            : contato.Observacao.Trim();

        contato.Telefones ??= new();

        contato.Telefones = contato.Telefones
            .Where(t => !string.IsNullOrWhiteSpace(t.Telefone))
            .DistinctBy(t => t.Telefone)
            .ToList();

        if (contato.Endereco != null)
        {
            contato.Endereco.Logradouro = contato.Endereco.Logradouro.Trim();
            contato.Endereco.Cidade = contato.Endereco.Cidade.Trim();
            contato.Endereco.Numero = contato.Endereco.Numero?.Trim();
            contato.Endereco.CEP = contato.Endereco.CEP?.Trim();
        }
    }

    private void PrepararRelacionamentos(Contato contato)
    {
        foreach (var telefone in contato.Telefones)
            telefone.Contato = null;

        if (contato.Endereco != null)
            contato.Endereco.Contato = null;
    }

    private void AtualizarTelefones(Contato contatoBanco, List<ContatoTelefone>? telefones)
    {
        contatoBanco.Telefones.Clear();

        if (telefones == null)
            return;

        foreach (var telefone in telefones)
        {
            contatoBanco.Telefones.Add(new ContatoTelefone
            {
                ContatoId = contatoBanco.Id,
                Telefone = telefone.Telefone
            });
        }
    }

    public async Task<List<Contato>> GetAgendaAsync(
        string? cidade,
        int pagina = 1,
        int tamanhoPagina = 20)
    {
        if (pagina < 1)
            pagina = 1;


        if (tamanhoPagina <= 0)
            tamanhoPagina = 20;

        return await _repository.GetAgendaAsync(
            cidade,
            pagina,
            tamanhoPagina);
    }
}