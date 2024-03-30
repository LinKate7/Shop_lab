using ShopWebApplication.Models;

namespace ShopWebApplication;

public interface ICartRepository
{
	public Task<int> AddItemAsync(int productId, int quantity);
	public Task<int> RemoveItemAsync(int productId);
	public Task<Cart> GetCartAsync(string userId);
	public Task<IEnumerable<Cart>> GetUserCartAsync();
	public Task<int> GetCartItemCountAsync(string userId);
}