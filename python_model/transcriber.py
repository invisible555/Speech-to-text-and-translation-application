from vosk import Model, KaldiRecognizer
import wave
import json
import os
from config import MODEL_PATH

model = Model(MODEL_PATH)

def transcribe_from_path(path: str) -> str:
    wf = wave.open(path, "rb")
    recognizer = KaldiRecognizer(model, wf.getframerate())

    final_text = ""

    while True:
        data = wf.readframes(4000)
        if len(data) == 0:
            break
        if recognizer.AcceptWaveform(data):
            partial_result = json.loads(recognizer.Result())
            final_text += partial_result.get("text", "") + " "

    final_result = json.loads(recognizer.FinalResult())
    final_text += final_result.get("text", "")

    wf.close()

    
    txt_path = os.path.splitext(path)[0] + ".txt"
    with open(txt_path, "w", encoding="utf-8") as f:
        f.write(final_text.strip())

    return os.path.abspath(txt_path)
