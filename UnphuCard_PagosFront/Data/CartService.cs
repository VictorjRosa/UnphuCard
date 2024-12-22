using UnphuCard_Api.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using System.Text.Json;

namespace UnphuCard_PagosFront.Data
{
    public class CartService
    {
        private readonly List<CartItem> cart = new();
        private readonly IJSRuntime jsRuntime;
        private const string StorageKey = "CartData";

        public CartService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task InitializeCartAsync()
        {
            var savedCart = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(savedCart))
            {
                var items = JsonSerializer.Deserialize<List<CartItem>>(savedCart);
                cart.Clear();
                cart.AddRange(items);
            }
        }

        public class CartItem : VwProducto
        {
            public int Quantity { get; set; }
        }

        public IReadOnlyList<CartItem> GetCartItems() => cart.AsReadOnly();

        public async Task AddToCartAsync(CartItem item)
        {
            var existingItem = cart.FirstOrDefault(i => i.IdDelProducto == item.IdDelProducto);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
            await SaveCartAsync();
        }

        public async Task UpdateCartItemQuantityAsync(int id, int newQuantity)
        {
            var item = cart.FirstOrDefault(i => i.IdDelProducto == id);
            if (item != null)
            {
                if (newQuantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = newQuantity;
                }
                await SaveCartAsync();
            }
        }

        public async Task RemoveFromCartAsync(int id)
        {
            cart.RemoveAll(item => item.IdDelProducto == id);
            await SaveCartAsync();
        }

        public decimal GetTotal() => cart.Sum(item => (item.PrecioDelProducto ?? 0) * item.Quantity);

        public async Task ClearCartAsync()
        {
            cart.Clear();
            await SaveCartAsync();
        }

        private async Task SaveCartAsync()
        {
            var serializedCart = JsonSerializer.Serialize(cart);
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, serializedCart);
        }
    }


}

