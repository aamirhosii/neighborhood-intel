<template>
  <form @submit.prevent="submit" class="search-row" @keydown.esc="closeSuggestions">
    <div class="input-wrap" ref="wrapRef">
      <span class="input-icon">🔍</span>
      <input
        v-model="address"
        type="text"
        placeholder="Enter an address — e.g. 123 King St, Toronto"
        class="address-input"
        :disabled="loading"
        autocomplete="off"
        @input="onInput"
        @keydown.down.prevent="moveCursor(1)"
        @keydown.up.prevent="moveCursor(-1)"
        @keydown.enter.prevent="selectOrSubmit"
        @focus="showList = suggestions.length > 0"
        @blur="onBlur"
      />

      <Transition name="dropdown">
        <ul v-if="showList && suggestions.length" class="suggestions">
          <li
            v-for="(s, i) in suggestions"
            :key="s.placeId"
            :class="['suggestion-item', { active: i === cursor }]"
            @mousedown.prevent="pickSuggestion(s.description)"
          >
            <span class="sug-icon">📍</span>
            {{ s.description }}
          </li>
        </ul>
      </Transition>
    </div>

    <select v-model="radius" class="radius-select" :disabled="loading">
      <option value="500">500 m</option>
      <option value="1000">1 km</option>
      <option value="2000">2 km</option>
    </select>

    <button type="submit" class="btn-analyze" :disabled="loading || !address.trim()">
      <span v-if="loading" class="spinner" />
      <span v-else>Analyze</span>
    </button>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { autocomplete } from '../api.js'

const props = defineProps({ loading: Boolean })
const emit  = defineEmits(['analyze'])

const address     = ref('')
const radius      = ref(1000)
const suggestions = ref([])
const showList    = ref(false)
const cursor      = ref(-1)
const wrapRef     = ref(null)

let debounceTimer = null

function onInput() {
  cursor.value = -1
  clearTimeout(debounceTimer)
  if (address.value.length < 2) { suggestions.value = []; showList.value = false; return }
  debounceTimer = setTimeout(async () => {
    suggestions.value = await autocomplete(address.value)
    showList.value = suggestions.value.length > 0
  }, 280)
}

function moveCursor(dir) {
  if (!showList.value) return
  cursor.value = Math.max(-1, Math.min(cursor.value + dir, suggestions.value.length - 1))
}

function selectOrSubmit() {
  if (cursor.value >= 0 && suggestions.value[cursor.value]) {
    pickSuggestion(suggestions.value[cursor.value].description)
  } else {
    submit()
  }
}

function pickSuggestion(desc) {
  address.value = desc
  closeSuggestions()
  submit()
}

function closeSuggestions() {
  showList.value = false
  cursor.value = -1
}

function onBlur() {
  setTimeout(closeSuggestions, 150)
}

function submit() {
  if (address.value.trim())
    emit('analyze', { address: address.value.trim(), radius: Number(radius.value) })
}
</script>

<style scoped>
.search-row {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
  align-items: flex-start;
}

.input-wrap {
  flex: 1;
  min-width: 220px;
  position: relative;
}

.input-icon {
  position: absolute;
  left: 0.85rem;
  top: 50%;
  transform: translateY(-50%);
  font-size: 1rem;
  pointer-events: none;
  z-index: 1;
}

.address-input {
  width: 100%;
  background: var(--surface2);
  border: 1px solid var(--border);
  color: var(--text);
  border-radius: 8px;
  padding: 0.78rem 1rem 0.78rem 2.4rem;
  font-size: 0.95rem;
  transition: border-color 0.2s;
}

.address-input:focus { border-color: var(--accent); }
.address-input::placeholder { color: var(--muted); }

.suggestions {
  position: absolute;
  top: calc(100% + 6px);
  left: 0; right: 0;
  background: var(--surface2);
  border: 1px solid var(--border);
  border-radius: 10px;
  list-style: none;
  z-index: 100;
  box-shadow: 0 12px 32px rgba(0,0,0,0.4);
  overflow: hidden;
}

.suggestion-item {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  padding: 0.65rem 1rem;
  font-size: 0.9rem;
  cursor: pointer;
  transition: background 0.15s;
  color: var(--text);
}

.suggestion-item:hover,
.suggestion-item.active { background: var(--surface); }

.sug-icon { font-size: 0.85rem; flex-shrink: 0; }

.radius-select {
  background: var(--surface2);
  border: 1px solid var(--border);
  color: var(--text);
  border-radius: 8px;
  padding: 0.78rem 0.75rem;
  font-size: 0.9rem;
  cursor: pointer;
}

.btn-analyze {
  background: linear-gradient(135deg, var(--accent), var(--accent2));
  color: #fff;
  border-radius: 8px;
  padding: 0.78rem 1.75rem;
  font-weight: 600;
  font-size: 0.95rem;
  transition: opacity 0.2s;
  min-width: 110px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-analyze:disabled { opacity: 0.5; cursor: not-allowed; }

.spinner {
  width: 18px; height: 18px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
  display: inline-block;
}

@keyframes spin { to { transform: rotate(360deg); } }

.dropdown-enter-active { transition: opacity 0.15s, transform 0.15s; }
.dropdown-leave-active { transition: opacity 0.1s; }
.dropdown-enter-from  { opacity: 0; transform: translateY(-4px); }
.dropdown-leave-to    { opacity: 0; }
</style>
