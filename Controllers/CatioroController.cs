using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using escola_dos_catioros.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using escola_dos_catioros.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage;

namespace escola_dos_catioros.Controllers;

public class CatioroController(ILogger<CatioroController> logger, EscolaDbContext context) : Controller
{
    private readonly EscolaDbContext _db = context;
    private readonly ILogger<CatioroController> _logger = logger;

    public async Task<ActionResult> Index()
    {
        var lista_catioros = await _db.Catioros.ToListAsync();
        return View(lista_catioros);
    }

    [HttpGet]
    public ActionResult Create() => View();

    [HttpPost]
    public async Task<ActionResult> Create(Catioro c)
    {
        if (!ModelState.IsValid)
        {
            return View(c);
        }
        try
        {
            _db.Catioros.Add(c);
            await _db.SaveChangesAsync();
            Console.WriteLine($"Redirecionando para Details com id={c.Id}");
            return RedirectToAction("Detail", new { id = c.Id });
        }
        catch (DbUpdateException e)
        {
            // não sei o que pode ter acontecido, então mostra aquela tela feia mesmo.
            if (e.GetBaseException() is not SqliteException sqlite_ex)
                throw;
            var erro = sqlite_ex.SqliteErrorCode switch
            {
                19 => "Já existe um cachorro chamado " + c.Nome,
                _ => "Não sei o que aconteceu, mas foi coisa algo grave"
            };
            // como você sabe que foi no Nome que deu erro?
            // porque eu defini uma constraint unique no nome
            // mas podem acontecer outras coisas estranhas e elas vão aparecer
            // debaixo do nome
            ModelState.AddModelError("Nome", erro);
        }
        return View(c);
    }



    [HttpGet]
    public async Task<ActionResult> Edit(int? id)
    {
        if (!id.HasValue)
        {
            return RedirectToAction("Index");
        }
        var catioro = await _db.Catioros.FirstOrDefaultAsync(c => c.Id == id.Value);
        if (catioro == null)
            return View("NaoEncontrado");
        return View(catioro);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(Catioro c)
    {
        if (!ModelState.IsValid)
        {
            return View(c);
        }
        try
        {
            _db.Entry(c).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = c.Id });
        }
        catch (DbUpdateException e)
        {
            if (e.GetBaseException() is not SqliteException sqlite_ex)
                throw;
            var erro = sqlite_ex.SqliteErrorCode switch
            {
                19 => "Já existe outro cachorro chamado " + c.Nome,
                _ => sqlite_ex.Message
            };
            ModelState.AddModelError("Nome", erro);
        }
        return View(c);

    }

    public async Task<ActionResult> Detail(int? id)
    {
        if (!id.HasValue)
            return RedirectToAction("Index");
        var catioro = await _db.Catioros.FirstOrDefaultAsync(c => c.Id == id.Value);
        if (catioro == null)
            return View("NaoEncontrado");
        return View(catioro);        
    }

    [HttpGet]
    public ActionResult Delete(int? id)
    {
        if (!id.HasValue)
        {
            return RedirectToAction("Index");
        }
        var catioro = _db.Catioros.FirstOrDefault(c => c.Id == id.Value);
        if (catioro == null)
            return View("NaoEncontrado");
        return View(catioro);
    }

    [HttpPost]
    public async Task<ActionResult> Delete(Catioro c)
    {
        if (c == null || c.Id == 0)
        {
            return View("NaoEncontrado");
        }
        _db.Catioros.Remove(c);
        await _db.SaveChangesAsync();
        Console.WriteLine($"Redirecionando para Deleted");
        return RedirectToAction("Deleted");
    }

    public ActionResult Deleted()
    {
        return View();
    }

    public IActionResult NaoEncontrado()
    {
        Response.StatusCode = 404;
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public ActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
