// eslint-disable-next-line no-unused-vars
import React from "react";
import { Route, Routes } from "react-router-dom";
import SideNav from "../../components/SideNav";
import Header from "../../components/Header";
import { Outlet } from "react-router-dom";
import ViewProduct from "./ViewProduct";
import AddProduct from "./AddProduct";
import UpdateProduct from "./UpdateProduct";
import ProductDetails from "./ProductDetails";

const ProductLayoutAdmin = () => {
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
                <Route path="view-product" element={<ViewProduct />} />
                <Route path="add-product" element={<AddProduct />} />
                <Route path="update-product/:id" element={<UpdateProduct />} />
                <Route
                  path="view-product-details/:id"
                  element={<ProductDetails />}
                />
              </Route>
            </Routes>
          </div>
        </div>
      </div>
    </>
  );
};

export default ProductLayoutAdmin;
