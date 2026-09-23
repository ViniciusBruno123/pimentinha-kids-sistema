using CRBVendas.Models;
using CRBVendas.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRBVendas.Controllers;

public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    public async Task<IActionResult> Index(string? nome, string? nomefantasia, string? cidade)
    {
        List<Cliente> clientes;

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            clientes = await _clienteService.GetByCidadeAsync(cidade);
            ViewBag.FiltroCidade = cidade;
            ViewBag.FiltroNome = nome;
            ViewBag.FiltroNomeFantasia = nomefantasia;
        }
        else if (!string.IsNullOrWhiteSpace(nome))
        {
            clientes = await _clienteService.GetByNomeAsync(nome);
            ViewBag.FiltroNome = nome;
            ViewBag.FiltroCidade = cidade;
            ViewBag.FiltroNomeFantasia = nomefantasia;
        }
        else if (!string.IsNullOrWhiteSpace(nomefantasia))
        {
            clientes = await _clienteService.GetByNomeFantasiaAsync(nomefantasia);
            ViewBag.FiltroNome = nome;
            ViewBag.FiltroNomeFantasia = nomefantasia;
            ViewBag.FiltroCidade = cidade;
        }
        else
        {
            clientes = await _clienteService.GetAllAsync();
            ViewBag.FiltroNome = nome;
            ViewBag.FiltroNomeFantasia = nomefantasia;
            ViewBag.FiltroCidade = cidade;
        }

        return View(clientes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var cliente = new Cliente
        {
            Telefones = new List<ClienteTelefone> { new ClienteTelefone() },
            Enderecos = new List<ClienteEndereco> { new ClienteEndereco() }
        };

        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        try
        {
            RemoverItensVazios(cliente);

            if (!ModelState.IsValid)
            {
                GarantirColecoes(cliente);
                return View(cliente);
            }

            await _clienteService.AddAsync(cliente);

            TempData["Sucesso"] = "Cliente cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            GarantirColecoes(cliente);
            return View(cliente);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);

        if (cliente == null)
            return NotFound();

        GarantirColecoes(cliente);
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
            return BadRequest();

        try
        {
            RemoverItensVazios(cliente);

            if (!ModelState.IsValid)
            {
                GarantirColecoes(cliente);
                return View(cliente);
            }

            await _clienteService.UpdateAsync(cliente);

            TempData["Sucesso"] = "Cliente atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            GarantirColecoes(cliente);
            return View(cliente);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _clienteService.DeleteAsync(id);
            TempData["Sucesso"] = "Cliente excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    private void GarantirColecoes(Cliente cliente)
    {
        cliente.Telefones ??= new List<ClienteTelefone>();
        cliente.Enderecos ??= new List<ClienteEndereco>();

        if (!cliente.Telefones.Any())
            cliente.Telefones.Add(new ClienteTelefone());

        if (!cliente.Enderecos.Any())
            cliente.Enderecos.Add(new ClienteEndereco());
    }

    private void RemoverItensVazios(Cliente cliente)
    {
        cliente.Telefones ??= new List<ClienteTelefone>();
        cliente.Enderecos ??= new List<ClienteEndereco>();

        cliente.Telefones = cliente.Telefones
            .Where(t => !string.IsNullOrWhiteSpace(t.Telefone))
            .ToList();

        cliente.Enderecos = cliente.Enderecos
            .Where(e =>
                !string.IsNullOrWhiteSpace(e.Logradouro) ||
                !string.IsNullOrWhiteSpace(e.Numero) ||
                !string.IsNullOrWhiteSpace(e.Cidade) ||
                !string.IsNullOrWhiteSpace(e.CEP))
            .ToList();
    }
}