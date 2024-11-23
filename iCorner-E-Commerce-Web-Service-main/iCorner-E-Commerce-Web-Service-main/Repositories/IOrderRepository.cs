namespace EAD.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EAD.Models;

    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<Order> GetOrderByIdAsync(string id);

        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId);

        Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorId);

        Task CreateOrderAsync(Order order);

        Task<Order> UpdateOrderAsync(string id, Order order);

        Task DeleteOrderAsync(string id);

        Task<
            IEnumerable<(Product Product, int Quantity, string Status)>
        > GetVendorProductsInOrderAsync(string vendorEmail, string orderId);

        Task<bool> AcceptVendorProductsInOrderAsync(string vendorEmail, string orderId);
    }
}
