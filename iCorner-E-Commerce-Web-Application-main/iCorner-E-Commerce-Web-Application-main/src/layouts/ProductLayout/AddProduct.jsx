import React, { useState, useEffect } from "react";
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import { useNavigate } from 'react-router-dom';
import axios from "axios";
import { jwtDecode } from "jwt-decode"; // Ensure jwt-decode is installed


// Component for adding a new product
const AddProduct = () => {
  // State hooks for form fields and validations
  const [name, setName] = useState(''); // Product name
  const [description, setDescription] = useState(''); // Product description
  const [price, setPrice] = useState(''); // Product price
  const [category, setCategory] = useState(''); // Selected category
  const [categories, setCategories] = useState([]); // List of available categories
  const [isActive, setIsActive] = useState(true); // Status of the product (active or inactive)
  const [imageFile, setImageFile] = useState(null); // State to handle the uploaded image file
  const [vendorId, setVendorId] = useState(''); // vendor email
  const [errors, setErrors] = useState({}); // Error state for form validation
  const navigate = useNavigate(); // To programmatically navigate between routes

  // Fetch available product categories when the component mounts
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await axios.get('http://192.168.109.81/iCorner/api/category'); // Fetch categories from API
        setCategories(response.data); // Set fetched categories to state
      } catch (error) {
        console.error('Error fetching categories:', error); // Log error if fetching fails
      }
    };

    fetchCategories(); // Call the function to fetch categories
  }, []); // Empty dependency array ensures this runs only once

  const validateForm = () => {
    let formErrors = {};
    if (name.length < 3) {
      formErrors.name = "Product name must be at least 3 characters long";
    }
    if (price <= 0) {
      formErrors.price = "Price must be a positive number";
    }
    if (!category) {
      formErrors.category = "Please select a category";
    }
    if (description.length < 10) {
      formErrors.description = "Description must be at least 10 characters long";
    }
    setErrors(formErrors);
    return Object.keys(formErrors).length === 0;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    const token = localStorage.getItem("token"); // Get the token
      if (token) {
        const decodedToken = jwtDecode(token);
        setVendorId(decodedToken.email);
      }

    
    if (validateForm()) {
      try {
        const productResponse = await axios.post('http://192.168.109.81/iCorner/api/products', {
          vendorId: vendorId,
          name,
          description,
          price,
          categoryId: category,
          isActive,
        });
  
        const productId = productResponse.data.id;
  
        let uploadedImagePath = '';
        if (imageFile) {
          const imageFormData = new FormData();
          imageFormData.append('productId', productId);
          imageFormData.append('imageFile', imageFile);
  
          const uploadResponse = await axios.post('http://192.168.109.81/iCorner/api/products/upload-image', imageFormData, {
            headers: {
              'Content-Type': 'multipart/form-data',
            },
          });
  
          if (uploadResponse.data && uploadResponse.data.imagePath) {
            uploadedImagePath = uploadResponse.data.imagePath;
          } else {
            console.error('Image upload failed:', uploadResponse.data);
            return;
          }
        }
  
        await axios.put(`http://192.168.109.81/iCorner/api/products/${productId}`, {
          vendorId: vendorId,
          name,
          description,
          price,
          categoryId: category,
          isActive,
          imagePath: uploadedImagePath, 
        });
  
        console.log('Navigation to /product/view-product');
        navigate('/product/view-product');
  
      } catch (error) {
        console.error('Error adding product:', error);
        navigate('/product/view-product');
        // Optionally show a message to the user
      }
    }
  };
  

  return (
    <div className="container d-flex justify-content-center align-items-center vh-100">
      <div className="w-75">
        <h1 className="text-center mb-4">Add New Product</h1>
        <Form onSubmit={handleSubmit}>
          <Row className="mb-3">
            <Form.Group as={Col} controlId="formProductName">
              <Form.Label>Product Name</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter product name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                isInvalid={!!errors.name}
                required
              />
              <Form.Control.Feedback type="invalid">
                {errors.name}
              </Form.Control.Feedback>
            </Form.Group>

            <Form.Group as={Col} controlId="formProductPrice">
              <Form.Label>Price</Form.Label>
              <Form.Control
                type="number"
                step="0.01"
                placeholder="Enter product price"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                isInvalid={!!errors.price}
                required
              />
              <Form.Control.Feedback type="invalid">
                {errors.price}
              </Form.Control.Feedback>
            </Form.Group>
          </Row>

          <Form.Group className="mb-3" controlId="formProductDescription">
            <Form.Label>Description</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              placeholder="Enter product description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              isInvalid={!!errors.description}
              required
            />
            <Form.Control.Feedback type="invalid">
              {errors.description}
            </Form.Control.Feedback>
          </Form.Group>

          <Form.Group className="mb-3" controlId="formProductCategory">
            <Form.Label>Category</Form.Label>
            <Form.Control
              as="select"
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              isInvalid={!!errors.category}
              required
            >
              <option value="">Select a category</option>
              {categories.map(cat => (
                <option key={cat.id} value={cat.id}>
                  {cat.name}
                </option>
              ))}
            </Form.Control>
            <Form.Control.Feedback type="invalid">
              {errors.category}
            </Form.Control.Feedback>
          </Form.Group>

          <Form.Group className="mb-3" controlId="formProductImage">
            <Form.Label>Product Image</Form.Label>
            <Form.Control
              type="file"
              accept="image/*"
              onChange={(e) => setImageFile(e.target.files[0])}
            />
          </Form.Group>

          <Form.Group className="mb-3" id="formProductStatus">
            <Form.Check
              type="checkbox"
              label="Active"
              checked={isActive}
              onChange={(e) => setIsActive(e.target.checked)}
            />
          </Form.Group>

          <Button variant="outline-primary" type="submit" className="w-100">
            Submit
          </Button>
        </Form>
      </div>
    </div>
  );
};

export default AddProduct;
