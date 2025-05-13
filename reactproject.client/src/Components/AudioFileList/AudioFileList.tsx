import React, { useEffect, useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';
import AudioFileType from './AudioFileListType';
import AudioFileItem from "../AudioFileItem/AudioFileItem" 
import './AudioFileList.css';


const AudioFileList: React.FC = () => {
  const [files, setFiles] = useState<AudioFileType[]>([]);

  useEffect(() => {
    const fetchFiles = async () => {
      try {
        const response = await axiosInstance.get<AudioFileType[]>('/File/files');
        setFiles(response.data);
      } catch (error) {
        console.error('Błąd pobierania plików:', error);
      }
    };

    fetchFiles();
  }, []);

  return (
     <div className="audio-file-list">
    <h2>Lista plików audio</h2>
    {files.map(file => (
      <AudioFileItem key={file.id} file={file} />
    ))}
  </div>
  );
};

export default AudioFileList;