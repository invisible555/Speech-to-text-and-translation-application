# ReactProject – Aplikacja z ASP.NET i React

## 📦 Zawartość projektu

- **.client** – frontend w React
- **.server** – backend w ASP.NET Core
- **App_Data/UserFiles** – katalog na pliki użytkowników

### Wymagania

- Node.js (v18+)
- .NET 7.0 lub wyższy
- Visual Studio 2022 lub VS Code (opcjonalnie)


Instrukcja uruchomienia projektu

**Frontend** (React):

Przejdź do folderu  reactproject.client i uruchom frontend:

cd reactproject.client

npm install   # za pierwszym razem

npm run dev

 **Backend** (ASP.NET):

W nowej zakładce terminala przejdź do folderu ReactProject.server i uruchom backend:

cd ReactProject.server

dotnet ef database update   # za pierwszym razem

dotnet run

**Python**

Wymaga ffmpeg

uruchomić start.bat, uruchomi serwer python dla fastapi oraz celery dla asynchronicznego kolejkowania zadań

Celery dla wersji 4.0+ może nie działać na windows, wersja 4.0 najlepiej działa na wersji python 3.8 oraz pip w wersji naprzykład 21.0.0. Aby temu zaradzić można użyć miniconda aby stworzyć nowe środowisko z python 3.10.