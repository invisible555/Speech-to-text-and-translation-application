import React, { useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';
import AudioFileItemType from './AudioFileItemType';
import './AudioFileItem.css';

const AudioFileItem: React.FC<AudioFileItemType> = ({ file }) => {
  const [expanded, setExpanded] = useState(false);
  const [transcript, setTranscript] = useState<string | null>(null);
  const [audioSrc, setAudioSrc] = useState<string | null>(null);

  const handleClick = async () => {
    if (!expanded) {
      try {
        // 📥 1. Pobranie transkrypcji (z automatycznym generowaniem po stronie backendu)
        const transcriptResponse = await axiosInstance.get(`File/download/transcription/${file.fileName}`);
        setTranscript(transcriptResponse.data.transcript);
      } catch (error) {
        console.error('Błąd podczas ładowania transkrypcji:', error);
        setTranscript('Błąd pobierania transkrypcji');
      }

      try {
        // 🎵 2. Pobranie pliku audio jako blob
        const audioResponse = await axiosInstance.get(`File/download/file/${file.fileName}`, {
          responseType: 'blob',
        });

        const blobUrl = URL.createObjectURL(audioResponse.data);
        setAudioSrc(blobUrl);
      } catch (error) {
        console.error('Błąd pobierania pliku audio:', error);
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