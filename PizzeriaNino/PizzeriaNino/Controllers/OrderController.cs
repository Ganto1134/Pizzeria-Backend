using Microsoft.AspNetCore.Mvc;
using PizzeriaNino.Models;
using PizzeriaNino.Data;
using PizzeriaNino.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class OrderController : Controller
{
    private readonly PizzeriaContext _context;
    private readonly CartService _cartService;

    public OrderController(PizzeriaContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }
    public async Task<IActionResult> Menu()
    {
        var pizze = await _context.Pizze.Include(p => p.PizzaIngredienti)
                                         .ThenInclude(pi => pi.Ingrediente)
                                         .ToListAsync();
        return View(pizze);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddToCart(int pizzaId, int quantity)
    {
        Console.WriteLine($"Tentativo di aggiungere Pizza ID: {pizzaId}, Quantità: {quantity}");

        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "La quantità deve essere maggiore di zero.";
            return RedirectToAction("Menu");
        }

        var pizza = _context.Pizze.Include(p => p.PizzaIngredienti)
                                  .ThenInclude(pi => pi.Ingrediente)
                                  .FirstOrDefault(p => p.Id == pizzaId);

        if (pizza != null)
        {
            _cartService.AddToCart(pizza, quantity);
            TempData["SuccessMessage"] = "Pizza aggiunta al carrello con successo!";
        }
        else
        {
            TempData["ErrorMessage"] = "Pizza non trovata.";
        }

        return RedirectToAction("Cart");
    }

    public IActionResult Cart()
    {
        var cartItems = _cartService.GetCartItems();
        return View(cartItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmOrder(string shippingAddress, string notes)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            ModelState.AddModelError("", "L'indirizzo di spedizione è obbligatorio.");
            return RedirectToAction("Cart"); 
        }

        var order = new Order
        {
            ShippingAddress = shippingAddress,
            Notes = notes ?? string.Empty, 
            OrderDate = DateTime.Now,
            Status = "Pending",
            OrderItems = _cartService.GetCartItems().Select(ci => new OrderItem
            {
                PizzaId = ci.Pizza.Id,
                Quantity = ci.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return View("OrderConfirmed", order);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ManageOrders()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var allOrders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Pizza)
            .ToListAsync();

        var completedOrdersToday = allOrders
            .Where(o => o.Status == "Completed" && o.OrderDate >= today && o.OrderDate < tomorrow)
            .ToList();

        var totalOrdersCompleted = completedOrdersToday.Count;
        var totalRevenue = completedOrdersToday.Sum(o => o.OrderItems.Sum(oi => oi.Pizza.Prezzo * oi.Quantity));

        ViewBag.TotalOrdersCompleted = totalOrdersCompleted;
        ViewBag.TotalRevenue = totalRevenue;

        return View(allOrders); 
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAsCompleted(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = "Completed";
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("ManageOrders");
    }
}