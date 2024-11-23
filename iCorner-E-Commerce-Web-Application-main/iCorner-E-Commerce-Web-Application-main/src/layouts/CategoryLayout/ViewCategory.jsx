import { useEffect, useState } from "react";
import Table from "react-bootstrap/Table";
import Button from "react-bootstrap/Button";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const ViewCategory = () => {
  const [categories, setCategories] = useState([]);
  const [filter, setFilter] = useState("all"); // State for filter criteria
  const navigate = useNavigate(); // Initialize navigate function

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await axios.get("http://192.168.109.81/iCorner/api/category");
        setCategories(response.data);
      } catch (error) {
        console.error("Error fetching categories:", error);
      }
    };

    fetchCategories();
  }, []);

  // Filter categories based on the selected filter
  const filteredCategories = categories.filter((category) => {
    if (filter === "active") return category.isActive;
    if (filter === "inactive") return !category.isActive;
    return true; // Show all categories if 'all' is selected
  });

  // Handler for Edit button
  const handleEdit = (id) => {
    // Navigate to edit category page (adjust route as necessary)
    navigate(`/category/update-category/${id}`);
  };

  // Handler for Delete button
  const handleDelete = async (id) => {
    const confirmed = window.confirm(
      "Are you sure you want to delete this category?"
    );
    if (!confirmed) return; // Exit if the user cancels

    try {
      await axios.delete(`http://192.168.109.81/iCorner/api/category/${id}`);
      // Refresh the category list after deletion
      setCategories(categories.filter((category) => category.id !== id));
    } catch (error) {
      console.error("Error deleting category:", error);
    }
  };

  // Handler for Toggle Active/Inactive
  const handleToggleActive = async (id, isActive) => {
    try {
      if (isActive) {
        await axios.put(`http://192.168.109.81/iCorner/api/category/deactivate/${id}`);
      } else {
        await axios.put(`http://192.168.109.81/iCorner/api/category/activate/${id}`);
      }
      // Refresh the category list after toggling status
      const updatedCategories = categories.map((category) =>
        category.id === id ? { ...category, isActive: !isActive } : category
      );
      setCategories(updatedCategories);
    } catch (error) {
      console.error("Error toggling category status:", error);
    }
  };

  return (
    <>
      <h1 className="text-center">Categories</h1>

      <div className="d-flex justify-content-end mb-3">
        <Button
          variant="outline-primary"
          onClick={() => navigate("/category/add-category")} // Navigate to add category page
        >
          Add Category
        </Button>
      </div>

      <div>
      <Button
                variant={filter === "all" ? "outline-info" : "outline-primary"}
                onClick={() => setFilter("all")}
              >
                All
              </Button>
              <Button
                variant={
                  filter === "active" ? "outline-info" : "outline-primary"
                }
                className="ml-2"
                onClick={() => setFilter("active")}
              >
                Active
              </Button>
              <Button
                variant={
                  filter === "inactive" ? "outline-info" : "outline-primary"
                }
                className="ml-2"
                onClick={() => setFilter("inactive")}
              >
                Deactive
              </Button>
      </div>

      <Table striped bordered hover>
        <thead>
          {/* Filter Row */}
          {/* <tr>
            <td colSpan="4" className="text-center">
              <Button
                variant={filter === "all" ? "outline-info" : "outline-primary"}
                onClick={() => setFilter("all")}
              >
                All
              </Button>
              <Button
                variant={
                  filter === "active" ? "outline-info" : "outline-primary"
                }
                className="ml-2"
                onClick={() => setFilter("active")}
              >
                Active
              </Button>
              <Button
                variant={
                  filter === "inactive" ? "outline-info" : "outline-primary"
                }
                className="ml-2"
                onClick={() => setFilter("inactive")}
              >
                Deactive
              </Button>
            </td>
          </tr> */}
          {/* Table Header Row */}
          <tr>
            <th>#</th>
            <th>Name</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {filteredCategories.map((category, index) => (
            <tr key={category.id}>
              <td>{index + 1}</td>
              <td>{category.name}</td>
              <td>
                {category.isActive ? "Active" : "Inactive"}
                <Button
                  variant={
                    category.isActive ? "outline-warning" : "outline-success"
                  }
                  className="ml-2"
                  onClick={() =>
                    handleToggleActive(category.id, category.isActive)
                  }
                >
                  {category.isActive ? "Deactivate" : "Activate"}
                </Button>
              </td>
              <td>
                <Button
                  variant="outline-primary"
                  onClick={() => handleEdit(category.id)}
                >
                  Edit
                </Button>
                <Button
                  variant="outline-danger"
                  className="ml-2"
                  onClick={() => handleDelete(category.id)}
                >
                  Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </>
  );
};

export default ViewCategory;
