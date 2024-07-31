using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzeriaNino.Models;
using PizzeriaNino.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]
public class PizzeController : Controller
{
    private readonly PizzeriaContext _context;

    public PizzeController(PizzeriaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var pizze = await _context.Pizze.Include(p => p.PizzaIngredienti)
                                         .ThenInclude(pi => pi.Ingrediente)
                                         .ToListAsync();
        return View(pizze);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pizza = await _context.Pizze.Include(p => p.PizzaIngredienti)
                                        .ThenInclude(pi => pi.Ingrediente)
                                        .FirstOrDefaultAsync(m => m.Id == id);
        if (pizza == null)
        {
            return NotFound();
        }

        return View(pizza);
    }

    public IActionResult Create()
    {
        ViewData["Ingredienti"] = _context.Ingredienti.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PizzaCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var pizza = new Pizza
            {
                Nome = model.Nome,
                Foto = model.Foto,
                Prezzo = model.Prezzo,
                Tempo = model.Tempo
            };

            _context.Pizze.Add(pizza);
            await _context.SaveChangesAsync();

            if (model.IngredientiIds != null && model.IngredientiIds.Any())
            {
                foreach (var ingredienteId in model.IngredientiIds)
                {
                    _context.PizzaIngrediente.Add(new PizzaIngrediente
                    {
                        PizzaId = pizza.Id,
                        IngredienteId = ingredienteId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["Ingredienti"] = _context.Ingredienti.ToList();
        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pizza = await _context.Pizze.Include(p => p.PizzaIngredienti)
                                        .ThenInclude(pi => pi.Ingrediente)
                                        .FirstOrDefaultAsync(m => m.Id == id);
        if (pizza == null)
        {
            return NotFound();
        }

        ViewData["Ingredienti"] = _context.Ingredienti.ToList();
        return View(pizza);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PizzaCreateViewModel model)
    {

        if (ModelState.IsValid)
        {
            var pizza = await _context.Pizze.Include(p => p.PizzaIngredienti)
                                            .FirstOrDefaultAsync(p => p.Id == id);
            if (pizza == null)
            {
                return NotFound();
            }

            // Aggiorna le proprietà della pizza
            pizza.Nome = model.Nome;
            pizza.Foto = model.Foto;
            pizza.Prezzo = model.Prezzo;
            pizza.Tempo = model.Tempo;

            try
            {
                _context.Pizze.Update(pizza);
                await _context.SaveChangesAsync();

                // Rimuovi gli ingredienti esistenti
                var existingIngredients = _context.PizzaIngrediente.Where(pi => pi.PizzaId == id).ToList();
                _context.PizzaIngrediente.RemoveRange(existingIngredients);
                await _context.SaveChangesAsync();

                // Aggiungi i nuovi ingredienti
                if (model.IngredientiIds != null && model.IngredientiIds.Any())
                {
                    foreach (var ingredienteId in model.IngredientiIds)
                    {
                        _context.PizzaIngrediente.Add(new PizzaIngrediente
                        {
                            PizzaId = pizza.Id,
                            IngredienteId = ingredienteId
                        });
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaExists(pizza.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["Ingredienti"] = _context.Ingredienti.ToList();
        return View(model);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pizza = await _context.Pizze.Include(p => p.PizzaIngredienti)
                                        .ThenInclude(pi => pi.Ingrediente)
                                        .FirstOrDefaultAsync(m => m.Id == id);
        if (pizza == null)
        {
            return NotFound();
        }

        return View(pizza);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pizza = await _context.Pizze.FindAsync(id);
        _context.Pizze.Remove(pizza);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PizzaExists(int id)
    {
        return _context.Pizze.Any(e => e.Id == id);
    }
}
