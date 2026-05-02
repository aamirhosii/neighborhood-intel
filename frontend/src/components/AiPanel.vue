<template>
  <div class="card ai-panel">
    <div class="ai-header">
      <div class="ai-title">
        <span class="ai-dot" />
        AI Advisor Summary
      </div>
      <button
        class="btn-ai"
        :disabled="loading || !!summary"
        @click="$emit('request')"
      >
        <span v-if="loading" class="spinner" />
        <span v-else-if="summary">Analysis complete</span>
        <span v-else>✦ Analyze Location</span>
      </button>
    </div>

    <div v-if="summary" class="ai-body">
      <p v-for="(para, i) in paragraphs" :key="i" class="ai-para">{{ para }}</p>
    </div>

    <div v-else-if="!loading" class="ai-placeholder">
      <p>Click "Analyze Location" for an AI-generated evaluation from a real estate advisor perspective.</p>
    </div>

    <div v-else class="ai-loading">
      <div class="typing-dots">
        <span /><span /><span />
      </div>
      <p>Generating your neighborhood report…</p>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  summary: String,
  loading: Boolean,
})

defineEmits(['request'])

const paragraphs = computed(() =>
  props.summary
    ? props.summary.split(/\n+/).filter(p => p.trim())
    : []
)
</script>

<style scoped>
.ai-panel { margin-top: 1.5rem; }

.ai-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.ai-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 700;
  font-size: 1rem;
}

.ai-dot {
  width: 8px; height: 8px;
  border-radius: 50%;
  background: var(--accent);
  box-shadow: 0 0 8px var(--accent);
}

.btn-ai {
  background: linear-gradient(135deg, #6c63ff, #b06ab3);
  color: #fff;
  border-radius: 8px;
  padding: 0.6rem 1.4rem;
  font-weight: 600;
  font-size: 0.9rem;
  min-width: 160px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  transition: opacity 0.2s;
}

.btn-ai:disabled { opacity: 0.6; cursor: not-allowed; }

.ai-para {
  color: var(--text);
  font-size: 0.95rem;
  line-height: 1.7;
  margin-bottom: 0.75rem;
}

.ai-placeholder {
  color: var(--muted);
  font-size: 0.9rem;
  font-style: italic;
  padding: 1rem 0;
}

.ai-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 1.5rem 0;
  color: var(--muted);
  font-size: 0.9rem;
}

.typing-dots {
  display: flex;
  gap: 6px;
}

.typing-dots span {
  width: 8px; height: 8px;
  border-radius: 50%;
  background: var(--accent);
  animation: bounce 1.2s infinite;
}

.typing-dots span:nth-child(2) { animation-delay: 0.2s; }
.typing-dots span:nth-child(3) { animation-delay: 0.4s; }

@keyframes bounce {
  0%, 80%, 100% { transform: translateY(0); opacity: 0.4; }
  40% { transform: translateY(-8px); opacity: 1; }
}

.spinner {
  width: 16px; height: 16px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }
</style>
