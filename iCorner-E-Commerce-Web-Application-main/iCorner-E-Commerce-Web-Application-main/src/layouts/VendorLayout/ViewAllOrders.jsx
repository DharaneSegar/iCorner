import React, { useEffect, useState } from "react";
import { Table, Container, Spinner, Form } from "react-bootstrap";
import axios from "axios";
import { jwtDecode } from "jwt-decode"; // Ensure jwt-decode is installed

const ViewAllOrders = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filterStatus, setFilterStatus] = useState("All"); // New state for filter
  const [email, setEmail] = useState("");

  // Fetch token from localStorage and decode it to get username
  useEffect(() => {
    const token = localStorage.getItem("token"); // Get token from localStorage
    if (token) {
      const decodedToken = jwtDecode(token);
      setEmail(decodedToken.email);
    }
  }, []);

  useEffect(() => {
    const fetchOrders = async () => {
      const token = localStorage.getItem("token");
      if (token) {
        const decodedToken = jwtDecode(token);
        setEmail(decodedToken.email);
        try {
          const response = await axios.get(
            `http://192.168.109.81/iCorner/api/order/vendor/${decodedToken.email}`
          );
          setOrders(response.data);
        } catch (err) {
          setError("Error fetching orders");
        } finally {
          setLoading(false);
        }
      }
    };

    fetchOrders();
  }, []);

  if (loading) {
    return (
      <Container className="text-center mt-5">
        <Spinner animation="border" />
        <p>Loading orders...</p>
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="text-center mt-5">
        <p>{error}</p>
      </Container>
    );
  }

  // Filter orders based on selected status
  const filteredOrders =
    filterStatus === "All"
      ? orders
      : orders.filter((order) => order.status === filterStatus);

  return (
    <Container className="mt-5">
      <h1 className="mb-4 text-center">All Orders</h1>
      <Form.Group controlId="statusFilter" className="mb-3">
        <Form.Label>Filter by Status</Form.Label>
        <Form.Select
          value={filterStatus}
          onChange={(e) => setFilterStatus(e.target.value)}
        >
          <option value="All">All</option>
          <option value="Pending">Pending</option>
          <option value="Shipped">Shipped</option>
          <option value="Delivered">Delivered</option>
          <option value="Cancelled">Cancelled</option>
        </Form.Select>
      </Form.Group>
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Order ID</th>
            <th>Customer ID</th>
            <th>Cart ID</th>
            <th>Total Price</th>
            <th>Shipping Address</th>
            <th>Order Date</th>
            <th>Status</th>
            <th>Payment Status</th>
            <th>Notes</th>
          </tr>
        </thead>
        <tbody>
          {filteredOrders.map((order) => (
            <tr key={order.id}>
              <td>{order.id}</td>
              <td>{order.customerId}</td>
              <td>{order.cart}</td>
              <td>{order.totalPrice}</td>
              <td>{order.shippingAddress}</td>
              <td>{new Date(order.orderDate).toLocaleString()}</td>
              <td>{order.status}</td>
              <td>{order.paymentStatus}</td>
              <td>{order.notes}</td>
            </tr>
          ))}
        </tbody>
      </Table>
    </Container>
  );
};

export default ViewAllOrders;
