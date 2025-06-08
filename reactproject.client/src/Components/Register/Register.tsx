import React, { useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';
import RegisterType from './RegisterType';
import axios from 'axios';
const Register: React.FC<RegisterType> = () => {
    const [email, setEmail] = useState('');
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessages, setErrorMessages] = useState<string[]>([]);
    const [successMessage, setSuccessMessage] = useState('');

    const validateEmail = (email: string) => /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email);
    const validateLogin = (login: string) => login.length >= 3;
    const validatePassword = (password: string) => password.length >= 6;

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const localErrors = [];
        if (!validateEmail(email)) localErrors.push('Proszę podać poprawny adres email!');
        if (!validateLogin(login)) localErrors.push('Login musi mieć co najmniej 3 znaki!');
        if (!validatePassword(password)) localErrors.push('Hasło musi mieć co najmniej 6 znaków!');

        if (localErrors.length > 0) {
            setErrorMessages(localErrors);
            return;
        }

        setErrorMessages([]); // Reset błędów

        try {
            const response = await axiosInstance.post('User/register/user', {
                login,
                email,
                password,
            });
           

            if (response.status === 200) {
                setSuccessMessage('Rejestracja zakończona sukcesem! Proszę się zalogować.');
                setEmail('');
                setLogin('');
                setPassword('');
            }
        } catch (error: any) {
            if (error.response) {
                const data = error.response.data;

                if (data.errors) {
                    // Zbierz wszystkie błędy walidacyjne z backendu
                    const backendErrors = Object.values(data.errors).flat() as string[];
                    setErrorMessages(backendErrors);
                } else {
                    setErrorMessages([data.title || 'Błąd rejestracji']);
                }
            } else if (error.request) {
                setErrorMessages(['Brak odpowiedzi z serwera']);
            } else {
                setErrorMessages(['Wystąpił błąd podczas próby rejestracji']);
            }
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
            <div className="card p-4 shadow w-100" style={{ maxWidth: '400px' }}>
                <h3 className="mb-4 text-center">Rejestracja</h3>

                {successMessage && (
                    <div className="alert alert-success">{successMessage}</div>
                )}

                {errorMessages.length > 0 && (
                    <div className="alert alert-danger">
                        <ul className="mb-0">
                            {errorMessages.map((msg, index) => (
                                <li key={index}>{msg}</li>
                            ))}
                        </ul>
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label className="form-label">Email:</label>
                        <input
                            type="email"
                            className="form-control"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <label className="form-label">Login:</label>
                        <input
                            type="text"
                            className="form-control"
                            value={login}
                            onChange={(e) => setLogin(e.target.value)}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <label className="form-label">Hasło:</label>
                        <input
                            type="password"
                            className="form-control"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    <button type="submit" className="btn btn-success w-100">
                        Zarejestruj się
                    </button>
                </form>
            </div>
        </div>
    );
};

export default Register;
