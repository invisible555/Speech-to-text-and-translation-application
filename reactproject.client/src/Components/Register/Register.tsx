import React, { useState } from 'react';
import axios from 'axios';
import RegisterType from './RegisterType';

const Register: React.FC<RegisterType> = () => {
    const [email, setEmail] = useState('');
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');

    const validateEmail = (email: string) => {
        const re = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return re.test(email);
    };

    const validateLogin = (login: string) => {
        return login.length >= 3;
    };

    const validatePassword = (password: string) => {
        return password.length >= 6;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!validateEmail(email)) {
            setErrorMessage('Proszę podać poprawny adres email!');
            return;
        }

        if (!validateLogin(login)) {
            setErrorMessage('Login musi mieć co najmniej 3 znaki!');
            return;
        }

        if (!validatePassword(password)) {
            setErrorMessage('Hasło musi mieć co najmniej 6 znaków!');
            return;
        }

        setErrorMessage('');  // Reset error message

        try {
            // Send registration data to backend
            const response = await axios.post('http://localhost:5212/api/Users/register/user', {
             
                login,
                email,
                password,
            });

            // Handle successful registration
            if (response.status === 200) {
                setSuccessMessage('Rejestracja zakończona sukcesem! Proszę zalogować się.');
                setEmail('');
                setLogin('');
                setPassword('');
            }
        } catch (error: any) {
            // Handle errors
            if (error.response) {
                // Server responded with a status other than 2xx
                setErrorMessage(error.response.data.message || 'Błąd rejestracji');
            } else if (error.request) {
                // Request was made but no response was received
                setErrorMessage('Brak odpowiedzi z serwera');
            } else {
                // Something else happened while setting up the request
                setErrorMessage('Wystąpił błąd podczas próby rejestracji');
            }
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
            <div className="card p-4 shadow w-100" style={{ maxWidth: '400px' }}>
                <h3 className="mb-4 text-center">Rejestracja</h3>

                {/* Display success message */}
                {successMessage && <div className="alert alert-success">{successMessage}</div>}

                {/* Display error message */}
                {errorMessage && <div className="alert alert-danger">{errorMessage}</div>}

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
