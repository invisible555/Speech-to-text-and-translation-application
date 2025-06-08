import os

# Ścieżka do modelu Vosk - można podać absolutną lub względną
VOSK_MODEL_PATH = "python_model/modelvosk"

# Ścieżka do katalogu ze wszystkimi plikami użytkowników
STORAGE_PATH = "/ReactProject.Server/App_Data/UserFiles"

# Konfiguracja Celery - adres brokera i backendu Redis
CELERY_BROKER_URL = 'redis://redis:6379/0'
CELERY_RESULT_BACKEND = 'redis://redis:6379/0'
