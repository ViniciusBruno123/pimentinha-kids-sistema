using CRBVendas.Models;
using CRBVendas.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRBVendas.Controllers;

public class FornecedoresController : Controller
{
    private readonly IFornecedorService _fornecedorService;

    public FornecedoresController(IFornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    public async Task<IActionResult> Index(string? nome)
    {
        List<Fornecedor> fornecedores;

        if (!string.IsNullOrWhiteSpace(nome))
        {
            fornecedores = await _fornecedorService.GetByNomeAsync(nome);
            ViewBag.FiltroNome = nome;
        }
        else
        {
            fornecedores = await _fornecedorService.GetAllAsync();
            ViewBag.FiltroNome = nome;
        }

        return View(fornecedores);
    }

    public async Task<IActionResult> Details(int id)
    {
        var fornecedor = await _fornecedorService.GetByIdAsync(id);

        if (fornecedor == null)
            return NotFound();

        return View(fornecedor);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var fornecedor = new Fornecedor
        {
            Telefones = new List<FornecedorTelefone> { new FornecedorTelefone() },
            Enderecos = new List<FornecedorEndereco> { new FornecedorEndereco() }
        };

        return View(fornecedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Fornecedor fornecedor)
    {
        try
        {
            RemoverItensVazios(fornecedor);

            if (!ModelState.IsValid)
            {
                GarantirColecoes(fornecedor);
                return View(fornecedor);
            }

            await _fornecedorService.AddAsync(fornecedor);

            TempData["Sucesso"] = "Fornecedor cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            GarantirColecoes(fornecedor);
            return View(fornecedor);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var fornecedor = await _fornecedorService.GetByIdAsync(id);

        if (fornecedor == null)
            return NotFound();

        GarantirColecoes(fornecedor);
        return View(fornecedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Fornecedor fornecedor)
    {
        if (id != fornecedor.Id)
            return BadRequest();

        try
        {
            RemoverItensVazios(fornecedor);

            if (!ModelState.IsValid)
            {
                GarantirColecoes(fornecedor);
                return View(fornecedor);
            }

            await _fornecedorService.UpdateAsync(fornecedor);

            TempData["Sucesso"] = "Fornecedor atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            GarantirColecoes(fornecedor);
            return View(fornecedor);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var fornecedor = await _fornecedorService.GetByIdAsync(id);

        if (fornecedor == null)
            return NotFound();

        return View(fornecedor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _fornecedorService.DeleteAsync(id);
            TempData["Sucesso"] = "Fornecedor excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    private void GarantirColecoes(Fornecedor fornecedor)
    {
        fornecedor.Telefones ??= new List<FornecedorTelefone>();
        fornecedor.Enderecos ??= new List<FornecedorEndereco>();

        if (!fornecedor.Telefones.Any())
            fornecedor.Telefones.Add(new FornecedorTelefone());

        if (!fornecedor.Enderecos.Any())
            fornecedor.Enderecos.Add(new FornecedorEndereco());
    }

    private void RemoverItensVazios(Fornecedor fornecedor)
    {
        fornecedor.Telefones ??= new List<FornecedorTelefone>();
        fornecedor.Enderecos ??= new List<FornecedorEndereco>();

        fornecedor.Telefones = fornecedor.Telefones
            .Where(t => !string.IsNullOrWhiteSpace(t.Telefone))
            .ToList();

        fornecedor.Enderecos = fornecedor.Enderecos
            .Where(e =>
                !string.IsNullOrWhiteSpace(e.Logradouro) ||
                !string.IsNullOrWhiteSpace(e.Numero) ||
                !string.IsNullOrWhiteSpace(e.Cidade) ||
                !string.IsNullOrWhiteSpace(e.CEP))
            .ToList();
    }
}