// eslint-disable-next-line no-unused-vars
import React from "react";
import { Route, Routes } from "react-router-dom";
import SideNav from "../../components/SideNav";
import Header from "../../components/Header";
import { Outlet } from "react-router-dom";
import AddAdmin from "./Admins/AddAdmins";
import ViewAdmins from "./Admins/ViewAdmins";
import UpdateAdmin from "./Admins/UpdateAdmins";
import AddVendor from "./Vendors/AddVendors";
import ViewVendors from "./Vendors/ViewVendors";
import UpdateVendor from "./Vendors/UpdateVendors";
import AddCSR from "./CSRs/AddCSRs";
import ViewCSRs from "./CSRs/ViewCSRs";
import UpdateCSR from "./CSRs/UpdateCSRs";
import ViewCustomers from "./Customers/ViewCustomers";

const UserManagementLayout = () => {
  return (
    <>
      <Header />
      <div className="flex sticky top-0 left-0">
        <SideNav />
        <div className="flex flex-col flex-1" style={{ marginTop: "40px" }}>
          <div className="overflow-y-scroll pt-10 ml-10 mr-10">
            <Outlet />
            <Routes>
              <Route>
                <Route path="add-vendors" element={<AddVendor />} />
                <Route path="view-vendors" element={<ViewVendors />} />
                <Route path="update-vendor/:id" element={<UpdateVendor />} />
                <Route path="add-admins" element={<AddAdmin />} />
                <Route path="view-admins" element={<ViewAdmins />} />
                <Route path="update-admin/:id" element={<UpdateAdmin />} />
                <Route path="add-CSRs" element={<AddCSR />} />
                <Route path="view-CSRs" element={<ViewCSRs />} />
                <Route path="update-CSR/:id" element={<UpdateCSR />} />
                <Route path="view-customers" element={<ViewCustomers />} />
              </Route>
            </Routes>
          </div>
        </div>
      </div>
    </>
  );
};

export default UserManagementLayout;
