import React, { useEffect, useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';
import AudioFileType from '../AudioFileList/AudioFileListType';
import AudioFileItem from "../AudioFileItem/AudioFileItem";
import './AudioFileList.css';

const AudioFileList: React.FC = () => {
  const [files, setFiles] = useState<AudioFileType[]>([]);

  useEffect(() => {
    const fetchFiles = async () => {
      try {
        const response = await axiosInstance.get<AudioFileType[]>('/File/files');
        

         const filesWithUrls = response.data.map((file) => ({
          ...file,
          url: `File/download/file/${file.fileName}`, 
          
        }));
        setFiles(filesWithUrls);
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
        <AudioFileItem key={file.fileName} file={file} />
      ))}
    </div>
  );
};

export default AudioFileList;
