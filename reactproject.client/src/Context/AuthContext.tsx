import React, { createContext, useContext, useState, useEffect } from 'react';
import AuthContextType from "./AuthContextType"
import {jwtDecode} from 'jwt-decode';
import { useNavigate } from 'react-router-dom';


const AuthContext = createContext<AuthContextType>({
    isLoggedIn: false,
    userLogin: null,
    token: null,
    role: null,
    login: () => { },
    logout: () => { },
    navigateToLogin: () => {},
});

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [token, setToken] = useState<string | null>(null);
    const [userLogin, setLogin] = useState<string | null>(null);
    const [role,setRole] = useState<string | null>(null);
    const navigate = useNavigate();


    const navigateToLogin = () => {
    navigate('/login'); // Funkcja do przekierowania
  };
    const decodeRole = (token:string) => {
        const decoded: any = jwtDecode(token);
        const userRole = decoded.role
        console.log(userRole)
        return userRole;
    }
    useEffect(() => {
        const savedToken = localStorage.getItem('token');
        const savedLogin = localStorage.getItem('login');
        const savedRole = localStorage.getItem('role')

        if (savedToken) {
            setToken(savedToken);
            setRole(savedRole)
        }
        if (savedLogin) {
            setLogin(savedLogin);
        }
    }, []);

    
    const login = (newToken: string,newUser:string) => {
        setToken(newToken);
        localStorage.setItem('token', newToken);
        const role = decodeRole(newToken);
        setRole(role);
        localStorage.setItem('role',role);
        setLogin(newUser);
        localStorage.setItem('login', newUser);

    };


    const logout = () => {
        setToken(null);
        localStorage.removeItem('token');
        setLogin(null);
        localStorage.removeItem('user');
        localStorage.removeItem('role');
        setRole(null);
    };

    
    return (
        <AuthContext.Provider value={{ isLoggedIn: !!token,role,userLogin, token, login, logout, navigateToLogin }}>
            {children}
        </AuthContext.Provider>
    );
};


export const useAuth = () => useContext(AuthContext);
