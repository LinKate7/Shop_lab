using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.Models;

namespace ShopWebApplication.Services
{
	public class CategoryImportService : IImportService<Category>
	{
		private readonly ShopContext _context;

		public CategoryImportService(ShopContext context)
		{
			_context = context;
		}

		public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
		{
			if(!stream.CanRead)
			{
				throw new ArgumentException("Data cannot be read", nameof(stream));
			}

			using (XLWorkbook workbook = new XLWorkbook(stream))
			{
				foreach (IXLWorksheet worksheet in workbook.Worksheets)
				{
					var categoryName = worksheet.Name;
					var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName, cancellationToken);
					if (category == null)
					{
						category = new Category();
						category.CategoryName = categoryName;
						_context.Add(category);
					}

					foreach (var row in worksheet.RowsUsed().Skip(1))
					{
						await AddProductAsync(row, cancellationToken, category);
					}
				}
			}
			await _context.SaveChangesAsync(cancellationToken);
		}

		private async Task AddProductAsync(IXLRow row, CancellationToken cancellationToken, Category category)
		{
			var product = new Product();
			product.ProductName = GetProductName(row);
			product.Price = GetProductPrice(row);
			product.Description = GetProductDescription(row);
			product.Category = category;
			product.ImageUrl = GetProductImageURL(row);

			await GetSizesAsync(row, product, cancellationToken);
		}

		private static string GetProductName(IXLRow row)
		{
			return row.Cell(1).Value.ToString();
		}

		private static int GetProductPrice(IXLRow row)
		{
			return (int)row.Cell(2).Value;
		}

        private static string GetProductDescription(IXLRow row)
        {
            return row.Cell(3).Value.ToString();
        }

        private static string GetProductImageURL(IXLRow row)
        {
            return row.Cell(4).Value.ToString();
        }

		private async Task GetSizesAsync(IXLRow row, Product product, CancellationToken cancellationToken)
		{
			for(int i = 5; i <= 8; i++)
			{
				if(row.Cell(i).Value.ToString().Length > 0)
				{
					var sizeName = row.Cell(i).Value.ToString();
					var size = await _context.Sizes.FirstOrDefaultAsync(s => s.SizeName == sizeName, cancellationToken);
					if(size == null)
					{
						size = new Size();
						size.SizeName = sizeName;
						_context.Sizes.Add(size);
					}

					ProductSize productSize = new ProductSize();
					productSize.Product = product;
					productSize.Size = size;

					_context.Add(productSize);
                }
			}
			await _context.SaveChangesAsync();
		}
    }
}

