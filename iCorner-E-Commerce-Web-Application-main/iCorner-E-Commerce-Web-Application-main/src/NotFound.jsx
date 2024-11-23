import { Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const NotFound = () => {
  const navigate = useNavigate();

  const handleBack = () => {
    navigate("/"); // Navigate back to the homepage or any default route
  };

  return (
    <div className="not-found-container text-center">
      <h1 className="display-4">404</h1>
      <p className="lead">Oops! The page you're looking for doesn't exist.</p>
      <Button
        variant="primary"
        className="text-white bg-primary"
        onClick={handleBack}
      >
        Go Back Home
      </Button>
    </div>
  );
};

export default NotFound;
