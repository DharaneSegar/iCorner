import React, { createContext, useState, useEffect, useContext } from "react";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [isLoggedIn, setIsLoggedIn] = useState(() => {
    // Initialize isLoggedIn based on localStorage
    const loggedIn = localStorage.getItem("isLoggedIn");
    return loggedIn === "true";
  });

  useEffect(() => {
    // Check localStorage for login status
    const loggedIn = localStorage.getItem("isLoggedIn");
    if (loggedIn === "true" || loggedIn === "false") {
      // Handle unexpected values
      setIsLoggedIn(loggedIn === "true");
    } else {
      setIsLoggedIn(false); // Default to false if value is unexpected
    }
  }, []);

  const login = () => {
    setIsLoggedIn(true);
    localStorage.setItem("isLoggedIn", "true");
  };

  const logout = () => {
    setIsLoggedIn(false);
    localStorage.removeItem("isLoggedIn");
  };

  return (
    <AuthContext.Provider value={{ isLoggedIn, setIsLoggedIn, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
