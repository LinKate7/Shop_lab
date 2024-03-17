using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.Models;

namespace ShopWebApplication.Repositories
{
	public class CartRepository : ICartRepository
	{
        private readonly ShopContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(ShopContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
		{
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int productId, int quantity)
        {
            using var transaction = _context.Database.BeginTransaction();
            int userId = GetUserId();
            try
            {
                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId
                    };
                    _context.Carts.Add(cart);
                }
                _context.SaveChanges();

                var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartId == cart.CartId && ci.ProductId == productId);
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
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            { }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> RemoveItem(int productId)
        {
            int userId = GetUserId();

            try
            {
                var cart = await GetCart(userId);
                if (cart == null)
                {
                    throw new Exception("There is no such cart.");
                }

                var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartId == cart.CartId && ci.ProductId == productId);
                if (cartItem == null)
                {
                    throw new Exception("The cart is already empty.");
                }
                if (cartItem.CartItemQuantity == 1)
                {
                    _context.CartItems.Remove(cartItem);
                }
                if (cartItem != null)
                {
                    cartItem.CartItemQuantity -= 1;
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }


        public async Task<Cart> GetCart(int userId)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            return cart;

        }

        public async Task<IEnumerable<Cart>> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                throw new Exception("Invalid UserId");
            }
            var cart = _context.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .Where(c => c.UserId == userId);
            return cart;
        }

        public int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            int userId = Convert.ToInt32(_userManager.GetUserId(user));
            return userId;
        }

        public async Task<int> GetCartItemCount(int userId)
        {
            if (userId == 0)
            {
                userId = GetUserId();
            }

            var cartItemData = await (from cart in _context.Carts
                                      join cartItem in _context.CartItems
                                      on cart.CartId equals cartItem.CartId
                                      where cart.UserId == userId
                                      select (cartItem)).ToListAsync();
            return cartItemData.Count();
        }
    }
}

