<template>
  <div class="card map-card">
    <div class="map-header">
      <span class="map-title">Map View</span>
      <div class="legend">
        <span v-for="l in legend" :key="l.label" class="legend-item">
          <span class="legend-dot" :style="{ background: l.color }" />
          {{ l.label }}
        </span>
      </div>
    </div>
    <div ref="mapRef" class="map-frame" />
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'

const props = defineProps({
  lat:    Number,
  lng:    Number,
  apiKey: String,
  places: { type: Array, default: () => [] },
})

const mapRef = ref(null)
let mapInstance = null

const categoryColors = {
  schools:     '#6c63ff',
  parks:       '#38d9a9',
  grocery:     '#fcc419',
  transit:     '#4facfe',
  restaurants: '#ff6b6b',
}

const legend = [
  { label: 'Schools',     color: '#6c63ff' },
  { label: 'Parks',       color: '#38d9a9' },
  { label: 'Grocery',     color: '#fcc419' },
  { label: 'Transit',     color: '#4facfe' },
  { label: 'Restaurants', color: '#ff6b6b' },
]

function loadMapsApi() {
  return new Promise((resolve) => {
    if (window.google?.maps) { resolve(); return }
    const script = document.createElement('script')
    script.src = `https://maps.googleapis.com/maps/api/js?key=${props.apiKey}`
    script.async = true
    script.onload = resolve
    document.head.appendChild(script)
  })
}

function hexToRgb(hex) {
  const r = parseInt(hex.slice(1, 3), 16)
  const g = parseInt(hex.slice(3, 5), 16)
  const b = parseInt(hex.slice(5, 7), 16)
  return { r, g, b }
}

function makeMarkerIcon(color) {
  const { r, g, b } = hexToRgb(color)
  const svg = `
    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="32" viewBox="0 0 24 32">
      <path d="M12 0C5.4 0 0 5.4 0 12c0 9 12 20 12 20s12-11 12-20C24 5.4 18.6 0 12 0z"
            fill="rgba(${r},${g},${b},0.9)" stroke="white" stroke-width="1.5"/>
      <circle cx="12" cy="12" r="5" fill="white" opacity="0.9"/>
    </svg>`
  return {
    url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(svg),
    scaledSize: new window.google.maps.Size(24, 32),
    anchor: new window.google.maps.Point(12, 32),
  }
}

async function initMap() {
  await loadMapsApi()

  mapInstance = new window.google.maps.Map(mapRef.value, {
    center: { lat: props.lat, lng: props.lng },
    zoom: 15,
    styles: darkMapStyles,
    disableDefaultUI: false,
    zoomControl: true,
    mapTypeControl: false,
    streetViewControl: false,
  })

  // center pin for the searched address
  new window.google.maps.Marker({
    position: { lat: props.lat, lng: props.lng },
    map: mapInstance,
    title: 'Searched location',
    icon: {
      path: window.google.maps.SymbolPath.CIRCLE,
      scale: 10,
      fillColor: '#ffffff',
      fillOpacity: 1,
      strokeColor: '#6c63ff',
      strokeWeight: 3,
    },
    zIndex: 999,
  })

  const infoWindow = new window.google.maps.InfoWindow()

  props.places.forEach(place => {
    const color = categoryColors[place.category] ?? '#aaa'
    const marker = new window.google.maps.Marker({
      position: { lat: place.lat, lng: place.lng },
      map: mapInstance,
      title: place.name,
      icon: makeMarkerIcon(color),
    })

    marker.addListener('click', () => {
      infoWindow.setContent(`
        <div style="font-family:Inter,sans-serif;padding:4px 6px;min-width:140px">
          <div style="font-weight:700;font-size:13px;margin-bottom:2px">${place.name}</div>
          <div style="font-size:11px;color:#888;text-transform:capitalize">${place.category}</div>
        </div>`)
      infoWindow.open(mapInstance, marker)
    })
  })
}

onMounted(initMap)

watch(() => [props.lat, props.lng, props.places], initMap)

// minimal dark map style to match the app theme
const darkMapStyles = [
  { elementType: 'geometry',            stylers: [{ color: '#1a1d27' }] },
  { elementType: 'labels.text.fill',    stylers: [{ color: '#7a82a6' }] },
  { elementType: 'labels.text.stroke',  stylers: [{ color: '#0f1117' }] },
  { featureType: 'road',                elementType: 'geometry', stylers: [{ color: '#22263a' }] },
  { featureType: 'road',                elementType: 'geometry.stroke', stylers: [{ color: '#0f1117' }] },
  { featureType: 'road.highway',        elementType: 'geometry', stylers: [{ color: '#2e3350' }] },
  { featureType: 'water',               elementType: 'geometry', stylers: [{ color: '#0f1117' }] },
  { featureType: 'poi',                 elementType: 'geometry', stylers: [{ color: '#1a1d27' }] },
  { featureType: 'transit',             elementType: 'geometry', stylers: [{ color: '#22263a' }] },
  { featureType: 'administrative',      elementType: 'geometry.stroke', stylers: [{ color: '#2e3350' }] },
]
</script>

<style scoped>
.map-card { margin-bottom: 1.5rem; padding: 1rem; }

.map-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.map-title {
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: var(--muted);
}

.legend {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  font-size: 0.75rem;
  color: var(--muted);
}

.legend-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}

.map-frame {
  width: 100%;
  height: 380px;
  border-radius: 8px;
  overflow: hidden;
}
</style>
