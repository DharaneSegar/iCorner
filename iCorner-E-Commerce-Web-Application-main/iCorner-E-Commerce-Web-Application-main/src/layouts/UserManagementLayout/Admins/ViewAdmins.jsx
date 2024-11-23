import { useEffect, useState } from "react";
import Table from "react-bootstrap/Table";
import Button from "react-bootstrap/Button";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";

const ViewAdmins = () => {
  const [admins, setAdmins] = useState([]);
  const [filteredAdmins, setFilteredAdmins] = useState([]);
  const [filter, setFilter] = useState("all");
  const [showModal, setShowModal] = useState(false);
  const [showActivationModal, setShowActivationModal] = useState(false);
  const [selectedAdminId, setSelectedAdminId] = useState(null);
  const [alertMessage, setAlertMessage] = useState("");
  const [activationAction, setActivationAction] = useState(null); // New state to determine action (activate/deactivate)
  const navigate = useNavigate();

  useEffect(() => {
    fetchAdmins();
  }, []);

  useEffect(() => {
    applyFilter();
  }, [admins, filter]);

  const fetchAdmins = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await axios.get(
        "http://192.168.109.81/iCorner/api/admin/admins",
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setAdmins(response.data);
    } catch (error) {
      console.error("Error fetching admins:", error);
    }
  };

  const applyFilter = () => {
    if (filter === "all") {
      setFilteredAdmins(admins);
    } else if (filter === "active") {
      setFilteredAdmins(admins.filter((admin) => admin.isActive));
    } else if (filter === "inactive") {
      setFilteredAdmins(admins.filter((admin) => !admin.isActive));
    }
  };

  const handleEdit = (id) => {
    navigate(`/users/update-admin/${id}`);
  };

  const handleDelete = async () => {
    if (!selectedAdminId) return;

    try {
      const token = localStorage.getItem("token");
      await axios.delete(
        `http://192.168.109.81/iCorner/api/user/delete/${selectedAdminId}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setAlertMessage("Vendor has been deleted successfully.");
      fetchAdmins();
      setTimeout(() => {
        setAlertMessage(""); // Reset the alert message after timeout
      }, 3000);
    } catch (error) {
      console.error("Error deleting admin:", error);
      alert("Error deleting admin");
    } finally {
      setShowModal(false);
      setSelectedAdminId(null);
    }
  };

  const openModal = (id) => {
    setSelectedAdminId(id);
    setShowModal(true);
  };

  const openActivationModal = (id, action) => {
    setSelectedAdminId(id);
    setActivationAction(action); // Set the action (activate/deactivate)
    setShowActivationModal(true);
  };

  const handleActivateDeactivate = async () => {
    if (!selectedAdminId) return;

    try {
      const token = localStorage.getItem("token");
      const endpoint =
        activationAction === "activate"
          ? `http://192.168.109.81/iCorner/api/admin/activate/${selectedAdminId}`
          : `http://192.168.109.81/iCorner/api/admin/deactivate/${selectedAdminId}`;

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
      fetchAdmins();
      setTimeout(() => {
        setAlertMessage(""); // Reset the alert message after timeout
      }, 3000);
    } catch (error) {
      console.error(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } admin:`,
        error
      );
      alert(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } admin`
      );
    } finally {
      setShowActivationModal(false);
      setSelectedAdminId(null);
      setActivationAction(null); // Reset activation action
    }
  };

  return (
    <>
      <h1 className="text-center">Admins</h1>

      {alertMessage && (
        <div className="alert alert-success" role="alert">
          {alertMessage}
        </div>
      )}

      <div className="d-flex justify-content-end mb-3">
        <Button
          variant="outline-primary"
          onClick={() => navigate("/users/add-admins")}
        >
          Add Admin
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
          {filteredAdmins.map((admin, index) => (
            <tr key={admin.id}>
              <td>{index + 1}</td>
              <td>{admin.username}</td>
              <td>{admin.email}</td>
              <td>{admin.mobileNumber}</td>
              <td>{admin.address}</td>
              <td>{admin.role}</td>
              <td>
                {admin.isActive ? "Active" : "Inactive"}
                <Button
                  variant={
                    admin.isActive ? "outline-warning" : "outline-success"
                  }
                  className="ml-2"
                  onClick={() =>
                    openActivationModal(
                      admin.id,
                      admin.isActive ? "deactivate" : "activate"
                    )
                  }
                >
                  {admin.isActive ? "Deactivate" : "Activate"}
                </Button>
              </td>
              <td>
                <Button
                  variant="outline-primary"
                  onClick={() => handleEdit(admin.id)}
                >
                  Edit
                </Button>
                <Button
                  variant="outline-danger"
                  className="ml-2"
                  onClick={() => openModal(admin.id)}
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
        <Modal.Body>Are you sure you want to delete this admin?</Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            className="text-white bg-secondary"
            onClick={() => setShowModal(false)}
          >
            Cancel
          </Button>
          <Button
            variant="danger"
            className="text-white bg-danger"
            onClick={handleDelete}
          >
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
          admin?
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

export default ViewAdmins;
