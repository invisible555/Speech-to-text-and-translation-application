import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App'
import { AuthProvider } from "./Context/AuthContext"
import { BrowserRouter as Router } from 'react-router-dom';

const rootElement = document.getElementById('root');
if (!rootElement) {
    throw new Error("Nie znaleziono elementu #root w HTML.");
}

createRoot(rootElement).render(
    <StrictMode>
        <Router>
            <AuthProvider>
                <App />
            </AuthProvider>
        </Router>
    </StrictMode>
);