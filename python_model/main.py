from celery.result import AsyncResult
from python_model.worker import celery_app
from fastapi import FastAPI, Request
from fastapi.responses import JSONResponse
from pydantic import BaseModel
from python_model.tasks import convert_task, transcribe_task, translate_task
import traceback
from fastapi import HTTPException


app = FastAPI()

class AudioRequest(BaseModel):
    username: str = None  
    filename: str

class TranscribeRequest(BaseModel):
    username: str
    filename: str
    lang: str

class TranslateRequest(BaseModel):
    source_lang: str
    target_lang: str
    original_filepath: str 

@app.middleware("http")
async def catch_exceptions_middleware(request: Request, call_next):
    try:
        return await call_next(request)
    except Exception as e:
        return JSONResponse(
            status_code=500,
            content={"detail": f"Wewnętrzny błąd serwera: {str(e)}"}
        )

@app.post("/convert")
async def convert_endpoint(req: AudioRequest):
    task = convert_task.delay(req.filename)
    return {"message": "Zadanie konwersji zostało dodane do kolejki", "task_id": task.id}




@app.post("/transcribe")
async def transcribe_endpoint(request: TranscribeRequest):
    try:
        # Dodaj zadanie do kolejki
        celery_result = transcribe_task.delay(request.username, request.filename, request.lang)
        # Zwróć ID zadania, aby klient mógł sprawdzać status
        return {"message": "Zadanie transkrypcji zostało dodane do kolejki", "task_id": celery_result.id}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Błąd przy dodawaniu zadania: {str(e)}")


@app.post("/translate")
async def translate_endpoint(req: TranslateRequest):
    print(req)
    task = translate_task.apply_async(kwargs={
    "source_lang": "en",
    "target_lang": "pl",
    "original_filepath":  req.original_filepath
}
    )
    return {"task_id": task.id}


@app.get("/translate/result/{task_id}")
async def get_translate_result(task_id: str):
    result = AsyncResult(task_id, app=celery_app)
    if result.state == "PENDING":
        return {"status": "pending"}
    elif result.state == "STARTED":
        return {"status": "started"}
    elif result.state == "SUCCESS":
        return {"status": "success", "result": result.result}
    elif result.state == "FAILURE":
        return {"status": "failure", "error": str(result.result)}
    else:
        return {"status": result.state}

