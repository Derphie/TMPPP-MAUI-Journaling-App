import os
import re
import json
import logging
import requests
from flask import Flask, request, jsonify
from flask_cors import CORS

try:
    from dotenv import load_dotenv
    load_dotenv()
except ImportError:
    pass 
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
)
log = logging.getLogger(__name__)

_api_key = os.environ.get("GEMINI_API_KEY", "")
if _api_key:
    log.info("GEMINI_API_KEY loaded (length=%d)", len(_api_key))
else:
    log.warning(
        "GEMINI_API_KEY is NOT set. "
        "Set it in backend/.env or export it before running. "
        "All AI calls will return an error response."
    )

app = Flask(__name__)
CORS(app)

MODEL = "gemini-2.5-flash"
GEMINI_API_URL = (
    f"https://generativelanguage.googleapis.com/v1beta/models/{MODEL}:generateContent"
)

SYSTEM_PROMPT = (
    "You are a warm, empathetic journaling companion. "
    "Acknowledge the user's feelings before offering any advice. "
    "If they sound stressed or tired, gently suggest rest or self-care. "
    "Give concise, practical replies (2-4 sentences). "
    "End with a short open question to invite further reflection. "
    "Always respond with valid JSON in exactly this format: "
    '{"reply": "<your response>", "mood": "<happy|calm|neutral|stressed|sad>"}'
)

VALID_MOODS = {"happy", "calm", "neutral", "stressed", "sad"}


def detect_mood_from_text(text: str) -> str:
    t = text.lower()
    if any(w in t for w in ["happy", "excited", "great", "wonderful", "joy", "love"]):
        return "happy"
    if any(w in t for w in ["calm", "peaceful", "relaxed", "content", "fine"]):
        return "calm"
    if any(w in t for w in ["stress", "anxious", "overwhelm", "worried", "panic"]):
        return "stressed"
    if any(w in t for w in ["sad", "unhappy", "depressed", "upset", "crying", "low"]):
        return "sad"
    return "neutral"


def extract_json(text: str) -> dict | None:
    """Parse JSON from text that may be wrapped in markdown fences or prose."""
    stripped = re.sub(r"```(?:json)?\s*([\s\S]*?)\s*```", r"\1", text).strip()
    candidates = [stripped, text]
    m = re.search(r"\{[\s\S]*\}", stripped)
    if m:
        candidates.insert(0, m.group(0))
    for candidate in candidates:
        try:
            return json.loads(candidate)
        except (json.JSONDecodeError, ValueError):
            continue
    return None


def build_contents(history: list, user_message: str) -> list:
    """Map flat history strings + new message into Gemini contents array.

    Gemini roles are "user" / "model" (not "assistant").
    The array must start with a user turn and alternate strictly.
    """
    contents = []
   
    for i, text in enumerate(history):
        role = "user" if i % 2 == 0 else "model"
        contents.append({"role": role, "parts": [{"text": text}]})
    contents.append({"role": "user", "parts": [{"text": user_message}]})
    return contents


def call_gemini(history: list, user_message: str) -> dict:
    api_key = os.environ.get("GEMINI_API_KEY", "")
    if not api_key:
        log.error("call_gemini: GEMINI_API_KEY is empty — cannot call API")
        return {"reply": "AI service is not configured. Please set GEMINI_API_KEY.", "mood": "neutral"}

    headers = {
        "Content-Type": "application/json",
        "x-goog-api-key": api_key,
    }
    payload = {
        "systemInstruction": {"parts": [{"text": SYSTEM_PROMPT}]},
        "contents": build_contents(history, user_message),
    }

    resp = None
    try:
        log.info("Calling Gemini: model=%s, history_len=%d", MODEL, len(history))
        resp = requests.post(GEMINI_API_URL, headers=headers, json=payload, timeout=30)
        log.info("Gemini response: HTTP %d", resp.status_code)

        if not resp.ok:
            log.error("Gemini error body: %s", resp.text)
            resp.raise_for_status()

        body = resp.json()
        content = body["candidates"][0]["content"]["parts"][0]["text"]
        log.info("Gemini raw content: %r", content)

        parsed = extract_json(content)
        if parsed is not None:
            reply = parsed.get("reply") or content
            mood = parsed.get("mood", "neutral")
            if mood not in VALID_MOODS:
                mood = detect_mood_from_text(reply)
            log.info("Parsed OK — mood=%s, reply_len=%d", mood, len(reply))
            return {"reply": reply, "mood": mood}

        log.warning("Could not parse JSON from model output; using raw text as reply")
        return {"reply": content, "mood": detect_mood_from_text(content)}

    except requests.HTTPError as e:
        log.error("HTTPError from Gemini: %r | body: %s", e, resp.text if resp is not None else "n/a")
        return {"reply": "I'm here for you. Tell me more about what's on your mind.", "mood": "neutral"}
    except requests.RequestException as e:
        log.error("Network error calling Gemini: %r", e)
        return {"reply": "I'm here for you. Tell me more about what's on your mind.", "mood": "neutral"}
    except (KeyError, IndexError) as e:
        body_text = resp.text if resp is not None else "n/a"
        log.error("Unexpected Gemini response shape: %r | body: %s", e, body_text)
        return {"reply": "I'm here for you. Tell me more about what's on your mind.", "mood": "neutral"}
    except Exception as e:
        log.error("Unexpected error in call_gemini: %r", e)
        return {"reply": "I'm here for you. Tell me more about what's on your mind.", "mood": "neutral"}



@app.route("/health", methods=["GET"])
def health():
    return jsonify({"status": "ok"})


@app.route("/debug", methods=["GET"])
def debug():
    """Minimal Gemini call — returns raw HTTP status + body for connectivity check."""
    api_key = os.environ.get("GEMINI_API_KEY", "")
    if not api_key:
        return jsonify({"error": "GEMINI_API_KEY not set", "key_present": False}), 500

    headers = {
        "Content-Type": "application/json",
        "x-goog-api-key": api_key,
    }
    payload = {
        "contents": [{"role": "user", "parts": [{"text": "Say hello in one word."}]}]
    }
    try:
        resp = requests.post(GEMINI_API_URL, headers=headers, json=payload, timeout=30)
        try:
            body = resp.json()
        except Exception:
            body = resp.text
        return jsonify({
            "key_present": True,
            "key_length": len(api_key),
            "model": MODEL,
            "http_status": resp.status_code,
            "gemini_body": body,
        })
    except Exception as e:
        return jsonify({"key_present": True, "error": repr(e)}), 500


@app.route("/chat", methods=["POST"])
def chat():
    data = request.get_json(silent=True) or {}
    user_message = data.get("message", "")
    history = data.get("history", [])
    result = call_gemini(history, user_message)
    return jsonify(result)


@app.route("/reflect", methods=["POST"])
def reflect():
    data = request.get_json(silent=True) or {}
    user_message = data.get("message", "")
    result = call_gemini([], user_message)
    return jsonify(result)


if __name__ == "__main__":
    port = int(os.environ.get("PORT", 5000))
    app.run(host="0.0.0.0", port=port, debug=False)
