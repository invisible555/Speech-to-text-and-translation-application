import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../Context/AuthContext';
import axiosInstance from '../../../utils/axiosConfig';

const Login = () => {
    const [loginInput, setLoginInput] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        try {
            const response = await axiosInstance.post('User/login', {  
                login: loginInput,
                password: password,
            });
            const data = response.data;
            const token = data.accessToken;
            const user = data.login;

            if (token && user) {
                login(token, user);
                navigate('/');
            } else {
                throw new Error('Brak tokena w odpowiedzi serwera');
            }
        } catch (err: any) {
            console.error(err);
            setError('Wystąpił błąd podczas logowania');
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
            <div className="card p-4 shadow w-100" style={{ maxWidth: '400px' }}>
                <h3 className="mb-4 text-center">Logowanie</h3>

                {/* Display error message */}
                {error && <div className="alert alert-danger">{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label className="form-label">Login</label>
                        <input
                            type="text"
                            className="form-control"
                            value={loginInput}
                            onChange={(e) => setLoginInput(e.target.value)}
                            required
                        />
                    </div>

                    <div className="mb-3">
                        <label className="form-label">Hasło</label>
                        <input
                            type="password"
                            className="form-control"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    <button type="submit" className="btn btn-success w-100">
                        Zaloguj się
                    </button>
                </form>
            </div>
        </div>
    );
};

export default Login;
