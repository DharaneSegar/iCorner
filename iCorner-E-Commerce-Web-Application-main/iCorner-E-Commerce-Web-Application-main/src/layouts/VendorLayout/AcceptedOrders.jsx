import React, { useEffect, useState } from 'react';
import { Table, Container, Spinner, Modal, Button } from 'react-bootstrap';
import axios from 'axios';
import './order.css';

const AcceptedOrders = () => {
    const [acceptedOrders, setAcceptedOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [products, setProducts] = useState([]);
    const [selectedOrderId, setSelectedOrderId] = useState('');
    const [acceptError, setAcceptError] = useState(null);

    useEffect(() => {
        const fetchAcceptedOrders = async () => {
            try {
                const response = await axios.get('http://192.168.109.81/iCorner/api/order/vendor/sajeesiva12@gmail.com');
                // Filter only accepted orders
                const filteredOrders = response.data.filter(order => order.status === 'Dispatched' || order.status === 'dispatched' || order.status === 'Delivering' || order.status === 'delivering');
                setAcceptedOrders(filteredOrders);
            } catch (err) {
                setError('Error fetching accepted orders');
            } finally {
                setLoading(false);
            }
        };

        fetchAcceptedOrders();
    }, []);

    const handleRowClick = async (orderId) => {
        try {
            const response = await axios.get(`http://192.168.109.81/iCorner/api/order/vendor/sajeesiva12@gmail.com/order/${orderId}/products`);
            if (response.data.success) {
                setProducts(response.data.products);
                setSelectedOrderId(orderId);
                setShowModal(true);
            }
        } catch (err) {
            setError('Error fetching products for this order');
        }
    };

    const handleClose = () => {
        setShowModal(false);
        setAcceptError(null); // Reset the acceptance error on close
    };

    const handleAccept = async () => {
        try {
            const response = await axios.post(`http://192.168.109.81/iCorner/api/order/vendor/sajeesiva12@gmail.com/order/${selectedOrderId}/accept`);
            if (response.data.success) {
                // You can update the acceptedOrders state or refresh the orders after acceptance
                setAcceptedOrders((prev) => prev.filter(order => order.id !== selectedOrderId));
                handleClose();
            } else {
                setAcceptError('Failed to accept all items in the order.');
            }
        } catch (err) {
            setAcceptError('Error accepting the order.');
        }
    };

    if (loading) {
        return (
            <Container className="text-center mt-5">
                <Spinner animation="border" />
                <p>Loading accepted orders...</p>
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
            <h1 className="mb-4 text-center">Accepted Orders</h1>
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
                    {acceptedOrders.map(order => (
                        <tr key={order.id} onClick={() => handleRowClick(order.id)}>
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

            <Modal show={showModal} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Products in Order {selectedOrderId}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {acceptError && <p className="text-danger">{acceptError}</p>}
                    <Table striped bordered hover>
                        <thead>
                            <tr>
                                <th>Product ID</th>
                                <th>Product Name</th>
                                <th>Quantity</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {products.map(product => (
                                <tr key={product.productId}>
                                    <td>{product.productId}</td>
                                    <td>{product.productName}</td>
                                    <td>{product.quantity}</td>
                                    <td>{product.status}</td>
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                </Modal.Body>
                <Modal.Footer>
                    <Button className='button-gray' variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button className='button-blue' variant="primary" onClick={handleAccept}>
                        Accept
                    </Button>
                </Modal.Footer>
            </Modal>
        </Container>
    );
};

export default AcceptedOrders;
