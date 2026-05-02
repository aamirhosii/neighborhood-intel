import axios from 'axios'

// Dev: Vite proxies /api → backend. Production (e.g. Vercel): set VITE_API_BASE_URL to your API origin (https://…, no trailing slash).
function normalizeApiOrigin(raw) {
  const trimmed = String(raw).trim().replace(/\/$/, '')
  if (!trimmed) return ''
  return /^https?:\/\//i.test(trimmed) ? trimmed : `https://${trimmed}`
}
const baseURL = import.meta.env.VITE_API_BASE_URL
  ? `${normalizeApiOrigin(import.meta.env.VITE_API_BASE_URL)}/api`
  : '/api'

const http = axios.create({ baseURL })

export async function analyzeLocation(address, radiusMeters = 1000) {
  const { data } = await http.post('/analyze-location', { address, radiusMeters })
  return data
}

export async function getAiSummary(address, counts, score) {
  const { data } = await http.post('/ai-summary', { address, counts, score })
  return data.summary
}

export async function autocomplete(input) {
  if (!input || input.length < 2) return []
  const { data } = await http.get('/autocomplete', { params: { input } })
  return data.predictions ?? []
}
