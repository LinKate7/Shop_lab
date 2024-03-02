using System;
using ShopWebApplication.Models;

namespace ShopWebApplication
{
	public interface ICartRepository
	{
        Task<int> AddItem(int productId, int quantity);
        Task<int> RemoveItem(int productId);
        Task<Cart> GetCart(int userId);
        Task<IEnumerable<Cart>> GetUserCart();
        Task<int> GetCartItemCount(int userId);

    }
}

