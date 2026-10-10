using HeroesWeb.Data;
using HeroesWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroesWeb.Controllers;

public class LaboratorioEfController : Controller
{
    private readonly HeroesContext _context;

    public LaboratorioEfController(HeroesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var heroe = await _context.Heroes
            .FirstOrDefaultAsync(h => h.Id == id);

        if (heroe is null)
            return NotFound();

        return View(heroe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id, string? nombre, bool guardar = false)
    {
        var heroe = await _context.Heroes
            .FirstOrDefaultAsync(h => h.Id == id);

        if (heroe is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(nombre) ||
            nombre.Trim().Length > 100)
        {
            ModelState.AddModelError(
                "Nombre",
                "Ingrese un nombre de 1 a 100 caracteres.");

            return View(heroe);
        }

        var nombreOriginal = heroe.Nombre;

        heroe.Nombre = nombre.Trim();

        _context.ChangeTracker.DetectChanges();

        var estadoAntes = _context.Entry(heroe).State;

        if (guardar)
        {
            await _context.SaveChangesAsync();

            TempData["Resultado"] =
                $"Antes: {estadoAntes}. Después: " +
                $"{_context.Entry(heroe).State}. " +
                "Se ejecutó SaveChangesAsync; compruebe el dato en SQL Server.";

            return RedirectToAction(
                nameof(Editar),
                new { id });
        }

        ViewData["Resultado"] =
            $"Original: {nombreOriginal}. " +
            $"En memoria: {heroe.Nombre}. " +
            $"Estado: {estadoAntes}. " +
            "No se llamó a SaveChangesAsync.";

        return View(heroe);
    }
    [HttpGet]
    public async Task<IActionResult> Ciudad(int id)
    {
        var ciudad = await _context.Ciudades
            .Include(c => c.Villanos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (ciudad is null)
            return NotFound();

        return Json(new
        {
            ciudad.Id,
            ciudad.Nombre,
            ciudad.Pais,

            Villanos = ciudad.Villanos.Select(v => new
            {
                v.Alias,
                v.NivelAmenaza,
                v.Recompensa,
                v.FechaRegistro
            }).ToList()
        });
    }

    [HttpGet]
    public IActionResult ModeloVillano()
    {
        var entidad =
            _context.Model.FindEntityType(typeof(Villano))!;

        return Json(new
        {
            Tabla = entidad.GetTableName(),

            Esquema = entidad.GetSchema(),

            Columnas = entidad.GetProperties()
                .Select(p => new
                {
                    Propiedad = p.Name,
                    Columna = p.GetColumnName(),
                    Obligatoria = !p.IsNullable,
                    Longitud = p.GetMaxLength(),
                    Precision = p.GetPrecision(),
                    Escala = p.GetScale(),
                    DefaultSql = p.GetDefaultValueSql()
                })
        });
    }

    [HttpGet]
    public async Task<IActionResult> Cuartel(int id)
    {
        var cuartel = await _context.Cuarteles
            .Include(c => c.Vehiculos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cuartel is null)
            return NotFound();

        return Json(new
        {
            cuartel.Id,
            cuartel.Nombre,
            cuartel.Capacidad,

            Vehiculos =
                cuartel.Vehiculos.Select(v => new
                {
                    v.Placa,
                    v.Modelo,
                    v.CostoMantenimiento,
                    v.FechaAlta
                }).ToList()
        });
    }
}