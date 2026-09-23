using CRBVendas.Models;
using CRBVendas.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRBVendas.Controllers;

public class ContatosController : Controller
{
    private readonly IContatoService _contatoService;

    private const int TamanhoPagina = 20;

    public ContatosController(IContatoService contatoService)
    {
        _contatoService = contatoService;
    }


    public async Task<IActionResult> Index(string? cidade, int pagina = 1)
    {
        if (pagina < 1)
            pagina = 1;

        var contatos = await _contatoService
            .GetAgendaAsync(cidade, pagina, TamanhoPagina);

        var total = await _contatoService
            .GetTotalRegistrosAsync(cidade);

        ViewBag.PaginaAtual = pagina;
        ViewBag.Cidade = cidade;
        ViewBag.TotalPaginas = Math.Max(1, (int)Math.Ceiling(
            total / (double)TamanhoPagina
        ));

        return View(contatos);
    }


    public async Task<IActionResult> Details(int id)
    {
        var contato = await _contatoService.GetByIdAsync(id);

        if (contato == null)
            return NotFound();

        return View(contato);
    }


    [HttpGet]
    public IActionResult Create()
    {
        var contato = new Contato
        {
            Telefones = new List<ContatoTelefone>
            {
                new ContatoTelefone()
            }
        };

        return View(contato);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Contato contato)
    {
        try
        {
            RemoverItensVazios(contato);

            if (!ModelState.IsValid)
            {
                GarantirColecoes(contato);
                return View(contato);
            }

            await _contatoService.AddAsync(contato);

            TempData["Sucesso"] =
                "Contato cadastrado com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch(Exception ex)
        {
            ModelState.AddModelError("", ex.Message);

            GarantirColecoes(contato);

            return View(contato);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var contato = await _contatoService.GetByIdAsync(id);

        if (contato == null)
            return NotFound();

        GarantirColecoes(contato);

        return View(contato);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Contato contato)
    {
        if(id != contato.Id)
            return BadRequest();


        try
        {
            RemoverItensVazios(contato);

            if(!ModelState.IsValid)
            {
                GarantirColecoes(contato);
                return View(contato);
            }


            await _contatoService.UpdateAsync(contato);


            TempData["Sucesso"] =
                "Contato atualizado com sucesso.";


            return RedirectToAction(nameof(Index));
        }
        catch(Exception ex)
        {
            ModelState.AddModelError("", ex.Message);

            GarantirColecoes(contato);

            return View(contato);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var contato = await _contatoService.GetByIdAsync(id);

        if(contato == null)
            return NotFound();


        return View(contato);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _contatoService.DeleteAsync(id);

            TempData["Sucesso"] =
                "Contato excluído com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch(Exception ex)
        {
            TempData["Erro"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }



    private void GarantirColecoes(Contato contato)
    {
        contato.Telefones ??= new List<ContatoTelefone>();

        if(!contato.Telefones.Any())
            contato.Telefones.Add(new ContatoTelefone());
    }



    private void RemoverItensVazios(Contato contato)
    {
        contato.Telefones ??= new List<ContatoTelefone>();

        contato.Telefones = contato.Telefones
            .Where(t => !string.IsNullOrWhiteSpace(t.Telefone))
            .ToList();


        if(contato.Endereco != null)
        {
            if(string.IsNullOrWhiteSpace(contato.Endereco.Logradouro) &&
               string.IsNullOrWhiteSpace(contato.Endereco.Cidade))
            {
                contato.Endereco = null;
            }
        }
    }
}