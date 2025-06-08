import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../Context/AuthContext';
import axiosInstance from '../../../utils/axiosConfig';
import AudioFileList from '../AudioFileList/AudioFileList';

const AudioUpload: React.FC = () => {
    const { token } = useAuth();
    const navigate = useNavigate();

    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [uploading, setUploading] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [selectedLanguage, setLanguage] = useState<string>('pl');
    const [authChecked, setAuthChecked] = useState(false);

    useEffect(() => {
 
        if (token === null) {
            const timeout = setTimeout(() => {
                setAuthChecked(true);
                navigate('/login');
            }, 200); 
            return () => clearTimeout(timeout);
        } else {
            setAuthChecked(true);
        }
    }, [token, navigate]);

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            setSelectedFile(file);
            setMessage(null);
        }
    };

    const handleLanguageChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        setLanguage(event.target.value);
    };

    const handleUpload = async () => {
        if (!selectedFile) {
            setMessage('Wybierz plik dźwiękowy przed wysłaniem.');
            return;
        }

        const formData = new FormData();
        formData.append('file', selectedFile);
        formData.append('language', selectedLanguage);
        setUploading(true);

        try {
            const response = await axiosInstance.post('File/upload', formData);
            setMessage('Plik został pomyślnie przesłany!');
            setSelectedFile(null); // wyczyść input po sukcesie
            console.log(response);
        } catch (error: any) {
            console.error(error);
            setMessage('Błąd podczas przesyłania pliku.');
        } finally {
            setUploading(false);
        }
    };

    // Jeśli autoryzacja jeszcze się nie sprawdziła, nie renderuj zawartości (żeby nie migała nieautoryzowana strona)
    if (!authChecked) {
        return null; // Możesz zwrócić np. spinner
    }

    return (
        <div className="audio-upload">
            <h2>Wyślij plik dźwiękowy</h2>
            <label>Wybierz język</label>
            <select value={selectedLanguage} onChange={handleLanguageChange}>
                <option value="pl">Polski</option>
                <option value="en">Angielski</option>
            </select>
            <input type="file" accept="audio/*" onChange={handleFileChange} />
            <button onClick={handleUpload} disabled={uploading}>
                {uploading ? 'Wysyłanie...' : 'Wyślij'}
            </button>
            {message && <p>{message}</p>}

            <div><AudioFileList /></div>
        </div>
    );
};

export default AudioUpload;
