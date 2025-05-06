import { useEffect, useState } from 'react';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css'
import Login from "./Components/Login/Login"
import Register from "./Components/Register/Register"


import { BrowserRouter as Router, Route,Routes, Link } from 'react-router-dom';
//TODO routing instead of changing component
function App() {


    function onSubmit() {

    }



    return (
        <div>
            <Router>
                <nav>
                    <Link to="/">Home </Link>
                    <Link to="/login">Login</Link>
                    <Link to="/register">Register</Link>
                </nav>
                <Routes>
                    <Route path="/login" element={<Login onSubmit={onSubmit} />} />
                    <Route path="/register" element={<Register onSubmit={onSubmit} />} />
                    <Route path="/" />
                </Routes>
            </Router>




        </div>
    );


}


export default App;