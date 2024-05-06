using System.Data.SqlClient;
using tutorial6.Models;

namespace tutorial6.Repository;

public class ProductRepository
{
    public bool CheckProduct(IConfiguration _configuration, int id)
    {
        using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        con.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = con;
        command.CommandText = "SELECT count(*) FROM [Product] where IdProduct=@id";
        command.Parameters.AddWithValue("id", id);

        var count = (int)command.ExecuteScalar();
        return count != 0;
    }

    public bool CheckWarehouse(IConfiguration _configuration, int id)
    {
        using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        con.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = con;
        command.CommandText = "SELECT count(*) FROM [Warehouse] where IdWarehouse=@id";
        command.Parameters.AddWithValue("id", id);
        
        var count = (int)command.ExecuteScalar();
        return count != 0;
    }

    public (bool orderExists, int orderId) CheckOrder(IConfiguration _configuration, int productId, int warehouseId, int amount, DateTime createdAt)
    {
        using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        con.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = con;
        command.CommandText = "SELECT TOP 1 IdOrder FROM [Order] WHERE IdProduct = @IdProduct  AND Amount = @Amount  AND CreatedAt < @CreatedAt;";
        command.Parameters.AddWithValue("IdProduct", productId);
        command.Parameters.AddWithValue("Amount", amount);
        command.Parameters.AddWithValue("CreatedAt", createdAt);
        
        var orderId = (int?)command.ExecuteScalar();
        return (orderId.HasValue, orderId.GetValueOrDefault());
    }

    public bool IsOrderCompleted(IConfiguration _configuration, int orderId)
    {
        using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        con.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = con;
        command.CommandText = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @OrderId";
        command.Parameters.AddWithValue("@OrderId", orderId);
        
        int count = (int)command.ExecuteScalar();
        return count > 0;
    }

    public void UpdateOrderFulfilledAt(IConfiguration _configuration, int orderId)
    {
        using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        con.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = con;
        command.CommandText = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @OrderId";
        command.Parameters.AddWithValue("@OrderId", orderId);
        command.ExecuteNonQuery();
    }

    public decimal GetProductPrice(IConfiguration _configuration, int productId)
    {
        decimal price = 0;
        using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        con.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = con;
        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @ProductId";
        command.Parameters.AddWithValue("@ProductId", productId);
        
        SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            price = reader.GetDecimal(0);
        }
        reader.Close();

        return price;
    }

    public int InsertProductWarehouse(IConfiguration _configuration, int productId, int warehouseId, int amount, decimal price, DateTime createdAt)
    {
        int productWarehouseId = 0;
        
        using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Default"));
        con.Open();
        SqlTransaction transaction = con.BeginTransaction();
        using SqlCommand command = con.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, Amount, Price, CreatedAt) VALUES (@IdProduct, @IdWarehouse, @Amount, @Price, @CreatedAt)";
        command.Parameters.AddWithValue("@IdProduct", productId);
        command.Parameters.AddWithValue("@IdWarehouse", warehouseId);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@Price", price);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);
        
        productWarehouseId = Convert.ToInt32(command.ExecuteScalar());
        
        return productWarehouseId;
    }
    
}