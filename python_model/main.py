from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import os

from transcriber import transcribe_from_path

app = FastAPI()

class TranscriptionRequest(BaseModel):
    path: str

@app.post("/transcribe/")
async def transcribe(request: TranscriptionRequest):
    if not os.path.exists(request.path):
        raise HTTPException(status_code=400, detail="Plik nie istnieje.")

    try:
        transcript_path = transcribe_from_path(request.path)
        return {
            "original_audio": os.path.abspath(request.path),
            "transcript_file": transcript_path
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
