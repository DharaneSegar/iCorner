import React, { useState } from "react";
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useNavigate } from 'react-router-dom';
import axios from "axios";

const AddCategory = () => {
 // State to hold the category name and status
 const [name, setName] = useState(''); // State for category name
 const [isActive, setIsActive] = useState(true); // State for category status (active/inactive)
 const navigate = useNavigate(); // React Router's navigation hook

 // Function to handle form submission
 const handleSubmit = async (event) => {
   event.preventDefault(); // Prevent the default form submission behavior

   try {
     // Make POST request to API to add the new category
     await axios.post('http://192.168.109.81/iCorner/api/category', { name, isActive });
     navigate('/category/view-category'); // Redirect to view category page after successful submission
   } catch (error) {
     console.error('Error adding category:', error); // Log error in case of failure
   }
 };

  return (
    <div className="container d-flex justify-content-center align-items-center vh-100">
      <div className="w-50">
        <h1 className="text-center mb-4">Add New Category</h1>
        <Form onSubmit={handleSubmit}>
          <Form.Group className="mb-3" controlId="formCategoryName">
            <Form.Label>Category Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter category name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
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
            Submit
          </Button>
        </Form>
      </div>
    </div>
  );
};

export default AddCategory;
