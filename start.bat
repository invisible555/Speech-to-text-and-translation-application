@echo off
REM start.bat - uruchomienie FastAPI i Celery z poprawnym katalogiem roboczym

REM Ustaw zmienną środowiskową PYTHONPATH na bieżący katalog
set PYTHONPATH=%cd%

echo Uruchamiam FastAPI na porcie 8000...
start cmd /k "uvicorn python_model.main:app --reload"

REM Poczekaj chwilę, aby FastAPI zdążyło się uruchomić
timeout /t 3 /nobreak >nul

echo Uruchamiam Celery worker...
start cmd /k "celery -A python_model.worker.celery_app worker --pool=solo -Q transcription --loglevel=info"

echo Wszystko uruchomione.
pause
