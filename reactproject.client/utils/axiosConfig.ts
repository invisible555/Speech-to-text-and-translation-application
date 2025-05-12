import axios from 'axios';
import { useAuth } from '../src/Context/AuthContext';  // Upewnij się, że masz odpowiedni kontekst autoryzacji



const axiosInstance = axios.create({
  baseURL: 'http://localhost:5212/api/',  // Zmienna do podstawowego URL API
  
});

// Interceptor dla requestów - dodawanie tokenu do nagłówka
axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');  // Pobierz token z localStorage
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;  // Dodaj token do nagłówka
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor dla odpowiedzi - obsługa błędów (np. 401)
axiosInstance.interceptors.response.use(
  (response) => response,  // Jeśli odpowiedź jest sukcesem, po prostu ją zwróć
  (error) => {
    if (error.response && error.response.status === 401) {
      // Jeśli błąd to 401 (Unauthorized), przekieruj do logowania
      const { logout, navigateToLogin } = useAuth();
      //logout(); // Wyloguj użytkownika
      //navigateToLogin();
       // Przekierowanie do strony logowania
       
       console.log("logout")
    }
    return Promise.reject(error);  // Inne błędy pozostają bez zmian
  }
);

export default axiosInstance;
