import React, { useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';
import AudioFileItemType from "./AudioFileItemType";
import './AudioFileItem.css';

const AudioFileItem: React.FC<AudioFileItemType> = ({ file }) => {
  const [expanded, setExpanded] = useState(false);
  const [transcript, setTranscript] = useState<string | null>(null);
  const [audioSrc, setAudioSrc] = useState<string | null>(null);

  const handleClick = async () => {
    if (!expanded) {
      try {
    
        const transcriptResponse = await axiosInstance.get(`File/transcription/${file.fileName}`);
        setTranscript(transcriptResponse.data.transcript);
      }
      catch(error)
      {
          console.error('Błąd podczas ładowania transkrypcji:', error);
          setTranscript('Błąd pobierania transkrypcji');
      }
      
        const audioResponse = await axiosInstance.get(`File/download/${file.fileName}`, {
          responseType: 'blob', 
        
        });

        try{
        const blobUrl = URL.createObjectURL(audioResponse.data);
        setAudioSrc(blobUrl);
        

      } catch (error) {
   
        console.log("Bład pobierania pliku")
        
      }
    }
    setExpanded(!expanded);
  };

  return (
    <div className="audio-file-item">
      <div className="audio-file-name" onClick={handleClick}>
        {file.fileName}
      </div>
      {expanded && (
        <div className="audio-file-details">
          {audioSrc ? (
            <audio controls src={audioSrc}></audio>
          ) : (
            <p>Ładowanie pliku audio...</p>
          )}
          <p className="transcript">{transcript ?? 'Ładowanie transkrypcji...'}</p>
        </div>
      )}
    </div>
  );
};

export default AudioFileItem;
