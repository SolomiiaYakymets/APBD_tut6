using System.Data.SqlClient;
using tutorial6.Repository;

namespace tutorial6.Services;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public ProductService()
    {
        _productRepository = new ProductRepository();
    }

    public int AddProduct(IConfiguration _configuration, int productId, int warehouseId, int amount, DateTime createdAt)
    {
        if (!_productRepository.CheckProduct(_configuration, productId))
        {
            throw new ArgumentException($"Product with ID {productId} does not exist.");
        }
        if (!_productRepository.CheckWarehouse(_configuration, warehouseId))
        {
            throw new ArgumentException($"Warehouse with ID {warehouseId} does not exist.");
        }
        var (orderExists, orderId) = _productRepository.CheckOrder(_configuration, productId, warehouseId, amount, createdAt);
        if (!orderExists)
        {
            throw new ArgumentException("There is no matching order in the Order table.");
        }
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0.");
        }
        if (_productRepository.IsOrderCompleted(_configuration, orderId))
        {
            throw new InvalidOperationException("The order has already been completed.");
        }
        
        _productRepository.UpdateOrderFulfilledAt(_configuration, orderId);
        
        decimal price = _productRepository.GetProductPrice(_configuration, productId);
        DateTime currentDateTime = DateTime.Now;
        int productWarehouseId = _productRepository.InsertProductWarehouse(_configuration, warehouseId, productId, orderId,  price * amount, currentDateTime);

        return productWarehouseId;
    }
    
}