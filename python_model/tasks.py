from celery import shared_task
from python_model.audio_processing.transcribe import transcribe_audio_sync
from pydantic import BaseModel
from python_model.audio_processing.convert import convert_audio_sync
from python_model.translate import translate_text_async
import asyncio

@shared_task
def convert_task(filename: str):
    req = AudioRequest(filename=filename)
    result = convert_audio_sync(req)
    print(f"[Celery] Convert result: {result}")
    return result

@shared_task
def transcribe_task(username: str, filename: str):
    try:
        print("hello2")
        req = AudioRequest(username=username, filename=filename)
        result = transcribe_audio_sync(req)
        print(f"[Celery] Transcribe result: {result}")
        return result
    except Exception as e:
        print(f"[Celery] ERROR in transcribe_task: {e}")
        raise e


import asyncio
from celery import shared_task
from python_model.translate import translate_text_async

@shared_task
def translate_task(source_lang: str, target_lang: str, original_filepath: str):
    return asyncio.run(
        translate_text_async(
            text=text,
            source_lang=source_lang,
            target_lang=target_lang,
            original_filepath=original_filepath
        )
    )
