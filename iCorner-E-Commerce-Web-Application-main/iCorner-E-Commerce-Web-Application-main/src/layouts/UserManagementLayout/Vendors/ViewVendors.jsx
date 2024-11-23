import { useEffect, useState } from "react";
import Table from "react-bootstrap/Table";
import Button from "react-bootstrap/Button";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";

const ViewVendors = () => {
  const [vendors, setVendors] = useState([]);
  const [filteredVendors, setFilteredVendors] = useState([]);
  const [filter, setFilter] = useState("all");
  const [showModal, setShowModal] = useState(false);
  const [showActivationModal, setShowActivationModal] = useState(false);
  const [selectedVendorId, setSelectedVendorId] = useState(null);
  const [alertMessage, setAlertMessage] = useState("");
  const [activationAction, setActivationAction] = useState(null); // New state to determine action (activate/deactivate)
  const navigate = useNavigate();

  useEffect(() => {
    fetchVendors();
  }, []);

  useEffect(() => {
    applyFilter();
  }, [vendors, filter]);

  const fetchVendors = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await axios.get(
        "http://192.168.109.81/iCorner/api/admin/vendors",
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setVendors(response.data);
    } catch (error) {
      console.error("Error fetching vendors:", error);
    }
  };

  const applyFilter = () => {
    if (filter === "all") {
      setFilteredVendors(vendors);
    } else if (filter === "active") {
      setFilteredVendors(vendors.filter((vendor) => vendor.isActive));
    } else if (filter === "inactive") {
      setFilteredVendors(vendors.filter((vendor) => !vendor.isActive));
    }
  };

  const handleEdit = (id) => {
    navigate(`/users/update-vendor/${id}`);
  };

  const handleDelete = async () => {
    if (!selectedVendorId) return;

    try {
      const token = localStorage.getItem("token");
      await axios.delete(
        `http://192.168.109.81/iCorner/api/user/delete/${selectedVendorId}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setAlertMessage("Vendor has been deleted successfully.");
      fetchVendors();
      setTimeout(() => {
        setAlertMessage(""); // Reset the alert message after timeout
      }, 3000);
    } catch (error) {
      console.error("Error deleting vendor:", error);
      alert("Error deleting vendor");
    } finally {
      setShowModal(false);
      setSelectedVendorId(null);
    }
  };

  const openModal = (id) => {
    setSelectedVendorId(id);
    setShowModal(true);
  };

  const openActivationModal = (id, action) => {
    setSelectedVendorId(id);
    setActivationAction(action); // Set the action (activate/deactivate)
    setShowActivationModal(true);
  };

  const handleActivateDeactivate = async () => {
    if (!selectedVendorId) return;

    try {
      const token = localStorage.getItem("token");
      const endpoint =
        activationAction === "activate"
          ? `http://192.168.109.81/iCorner/api/admin/activate/${selectedVendorId}`
          : `http://192.168.109.81/iCorner/api/admin/deactivate/${selectedVendorId}`;

      await axios.patch(
        endpoint,
        {},
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setAlertMessage(
        `Vendor account has been ${
          activationAction === "activate" ? "activated" : "deactivated"
        }.`
      );
      fetchVendors();
      setTimeout(() => {
        setAlertMessage(""); // Reset the alert message after timeout
      }, 3000);
    } catch (error) {
      console.error(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } vendor:`,
        error
      );
      alert(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } vendor`
      );
    } finally {
      setShowActivationModal(false);
      setSelectedVendorId(null);
      setActivationAction(null); // Reset activation action
    }
  };

  return (
    <>
      <h1 className="text-center">Vendors</h1>

      {alertMessage && (
        <div className="alert alert-success" role="alert">
          {alertMessage}
        </div>
      )}

      <div className="d-flex justify-content-end mb-3">
        <Button
          variant="outline-primary"
          onClick={() => navigate("/users/add-vendors")}
        >
          Add Vendor
        </Button>
      </div>

      <div className="d-flex justify-content-center mb-3">
        <Button
          variant="outline-primary"
          className="mx-2"
          onClick={() => setFilter("all")}
        >
          Show All
        </Button>
        <Button
          variant="outline-success"
          className="mx-2"
          onClick={() => setFilter("active")}
        >
          Show Active
        </Button>
        <Button
          variant="outline-secondary"
          className="mx-2"
          onClick={() => setFilter("inactive")}
        >
          Show Deactivated
        </Button>
      </div>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>#</th>
            <th>Username</th>
            <th>Email</th>
            <th>Mobile Number</th>
            <th>Address</th>
            <th>Role</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {filteredVendors.map((vendor, index) => (
            <tr key={vendor.id}>
              <td>{index + 1}</td>
              <td>{vendor.username}</td>
              <td>{vendor.email}</td>
              <td>{vendor.mobileNumber}</td>
              <td>{vendor.address}</td>
              <td>{vendor.role}</td>
              <td>
                {vendor.isActive ? "Active" : "Inactive"}
                <Button
                  variant={
                    vendor.isActive ? "outline-warning" : "outline-success"
                  }
                  className="ml-2"
                  onClick={() =>
                    openActivationModal(
                      vendor.id,
                      vendor.isActive ? "deactivate" : "activate"
                    )
                  }
                >
                  {vendor.isActive ? "Deactivate" : "Activate"}
                </Button>
              </td>
              <td>
                <Button
                  variant="outline-primary"
                  onClick={() => handleEdit(vendor.id)}
                >
                  Edit
                </Button>
                <Button
                  variant="outline-danger"
                  className="ml-2"
                  onClick={() => openModal(vendor.id)}
                >
                  Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      {/* Confirmation Modal for Deletion */}
      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Confirm Deletion</Modal.Title>
        </Modal.Header>
        <Modal.Body>Are you sure you want to delete this vendor?</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Cancel
          </Button>
          <Button variant="danger" onClick={handleDelete}>
            Delete
          </Button>
        </Modal.Footer>
      </Modal>

      {/* Confirmation Modal for Activation/Deactivation */}
      <Modal
        show={showActivationModal}
        onHide={() => setShowActivationModal(false)}
      >
        <Modal.Header closeButton>
          <Modal.Title>
            Confirm{" "}
            {activationAction === "activate" ? "Activation" : "Deactivation"}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to{" "}
          {activationAction === "activate" ? "activate" : "deactivate"} this
          vendor?
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            className="text-white bg-secondary"
            onClick={() => setShowActivationModal(false)}
          >
            Cancel
          </Button>
          <Button
            variant="primary"
            className="text-white bg-primary"
            onClick={handleActivateDeactivate}
          >
            Confirm
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};

export default ViewVendors;
