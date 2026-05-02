<template>
  <div class="container">

    <!-- Landing hero (shown until first result) -->
    <Transition name="fade">
      <LandingHero
        v-if="!result"
        :error="analyzeError"
        @pick="prefillAndAnalyze"
      >
        <AddressInput
          :loading="analyzing"
          ref="inputRef"
          @analyze="handleAnalyze"
        />
      </LandingHero>
    </Transition>

    <!-- Results view -->
    <Transition name="fade">
      <div v-if="result" class="results">

        <!-- Compact search bar at top -->
        <div class="results-header">
          <div class="results-brand">
            <span>📍</span>
            <span class="brand-name">NeighborhoodIntel</span>
          </div>
          <div class="results-search">
            <AddressInput :loading="analyzing" @analyze="handleAnalyze" />
            <p v-if="analyzeError" class="error-msg">{{ analyzeError }}</p>
          </div>
        </div>

        <MapEmbed
          v-if="googleMapsKey"
          :lat="result.latitude"
          :lng="result.longitude"
          :api-key="googleMapsKey"
          :places="result.places"
        />
        <p v-else class="map-fallback">Map is hidden until you set <code>VITE_GOOGLE_MAPS_KEY</code> on Vercel (browser key; separate from the server key on Railway).</p>

        <ScoreDisplay
          :score="result.score"
          :label="result.scoreLabel"
          :address="result.address"
        />

        <StatsCards
          :counts="result.counts"
          :radius-meters="result.radiusMeters"
        />

        <AiPanel
          :summary="aiSummary"
          :loading="aiLoading"
          @request="handleAiRequest"
        />
      </div>
    </Transition>

  </div>
</template>

<script setup>
import { ref } from 'vue'
import LandingHero  from './components/LandingHero.vue'
import AddressInput from './components/AddressInput.vue'
import StatsCards   from './components/StatsCards.vue'
import ScoreDisplay from './components/ScoreDisplay.vue'
import AiPanel      from './components/AiPanel.vue'
import MapEmbed     from './components/MapEmbed.vue'
import { analyzeLocation, getAiSummary } from './api.js'

const result       = ref(null)
const analyzing    = ref(false)
const analyzeError = ref('')
const aiSummary    = ref('')
const aiLoading    = ref(false)
const inputRef     = ref(null)

const googleMapsKey = import.meta.env.VITE_GOOGLE_MAPS_KEY ?? ''

function apiErrorMessage(err) {
  const d = err.response?.data
  if (typeof d === 'string') return d
  if (d && typeof d === 'object' && d.error) return String(d.error)
  if (d && typeof d === 'object' && d.title) return String(d.title)
  return err.message ?? 'Something went wrong.'
}

async function handleAnalyze({ address, radius }) {
  analyzing.value    = true
  analyzeError.value = ''
  aiSummary.value    = ''

  try {
    result.value = await analyzeLocation(address, radius)
  } catch (err) {
    analyzeError.value = apiErrorMessage(err)
    result.value = null
  } finally {
    analyzing.value = false
  }
}

function prefillAndAnalyze(address) {
  handleAnalyze({ address, radius: 1000 })
}

async function handleAiRequest() {
  if (!result.value) return
  aiLoading.value = true
  try {
    aiSummary.value = await getAiSummary(
      result.value.address,
      result.value.counts,
      result.value.score,
    )
  } catch (err) {
    aiSummary.value = 'AI summary unavailable: ' + apiErrorMessage(err)
  } finally {
    aiLoading.value = false
  }
}
</script>

<style>
.results { animation: slideUp 0.4s ease; }

@keyframes slideUp {
  from { opacity: 0; transform: translateY(16px); }
  to   { opacity: 1; transform: translateY(0); }
}

.fade-enter-active, .fade-leave-active { transition: opacity 0.25s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

.results-header {
  display: flex;
  align-items: flex-start;
  gap: 1.5rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.results-brand {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.1rem;
  font-weight: 700;
  white-space: nowrap;
  padding-top: 0.75rem;
}

.brand-name {
  background: linear-gradient(135deg, var(--accent), var(--accent2));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.results-search { flex: 1; min-width: 240px; }

.error-msg {
  color: var(--danger);
  margin-top: 0.5rem;
  font-size: 0.88rem;
}

.map-fallback {
  font-size: 0.88rem;
  color: var(--muted);
  margin-bottom: 1rem;
  line-height: 1.45;
}

.map-fallback code {
  font-size: 0.82rem;
}
</style>
