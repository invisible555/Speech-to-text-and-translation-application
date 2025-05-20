import React, { createContext, useContext, useState, useEffect } from 'react';
import AuthContextType from './AuthContextType';
import { logoutRequest } from '../Components/AuthService/AuthService';
const AuthContext = createContext<AuthContextType>({
  token: null,
  refreshToken: null,
  userLogin: null,
  role: null,
  login: () => {},
  logout: () => {},
});

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [token, setToken] = useState<string | null>(null);
  const [refreshToken, setRefreshToken] = useState<string | null>(null);
  const [userLogin, setUserLogin] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);

  const login = (token: string, refreshToken: string, login: string, role: string) => {
    setToken(token);
    setRefreshToken(refreshToken);
    setUserLogin(login);
    setRole(role);

    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('login', login);
    localStorage.setItem('role', role);
  };

  const logout = async () => {
  try {
    await logoutRequest(token);
  } catch (error) {
    console.error(error);
  } finally {
    setToken(null);
    setRefreshToken(null);
    setUserLogin(null);
    setRole(null);
    localStorage.clear();
  }
};

  return (
    <AuthContext.Provider value={{ token, refreshToken, userLogin, role, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
