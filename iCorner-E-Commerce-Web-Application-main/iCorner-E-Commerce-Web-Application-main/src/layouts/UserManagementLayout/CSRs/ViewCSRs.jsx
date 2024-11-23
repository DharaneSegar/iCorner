import { useEffect, useState } from "react";
import Table from "react-bootstrap/Table";
import Button from "react-bootstrap/Button";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";

const ViewCSRs = () => {
  const [CSRs, setCSRs] = useState([]);
  const [filteredCSRs, setFilteredCSRs] = useState([]);
  const [filter, setFilter] = useState("all");
  const [showModal, setShowModal] = useState(false);
  const [showActivationModal, setShowActivationModal] = useState(false);
  const [selectedCSRId, setSelectedCSRId] = useState(null);
  const [alertMessage, setAlertMessage] = useState("");
  const [activationAction, setActivationAction] = useState(null); // New state to determine action (activate/deactivate)
  const navigate = useNavigate();

  useEffect(() => {
    fetchCSRs();
  }, []);

  useEffect(() => {
    applyFilter();
  }, [CSRs, filter]);

  const fetchCSRs = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await axios.get("http://192.168.109.81/iCorner/api/admin/csrs", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      setCSRs(response.data);
    } catch (error) {
      console.error("Error fetching CSRs:", error);
    }
  };

  const applyFilter = () => {
    if (filter === "all") {
      setFilteredCSRs(CSRs);
    } else if (filter === "active") {
      setFilteredCSRs(CSRs.filter((CSR) => CSR.isActive));
    } else if (filter === "inactive") {
      setFilteredCSRs(CSRs.filter((CSR) => !CSR.isActive));
    }
  };

  const handleEdit = (id) => {
    navigate(`/users/update-CSR/${id}`);
  };

  const handleDelete = async () => {
    if (!selectedCSRId) return;

    try {
      const token = localStorage.getItem("token");
      await axios.delete(
        `http://192.168.109.81/iCorner/api/user/delete/${selectedCSRId}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setAlertMessage("CSR has been deleted successfully.");
      fetchCSRs();
      setTimeout(() => {
        setAlertMessage(""); // Reset the alert message after timeout
      }, 3000);
    } catch (error) {
      console.error("Error deleting CSR:", error);
      alert("Error deleting CSR");
    } finally {
      setShowModal(false);
      setSelectedCSRId(null);
    }
  };

  const openModal = (id) => {
    setSelectedCSRId(id);
    setShowModal(true);
  };

  const openActivationModal = (id, action) => {
    setSelectedCSRId(id);
    setActivationAction(action); // Set the action (activate/deactivate)
    setShowActivationModal(true);
  };

  const handleActivateDeactivate = async () => {
    if (!selectedCSRId) return;

    try {
      const token = localStorage.getItem("token");
      const endpoint =
        activationAction === "activate"
          ? `http://192.168.109.81/iCorner/api/admin/activate/${selectedCSRId}`
          : `http://192.168.109.81/iCorner/api/admin/deactivate/${selectedCSRId}`;

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
        `CSR account has been ${
          activationAction === "activate" ? "activated" : "deactivated"
        }.`
      );
      fetchCSRs();
      setTimeout(() => {
        setAlertMessage(""); // Reset the alert message after timeout
      }, 3000);
    } catch (error) {
      console.error(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } CSR:`,
        error
      );
      alert(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } CSR`
      );
    } finally {
      setShowActivationModal(false);
      setSelectedCSRId(null);
      setActivationAction(null); // Reset activation action
    }
  };

  return (
    <>
      <h1 className="text-center">CSRs</h1>

      {alertMessage && (
        <div className="alert alert-success" role="alert">
          {alertMessage}
        </div>
      )}

      <div className="d-flex justify-content-end mb-3">
        <Button
          variant="outline-primary"
          onClick={() => navigate("/users/add-CSRs")}
        >
          Add CSR
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
          {filteredCSRs.map((CSR, index) => (
            <tr key={CSR.id}>
              <td>{index + 1}</td>
              <td>{CSR.username}</td>
              <td>{CSR.email}</td>
              <td>{CSR.mobileNumber}</td>
              <td>{CSR.address}</td>
              <td>{CSR.role}</td>
              <td>
                {CSR.isActive ? "Active" : "Inactive"}
                <Button
                  variant={CSR.isActive ? "outline-warning" : "outline-success"}
                  className="ml-2"
                  onClick={() =>
                    openActivationModal(
                      CSR.id,
                      CSR.isActive ? "deactivate" : "activate"
                    )
                  }
                >
                  {CSR.isActive ? "Deactivate" : "Activate"}
                </Button>
              </td>
              <td>
                <Button
                  variant="outline-primary"
                  onClick={() => handleEdit(CSR.id)}
                >
                  Edit
                </Button>
                <Button
                  variant="outline-danger"
                  className="ml-2"
                  onClick={() => openModal(CSR.id)}
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
        <Modal.Body>Are you sure you want to delete this CSR?</Modal.Body>
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
          CSR?
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

export default ViewCSRs;
