@echo off
REM start.bat - uruchamianie FastAPI i Celery przez środowisko Anaconda

set ENV_NAME=reactproject

REM Aktywuj środowisko Conda i uruchom FastAPI
start cmd /k "call C:\Users\niewi\miniconda3\Scripts\activate.bat %ENV_NAME% && uvicorn python_model.main:app --reload"

REM Poczekaj 3 sekundy
timeout /t 3 /nobreak >nul

REM Aktywuj środowisko Conda i uruchom Celery
start cmd /k "call C:\Users\niewi\miniconda3\Scripts\activate.bat %ENV_NAME% && celery -A python_model.worker.celery_app worker --pool=solo -Q transcription --loglevel=info"

pause