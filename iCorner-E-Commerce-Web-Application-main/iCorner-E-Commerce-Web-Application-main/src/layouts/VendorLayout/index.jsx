import SideNav from "../../components/SideNav";
import Header from "../../components/Header";
import { Outlet, Route, Routes } from "react-router-dom";
import Dashboard from "./Dashboard";
import VendorComments from "./ViewComments";
import ViewAllOrders from "./ViewAllOrders";
import CompletedOrders from "./CompletedOrders";
import PendingOrders from "./PendingOrders";
import AcceptedOrders from "./AcceptedOrders";
import './order.css'

const VendorLayout = () => {
  return (
    <>
      <Header />
      <div className="flex sticky top-0 left-0">
        <SideNav />
        <div className="flex flex-col flex-1" style={{ marginTop: "70px" }}>
          {" "}
          {/* Adjust the marginTop value as needed */}
          <div className="overflow-y-scroll h-full">
            {" "}
            {/* Ensure the container takes full height */}
            <Outlet />
            <Routes>
              <Route>
                <Route path="dashboard" element={<Dashboard />} />
                <Route path="comments" element={<VendorComments />} />
                <Route path="orders" element={<ViewAllOrders />} />
                <Route path="orders/completed" element={<CompletedOrders />} />
                <Route path="orders/pending" element={<PendingOrders />} />
                <Route path="orders/accepted" element={<AcceptedOrders />} />
              </Route>
            </Routes>
          </div>
        </div>
      </div>
    </>
  );
};

export default VendorLayout;
