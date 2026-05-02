<template>
  <div class="stats-grid">
    <div v-for="item in items" :key="item.label" class="stat-card card">
      <div class="stat-icon">{{ item.icon }}</div>
      <div class="stat-body">
        <div class="stat-count">{{ item.count }}</div>
        <div class="stat-label">{{ item.label }}</div>
        <div class="stat-sub">within {{ radiusLabel }}</div>
      </div>
      <div class="stat-bar">
        <div class="stat-fill" :style="{ width: barWidth(item.count) + '%', background: item.color }" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  counts: Object,
  radiusMeters: Number,
})

const radiusLabel = computed(() =>
  props.radiusMeters >= 1000
    ? `${props.radiusMeters / 1000} km`
    : `${props.radiusMeters} m`
)

const items = computed(() => [
  { icon: '🏫', label: 'Schools',     count: props.counts.schools,     color: '#6c63ff' },
  { icon: '🌳', label: 'Parks',       count: props.counts.parks,       color: '#38d9a9' },
  { icon: '🛒', label: 'Grocery',     count: props.counts.grocery,     color: '#fcc419' },
  { icon: '🚌', label: 'Transit',     count: props.counts.transit,     color: '#4facfe' },
  { icon: '🍽️', label: 'Restaurants', count: props.counts.restaurants, color: '#ff6b6b' },
])

function barWidth(count) {
  return Math.min((count / 20) * 100, 100)
}
</script>

<style scoped>
.stats-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 1rem;
  margin-bottom: 1.5rem;
}

@media (max-width: 700px) {
  .stats-grid { grid-template-columns: repeat(2, 1fr); }
}

.stat-card {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  position: relative;
  overflow: hidden;
  transition: transform 0.2s, box-shadow 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.3);
}

.stat-icon { font-size: 1.6rem; }

.stat-count {
  font-size: 2rem;
  font-weight: 700;
  line-height: 1;
}

.stat-label {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--text);
}

.stat-sub {
  font-size: 0.75rem;
  color: var(--muted);
}

.stat-bar {
  height: 3px;
  background: var(--border);
  border-radius: 99px;
  margin-top: 0.25rem;
}

.stat-fill {
  height: 100%;
  border-radius: 99px;
  transition: width 1s cubic-bezier(0.4, 0, 0.2, 1);
}
</style>
