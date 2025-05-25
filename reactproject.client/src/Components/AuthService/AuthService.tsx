// authService.ts
import axiosInstance from '../../../utils/axiosConfig';

export const logoutRequest = (token: string | null) => {
  if (!token) return Promise.resolve();
  return axiosInstance.post('Users/logout', null, {
    headers: { Authorization: `Bearer ${token}` },
  });
};
