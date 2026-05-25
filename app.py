content = resp.json()["choices"][0]["message"]["content"]
        # strip markdown fences and extract the JSON object if wrapped in extra text
        cleaned = content.strip()
        if cleaned.startswith("```"):
            cleaned = cleaned.split("```")[1]
            if cleaned.startswith("json"):
                cleaned = cleaned[4:]
            cleaned = cleaned.strip()
        try:
            parsed = json.loads(cleaned)
            reply = parsed.get("reply") or content
            mood = parsed.get("mood", "neutral")
        except json.JSONDecodeError:
            # model didn't return JSON — use its text as the reply, detect mood ourselves
            reply = content
            mood = detect_mood_from_text(content)
