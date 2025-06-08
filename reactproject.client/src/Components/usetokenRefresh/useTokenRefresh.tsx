// useTokenRefresh.ts
import { useEffect } from 'react';
import { useAuth } from '../../Context/AuthContext';
import {jwtDecode} from 'jwt-decode';
import { useNavigate } from 'react-router-dom';
import axiosInstance from '../../../utils/axiosConfig';
import axios from 'axios';

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
        // const response = await axiosInstance.post('User/refresh-access-token', {
        //   refreshToken: savedRefreshToken,
        // });
          const response = await axios.post('User/refresh-access-token', {
           refreshToken: savedRefreshToken,
         });
        const newToken = response.data.accessToken;
        const role = decodeRole(newToken);
        login(newToken, savedRefreshToken, savedLogin ?? '', role);
      } catch (error) {
      
        console.error("Refresh token error", error);
        logout();
        
        //setTimeout(() => navigate('/login'), 0);
      }
    } else {
      logout();
      //setTimeout(() => navigate('/login'), 0);
    }
  };

  initializeAuth();
  }, [login, logout, navigate]);

};
