using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzeriaNino.Models;
using PizzeriaNino.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]
public class IngredientiController : Controller
{
    private readonly PizzeriaContext _context;

    public IngredientiController(PizzeriaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ingredienti = await _context.Ingredienti.ToListAsync();
        return View(ingredienti);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IngredientiCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var ingrediente = new Ingrediente
            {
                Nome = model.Nome
            };

            _context.Ingredienti.Add(ingrediente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ingrediente = await _context.Ingredienti.FindAsync(id);
        if (ingrediente == null)
        {
            return NotFound();
        }
        return View(ingrediente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] Ingrediente ingrediente)
    {
        if (id != ingrediente.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(ingrediente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredienteExists(ingrediente.Id))
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
        return View(ingrediente);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ingrediente = await _context.Ingredienti
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ingrediente == null)
        {
            return NotFound();
        }

        return View(ingrediente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ingrediente = await _context.Ingredienti.FindAsync(id);
        _context.Ingredienti.Remove(ingrediente);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool IngredienteExists(int id)
    {
        return _context.Ingredienti.Any(e => e.Id == id);
    }
}
