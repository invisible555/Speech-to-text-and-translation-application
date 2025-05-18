from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
import os
import wave
import json
from vosk import Model, KaldiRecognizer
from config.audio_config import VOSK_MODEL_PATH, STORAGE_PATH


vosk_model = Model(VOSK_MODEL_PATH)

class AudioRequest(BaseModel):
    username: str    
    filename: str     

router = APIRouter()

@router.post("/transcribe")
async def transcribe_audio(req: AudioRequest):
    
    audio_path = os.path.join(STORAGE_PATH, req.username, "files", req.filename)

    if not os.path.exists(audio_path):
        raise HTTPException(status_code=404, detail="Plik audio nie istnieje.")
    
    try:
        wf = wave.open(audio_path, "rb")
    except Exception as e:
        raise HTTPException(status_code=400, detail=f"Nie można otworzyć pliku audio: {str(e)}")
 
    # Sprawdzenie parametrów audio: mono, 16-bit PCM, brak kompresji
    if wf.getnchannels() != 1 or wf.getsampwidth() != 2 or wf.getcomptype() != "NONE":
        raise HTTPException(status_code=400, detail="Plik audio musi być WAV, mono, 16-bit PCM.")

    rec = KaldiRecognizer(vosk_model, wf.getframerate())

    results = []
    while True:
        data = wf.readframes(4000)
        if len(data) == 0:
            break
        if rec.AcceptWaveform(data):
            result = json.loads(rec.Result())
            results.append(result.get("text", ""))
    final_result = json.loads(rec.FinalResult())
    results.append(final_result.get("text", ""))

    transcript = " ".join(results).strip()

    
    transcription_dir = os.path.join(STORAGE_PATH, req.username, "transcription")
    os.makedirs(transcription_dir, exist_ok=True)

    
    transcript_path = os.path.join(transcription_dir, f"{os.path.splitext(req.filename)[0]}.txt")

    # Zapis transkrypcji do pliku
    with open(transcript_path, "w", encoding="utf-8") as f:
        f.write(transcript)

    print(f"Transkrypcja zapisana do: {transcript_path}")

    return {"text": transcript}