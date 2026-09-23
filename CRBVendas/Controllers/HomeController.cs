using CRBVendas.Models.ViewModels;
using CRBVendas.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRBVendas.Controllers;

public class HomeController : Controller
{
    private readonly IVendaService _vendaService;

    public HomeController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    public async Task<IActionResult> Index(int? ano, int? mes)
    {
        var hoje = DateTime.Today;
        var anoReferencia = ano ?? hoje.Year;
        var mesReferencia = mes ?? hoje.Month;

        var vm = new HomeIndexViewModel
        {
            AnoReferencia = anoReferencia,
            MesReferencia = mesReferencia,
            ComissaoFaturadaNoMes = await _vendaService.ObterComissaoFaturadaNoMesAsync(anoReferencia, mesReferencia),
            ComissaoPendenteNoMes = await _vendaService.ObterComissaoPendenteNoMesAsync(anoReferencia, mesReferencia),
            VendasFaturadasNoMes = await _vendaService.ObterVendasFaturadasNoMesAsync(anoReferencia, mesReferencia),
            VendasPendentesNoMes = await _vendaService.ObterVendasPendentesNoMesAsync(anoReferencia, mesReferencia)
        };

        vm.QuantidadeVendasFaturadasNoMes = vm.VendasFaturadasNoMes.Count;
        vm.QuantidadeVendasPendentesNoMes = vm.VendasPendentesNoMes.Count;

        vm.FaturamentoBrutoNoMes = vm.VendasFaturadasNoMes.Sum(v => v.Valor)
            + vm.VendasPendentesNoMes.Sum(v => v.Valor);

        return View(vm);
    }
}