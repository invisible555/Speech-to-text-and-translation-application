// authService.ts
import axios from 'axios';

export const logoutRequest = (token: string | null) => {
  if (!token) return Promise.resolve();
  return axios.post('/api/Users/logout', null, {
    headers: { Authorization: `Bearer ${token}` },
  });
};
