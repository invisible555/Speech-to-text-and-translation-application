from celery import Celery
from python_model.config.audio_config import CELERY_BROKER_URL, CELERY_RESULT_BACKEND

celery_app = Celery(
    "audio_tasks",
    broker=CELERY_BROKER_URL,
    backend=CELERY_RESULT_BACKEND
)

celery_app.conf.task_routes = {
    "python_model.tasks.*": {"queue": "transcription"}
}

celery_app.autodiscover_tasks(["python_model"])

celery_app.conf.update(
    task_serializer='json',
    result_serializer='json',
    accept_content=['json'],
)
