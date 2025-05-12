import React, { useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';  // Zaimportuj axiosInstance z pliku, w którym masz skonfigurowane interceptory

const AudioUpload: React.FC = () => {
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [uploading, setUploading] = useState(false);
    const [message, setMessage] = useState<string | null>(null);

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            setSelectedFile(file);
            setMessage(null);
        }
    };

    const handleUpload = async () => {
        if (!selectedFile) {
            setMessage("Wybierz plik dźwiękowy przed wysłaniem.");
            return;
        }

        const formData = new FormData();
        formData.append("file", selectedFile);

        setUploading(true);
        try {
            const response = await axiosInstance.post('http://localhost:5212/api/File/upload', formData);

            setMessage("Plik został pomyślnie przesłany!");
            console.log(response)
        } catch (error: any) {
            console.error(error);
            setMessage("Błąd podczas przesyłania pliku.");
        } finally {
            setUploading(false);
        }
    };

    return (
        <div className="audio-upload">
            <h2>Wyślij plik dźwiękowy</h2>
            <input type="file" accept="audio/*" onChange={handleFileChange} />
            <button onClick={handleUpload} disabled={uploading}>
                {uploading ? "Wysyłanie..." : "Wyślij"}
            </button>
            {message && <p>{message}</p>}

            <div>Lista plików:</div>
        </div>
        
    );
};

export default AudioUpload;
