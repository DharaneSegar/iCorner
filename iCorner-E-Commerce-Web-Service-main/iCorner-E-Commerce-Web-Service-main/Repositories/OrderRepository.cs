namespace EAD.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EAD.Models;
    using MongoDB.Driver;

    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Cart> _carts;

        public OrderRepository(IMongoDatabase database)
        {
            _orders = database.GetCollection<Order>("Orders");
            _products = database.GetCollection<Product>("Products");
            _carts = database.GetCollection<Cart>("Carts");
        }

        // Get all orders
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(_ => true).ToListAsync();
        }

        // Get order by ID
        public async Task<Order> GetOrderByIdAsync(string id)
        {
            return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
        }

        // Get orders by Customer ID
        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            return await _orders.Find(o => o.CustomerId == customerId).ToListAsync();
        }

        // Get orders by vendor ID
        public async Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorEmail)
        {
            // Fetch the products associated with the given vendor
            var products = await _products.Find(p => p.VendorId == vendorEmail).ToListAsync();

            // Get the product IDs from the vendor's products
            var vendorProductIds = products.Select(p => p.Id).ToList();

            if (!vendorProductIds.Any())
            {
                // If no products found for the vendor, return an empty list
                return Enumerable.Empty<Order>();
            }

            // Retrieve all orders and filter in memory
            var orders = await _orders.Find(o => true).ToListAsync();
            var filteredOrders = new List<Order>();

            foreach (var order in orders)
            {
                // Retrieve the associated cart for each order
                var cart = await _carts.Find(c => c.Id == order.Cart).FirstOrDefaultAsync();

                // Check if the cart and its items match any of the vendor's products
                if (cart != null && cart.Items != null)
                {
                    // Check if any of the cart items match the vendor's products
                    if (cart.Items.Any(item => vendorProductIds.Contains(item.ProductId)))
                    {
                        filteredOrders.Add(order);
                    }
                }
            }

            return filteredOrders;
        }

        // Create a new order
        public async Task CreateOrderAsync(Order order)
        {
            Console.WriteLine("Order ID >|: " + order.Id);
            if (string.IsNullOrEmpty(order.Id))
            {
                order.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                await _orders.InsertOneAsync(order);
            }
            else
            {
                Console.WriteLine("Order ID Sucess :  " + order.Id);
                await UpdateOrderAsync(order.Id, order);
            }
        }

        // Update an existing order
        public async Task<Order> UpdateOrderAsync(string id, Order order)
        {
            var result = await _orders.ReplaceOneAsync(o => o.Id == id, order);

            if (result.MatchedCount == 0)
            {
                throw new Exception("Order not found for update.");
            }

            return order;
        }

        // Delete an order by ID
        public async Task DeleteOrderAsync(string id)
        {
            var result = await _orders.DeleteOneAsync(o => o.Id == id);

            if (result.DeletedCount == 0)
            {
                throw new Exception("Order not found for deletion.");
            }
        }

        // Get Products of Vendor in an order
        public async Task<
            IEnumerable<(Product Product, int Quantity, string Status)>
        > GetVendorProductsInOrderAsync(string vendorEmail, string orderId)
        {
            Console.WriteLine(
                $"[DEBUG] Starting GetVendorProductsInOrderAsync for Vendor: {vendorEmail}, Order ID: {orderId}"
            );

            // Fetch the order by ID
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                Console.WriteLine($"[DEBUG] Order with ID {orderId} not found.");
                throw new Exception("Order not found.");
            }
            Console.WriteLine($"[DEBUG] Found Order: {order.Id}, Cart ID: {order.Cart}");

            // Fetch the cart associated with the order
            var cart = await _carts.Find(c => c.Id == order.Cart).FirstOrDefaultAsync();
            if (cart == null)
            {
                Console.WriteLine($"[DEBUG] Cart with ID {order.Cart} not found.");
                return Enumerable.Empty<(Product, int, string)>();
            }
            if (cart.Items == null || !cart.Items.Any())
            {
                Console.WriteLine($"[DEBUG] Cart with ID {order.Cart} has no items.");
                return Enumerable.Empty<(Product, int, string)>();
            }
            Console.WriteLine(
                $"[DEBUG] Found Cart: {cart.Id}, Number of Items: {cart.Items.Count}"
            );

            // Prepare a list to hold the vendor's products, their quantities, and status
            var vendorProducts = new List<(Product, int, string)>();

            // Loop through the items in the cart
            foreach (var item in cart.Items)
            {
                Console.WriteLine(
                    $"[DEBUG] Processing Cart Item: Product ID: {item.ProductId}, Quantity: {item.Quantity}"
                );

                // Fetch the product by productId
                var product = await _products
                    .Find(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();
                if (product == null)
                {
                    Console.WriteLine($"[DEBUG] Product with ID {item.ProductId} not found.");
                    continue;
                }
                Console.WriteLine(
                    $"[DEBUG] Found Product: {product.Id}, Vendor ID: {product.VendorId}"
                );

                // Check if the product belongs to the vendor
                if (product.VendorId == vendorEmail)
                {
                    Console.WriteLine(
                        $"[DEBUG] Product {product.Id} belongs to Vendor {vendorEmail}, adding to result."
                    );
                    // Assuming item.Status contains the status
                    vendorProducts.Add((product, item.Quantity, item.Status)); // Include item.Status here
                }
                else
                {
                    Console.WriteLine(
                        $"[DEBUG] Product {product.Id} does not belong to Vendor {vendorEmail}, skipping."
                    );
                }
            }

            Console.WriteLine(
                $"[DEBUG] Completed processing. Total products for vendor: {vendorProducts.Count}"
            );

            return vendorProducts;
        }

        // Update the status of accepted items in a vendor's order
        public async Task<bool> AcceptVendorProductsInOrderAsync(string vendorEmail, string orderId)
        {
            Console.WriteLine(
                $"[DEBUG] Starting AcceptVendorProductsInOrderAsync for Vendor: {vendorEmail}, Order ID: {orderId}"
            );

            // Fetch the order by ID
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                Console.WriteLine($"[DEBUG] Order with ID {orderId} not found.");
                throw new Exception("Order not found.");
            }
            Console.WriteLine($"[DEBUG] Found Order: {order.Id}, Cart ID: {order.Cart}");

            // Fetch the cart associated with the order
            var cart = await _carts.Find(c => c.Id == order.Cart).FirstOrDefaultAsync();
            if (cart == null)
            {
                Console.WriteLine($"[DEBUG] Cart with ID {order.Cart} not found.");
                return false;
            }
            if (cart.Items == null || !cart.Items.Any())
            {
                Console.WriteLine($"[DEBUG] Cart with ID {order.Cart} has no items.");
                return false;
            }
            Console.WriteLine(
                $"[DEBUG] Found Cart: {cart.Id}, Number of Items: {cart.Items.Count}"
            );

            // Track whether all items are accepted
            bool allItemsAccepted = true;

            // Loop through the items in the cart
            foreach (var item in cart.Items)
            {
                Console.WriteLine(
                    $"[DEBUG] Processing Cart Item: Product ID: {item.ProductId}, Quantity: {item.Quantity}, Current Status: {item.Status}"
                );

                // Fetch the product by productId
                var product = await _products
                    .Find(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();
                if (product == null)
                {
                    Console.WriteLine($"[DEBUG] Product with ID {item.ProductId} not found.");
                    allItemsAccepted = false;
                    continue;
                }
                Console.WriteLine(
                    $"[DEBUG] Found Product: {product.Id}, Vendor ID: {product.VendorId}"
                );

                // Check if the product belongs to the vendor
                if (product.VendorId == vendorEmail)
                {
                    // Update the item's status to 'accepted'
                    item.Status = "accepted"; // Update status to accepted

                    Console.WriteLine(
                        $"[DEBUG] Product {product.Id} belongs to Vendor {vendorEmail}, status updated to accepted."
                    );
                }
                else
                {
                    Console.WriteLine(
                        $"[DEBUG] Product {product.Id} does not belong to Vendor {vendorEmail}, skipping."
                    );
                    allItemsAccepted = false;
                }
            }

            // Check if all items have been accepted
            if (allItemsAccepted)
            {
                // Update order status to 'dispatched'
                order.Status = "Dispatched"; // Update order status
                await _orders.ReplaceOneAsync(o => o.Id == order.Id, order); // Update the order in the database
                Console.WriteLine(
                    $"[DEBUG] All items accepted. Order ID {order.Id} status updated to dispatched."
                );
            }
            else
            {
                Console.WriteLine($"[DEBUG] Not all items were accepted for Order ID {order.Id}.");
            }

            return allItemsAccepted;
        }
    }
}
