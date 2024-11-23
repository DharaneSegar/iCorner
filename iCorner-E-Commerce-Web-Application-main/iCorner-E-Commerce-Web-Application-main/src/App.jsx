import FrontendRoutes from "./routes";
import { fas } from "@fortawesome/free-solid-svg-icons";
import { library } from "@fortawesome/fontawesome-svg-core";

function App() {

  library.add(fas);

  return (
    <>
      <FrontendRoutes />
    </>
  );
}

export default App;
