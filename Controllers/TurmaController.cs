using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using escola_dos_catioros.Data;
using escola_dos_catioros.Models;
using escola_dos_catioros.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace escola_dos_catioros.Controllers;

public class TurmaController(ILogger<TurmaController> logger, EscolaDbContext context) : Controller
{
    private readonly EscolaDbContext _db = context;
    private readonly ILogger<TurmaController> _logger = logger;

    public ActionResult Index()
    {
        var turmas = _db.Turmas.ToList();
        return View(turmas);
    }

    public ActionResult Create() => View();

    public async Task<ActionResult> DeleteAsync(int? id)
    {
        if (!id.HasValue)
        {
            return RedirectToAction("Index");
        }
        var turma = await _db.Turmas.FirstOrDefaultAsync(t => t.Id == id.Value);
        if (turma == null)
            return View("NaoEncontrado");
        bool tem_matriculas = _db.Matriculas.Any(m => m.TurmaId == id.Value);
        if (tem_matriculas)
            return View("TurmaNaoVazia");
        _db.Turmas.Remove(turma);
        _db.SaveChanges();

        // redirect to "turma not empty" page if 
        // there are matriculas with turmaid = id

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ActionResult> Create(Turma t)
    {
        if (!ModelState.IsValid)
        {
            return View(t);
        }
        try
        {
            _db.Turmas.Add(t);
            await _db.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = t.Id });
        }
        catch (DbUpdateException e)
        {
            if (e.GetBaseException() is not SqliteException sqlite_ex)
                throw;
            var erro = sqlite_ex.SqliteErrorCode switch
            {
                19 => "Já existe um turma chamada " + t.Apelido,
                _ => "Não sei o que aconteceu, mas foi coisa algo grave"
            };
            ModelState.AddModelError("Apelido", erro);
        }
        return View(t);
    }

    public async Task<ActionResult> Detail(int? id)
    {
        if (!id.HasValue)
        {
            return RedirectToAction("Index");
        }
        var t = await _db.Turmas
            .Include(t => t.Matriculas)
            .ThenInclude(m => m.Catioro)
            .FirstOrDefaultAsync(t => t.Id == id.Value);
        if (t == null)
            return View("NaoEncontrado");
        var vm = new TurmaViewModel
        {
            Turma = t,
            Matriculas = t.Matriculas.ToList()
        };
        return View(vm);
    }

    public async Task<ActionResult> Matricular(int? id)
    {
        if (!id.HasValue)
        {
            return RedirectToAction("Index");
        }
        var t = await _db.Turmas
                .Include(t => t.Matriculas)
                .ThenInclude(m => m.Catioro)
                .FirstOrDefaultAsync(t => t.Id == id.Value);
        if (t == null)
            return View("NaoEncontrado");

        var nao_matriculados = await _db.Catioros
            .Where(c => !t.Matriculas.Select(m => m.CatioroId).Contains(c.Id))
            .ToListAsync();

        var vm = new NovaMatriculaViewModel
        {
            Turma = t,
            NaoMatriculados = nao_matriculados
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Matricular(int TurmaId, int CatioroId)
    {
        // eu acho que dá um exception se for uma turma ou
        // catioro que não existem
        var m = new Matricula
        {
            TurmaId = TurmaId,
            CatioroId = CatioroId
        };
        try
        {
            _db.Matriculas.Add(m);
            await _db.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = TurmaId });
        }
        catch (DbUpdateException e)
        {
            if (e.GetBaseException() is not SqliteException sqlite_ex)
                throw;
            ModelState.AddModelError("SQL", sqlite_ex.Message);
            return await Matricular(TurmaId);
        }
    }


    [HttpGet]
    public async Task<ActionResult> Desmatricular(int? id)
    {
        if (!id.HasValue)
            return RedirectToAction("Index");
        var m = await _db.Matriculas
                    .Include(m => m.Turma)
                    .Include(m => m.Catioro)
                    .FirstOrDefaultAsync(m => m.Id == id.Value);
        if (m == null)
            return View("MatriculaNaoEncontrada");
        return View(m);
    }

    [HttpPost]
    public async Task<ActionResult> DesmatricularAsync(Matricula m)
    {
        if (m == null || m.Id == 0)
            return View("Index");
        _db.Matriculas.Remove(m);
        await _db.SaveChangesAsync();
        return RedirectToAction("Detail",new { id = m.TurmaId });
    }

    public ActionResult NaoEncontrado() => View();
    public ActionResult MatriculaNaoEncontrada() => View();
}