// eslint-disable-next-line no-unused-vars
import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import "antd/dist/reset.css";
import CommonLayout from "../layouts/CommonLayout";
import SignIn from "../layouts/CommonLayout/SignIn";
import CategoryLayout from "../layouts/CategoryLayout";
import ProductLayout from "../layouts/ProductLayout";
import CategoryLayoutAdmin from "../layouts/CategoryLayoutAdmin";
import ProductLayoutAdmin from "../layouts/ProductLayoutAdmin";
import InventoryLayout from "../layouts/InventoryLayout";
import InventoryLayoutAdmin from "../layouts/InventoryLayoutAdmin";
import VendorLayout from "../layouts/VendorLayout";
import AdminLayout from "../layouts/AdminLayout";
import CSRLayout from "../layouts/CSRLayout";
import UserManagementLayout from "../layouts/UserManagementLayout";
import NotFound from "../NotFound";

const FrontendRoutes = () => {
  return (
    <>
      <Router>
        <Routes>
          <Route element={<CommonLayout />}>
            <Route path="/" element={<SignIn />} />
            <Route path="signup" />
          </Route>

          <Route path="vendor" element={<VendorLayout />}>
            <Route path="dashboard" />
            <Route path="comments" />
            <Route path="orders" />
            <Route path="orders/completed" />
            <Route path="orders/pending" />
            <Route path="orders/accepted" />
          </Route>

          <Route path="users" element={<UserManagementLayout />}>
            <Route path="add-vendors" />
            <Route path="view-vendors" />
            <Route path="update-vendor/:id" />
            <Route path="add-admins" />
            <Route path="view-admins" />
            <Route path="update-admin/:id" />
            <Route path="add-CSRs" />
            <Route path="view-CSRs" />
            <Route path="update-CSR/:id" />
            <Route path="view-customers" />
          </Route>

          <Route path="admin" element={<AdminLayout />}>
            <Route path="profile" />
            <Route path="dashboard" />
          </Route>

          <Route path="CSR" element={<CSRLayout />}>
            <Route path="dashboard" />
          </Route>

          <Route path="category" element={<CategoryLayout />}>
            <Route path="view-category" />
            <Route path="add-category" />
            <Route path="view-category-details/:id" />
            <Route path="update-category/:id" />
          </Route>

          <Route path="product" element={<ProductLayout />}>
            <Route path="view-product" />
            <Route path="add-product" />
            <Route path="view-product-details/:id" />
            <Route path="update-product/:id" />
          </Route>

          <Route path="inventory" element={<InventoryLayout />}>
            <Route path="view-inventory" />
            <Route path="add-inventory" />
            <Route path="view-inventory-details/:id" />
            <Route path="update-inventory/:id" />
          </Route>

          <Route path="admin-category" element={<CategoryLayoutAdmin />}>
            <Route path="view-category" />
            <Route path="add-category" />
            <Route path="view-category-details/:id" />
            <Route path="update-category/:id" />
          </Route>

          <Route path="admin-product" element={<ProductLayoutAdmin />}>
            <Route path="view-product" />
            <Route path="add-product" />
            <Route path="view-product-details/:id" />
            <Route path="update-product/:id" />
          </Route>

          <Route path="admin-inventory" element={<InventoryLayoutAdmin />}>
            <Route path="view-inventory" />
            <Route path="add-inventory" />
            <Route path="view-inventory-details/:id" />
            <Route path="update-inventory/:id" />
          </Route>

          <Route path="*" element={<NotFound />} />
        </Routes>
      </Router>
    </>
  );
};

export default FrontendRoutes;
