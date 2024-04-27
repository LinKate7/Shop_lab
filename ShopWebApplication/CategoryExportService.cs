﻿using ShopWebApplication.Models;
using ShopWebApplication.Services;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace ShopWebApplication
{
	public class CategoryExportService : IExportService<Category>
	{
        private const string RootWorksheetName = "";
        private readonly ShopContext _context;

        public CategoryExportService(ShopContext context)
        {
            _context = context;
        }

        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Product Name",
                "Price",
                "Description",
                "ImageURL",
                "Size1",
                "Size2",
                "Size3",
            };
    
        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private void WriteProduct(IXLWorksheet worksheet, Product product, int rowIndex)
        {
            var columnIndex = 1;

            worksheet.Cell(rowIndex, columnIndex++).Value = product.ProductName;
            worksheet.Cell(rowIndex, columnIndex++).Value = product.Price;
            worksheet.Cell(rowIndex, columnIndex++).Value = product.Description;
            worksheet.Cell(rowIndex, columnIndex++).Value = product.ImageUrl;

            var productSizes = _context.ProductSizes.Where(ps => ps.ProductId == product.ProductId)
                                                    .Include(ps => ps.Size)
                                                    .Distinct();
            foreach (var ps in productSizes.Take(3))
            {
                if (ps.SizeId != 0)
                {
                    var size = ps.Size;
                    worksheet.Cell(rowIndex, columnIndex++).Value = size.SizeName;
                }
            }
        }

        private void WriteProducts(IXLWorksheet worksheet, ICollection<Product> products)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var product in products)
            {
                WriteProduct(worksheet, product, rowIndex);
                rowIndex++;
            }
        }

        private void WriteCategories(XLWorkbook workbook, ICollection<Category> categories)
        {  
            foreach (var category in categories)
            {

                if (category is not null)
                {
                    var worksheet = workbook.Worksheets.Add(category.CategoryName);
                    WriteProducts(worksheet, category.Products.ToList());
                }
            }
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }
            var categories = await _context.Categories
                .Include(category => category.Products)
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();

            WriteCategories(workbook, categories);
            workbook.SaveAs(stream);
        }

    }

}


