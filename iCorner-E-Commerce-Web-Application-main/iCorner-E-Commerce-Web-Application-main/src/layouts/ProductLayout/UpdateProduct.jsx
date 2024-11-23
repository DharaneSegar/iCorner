import React, { useState, useEffect } from "react";
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import { useNavigate, useParams } from 'react-router-dom';
import axios from "axios";
import { jwtDecode } from "jwt-decode"; // Ensure jwt-decode is installed


const UpdateProduct = () => {
  const { id } = useParams(); // Get the product ID from the URL parameters
  const [name, setName] = useState(''); // State for the product name
  const [email, setEmail] = useState(''); // State for the vendor's email
  const [description, setDescription] = useState(''); // State for the product description
  const [price, setPrice] = useState(''); // State for the product price
  const [category, setCategory] = useState(''); // State for selected product category
  const [categories, setCategories] = useState([]); // State for available categories from API
  const [isActive, setIsActive] = useState(true); // State to track product active status
  const [imageFile, setImageFile] = useState(null); // State to store selected image file
  const [errors, setErrors] = useState({}); // State to store form validation errors
  const navigate = useNavigate(); // Hook to allow navigation after form submission

  // Fetch categories and product details when the component mounts
  useEffect(() => {
    const fetchCategories = async () => {
      const token = localStorage.getItem("token"); // Retrieve token from localStorage
      if (token) {
        const decodedToken = jwtDecode(token); // Decode the token to extract the email
        setEmail(decodedToken.email || ""); // Set email in the state
      }
      try {
        // Fetch the list of categories from the API
        const response = await axios.get('http://192.168.109.81/iCorner/api/category');
        setCategories(response.data); // Update the categories state
      } catch (error) {
        console.error('Error fetching categories:', error); // Log any error in fetching categories
      }
    };

    const fetchProduct = async () => {
      try {
        // Fetch product details by its ID
        const response = await axios.get(`http://192.168.109.81/iCorner/api/products/${id}`);
        const product = response.data;
        // Pre-fill the form with the fetched product data
        setName(product.name);
        setDescription(product.description);
        setPrice(product.price);
        setCategory(product.categoryId);
        setIsActive(product.isActive);
      } catch (error) {
        console.error('Error fetching product:', error); // Log any error in fetching the product
      }
    };

    fetchCategories(); // Fetch the available categories
    fetchProduct(); // Fetch the product details
  }, [id]); // Dependencies to re-run the effect when ID changes

  const validateForm = () => {
    const formErrors = {};
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
    
    if (validateForm()) {
      try {
        let uploadedImagePath = '';
        if (imageFile) {
          const imageFormData = new FormData();
          imageFormData.append('productId', id); // Use the current product ID
          imageFormData.append('imageFile', imageFile);

          const uploadResponse = await axios.post('http://192.168.109.81/iCorner/api/products/upload-image', imageFormData, {
            headers: {
              'Content-Type': 'multipart/form-data',
            },
          });

          if (uploadResponse.data && uploadResponse.data.imagePath) {
            uploadedImagePath = uploadResponse.data.imagePath;
          }
        }

        await axios.put(`http://192.168.109.81/iCorner/api/products/${id}`, {
          Id: id,
          vendorId: email,  // TODO: Add real-time vendor logic
          name,
          description,
          price,
          categoryId: category,
          isActive,
          imagePath: uploadedImagePath || ''
        });

        navigate('/product/view-product');
      } catch (error) {
        console.error('Error updating product:', error);
      }
    }
  };

  return (
    <div className="container d-flex justify-content-center align-items-center vh-100">
      <div className="w-75">
        <h1 className="text-center mb-4">Update Product</h1>
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
            Update
          </Button>
        </Form>
      </div>
    </div>
  );
};

export default UpdateProduct;
