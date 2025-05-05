using Revisao.Data;
using Revisao.Models;

namespace Revisao.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDataContext _context;
    public ProductRepository(AppDataContext context)
    {
        _context = context;
    }
    public void Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public List<Product> GetProducts()
    {
        return _context.Products.ToList();
    }
}
