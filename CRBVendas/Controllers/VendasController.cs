using CRBVendas.Models;
using CRBVendas.Models.ViewModels;
using CRBVendas.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRBVendas.Controllers;

public class VendasController : Controller
{
    private readonly IVendaService _vendaService;
    private readonly IClienteService _clienteService;
    private readonly IFornecedorService _fornecedorService;

    public VendasController(
        IVendaService vendaService,
        IClienteService clienteService,
        IFornecedorService fornecedorService)
    {
        _vendaService = vendaService;
        _clienteService = clienteService;
        _fornecedorService = fornecedorService;
    }

    public async Task<IActionResult> Index(int? clienteId, int? fornecedorId, DateTime? dataInicial, DateTime? dataFinal)
    {
        var vm = new VendaIndexViewModel
        {
            ClienteId = clienteId,
            FornecedorId = fornecedorId,
            DataInicial = dataInicial,
            DataFinal = dataFinal
        };

        var vendas = await _vendaService.GetAllAsync();

        if (clienteId.HasValue && clienteId.Value > 0)
            vendas = vendas.Where(v => v.ClienteId == clienteId.Value).ToList();

        if (fornecedorId.HasValue && fornecedorId.Value > 0)
            vendas = vendas.Where(v => v.FornecedorId == fornecedorId.Value).ToList();

        if (dataInicial.HasValue)
            vendas = vendas.Where(v => v.DataVenda.Date >= dataInicial.Value.Date).ToList();

        if (dataFinal.HasValue)
            vendas = vendas.Where(v => v.DataVenda.Date <= dataFinal.Value.Date).ToList();

        vm.Vendas = vendas
            .OrderByDescending(v => v.DataVenda)
            .ToList();

        await CarregarCombosAsync(vm);

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var venda = await _vendaService.GetByIdAsync(id);

        if (venda == null)
            return NotFound();

        ViewBag.Comissao = _vendaService.CalcularComissao(venda);
        ViewBag.PercentualComissao = _vendaService.ObterPercentualComissao(venda.DescontoOfertado);

        return View(venda);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new VendaFormViewModel
        {
            DataVenda = DateTime.Today,
            PrevisaoFaturamento = DateTime.Today,
            DescontoOfertado = 0
        };

        await CarregarCombosAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendaFormViewModel vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await CarregarCombosAsync(vm);
                return View(vm);
            }

            var venda = MapearParaEntidade(vm);

            await _vendaService.AddAsync(venda);

            TempData["Sucesso"] = "Venda cadastrada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CarregarCombosAsync(vm);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var venda = await _vendaService.GetByIdAsync(id);

        if (venda == null)
            return NotFound();

        var vm = new VendaFormViewModel
        {
            Id = venda.Id,
            ClienteId = venda.ClienteId,
            FornecedorId = venda.FornecedorId,
            Valor = venda.Valor,
            DataVenda = venda.DataVenda,
            PrevisaoFaturamento = venda.PrevisaoFaturamento,
            Faturado = venda.Faturado,
            DescontoOfertado = venda.DescontoOfertado,
            Obs = venda.Obs
        };

        await CarregarCombosAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VendaFormViewModel vm)
    {
        if (id != vm.Id)
            return BadRequest();

        try
        {
            if (!ModelState.IsValid)
            {
                await CarregarCombosAsync(vm);
                return View(vm);
            }

            var venda = MapearParaEntidade(vm);

            await _vendaService.UpdateAsync(venda);

            TempData["Sucesso"] = "Venda atualizada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CarregarCombosAsync(vm);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var venda = await _vendaService.GetByIdAsync(id);

        if (venda == null)
            return NotFound();

        ViewBag.Comissao = _vendaService.CalcularComissao(venda);
        return View(venda);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _vendaService.DeleteAsync(id);
            TempData["Sucesso"] = "Venda excluída com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarFaturada(int id)
    {
        try
        {
            await _vendaService.MarcarComoFaturadaAsync(id);
            TempData["Sucesso"] = "Venda marcada como faturada.";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarNaoFaturada(int id)
    {
        try
        {
            await _vendaService.MarcarComoNaoFaturadaAsync(id);
            TempData["Sucesso"] = "Venda marcada como não faturada.";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Mes(int? ano, int? mes)
    {
        var hoje = DateTime.Today;
        var anoRef = ano ?? hoje.Year;
        var mesRef = mes ?? hoje.Month;

        var vm = new VendaMesViewModel
        {
            Ano = anoRef,
            Mes = mesRef,
            ComissaoFaturada = await _vendaService.ObterComissaoFaturadaNoMesAsync(anoRef, mesRef),
            ComissaoPendente = await _vendaService.ObterComissaoPendenteNoMesAsync(anoRef, mesRef),
            VendasFaturadas = await _vendaService.ObterVendasFaturadasNoMesAsync(anoRef, mesRef),
            VendasPendentes = await _vendaService.ObterVendasPendentesNoMesAsync(anoRef, mesRef)
        };

        return View(vm);
    }

    private async Task CarregarCombosAsync(VendaFormViewModel vm)
    {
        var clientes = await _clienteService.GetAllAsync();
        var fornecedores = await _fornecedorService.GetAllAsync();

        vm.Clientes = clientes
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nome
            })
            .ToList();

        vm.Fornecedores = fornecedores
            .OrderBy(f => f.Nome)
            .Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Nome
            })
            .ToList();

        vm.Descontos = new List<SelectListItem>
        {
            new() { Value = "0", Text = "0%" },
            new() { Value = "10", Text = "10%" },
            new() { Value = "20", Text = "20%" },
            new() { Value = "25", Text = "25%" },
            new() { Value = "30", Text = "30%" }
        };
    }

    private async Task CarregarCombosAsync(VendaIndexViewModel vm)
    {
        var clientes = await _clienteService.GetAllAsync();
        var fornecedores = await _fornecedorService.GetAllAsync();

        vm.Clientes = new List<SelectListItem>
        {
            new() { Value = "", Text = "Todos" }
        };

        vm.Clientes.AddRange(clientes
            .OrderBy(c => c.Nome)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nome
            }));

        vm.Fornecedores = new List<SelectListItem>
        {
            new() { Value = "", Text = "Todos" }
        };

        vm.Fornecedores.AddRange(fornecedores
            .OrderBy(f => f.Nome)
            .Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Nome
            }));
    }

    private static Venda MapearParaEntidade(VendaFormViewModel vm)
    {
        return new Venda
        {
            Id = vm.Id,
            ClienteId = vm.ClienteId,
            FornecedorId = vm.FornecedorId,
            Valor = vm.Valor,
            DataVenda = vm.DataVenda,
            PrevisaoFaturamento = vm.PrevisaoFaturamento,
            Faturado = vm.Faturado,
            DescontoOfertado = vm.DescontoOfertado,
            Obs = vm.Obs
        };
    }
}