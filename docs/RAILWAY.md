# Deploy the API on Railway (from GitHub)

The **Vue app** stays on Vercel. This guide deploys only the **ASP.NET Core** API from the same monorepo.

---

## 1. Create the service

1. Open [Railway](https://railway.app) → **New Project**.
2. Choose **Deploy from GitHub repo** (authorize GitHub if asked).
3. Select **`aamirhosii/neighborhood-intel`** (or your fork).

---

## 2. Monorepo: point Railway at `backend`

1. Open the new service → **Settings** (gear).
2. Under **Source**, set **Root Directory** to: **`backend`** and save.
3. This folder contains **`Dockerfile`** + **`railway.toml`**. Railway’s [ASP.NET guide](https://docs.railway.app/guides/aspnet-core) uses **Docker** (Railpack does not build .NET yet). The Dockerfile publishes `NeighborhoodIntel.Api` and starts Kestrel on **`PORT`**.

If you ever deploy from the **repo root** without changing Root Directory, set **Config as code** to the file path **`/backend/railway.toml`** in service settings.

First deploy may take a few minutes (SDK restore + publish image).

---

## 3. Environment variables

In the service → **Variables**, add:

| Name | Value |
|------|--------|
| `GoogleMaps__ApiKey` | Google key (Geocoding + Places enabled). Use **double** underscore: `GoogleMaps__ApiKey`. |
| `OpenAI__ApiKey` | OpenAI API key (`OpenAI__ApiKey`). |
| `CORS_ALLOWED_ORIGINS` | Your **production** Vercel origin, e.g. `https://your-app.vercel.app` — **https**, **no trailing slash**. Comma-separate multiple origins. |

Optional:

| Name | Value |
|------|--------|
| `Cors__AllowedOrigins__0`, `__1`, … | Alternative to `CORS_ALLOWED_ORIGINS`: ASP.NET style indexed env vars (see `appsettings.json`). |
| `CORS_ALLOW_VERCEL_PREVIEWS` | `true` or `false` — overrides preview behavior. If **unset**, any `vercel.app` origin in `CORS_ALLOWED_ORIGINS` also allows **preview** `https://*.vercel.app` URLs (see `Program.cs`). |

**Do not** commit real keys; set them only in Railway.

---

## 4. Public URL

1. **Settings** → **Networking** → **Generate Domain** (or attach a custom domain).
2. Copy the HTTPS URL, e.g. `https://neighborhood-intel-production-xxxx.up.railway.app`.

Use that value (no trailing slash) as **`VITE_API_BASE_URL`** on Vercel, then redeploy the frontend.

---

## 5. Smoke test

```bash
curl -s -X POST "https://YOUR-RAILWAY-URL/api/analyze-location" ^
  -H "Content-Type: application/json" ^
  -d "{\"address\":\"1101 Finch Ave W, North York, ON\",\"radiusMeters\":1000}"
```

You should get JSON with `counts`, `score`, and `address`.

---

## Troubleshooting

- **502 / never becomes healthy:** Confirm the app listens on **`PORT`**. This repo sets `UseUrls` from `PORT` in `Program.cs` for Railway.
- **CORS errors in the browser:** Use **`CORS_ALLOWED_ORIGINS`** with the **exact** frontend origin (`https://…`, no path, no trailing slash). Preview deploys use different `*.vercel.app` hostnames than production — either include a production `vercel.app` URL in `CORS_ALLOWED_ORIGINS` (this repo then allows preview subdomains) or set `CORS_ALLOW_VERCEL_PREVIEWS=true`.
- **Google / OpenAI errors:** Check API billing, enabled APIs (Geocoding, Places), and that keys are not restricted in a way that blocks Railway’s egress IPs (or use unrestricted server keys with rotation).
