import React, { createContext, useContext, useState, useEffect } from 'react';
import AuthContextType from "./AuthContextType";
import { jwtDecode } from 'jwt-decode';
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
    const [role, setRole] = useState<string | null>(null);
    const navigate = useNavigate();

    const navigateToLogin = () => {
        navigate('/login');
    };

    const decodeRole = (token: string): string => {
        const decoded: any = jwtDecode(token);
        return decoded.role;
    };

    const isTokenExpired = (token: string): boolean => {
        try {
            const decoded: any = jwtDecode(token);
            const currentTime = Date.now() / 1000;
            return decoded.exp < currentTime;
        } catch (error) {
            console.error('Nie udało się sprawdzić ważności tokena:', error);
            return true;
        }
    };

    useEffect(() => {
        const savedToken = localStorage.getItem('token');

        if (savedToken && !isTokenExpired(savedToken)) {
            const savedLogin = localStorage.getItem('login');
            const savedRole = localStorage.getItem('role');

            setToken(savedToken);
            setRole(savedRole);
            setLogin(savedLogin);
        } else {
            logout();
            navigateToLogin();
        }
    }, []);

    const login = (newToken: string, newUser: string) => {
        setToken(newToken);
        localStorage.setItem('token', newToken);

        const role = decodeRole(newToken);
        setRole(role);
        localStorage.setItem('role', role);

        setLogin(newUser);
        localStorage.setItem('login', newUser);
    };

    const logout = () => {
        setToken(null);
        localStorage.removeItem('token');

        setLogin(null);
        localStorage.removeItem('login'); // <- poprawka: było 'user', a powinno być 'login'

        setRole(null);
        localStorage.removeItem('role');
    };

    return (
        <AuthContext.Provider value={{ isLoggedIn: !!token, role, userLogin, token, login, logout, navigateToLogin }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
