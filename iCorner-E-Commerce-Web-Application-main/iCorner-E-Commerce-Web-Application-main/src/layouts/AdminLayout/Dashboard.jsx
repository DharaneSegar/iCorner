import { useState } from "react";
import { FiPlus } from "react-icons/fi";
import { motion } from "framer-motion";
import { FaUser, FaBoxes, FaList, FaTruck } from "react-icons/fa";

const Dashboard = () => {
  return (
    <div className="h-screen w-full bg-gray-100 text-black">
      <AdminBoard />
    </div>
  );
};

const AdminBoard = () => {
  const [sections, setSections] = useState([
    { id: "user-management", name: "User Management", icon: <FaUser /> },
    { id: "product-management", name: "Product Management", icon: <FaBoxes /> },
    { id: "order-management", name: "Order Management", icon: <FaList /> },
    {
      id: "inventory-management",
      name: "Inventory Management",
      icon: <FaTruck />,
    },
  ]);

  return (
    <>
      <div className="flex justify-center">
        <h1 className="text-4xl font-bold mb-8 mt-2">Admin Dashboard</h1>
      </div>
      <div className="grid grid-cols-2 gap-6 p-4">
        {sections.map((section) => (
          <DashboardSection key={section.id} {...section} />
        ))}
      </div>
    </>
  );
};

const DashboardSection = ({ id, name, icon }) => {
  return (
    <motion.div
      layout
      className="cursor-pointer border border-gray-400 bg-white p-4 rounded-lg shadow-md hover:shadow-lg"
    >
      <div className="flex items-center justify-between">
        <div className="text-xl font-bold flex items-center space-x-2">
          {icon}
          <span>{name}</span>
        </div>
        <FiPlus className="text-gray-500" />
      </div>
      <p className="mt-2 text-gray-600">
        Manage {name.toLowerCase()} and perform key actions.
      </p>
    </motion.div>
  );
};

export default Dashboard;
