import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";

// PrimeReact CSS
import "primereact/resources/themes/lara-dark-indigo/theme.css"; // theme tối đẹp
import "primereact/resources/primereact.min.css";
import "primeicons/primeicons.css";
import "primeflex/primeflex.css";

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(<App />);
