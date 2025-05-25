import os
from pydub import AudioSegment
from pydantic import BaseModel

class AudioRequest(BaseModel):
    filename: str  # pełna ścieżka do pliku (np. /app/files/user/audio.mp3)

def convert_audio_sync(req: AudioRequest) -> dict:
    """
    Konwertuje plik audio do formatu WAV 16kHz mono i usuwa oryginał.
    """
    input_path = req.filename
    print(f"[Audio Convert] Request for: {input_path}")

    if not os.path.exists(input_path):
        raise Exception("Plik audio nie istnieje.")

    try:
        ext = os.path.splitext(input_path)[1].lower()
        if ext == ".wav":
            return {
                "converted_path": input_path,
                "note": "Plik już jest w formacie .wav — pominięto konwersję."
            }

        # Konwersja: 16kHz, mono
        audio = AudioSegment.from_file(input_path)
        audio = audio.set_frame_rate(16000).set_channels(1)

        # Wyjściowa ścieżka
        base_name = os.path.splitext(os.path.basename(input_path))[0]
        output_path = os.path.join(os.path.dirname(input_path), f"{base_name}.wav")

        # Eksportuj do WAV
        audio.export(output_path, format="wav")

        # Usuń oryginalny plik
        os.remove(input_path)

        return {"converted_path": output_path}
    except Exception as e:
        raise Exception(f"Błąd konwersji audio: {str(e)}")
