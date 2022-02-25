using DataAccess;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductManager
    {
        private readonly AgencyContext _context;

        public ProductManager(AgencyContext context)
        {
            _context = context;
        }
        public async Task Add(Product product)
        {
           await _context.Products.AddAsync(product);
           await _context.SaveChangesAsync();
        }
        public async Task Update(Product product)
        {
             _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int? id)
        {
            var selectedProduct=await _context.Products.FirstOrDefaultAsync(x=>x.Id==id && !x.IsDeleted);
            if (selectedProduct == null) return;
            selectedProduct.IsDeleted=true;
            await _context.SaveChangesAsync();
        }
        public async Task<List<Product>> GetAll()
        {
           return await _context.Products
                .Include(x => x.ProductPictures)
                .ThenInclude(x => x.Picture)
                .Include(x => x.ProductRecords)
                .Include(x => x.Category)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x=>x.PublishDate).ToListAsync();

        }
        public async Task<List<Product>> SearchProduct(string? q,int? categoryId,decimal? minPrice,decimal? maxPrice,int? sortBy)
        {
            var products = _context.Products
                .Include(x => x.ProductPictures)
                .ThenInclude(x => x.Picture)
                .Include(x => x.ProductRecords)
                .Include(x => x.Category)
                .Where(x=>!x.IsDeleted)
                .AsQueryable();
            if (categoryId.HasValue)
            {
                products = products.Where(x => x.CategoryId == categoryId);
            }
            if(minPrice.HasValue && maxPrice.HasValue)
            {
                products=products.Where(x=>x.Price>=minPrice && x.Price<=maxPrice);
            }
            if(sortBy != null)
            {
                products = sortBy switch
                {
                    1 => products.OrderByDescending(x => x.Price),
                    2 => products.OrderBy(x => x.Price),
                    _ => products.OrderByDescending(x => x.PublishDate),
                };
            }
            if (!string.IsNullOrWhiteSpace(q))
            {
              products=products.Where(x=>x.ProductRecords.Any(x=>x.Name.ToLower().Contains(q.ToLower())));
            }
            return products.OrderByDescending(x => x.PublishDate).ToList();
        }
        public async Task<Product> GetById(int id)
        {
             var selectedProduct=await _context.Products.FirstOrDefaultAsync(x=>x.Id==id && !x.IsDeleted);
            if (selectedProduct == null) return null;
            return selectedProduct;
        }
    }
}
