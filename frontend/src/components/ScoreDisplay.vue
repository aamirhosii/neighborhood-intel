<template>
  <div class="card score-card">
    <div class="score-left">
      <div class="score-label-top">Neighborhood Score</div>
      <div class="score-address">{{ address }}</div>
    </div>

    <div class="score-right">
      <svg class="ring" viewBox="0 0 120 120">
        <circle class="ring-bg" cx="60" cy="60" r="50" />
        <circle
          class="ring-fill"
          cx="60" cy="60" r="50"
          :stroke="ringColor"
          :stroke-dasharray="`${circumference} ${circumference}`"
          :stroke-dashoffset="dashOffset"
        />
      </svg>
      <div class="score-number">
        <span class="score-val">{{ score }}</span>
        <span class="score-max">/100</span>
      </div>
      <div class="score-badge" :style="{ color: ringColor }">{{ label }}</div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  score: Number,
  label: String,
  address: String,
})

const circumference = 2 * Math.PI * 50

const dashOffset = computed(() =>
  circumference - (props.score / 100) * circumference
)

const ringColor = computed(() => {
  if (props.score >= 80) return '#38d9a9'
  if (props.score >= 60) return '#6c63ff'
  if (props.score >= 40) return '#fcc419'
  return '#ff6b6b'
})
</script>

<style scoped>
.score-card {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1.5rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.score-label-top {
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: var(--muted);
  margin-bottom: 0.5rem;
}

.score-address {
  font-size: 1.1rem;
  font-weight: 600;
  max-width: 400px;
}

.score-right {
  position: relative;
  width: 110px;
  height: 110px;
  flex-shrink: 0;
}

.ring {
  width: 110px;
  height: 110px;
  transform: rotate(-90deg);
}

.ring-bg {
  fill: none;
  stroke: var(--border);
  stroke-width: 8;
}

.ring-fill {
  fill: none;
  stroke-width: 8;
  stroke-linecap: round;
  transition: stroke-dashoffset 1.2s cubic-bezier(0.4, 0, 0.2, 1), stroke 0.4s;
}

.score-number {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1px;
}

.score-val {
  font-size: 1.8rem;
  font-weight: 700;
  line-height: 1;
}

.score-max {
  font-size: 0.8rem;
  color: var(--muted);
  align-self: flex-end;
  padding-bottom: 6px;
}

.score-badge {
  text-align: center;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  margin-top: 0.25rem;
}
</style>
