import React, { useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';
import AudioFileItemType from "./AudioFileItemType"
import './AudioFileItem.css';


const AudioFileItem: React.FC<AudioFileItemType> = ({ file }) => {
  const [expanded, setExpanded] = useState(false);
  const [transcript, setTranscript] = useState<string | null>(null);

  const handleClick = async () => {
    if (!expanded) {
      try {
        const response = await axiosInstance.get(`/File/transcription/${file.id}`);
        setTranscript(response.data.transcript); // załóżmy, że backend zwraca { transcript: "..." }
      } catch (error) {
        console.error('Błąd pobierania transkrypcji:', error);
        setTranscript('Błąd pobierania transkrypcji');
      }
    }
    setExpanded(!expanded);
  };

  return (
    <div className="audio-file-item">
      <div className="audio-file-name" onClick={handleClick}>
        {file.name}
      </div>
      {expanded && (
        <div className="audio-file-details">
          <audio controls src={file.url}></audio>
          <p className="transcript">{transcript ?? 'Ładowanie...'}</p>
        </div>
      )}
    </div>
  );
};

export default AudioFileItem;