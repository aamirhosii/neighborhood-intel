# Deployment: GitHub + Vercel + API

Vercel hosts the **Vue frontend** beautifully. The **ASP.NET Core API** must run on a platform that supports .NET (Vercel does not run this API as a long-lived server in the same way). Typical setup: **Vercel = UI**, **Railway / Azure App Service / Render = API**.

---

## 1. Publish to GitHub (public)

### One-time: fix accidental Git repo in your user folder (optional)

If `git status` from random folders under your user directory shows your entire home folder as untracked, a `.git` folder may exist at `C:\Users\<you>\`. **Only remove it if you are sure nothing important was committed** (check with `git log`). For this project we use a **dedicated** repo only inside `neighborhood-intel/`.

### Create the repository

From the `neighborhood-intel` directory:

```powershell
cd C:\Users\hosid\neighborhood-intel
git init
git add .
git commit -m "Initial commit: NeighborhoodIntel Vue + ASP.NET Core API"
```

Create a **new public** repository on GitHub (web UI or [GitHub CLI](https://cli.github.com/)):

- **Repository name:** `neighborhood-intel` (recommended)
- **Visibility:** Public
- Do **not** add a README/license on GitHub if you already committed them locally (avoids merge conflicts).

Then connect and push:

```powershell
git branch -M main
git remote add origin https://github.com/<YOUR_USERNAME>/neighborhood-intel.git
git push -u origin main
```

Using SSH:

```powershell
git remote add origin git@github.com:<YOUR_USERNAME>/neighborhood-intel.git
git push -u origin main
```

---

## 2. Connect Vercel

This repo includes a **root `vercel.json`** so Git-based builds work from the monorepo (`install` / `build` / `output` run under `frontend/`). You can still set **Root Directory** to `frontend` in the Vercel dashboard instead if you prefer; then you may remove the root `vercel.json`.

### This project’s production URLs (example team)

After the first production deploy, Vercel assigns aliases such as:

- `https://neighborhood-intel-amirs-projects-74ab5506.vercel.app`
- `https://neighborhood-intel-pied.vercel.app` (if you added that alias)

The slug `neighborhood-intel.vercel.app` may belong to **another** Vercel account globally — use your team URL or add a **custom domain** under Project → Settings → Domains.

### Environment variables on Vercel

In the project → **Settings → Environment Variables**:

| Name | Value | Notes |
|------|--------|--------|
| `VITE_API_BASE_URL` | `https://your-api-host.example.com` | **No** trailing slash. Must match where your .NET API is deployed. |
| `VITE_GOOGLE_MAPS_KEY` | Your browser Maps key | Optional; only if you use the map embed. Restrict key by HTTP referrer to `https://*.vercel.app` and your custom domain. |

Redeploy after changing variables.

---

## 3. Host the ASP.NET API

Pick one:

- **[Railway](https://railway.app/)** — connect repo or deploy from Dockerfile; set `PORT` if needed; add env vars for keys.
- **[Azure App Service](https://azure.microsoft.com/products/app-service/)** — .NET 8 runtime; configure CORS + application settings.
- **[Render](https://render.com/)** — Web service with `dotnet publish` start command.

Set these on the host:

- `GoogleMaps__ApiKey`
- `OpenAI__ApiKey`
- `Cors__AllowedOrigins__0` = `https://your-app.vercel.app`  
  (or use JSON / array format your host supports — equivalent to `Cors:AllowedOrigins` in appsettings)

Alternatively, set `Cors:AllowedOrigins` as a JSON array in configuration if your provider supports nested config.

### CORS

The API must allow your Vercel origin, for example:

`https://neighborhood-intel.vercel.app`

Add it to `Cors:AllowedOrigins` in `appsettings.json` on the server or via environment/configuration in your host’s dashboard.

---

## 4. Smoke test after deploy

1. Open the Vercel URL.
2. Run **Analyze** on a known address.
3. In browser **DevTools → Network**, confirm requests go to `https://<api-host>/api/analyze-location` (via `VITE_API_BASE_URL`) and return `200`.

---

## Security checklist

- Never commit `appsettings.Development.json` with real keys (this repo gitignores it; use `appsettings.Development.example.json` as a template).
- Restrict Google API keys by **HTTP referrer** (frontend) and **IP / service** (backend) per Google’s docs.
- Rotate any key that was ever committed or pasted into a ticket or chat.
