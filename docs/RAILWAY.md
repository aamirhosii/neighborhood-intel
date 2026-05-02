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
| `GoogleMaps__ApiKey` | Your Google Maps key (Geocoding + Places enabled) |
| `OpenAI__ApiKey` | Your OpenAI API key |
| `Cors__AllowedOrigins__0` | Your Vercel site, e.g. `https://neighborhood-intel-amirs-projects-74ab5506.vercel.app` |
| `Cors__AllowedOrigins__1` | `http://localhost:5173` (optional, for local dev against prod API) |

Add more `Cors__AllowedOrigins__2`, `__3`, … if you use extra domains.

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
- **CORS errors in the browser:** Add your exact Vercel origin (scheme + host, no path) to `Cors__AllowedOrigins__*`.
- **Google / OpenAI errors:** Check API billing, enabled APIs (Geocoding, Places), and that keys are not restricted in a way that blocks Railway’s egress IPs (or use unrestricted server keys with rotation).
