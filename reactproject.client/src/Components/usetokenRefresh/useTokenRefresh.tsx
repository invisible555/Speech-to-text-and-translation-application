// useTokenRefresh.ts
import { useEffect } from 'react';
import axios from 'axios';
import { useAuth } from '../../Context/AuthContext';
import {jwtDecode} from 'jwt-decode';
import { useNavigate } from 'react-router-dom';

export const useTokenRefresh = () => {
  const { token, refreshToken, login, logout } = useAuth();
  const navigate = useNavigate();

  const isTokenExpired = (token: string): boolean => {
    try {
      const decoded: any = jwtDecode(token);
      const currentTime = Date.now() / 1000;
      return decoded.exp < currentTime;
    } catch {
      return true;
    }
  };

  const decodeRole = (token: string): string => {
    try {
      const decoded: any = jwtDecode(token);
      return decoded.role ?? '';
    } catch {
      return '';
    }
  };

  useEffect(() => {
    const initializeAuth = async () => {
      const savedToken = localStorage.getItem('token');
      const savedRefreshToken = localStorage.getItem('refreshToken');
      const savedLogin = localStorage.getItem('login');

      if (savedToken && !isTokenExpired(savedToken)) {
        const role = decodeRole(savedToken);
        login(savedToken, savedRefreshToken ?? '', savedLogin ?? '', role);
      } else if (savedRefreshToken) {
        // próbuj odświeżyć token
        try {
          const response = await axios.post('https://localhost:7260/api/User/refresh-access-token', {
            refreshToken: savedRefreshToken,
          });
          const newToken = response.data.accessToken;
          const role = decodeRole(newToken);
          login(newToken, savedRefreshToken, savedLogin ?? '', role);
        } catch (error) {
          logout();
          navigate('/login');
        }
      } else {
        logout();
        navigate('/login');
      }
    };

    initializeAuth();
  }, [login, logout, navigate]);
};
