import os
import re
import httpx

async def translate_text_async(
    source_lang: str,
    target_lang: str,
    api_url: str = "http://localhost:5001/translate",
    original_filepath: str = None  # pełna ścieżka do oryginalnego pliku
) -> str:
    try:
        if not original_filepath:
            raise ValueError("original_filepath jest wymagany")

        if not os.path.exists(original_filepath):
            raise FileNotFoundError(f"Plik nie istnieje: {original_filepath}")

        # Wczytaj tekst z pliku
        with open(original_filepath, "r", encoding="utf-8") as f:
            text = f.read()

        payload = {
            "q": text,
            "source": source_lang,
            "target": target_lang,
            "format": "text"
        }

        async with httpx.AsyncClient(timeout=10) as client:
            response = await client.post(api_url, json=payload)
            response.raise_for_status()
            data = response.json()
            translated_text = data.get("translatedText", "")

            # Zapisz przetłumaczony tekst do nowego pliku
            folder = os.path.dirname(original_filepath)
            base, ext = os.path.splitext(os.path.basename(original_filepath))
            base = re.sub(r'_[a-z]{2,3}$', '', base, flags=re.IGNORECASE)
            output_filename = f"{base}_{target_lang}{ext}"
            output_path = os.path.join(folder, output_filename)

            with open(output_path, "w", encoding="utf-8") as f:
                f.write(translated_text)

            print(f"[Translate] Zapisano tłumaczenie do: {output_path}")
            return translated_text

    except httpx.RequestError as e:
        print(f"Błąd zapytania: {e}")
    except httpx.HTTPStatusError as e:
        print(f"Błąd HTTP: {e.response.status_code} - {e.response.text}")
    except Exception as e:
        print(f"[Translate] Błąd ogólny: {e}")

    return ""