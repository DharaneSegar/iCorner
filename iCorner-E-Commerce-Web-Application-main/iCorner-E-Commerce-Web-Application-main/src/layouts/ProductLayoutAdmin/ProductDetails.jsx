import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Table from 'react-bootstrap/Table';
import Button from 'react-bootstrap/Button';
import axios from "axios";

const ProductDetails = () => {
  const { id } = useParams(); // Get the product ID from the URL using useParams hook
  const navigate = useNavigate(); // Use navigate hook to programmatically navigate between routes
  const [product, setProduct] = useState(null); // State to store product details

  // useEffect hook to fetch the product details when the component mounts or when the ID changes
  useEffect(() => {
    const fetchProduct = async () => {
      try {
        // Fetch product details by making a GET request to the API
        const response = await axios.get(`http://192.168.109.81/iCorner/api/products/${id}`);
        setProduct(response.data); // Store the fetched product data in state
      } catch (error) {
        console.error('Error fetching product:', error); // Log any error encountered while fetching product
      }
    };

    fetchProduct(); // Call the function to fetch the product details
  }, [id]); // The effect runs whenever the product ID in the route changes

  // Handler function for Edit button
  const handleEdit = () => {
    navigate(`/product/update-product/${id}`); // Navigate to the edit page for the current product
  };

  // Handler function for Delete button
  const handleDelete = async () => {
    const confirmed = window.confirm('Are you sure you want to delete this product?'); // Confirm deletion with the user
    if (!confirmed) return; // If the user cancels, do nothing

    try {
      // Make a DELETE request to the API to delete the product
      await axios.delete(`http://192.168.109.81/iCorner/api/products/${id}`);
      navigate('/product/view-products'); // After deletion, navigate to the product list page
    } catch (error) {
      console.error('Error deleting product:', error); // Log any error encountered while deleting product
    }
  };

  // Handler for toggling the product's active/inactive status
  const handleToggleActive = async () => {
    try {
      // Toggle product status using separate activate or deactivate API endpoints
      if (product.isActive) {
        await axios.put(`http://192.168.109.81/iCorner/api/products/deactivate/${id}`); // API call to deactivate
      } else {
        await axios.put(`http://192.168.109.81/iCorner/api/products/activate/${id}`); // API call to activate
      }
      // Update the product's active status in the local state to reflect the change
      setProduct(prevProduct => ({ ...prevProduct, isActive: !prevProduct.isActive }));
    } catch (error) {
      console.error('Error toggling product status:', error); // Log any error encountered while toggling status
    }
  };

  // If the product data is not yet available (still loading), show a loading message
  if (!product) {
    return <p>Loading...</p>;
  }


  return (
    <>
      <h1 className="text-center mb-4">Product Details</h1>
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Field</th>
            <th>Value</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>ID</td>
            <td>{product.id}</td>
          </tr>
          <tr>
            <td>Name</td>
            <td>{product.name}</td>
          </tr>
          <tr>
            <td>Description</td>
            <td>{product.description}</td>
          </tr>
          <tr>
            <td>Price</td>
            <td>${product.price}</td>
          </tr>
          <tr>
            <td>Status</td>
            <td>{product.isActive ? 'Active' : 'Inactive'}</td>
          </tr>
        </tbody>
      </Table>
      
      <div className="d-flex justify-content-end mt-3">
        <Button 
          variant={product.isActive ? 'outline-warning' : 'outline-success'} 
          onClick={handleToggleActive}
          className="me-2"
        >
          {product.isActive ? 'Deactivate' : 'Activate'}
        </Button>
        <Button 
          variant="outline-primary" 
          onClick={handleEdit}
          className="me-2"
        >
          Edit
        </Button>
        <Button 
          variant="outline-danger" 
          onClick={handleDelete}
        >
          Delete
        </Button>
      </div>
    </>
  );
};

export default ProductDetails;
