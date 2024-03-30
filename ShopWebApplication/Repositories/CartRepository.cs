using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication;
using ShopWebApplication.Models;

public class CartRepository : ICartRepository
{
    private readonly ShopContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartRepository(ShopContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<int> AddItemAsync(int productId, int quantity)
    {
        var userId = GetUserId();
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cart = await GetCartAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.CartItemQuantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    CartItemQuantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }

        return await GetCartItemCountAsync(userId);
    }

    public async Task<int> RemoveItemAsync(int productId)
    {
        var userId = GetUserId();

        var cart = await GetCartAsync(userId);
        if (cart == null)
        {
            throw new Exception("There is no such cart.");
        }

        var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == productId);
        if (cartItem == null)
        {
            throw new Exception("The cart is already empty.");
        }

        if (cartItem.CartItemQuantity > 1)
        {
            cartItem.CartItemQuantity -= 1;
        }
        else
        {
            _context.CartItems.Remove(cartItem);
        }

        await _context.SaveChangesAsync();

        return await GetCartItemCountAsync(userId);
    }

    public async Task<Cart> GetCartAsync(string userId)
    {
        return await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<IEnumerable<Cart>> GetUserCartAsync()
    {
        var userId = GetUserId();
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return cart;
    }

    private string GetUserId()
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        return _userManager.GetUserId(user);
    }

    public async Task<int> GetCartItemCountAsync(string userId)
    {
        var cartItemData = await _context.Carts
            .Where(c => c.UserId == userId)
            .SelectMany(c => c.CartItems)
            .ToListAsync();

        return cartItemData.Sum(item => item.CartItemQuantity);
    }
}
