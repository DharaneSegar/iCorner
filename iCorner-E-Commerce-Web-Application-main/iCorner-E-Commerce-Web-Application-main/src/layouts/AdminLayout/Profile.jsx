import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMobile } from "@fortawesome/free-solid-svg-icons";
import { toast } from "react-toastify";
import background from "../../assets/apple.png";
import { useAuth } from "../../context/AuthContext";
import ConfirmationModal from "../CommonLayout/ConfirmationModal";
import { Button } from "react-bootstrap";
import { jwtDecode } from "jwt-decode"; // Correct the import

export default function UserProfile() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState(""); // Input password
  const [userName, setUserName] = useState("");
  const [contact, setContact] = useState("");
  const [address, setAddress] = useState("");
  const [id, setId] = useState("");
  const [role, setRole] = useState("");
  const [showConfirmationModal, setShowConfirmationModal] = useState(false);
  const [oldPassword, setOldPassword] = useState(""); // Store old password

  const navigate = useNavigate();
  const { logout } = useAuth();

  useEffect(() => {
    const getUserDetails = async () => {
      try {
        const token = localStorage.getItem("token");
        if (token) {
          const decodedToken = jwtDecode(token);
          const response = await axios.get(
            `http://192.168.109.81/iCorner/api/user/user/${decodedToken.id}`,
            {
              headers: {
                Authorization: `Bearer ${token}`,
              },
            }
          );
          console.log(response);

          setUserName(response.data.username);
          setEmail(response.data.email); // Non-editable field
          setContact(response.data.mobileNumber);
          setId(response.data.id);
          setAddress(response.data.address);
          setOldPassword(response.data.password); // Store old password
          setRole(response.data.role);
        }
      } catch (error) {
        toast.error("Error fetching user data.");
      }
    };

    getUserDetails();
  }, []);

  const updateProfile = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("token");

      // If no new password is entered, keep the old password
      const updatedData = {
        id,
        username: userName,
        email, // Non-editable
        mobileNumber: contact,
        address,
        password: password || oldPassword, // Use old password if no new password
        role: role,
        isActive: true,
      };

      console.log(updatedData);

      await axios.put(
        `http://192.168.109.81/iCorner/api/user/update/${id}`,
        updatedData,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      toast.success("You've updated your account successfully.");
      logout();
      navigate("/");
    } catch (error) {
      toast.error(
        error.response
          ? error.response.data.message
          : "An error occurred while updating the user."
      );
    }
  };

  const deleteProfile = (e) => {
    e.preventDefault();
    setShowConfirmationModal(true); // Show confirmation modal
  };

  const confirmDelete = async () => {
    try {
      const token = localStorage.getItem("token");
      const { data } = await axios.delete(
        `http://192.168.109.81/iCorner/api/user/delete/${id}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (data.success) {
        toast.success("You've deleted your account successfully.");
        logout();
        navigate("/");
      } else {
        toast.error(data.message);
      }
    } catch (error) {
      toast.error(
        error.response
          ? error.response.data.message
          : "An error occurred while deleting the user."
      );
    }
  };

  const cancelDelete = () => {
    setShowConfirmationModal(false); // Close the modal without deleting
  };

  return (
    <>
      <div
        className="d-flex justify-content-center align-items-center vh-250"
        style={{
          backgroundImage: `url(${background})`,
          backgroundSize: "cover",
          backgroundPosition: "center",
        }}
      >
        <div className="card w-100" style={{ maxWidth: "30rem" }}>
          <div className="card-body text-center">
            <h1 className="text-dark display-4">
              <FontAwesomeIcon className="me-2" icon={faMobile} />
              iCorner.
            </h1>
            <h3 className="mt-4 mb-4 display-6 fw-medium">Your User Profile</h3>

            <form onSubmit={updateProfile}>
              <div className="mb-3">
                <label htmlFor="userName" className="form-label">
                  User Name
                </label>
                <input
                  id="userName"
                  name="userName"
                  type="text"
                  required
                  className="form-control"
                  onChange={(e) => setUserName(e.target.value)}
                  value={userName}
                />
              </div>

              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  Email Address
                </label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  className="form-control"
                  value={email}
                  readOnly
                />
              </div>

              <div className="mb-3">
                <label htmlFor="password" className="form-label">
                  Password (leave blank if unchanged)
                </label>
                <input
                  id="password"
                  name="password"
                  type="password"
                  className="form-control"
                  onChange={(e) => setPassword(e.target.value)}
                  value={password}
                  placeholder="Enter new password"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="contact" className="form-label">
                  Contact Number
                </label>
                <input
                  id="contact"
                  name="contact"
                  type="text"
                  required
                  className="form-control"
                  onChange={(e) => setContact(e.target.value)}
                  value={contact}
                />
              </div>

              <div className="mb-3">
                <label htmlFor="address" className="form-label">
                  Address
                </label>
                <input
                  id="address"
                  name="address"
                  type="text"
                  required
                  className="form-control"
                  onChange={(e) => setAddress(e.target.value)}
                  value={address}
                />
              </div>

              <div className="d-flex justify-content-between">
                <Button variant="outline-primary" type="submit">
                  Update Profile
                </Button>

                <Button variant="outline-danger" onClick={deleteProfile}>
                  Delete Profile
                </Button>
              </div>
            </form>
          </div>
        </div>
      </div>

      {showConfirmationModal && (
        <ConfirmationModal
          message="Are you sure you want to delete your account? This action cannot be undone."
          onConfirm={confirmDelete}
          onCancel={cancelDelete}
        />
      )}
    </>
  );
}
