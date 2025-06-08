import React, { useState } from 'react';
import axiosInstance from '../../../utils/axiosConfig';
import AudioFileItemType from './AudioFileItemType';
import './AudioFileItem.css';

const AudioFileItem: React.FC<AudioFileItemType> = ({ file }) => {
  const [expanded, setExpanded] = useState(false);
  const [transcript, setTranscript] = useState<string | null>(null);
  const [audioSrc, setAudioSrc] = useState<string | null>(null);
  const [sourceLanguage, setSourceLanguage] = useState<string>('pl');
  const [translatedText, setTranslatedText] = useState<string | null>(null);
  const [isTranslating, setIsTranslating] = useState<boolean>(false);

  const handleClick = async () => {
    if (!expanded) {
      try {
        const transcriptResponse = await axiosInstance.get(`File/download/transcription/${file.fileName}`, {
          params: { language: sourceLanguage }
        });
        setTranscript(transcriptResponse.data.transcript);
      } catch (error) {
        console.error('Błąd podczas ładowania transkrypcji:', error);
        setTranscript('Błąd pobierania transkrypcji');
      }

      try {
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

  const handleTranslate = async (targetLang: string) => {
    setIsTranslating(true);
    setTranslatedText(null);
    try {
      const response = await axiosInstance.get(`Translate`, {
        params: {
          fileName: file.fileName,
          sourceLang: sourceLanguage,
          targetLang: targetLang
        }
      });
      setTranslatedText(response.data.translation);
    } catch (error) {
      console.error('Błąd tłumaczenia:', error);
      setTranslatedText('Nie udało się przetłumaczyć.');
    } finally {
      setIsTranslating(false);
    }
  };

  return (
    <div className="audio-file-item">
      <label>Wybierz język źródłowy:</label>
      <select
        value={sourceLanguage}
        onChange={(e) => setSourceLanguage(e.target.value)}
        disabled={expanded} // blokujemy zmianę po załadowaniu
      >
        <option value="pl">Polski</option>
        <option value="en">Angielski</option>
      </select>

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

          <p className="transcript">
            <strong>Transkrypcja:</strong><br />
            {transcript ?? 'Ładowanie transkrypcji...'}
          </p>

          <div className="translation-section">
            <label>Przetłumacz na:</label>
            <select onChange={e => handleTranslate(e.target.value)}>
              <option value="" disabled>Wybierz język docelowy</option>
              <option value="fr">Francuski</option>
              <option value="de">Niemiecki</option>
              <option value="en">Angielski</option>
              <option value="es">Hiszpański</option>
            </select>
          </div>

          {isTranslating ? (
            <p>Tłumaczenie...</p>
          ) : translatedText && (
            <p className="translated-text">
              <strong>Tłumaczenie:</strong><br />
              {translatedText}
            </p>
          )}
        </div>
      )}
    </div>
  );
};

export default AudioFileItem;
