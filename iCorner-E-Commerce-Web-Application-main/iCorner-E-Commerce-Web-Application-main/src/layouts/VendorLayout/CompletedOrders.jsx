import React, { useEffect, useState } from 'react';
import { Table, Container, Spinner } from 'react-bootstrap';
import axios from 'axios';

const CompletedOrders = () => {
    const [completedOrders, setCompletedOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchCompletedOrders = async () => {
            try {
                const response = await axios.get('http://192.168.109.81/iCorner/api/order/vendor/sajeesiva12@gmail.com');
                // Filter only completed orders
                const filteredOrders = response.data.filter(order => order.status === 'Completed');
                setCompletedOrders(filteredOrders);
            } catch (err) {
                setError('Error fetching completed orders');
            } finally {
                setLoading(false);
            }
        };

        fetchCompletedOrders();
    }, []);

    if (loading) {
        return (
            <Container className="text-center mt-5">
                <Spinner animation="border" />
                <p>Loading completed orders...</p>
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

    return (
        <Container className="mt-5">
            <h1 className="mb-4 text-center">Completed Orders</h1>
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
                    {completedOrders.map(order => (
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

export default CompletedOrders;
