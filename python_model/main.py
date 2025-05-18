from fastapi import FastAPI, Request
from fastapi.responses import JSONResponse

from audio_processing.convert import convert_audio
from audio_processing.transcribe import transcribe_audio

app = FastAPI()


@app.middleware("http")
async def catch_exceptions_middleware(request: Request, call_next):
    try:
        return await call_next(request)
    except Exception as e:
        return JSONResponse(
            status_code=500,
            content={"detail": f"Wewnętrzny błąd serwera: {str(e)}"}
        )


app.post("/convert")(convert_audio)
app.post("/transcribe")(transcribe_audio)
