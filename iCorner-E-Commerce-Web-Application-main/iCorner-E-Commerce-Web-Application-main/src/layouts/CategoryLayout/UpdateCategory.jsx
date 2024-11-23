import React, { useState, useEffect } from "react";
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useNavigate, useParams } from 'react-router-dom';
import axios from "axios";
import './Category.css'; // Custom CSS if needed

const UpdateCategory = () => {
  const { id } = useParams(); // Get the 'id' parameter from the URL to identify which category to update
  const [name, setName] = useState(''); // State to store the category name
  const [isActive, setIsActive] = useState(true); // State to store the active status of the category
  const [errors, setErrors] = useState({}); // State to store validation errors
  const navigate = useNavigate(); // Hook to handle redirection after update

  // Fetch the category data based on the category ID when the component loads
  useEffect(() => {
    const fetchCategory = async () => {
      try {
        const response = await axios.get(`http://192.168.109.81/iCorner/api/category/${id}`); // API call to get category by ID
        setName(response.data.name); // Set the category name
        setIsActive(response.data.isActive); // Set the category active status
      } catch (error) {
        console.error('Error fetching category:', error); // Handle error in fetching the category
      }
    };

    fetchCategory(); // Call the function to fetch category data
  }, [id]); // 'id' is the dependency, so it will fetch category data every time the 'id' changes

  // Validation function
  const validateForm = () => {
    const newErrors = {};

    // Validate category name
    if (!name) {
      newErrors.name = "Category name is required.";
    } else if (name.length < 3) {
      newErrors.name = "Category name must be at least 3 characters long.";
    } else if (!/^[a-zA-Z0-9\s]+$/.test(name)) {
      newErrors.name = "Category name can only contain letters, numbers, and spaces.";
    }

    return newErrors;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const validationErrors = validateForm();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      await axios.put(`http://192.168.109.81/iCorner/api/category/${id}`, { name, isActive });
      navigate('/category/view-category');
    } catch (error) {
      console.error('Error updating category:', error);
    }
  };

  return (
    <div className="container d-flex justify-content-center align-items-center vh-100">
      <div className="w-50">
        <h1 className="text-center mb-4">Update Category</h1>
        <Form onSubmit={handleSubmit}>
          <Form.Group className="mb-3" controlId="formCategoryName">
            <Form.Label>Category Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter category name"
              value={name}
              onChange={(e) => {
                setName(e.target.value);
                setErrors({ ...errors, name: '' }); // Clear error on change
              }}
              isInvalid={!!errors.name} // Add invalid style if there's an error
              required
            />
            <Form.Control.Feedback type="invalid">
              {errors.name}
            </Form.Control.Feedback>
          </Form.Group>

          <Form.Group className="mb-3" controlId="formCategoryStatus">
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

export default UpdateCategory;
