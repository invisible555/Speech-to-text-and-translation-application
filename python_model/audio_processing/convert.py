from fastapi import HTTPException
from pydantic import BaseModel
from pydub import AudioSegment
import os

class AudioRequest(BaseModel):
    filename: str

async def convert_audio(req: AudioRequest):
    path = req.filename
    print(f"Convert request: {path}")

    if not os.path.exists(path):
        raise HTTPException(status_code=404, detail="Plik audio nie istnieje.")

    try:
        ext = os.path.splitext(path)[1].lower()
        if ext == ".wav":
            return {"converted_path": path, "note": "Plik już jest w formacie .wav — pominięto konwersję."}

        audio = AudioSegment.from_file(path)
        audio = audio.set_frame_rate(16000).set_channels(1)

        base_name = os.path.splitext(os.path.basename(path))[0]
        original_dir = os.path.dirname(path)
        output_path = os.path.join(original_dir, f"{base_name}.wav")

        # Eksportuj do WAV
        audio.export(output_path, format="wav")

        # Usuń oryginalny plik
        os.remove(path)

        return {"converted_path": output_path}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Błąd konwersji audio: {str(e)}")
