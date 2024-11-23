import { useEffect, useState } from "react";
import Table from "react-bootstrap/Table";
import Button from "react-bootstrap/Button";
import axios from "axios";
import Modal from "react-bootstrap/Modal";

const ViewCustomers = () => {
  const [customers, setCustomers] = useState([]);
  const [filteredCustomers, setFilteredCustomers] = useState([]);
  const [filter, setFilter] = useState("all");
  const [showActivationModal, setShowActivationModal] = useState(false);
  const [selectedCustomerId, setSelectedCustomerId] = useState(null);
  const [alertMessage, setAlertMessage] = useState("");
  const [activationAction, setActivationAction] = useState(null); // New state to determine action (activate/deactivate)

  useEffect(() => {
    fetchCustomers();
  }, []);

  useEffect(() => {
    applyFilter();
  }, [customers, filter]);

  const fetchCustomers = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await axios.get("http://192.168.109.81/iCorner/api/customer", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      setCustomers(response.data);
    } catch (error) {
      console.error("Error fetching customers:", error);
    }
  };

  const applyFilter = () => {
    if (filter === "all") {
      setFilteredCustomers(customers);
    } else if (filter === "active") {
      setFilteredCustomers(customers.filter((customer) => customer.isActive));
    } else if (filter === "inactive") {
      setFilteredCustomers(customers.filter((customer) => !customer.isActive));
    }
  };

  const openActivationModal = (id, action) => {
    setSelectedCustomerId(id);
    setActivationAction(action); // Set the action (activate/deactivate)
    setShowActivationModal(true);
  };

  const handleActivateDeactivate = async () => {
    if (!selectedCustomerId) return;

    try {
      const token = localStorage.getItem("token");
      const endpoint =
        activationAction === "activate"
          ? `http://192.168.109.81/iCorner/api/customer/activate/${selectedCustomerId}`
          : `http://192.168.109.81/iCorner/api/customer/deactivate/${selectedCustomerId}`;

      await axios.put(
        endpoint,
        {},
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setAlertMessage(
        `Customer account has been ${
          activationAction === "activate" ? "activated" : "deactivated"
        }.`
      );
      fetchCustomers();
      setTimeout(() => {
        setAlertMessage(""); // Reset the alert message after timeout
      }, 3000);
    } catch (error) {
      console.error(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } customer:`,
        error
      );
      alert(
        `Error ${
          activationAction === "activate" ? "activating" : "deactivating"
        } customer`
      );
    } finally {
      setShowActivationModal(false);
      setSelectedCustomerId(null);
      setActivationAction(null); // Reset activation action
    }
  };

  return (
    <>
      <h1 className="text-center mb-3">Customers</h1>

      {alertMessage && (
        <div className="alert alert-success" role="alert">
          {alertMessage}
        </div>
      )}

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
            <th>Full Name</th>
            <th>Email</th>
            <th>Mobile Number</th>
            <th>Address</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {filteredCustomers.map((customer, index) => (
            <tr key={customer.id}>
              <td>{index + 1}</td>
              <td>{customer.fullName}</td>
              <td>{customer.email}</td>
              <td>{customer.mobileNumber}</td>
              <td>{customer.address}</td>
              <td>
                {customer.isActive ? "Active" : "Inactive"}
                <Button
                  variant={
                    customer.isActive ? "outline-warning" : "outline-success"
                  }
                  className="ml-2"
                  onClick={() =>
                    openActivationModal(
                      customer.email,
                      customer.isActive ? "deactivate" : "activate"
                    )
                  }
                >
                  {customer.isActive ? "Deactivate" : "Activate"}
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

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
          customer?
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

export default ViewCustomers;
