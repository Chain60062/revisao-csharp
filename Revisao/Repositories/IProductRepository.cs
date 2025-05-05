using System;
using Revisao.Models;

namespace Revisao.Repositories;

public interface IProductRepository
{
    void Add(Product produto);
    List<Product> GetProducts();
}
