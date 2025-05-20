import { useEffect, useState } from 'react';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import Login from "./Components/Login/Login"
import Register from "./Components/Register/Register"
import Navbar from "./Components/Navbar/Navbar"
import { BrowserRouter as Router, Route,Routes, Link } from 'react-router-dom';
import HomePage from './Components/HomePage/HomePage';
import AudioUpload from './Components/AudioUpload/AudioUpload';
import { useTokenRefresh } from './Components/usetokenRefresh/useTokenRefresh';

function App() {
    useTokenRefresh();

   
    return (
            <div>

                    <Navbar />  
                    <div className="container mt-4">
                        <Routes>
                        <Route path="/" element={<HomePage />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="/uploadfile" element={<AudioUpload/>}/>
                        </Routes>
                    </div>
            </div>
    );


}


export default App;