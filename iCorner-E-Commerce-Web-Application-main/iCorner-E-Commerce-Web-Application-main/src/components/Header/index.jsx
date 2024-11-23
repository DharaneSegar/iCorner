import { useEffect, useState } from "react";
import { Dropdown, Nav, Navbar, Modal, Button } from "react-bootstrap"; // Import Bootstrap components
import Person2Icon from "@mui/icons-material/Person2";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMobile } from "@fortawesome/free-solid-svg-icons";
import { Link, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { jwtDecode } from "jwt-decode"; // Ensure jwt-decode is installed

const userNavigation = [
  { name: "Your Profile", href: "/admin/profile" },
  { name: "Sign out" },
];

export default function Header() {
  const [username, setUsername] = useState(""); // State to store username
  const [showConfirm, setShowConfirm] = useState(false);
  let navigate = useNavigate();

  // Fetch token from localStorage and decode it to get username
  useEffect(() => {
    const token = localStorage.getItem("token"); // Get token from localStorage
    if (token) {
      const decodedToken = jwtDecode(token);
      // Assuming your token has "username" or "email" field
      setUsername(decodedToken.username || ""); // Set email or username
    }
  }, []);

  const handleSignOut = () => {
    setShowConfirm(true);
  };

  const handleConfirmSignOut = () => {
    try {
      toast.success("You've logged out successfully");
      localStorage.removeItem("token");
      navigate("/"); // Navigate to home or login page
      setShowConfirm(false);
    } catch (error) {
      toast.error("Error logging out");
    }
  };

  const handleCancelSignOut = () => {
    setShowConfirm(false);
  };

  return (
    <>
      <Navbar
        expand="lg"
        className="fixed top-0 left-0 w-full z-50" // Ensure the Header stays fixed at the top
        style={{
          background: "linear-gradient(135deg, #4a3a89 0%, #5a3d94 100%)", // Darker gradient to sync with sidebar
          zIndex: 1000,
        }}
      >
        <div className="container">
          <Navbar.Brand
            as={Link}
            to="/admin"
            style={{
              fontWeight: "bold",
              fontSize: "1.5rem",
              color: "white",
            }}
          >
            <FontAwesomeIcon icon={faMobile} className="me-2" />
            iCorner.
          </Navbar.Brand>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="ms-auto">
              <Dropdown align="end">
                <Dropdown.Toggle
                  variant="outline-light"
                  className="d-flex align-items-center"
                  style={{
                    color: "#d1e0f3", // Slightly muted light blue text for a softer contrast
                    backgroundColor: "#0c1326", // Darker background for contrast
                    borderColor: "#38bdf8", // Matching border color
                  }}
                >
                  {username}
                  <Person2Icon
                    className="ms-2"
                    style={{
                      fontSize: "24px",
                      color: "#38bdf8", // Cyan icon color for consistency
                    }}
                  />
                </Dropdown.Toggle>

                <Dropdown.Menu>
                  {userNavigation.map((item) => (
                    <Dropdown.Item
                      key={item.name}
                      as={Link}
                      to={item.href}
                      onClick={item.name === "Sign out" ? handleSignOut : null}
                      style={{
                        color: "#1e293b", // Dark text
                        fontWeight: "500", // Slightly bold for better readability
                      }}
                    >
                      {item.name}
                    </Dropdown.Item>
                  ))}
                </Dropdown.Menu>
              </Dropdown>
            </Nav>
          </Navbar.Collapse>
        </div>
      </Navbar>

      {/* Confirmation Modal */}
      <Modal show={showConfirm} onHide={handleCancelSignOut}>
        <Modal.Header closeButton>
          <Modal.Title>Confirm Logout</Modal.Title>
        </Modal.Header>
        <Modal.Body>Are you sure you want to log out?</Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            className="text-white bg-secondary"
            onClick={handleCancelSignOut}
          >
            Cancel
          </Button>
          <Button
            variant="primary"
            className="text-white bg-primary"
            onClick={handleConfirmSignOut}
          >
            Confirm
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}
