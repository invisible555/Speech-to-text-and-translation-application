import os
import json
import wave
from vosk import Model, KaldiRecognizer
from fastapi import HTTPException
from pydantic import BaseModel
from python_model.config.audio_config import VOSK_MODEL_PATH, STORAGE_PATH


# Załaduj model Vosk raz
vosk_model = Model(VOSK_MODEL_PATH)

class AudioRequest(BaseModel):
    username: str
    filename: str  # tylko nazwa pliku np. "audio.wav"
    lang: str

def transcribe_audio_sync(req: AudioRequest) -> dict:
    
    # Pełna ścieżka do pliku audio
    audio_path = os.path.join(STORAGE_PATH, req.username, "files", req.filename)

    if not os.path.exists(audio_path):
        raise HTTPException(status_code=404, detail="Plik audio nie istnieje.")

    try:
        wf = wave.open(audio_path, "rb")
    except Exception as e:
        raise HTTPException(status_code=400, detail=f"Nie można otworzyć pliku: {str(e)}")

    # Weryfikacja formatu WAV
    if wf.getnchannels() != 1 or wf.getsampwidth() != 2 or wf.getcomptype() != "NONE":
        raise HTTPException(status_code=400, detail="Plik audio musi być WAV, mono, 16-bit PCM.")

    rec = KaldiRecognizer(vosk_model, wf.getframerate())

    results = []
    while True:
        data = wf.readframes(4000)
        if not data:
            break
        if rec.AcceptWaveform(data):
            result = json.loads(rec.Result())
            results.append(result.get("text", ""))
    final_result = json.loads(rec.FinalResult())
    results.append(final_result.get("text", ""))

    transcript = " ".join(results).strip()

    # Zapis do pliku
    transcription_dir = os.path.join(STORAGE_PATH, req.username, "transcription")
    os.makedirs(transcription_dir, exist_ok=True)

    base_name = os.path.splitext(req.filename)[0]
    output_filename = f"{base_name}_{req.lang}.txt"
    output_path = os.path.join(transcription_dir, output_filename)

    with open(output_path, "w", encoding="utf-8") as f:
        f.write(transcript)

    print(f"[Transkrypcja] Zapisano: {output_path}")

    return {"text": transcript, "path": output_path}
