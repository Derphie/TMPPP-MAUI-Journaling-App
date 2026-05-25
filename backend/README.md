# Reflecta AI Backend

Flask server that proxies chat requests to **Google Gemini (AI Studio)** and returns
`{"reply": "...", "mood": "happy|calm|neutral|stressed|sad"}`.

Model: `gemini-2.5-flash`

---

## Local development

### Setup

```bash
cd backend
pip install -r requirements.txt
```

### Configuration

**Option A — `.env` file (recommended)**

```bash
cp .env.example .env
# Edit .env and set your real key
# Get a free key at: https://aistudio.google.com/app/apikey
```

**Option B — export in the same terminal**

```bash
export GEMINI_API_KEY="AIzaSy-..."
```

> `.env` is gitignored. Never commit a real key.

### Run

```bash
python app.py
# Listens on http://localhost:5000 (or $PORT if set)
```

On startup you should see:

```
[INFO] GEMINI_API_KEY loaded (length=39)
```

### Test

```bash
# Connectivity check
curl http://localhost:5000/debug

# Chat endpoint
curl -s -X POST http://localhost:5000/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"I had a rough day","history":[]}' | python3 -m json.tool
```

---

## Deploy to Render.com

### 1. Push the `backend/` folder to a Git repo

Render pulls from GitHub/GitLab. Make sure `backend/` is committed (`.env` is
already gitignored so the key won't be included).

### 2. Create a new Web Service on Render

| Setting | Value |
|---------|-------|
| **Environment** | Python |
| **Root Directory** | `backend` |
| **Build Command** | `pip install -r requirements.txt` |
| **Start Command** | `gunicorn app:app --bind 0.0.0.0:$PORT` |
| **Plan** | Free |

Alternatively, Render will auto-detect `render.yaml` if it's in the root of your
repo (or in `backend/` if that's the root directory).

### 3. Add the environment variable

In the Render dashboard → your service → **Environment**:

| Key | Value |
|-----|-------|
| `GEMINI_API_KEY` | your real key from AI Studio |

Render injects this as a plain env var at runtime — no `.env` file is needed in
production.

### 4. Update the MAUI app

Once Render gives you a URL like `https://reflecta-ai-backend.onrender.com`:

1. Set `AiBaseUrl` in `Reflecta/Services/AppConfig.cs` to that URL (no trailing slash)
2. Keep `UseRemoteAi = true`
3. Rebuild and deploy the app — no ngrok needed in production

---

## Endpoints

| Method | Path | Body | Response |
|--------|------|------|----------|
| GET | `/health` | — | `{"status":"ok"}` |
| GET | `/debug` | — | Raw Gemini status + body |
| POST | `/chat` | `{"message":"...","history":["msg1","msg2",...]}` | `{"reply":"...","mood":"..."}` |
| POST | `/reflect` | `{"message":"..."}` | `{"reply":"...","mood":"..."}` |
