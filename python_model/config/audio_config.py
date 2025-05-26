import os

# Ścieżka do modelu Vosk - można podać absolutną lub względną
VOSK_MODEL_PATH = os.path.abspath("python_model/modelvosk")

# Ścieżka do katalogu ze wszystkimi plikami użytkowników
STORAGE_PATH = os.path.abspath(
    r"C:\Users\niewi\Desktop\ReactProject\ReactProject.Server\App_Data\UserFiles"
)

# Konfiguracja Celery - adres brokera i backendu Redis
CELERY_BROKER_URL = 'redis://localhost:6379/0'
CELERY_RESULT_BACKEND = 'redis://localhost:6379/0'
