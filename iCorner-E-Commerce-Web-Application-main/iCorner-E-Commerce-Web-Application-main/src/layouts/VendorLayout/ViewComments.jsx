import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode"; // Correct import for jwtDecode

const VendorComments = () => {
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Retrieve and decode the JWT token
  const token = localStorage.getItem("token");
  let decodedToken;

  if (token) {
    try {
      decodedToken = jwtDecode(token);
    } catch (err) {
      console.error("Invalid token:", err);
      decodedToken = null;
    }
  } else {
    decodedToken = null;
  }

  useEffect(() => {
    if (decodedToken && decodedToken.id) {
      const fetchComments = async () => {
        try {
          const response = await fetch(
            `http://192.168.109.81/iCorner/api/comment/vendor/${decodedToken.id}`
          );

          if (!response.ok) {
            throw new Error("Failed to fetch comments");
          }

          const data = await response.json();
          setComments(data);
        } catch (err) {
          setError(err.message);
        } finally {
          setLoading(false);
        }
      };

      fetchComments();
    } else {
      setLoading(false);
      setError("Invalid or missing token.");
    }
  }, [decodedToken]);

  if (loading) {
    return <div className="text-center my-5">Loading comments...</div>;
  }

  if (error) {
    return <div className="alert alert-danger my-3">Error: {error}</div>;
  }

  return (
    <div className="container mt-4">
      <h1 className="mb-4 text-center">
        Comments for Vendor {decodedToken?.username}
      </h1>
      {comments.length > 0 ? (
        <table className="table table-bordered table-hover">
          <thead>
            <tr>
              <th>Rating</th>
              <th>Product</th>
              <th>Comment</th>

              <th>User</th>
            </tr>
          </thead>
          <tbody>
            {comments.map((comment) => (
              <tr key={comment.id}>
                <td>{comment.rating || "No rating"}</td>
                <td>{comment.productId || "No product"}</td>
                <td>{comment.comments || "No comment"}</td>
                <td>{comment.userId || "No user"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p className="text-muted text-center">
          No comments available for this vendor.
        </p>
      )}
    </div>
  );
};

export default VendorComments;
