import { useEffect, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBars, faChevronDown } from "@fortawesome/free-solid-svg-icons";
import adminSideNavigation from "../../data/AdminSideNavigation.json";
import vendorSideNavigation from "../../data/VendorSideNavigation.json";
import CSRSideNavigation from "../../data/CSRSideNavigation.json";
import { jwtDecode } from "jwt-decode";

const SideNav = () => {
  const [activePage, setActivePage] = useState("");
  const [isOpen, setIsOpen] = useState(false);
  const location = useLocation().pathname;
  const [userRole, setUserRole] = useState("");
  const [subMenuStates, setSubMenuStates] = useState({});

  useEffect(() => {
    setActivePage(location);
    const token = localStorage.getItem("token");
    if (token) {
      const decodedToken = jwtDecode(token);
      const userRole = decodedToken && decodedToken.role;
      setUserRole(userRole);
    }
  }, [location]);

  const handleMenu = () => {
    setIsOpen(!isOpen);
  };

  const toggleSubMenu = (link) => {
    setActivePage(link);
    setSubMenuStates({
      ...subMenuStates,
      [link]: !subMenuStates[link],
    });
  };

  const sideNavigationData =
    userRole === "Admin"
      ? adminSideNavigation.data
      : userRole === "Vendor"
      ? vendorSideNavigation.data
      : userRole === "CSR"
      ? CSRSideNavigation.data
      : null;

  return (
    <>
      {/* Header */}
      <div
        className="bg-primary text-light d-flex justify-content-between align-items-center px-3 py-3"
        style={{ height: "64px", zIndex: 10, position: "fixed", width: "100%" }}
      >
        <h4 className="m-0">
          {userRole === "admin" ? "Admin Panel" : "Instructor Panel"}
        </h4>
        <button
          className="btn btn-outline-light d-md-none"
          onClick={handleMenu}
        >
          <FontAwesomeIcon icon={faBars} />
        </button>
      </div>

      {/* Side Navigation */}
      <div
        className={`d-flex flex-column vh-100 position-sticky top-0 ${
          isOpen ? "position-absolute" : ""
        }`}
        style={{
          width: "250px",
          paddingTop: "64px",
          background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)", // Attractive gradient
          boxShadow: "2px 0 5px rgba(0, 0, 0, 0.2)", // Slight shadow
        }}
      >
        <div className="flex-grow-1 overflow-auto">
          {sideNavigationData && sideNavigationData.length ? (
            sideNavigationData.map((item) => (
              <div key={item.link || item.name}>
                <Link
                  to={item.link}
                  onClick={() => toggleSubMenu(item.link)}
                  className={`d-flex justify-content-between align-items-center px-3 py-2 text-light text-decoration-none ${
                    item.link === activePage ? "bg-dark bg-opacity-50" : ""
                  }`}
                  style={{
                    borderRadius: "8px", // Rounded corners for better appearance
                    margin: "4px 10px", // Spacing between links
                  }}
                >
                  <div className="d-flex align-items-center">
                    <FontAwesomeIcon icon={item.icon} className="me-2" />
                    {item.name}
                  </div>
                  {item.subMenu && item.subMenu.length > 0 && (
                    <FontAwesomeIcon
                      icon={faChevronDown}
                      className={`transition-transform ${
                        subMenuStates[item.link] ? "rotate-180" : ""
                      }`}
                    />
                  )}
                </Link>
                {item.subMenu && subMenuStates[item.link] && (
                  <div className="ms-3">
                    {item.subMenu.map((subItem) => (
                      <Link
                        key={subItem.link}
                        to={subItem.link}
                        className={`d-flex align-items-center px-3 py-2 text-light text-decoration-none ${
                          subItem.link === activePage
                            ? "bg-dark bg-opacity-50"
                            : ""
                        }`}
                        style={{
                          borderRadius: "8px",
                          margin: "4px 10px",
                        }}
                      >
                        <FontAwesomeIcon icon={subItem.icon} className="me-2" />
                        {subItem.name}
                      </Link>
                    ))}
                  </div>
                )}
              </div>
            ))
          ) : (
            <></>
          )}
        </div>
      </div>
    </>
  );
};

export default SideNav;
