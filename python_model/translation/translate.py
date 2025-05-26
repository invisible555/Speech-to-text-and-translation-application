import os
import re
import httpx

def translate_text_sync(
    source_lang: str,
    target_lang: str,
    original_filepath: str,
    api_url: str = "http://localhost:5001/translate"
) -> str:
    if not original_filepath:
        raise ValueError("original_filepath jest wymagany")

    if not os.path.exists(original_filepath):
        raise FileNotFoundError(f"Plik nie istnieje: {original_filepath}")

    try:
        with open(original_filepath, "r", encoding="utf-8") as f:
            text = f.read()

        payload = {
            "q": text,
            "source": source_lang,
            "target": target_lang,
            "format": "text"
        }

        response = httpx.post(api_url, json=payload, timeout=100)
        response.raise_for_status()
        data = response.json()

        translated_text = data.get("translatedText", "")
        if not translated_text:
            raise ValueError("Brak przetłumaczonego tekstu w odpowiedzi")

        folder = os.path.dirname(original_filepath)
        base, ext = os.path.splitext(os.path.basename(original_filepath))
        base = re.sub(r'_[a-z]{2,3}$', '', base, flags=re.IGNORECASE)
        output_filename = f"{base}_{target_lang}{ext}"
        output_path = os.path.join(folder, output_filename)

        with open(output_path, "w", encoding="utf-8") as f:
            f.write(translated_text)

        print(f"[Translate] {source_lang} → {target_lang} | zapisano: {output_path}")
        return translated_text

    except httpx.RequestError as e:
        print(f"[Translate] Błąd zapytania: {e}")
        raise
    except httpx.HTTPStatusError as e:
        print(f"[Translate] Błąd HTTP ({e.response.status_code}): {e.response.text}")
        raise
    except Exception as e:
        print(f"[Translate] Błąd ogólny: {e}")
        raise
