from celery import Celery
from python_model.config.audio_config import CELERY_BROKER_URL, CELERY_RESULT_BACKEND

celery_app = Celery(
    "audio_tasks",
    broker="redis://localhost:6379/0",
    backend="redis://localhost:6379/0"
)

celery_app.conf.task_routes = {
    "python_model.tasks.*": {"queue": "transcription"}
}

celery_app.autodiscover_tasks(['python_model'])
