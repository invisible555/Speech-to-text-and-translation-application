from celery import shared_task
from python_model.audio_processing.transcribe import transcribe_audio_sync
from pydantic import BaseModel
from python_model.audio_processing.convert import convert_audio_sync
from python_model.translation.translate import translate_text_sync
import asyncio
from python_model.worker import celery_app
class AudioRequest(BaseModel):
    username: str = None  # jeśli potrzebujesz
    filename: str
    lang: str
@shared_task
def convert_task(filename: str):
    req = AudioRequest(filename=filename)
    result = convert_audio_sync(req)
    print(f"[Celery] Convert result: {result}")
    return result

@shared_task
def transcribe_task(username: str, filename: str,lang: str ):
    try:
     
        req = AudioRequest(username=username, filename=filename, lang=lang)
        result = transcribe_audio_sync(req)
        print(f"[Celery] Transcribe result: {result}")
        return result
    except Exception as e:
        print(f"[Celery] ERROR in transcribe_task: {e}")
        raise e



@shared_task
def translate_task(source_lang: str, target_lang: str, original_filepath: str):
    print("hello2")
    return translate_text_sync(
            source_lang,
            target_lang,
            original_filepath
        )
    

