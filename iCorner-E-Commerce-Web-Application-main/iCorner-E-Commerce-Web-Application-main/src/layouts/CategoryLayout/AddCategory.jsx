import { useState } from "react";
import Button from "react-bootstrap/Button";
import Form from "react-bootstrap/Form";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import "./Category.css"; // Import custom CSS file if needed

const AddCategory = () => {
   // State to manage the form input values
   const [name, setName] = useState(""); // For storing the category name
   const [isActive, setIsActive] = useState(true); // Checkbox to manage active status
   const [errors, setErrors] = useState({}); // To store validation errors
 
   const navigate = useNavigate(); // React Router's navigation hook for redirection
 
   // Function to validate the form input
   const validateForm = () => {
     const newErrors = {};
 
     // Validate Category Name
     if (!name) {
       newErrors.name = "Category name is required."; // Ensure the name is not empty
     } else if (name.length < 3) {
       newErrors.name = "Category name must be at least 3 characters long."; // Minimum length validation
     } else if (!/^[a-zA-Z0-9\s]+$/.test(name)) {
       newErrors.name = "Category name can only contain letters, numbers, and spaces."; // Alphanumeric characters only
     }
 
     return newErrors; // Return any validation errors found
   };
 
   // Handle form submission
   const handleSubmit = async (event) => {
     event.preventDefault(); // Prevent default form submission behavior
 
     const validationErrors = validateForm(); // Perform validation
     if (Object.keys(validationErrors).length > 0) {
       setErrors(validationErrors); // If errors, show them
       return;
     }
 
     try {
       // If validation passes, make a POST request to the API to add the category
       await axios.post("http://192.168.109.81/iCorner/api/category", {
         name, // Category name
         isActive, // Category active status
       });
       navigate("/category/view-category"); // Redirect to category view page after success
     } catch (error) {
       console.error("Error adding category:", error); // Log error if the API call fails
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
              onChange={(e) => {
                setName(e.target.value);
                setErrors({ ...errors, name: "" });
              }}
              isInvalid={!!errors.name} // Display error styling if validation fails
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
            Submit
          </Button>
        </Form>
      </div>
    </div>
  );
};

export default AddCategory;
