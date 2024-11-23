import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faMobile,
  faExclamationCircle,
} from "@fortawesome/free-solid-svg-icons";
import background from "../../assets/apple.png";
import "bootstrap/dist/css/bootstrap.min.css";
import { Toast, ToastContainer } from "react-bootstrap"; // Import Toast components
import { jwtDecode } from "jwt-decode"; // Use named import
import "./custom.css";

export default function SignIn() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState("");
  const [toastType, setToastType] = useState("danger"); // Default to error toast

  const validateEmail = (email) => {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
  };

  const validatePassword = (password) => {
    return password.length >= 6;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setToastMessage("");

    if (!validateEmail(email)) {
      setToastMessage("Please enter a valid email address.");
      setShowToast(true);
      return;
    }

    if (!validatePassword(password)) {
      setToastMessage("Password must be at least 6 characters long.");
      setShowToast(true);
      return;
    }

    try {
      const response = await fetch("http://192.168.109.81/iCorner/api/user/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Login failed");
      }

      const data = await response.json();
      console.log(data);
      localStorage.setItem("token", data.token);

      const decodedToken = jwtDecode(data.token);
      const userRole = decodedToken.role;
      setToastMessage("Login successful!");
      setToastType("success");
      setShowToast(true);

      // Redirect based on the user role
      switch (userRole) {
        case "Admin":
          window.location.href = "/admin/dashboard"; // Redirect to Admin dashboard
          break;
        case "Vendor":
          window.location.href = "/vendor/dashboard"; // Redirect to Vendor dashboard
          break;
        case "CSR":
          window.location.href = "/CSR/dashboard"; // Redirect to CSR dashboard
          break;
      }
    } catch (error) {
      setToastMessage(error.message);
      setToastType("danger"); // Keep as error
      setShowToast(true);
    }
  };

  return (
    <div
      className="min-vh-100 d-flex justify-content-center align-items-center"
      style={{
        backgroundImage: `url(${background})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
      }}
    >
      <ToastContainer position="top-end" className="p-3">
        <Toast
          show={showToast}
          onClose={() => setShowToast(false)}
          delay={3000}
          autohide
          bg={toastType} // Set background color based on toast type
          className="toast"
        >
          <Toast.Body className="toast-body">
            <FontAwesomeIcon
              icon={toastType === "danger" ? faExclamationCircle : faMobile}
              color="white"
            />{" "}
            {toastMessage}
          </Toast.Body>
        </Toast>
      </ToastContainer>
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-lg-5 col-md-7">
            <div className="card shadow-lg border-0 rounded-lg mt-5">
              <div className="card-body">
                <div className="text-center">
                  <h1 className="text-dark display-4">
                    <FontAwesomeIcon className="me-2" icon={faMobile} />
                    iCorner.
                  </h1>
                  <h3 className="mt-4 mb-4 display-6 fw-medium">
                    Sign in to your account
                  </h3>
                </div>
                <form onSubmit={handleSubmit}>
                  <div className="mb-3">
                    <label htmlFor="email" className="form-label">
                      Email Address
                    </label>
                    <input
                      id="email"
                      name="email"
                      type="email"
                      autoComplete="email"
                      required
                      className="form-control"
                      onChange={(e) => setEmail(e.target.value)}
                      value={email}
                    />
                  </div>
                  <div className="mb-3">
                    <div className="d-flex justify-content-between">
                      <label htmlFor="password" className="form-label">
                        Password
                      </label>
                      <a href="/forgot" className="text-muted small">
                        Forgot Password?
                      </a>
                    </div>
                    <input
                      id="password"
                      name="password"
                      type="password"
                      autoComplete="current-password"
                      required
                      className="form-control"
                      onChange={(e) => setPassword(e.target.value)}
                      value={password}
                    />
                  </div>
                  <div className="d-grid">
                    <button type="submit" className="btn btn-dark btn-lg">
                      Sign in
                    </button>
                  </div>
                </form>
                <p className="mt-4 text-center text-muted">
                  Don’t have an account?
                  <a href="/signup" className="text-dark fw-semibold">
                    Sign Up
                  </a>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
