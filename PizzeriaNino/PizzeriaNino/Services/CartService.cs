using Newtonsoft.Json;
using PizzeriaNino.Data;
using PizzeriaNino.Models;
using System.Collections.Generic;
using System.Linq;

namespace PizzeriaNino.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public void AddToCart(Pizza pizza, int quantity)
        {
            var cartItems = GetCartItems();

            var cartItem = cartItems.FirstOrDefault(c => c.Pizza.Id == pizza.Id);
            if (cartItem == null)
            {
                cartItems.Add(new CartItem { Pizza = pizza, Quantity = quantity });
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            SaveCartItems(cartItems);
        }

        public List<CartItem> GetCartItems()
        {
            var cartItemsJson = Session.GetString("CartItems");
            return string.IsNullOrEmpty(cartItemsJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson);
        }

        private void SaveCartItems(List<CartItem> cartItems)
        {
            var cartItemsJson = JsonConvert.SerializeObject(cartItems);
            Session.SetString("CartItems", cartItemsJson);
        }

        public void ClearCart()
        {
            Session.Remove("CartItems");
        }
    }
}
