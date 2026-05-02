<template>
  <div class="hero">
    <!-- Header -->
    <div class="hero-header">
      <div class="badge">AI-Powered · Real-Time Data · Free</div>
      <h1 class="hero-title">
        Understand any neighbourhood<br />
        <span class="gradient-text">before you commit.</span>
      </h1>
      <p class="hero-sub">
        Enter an address and get an instant breakdown of schools, transit, green spaces,
        grocery access, and more — scored by an algorithm and explained by AI.
      </p>
    </div>

    <!-- Search slot -->
    <div class="search-slot">
      <slot />
      <p v-if="error" class="error-msg">{{ error }}</p>
    </div>

    <!-- Feature cards -->
    <div class="feature-grid">
      <div v-for="f in features" :key="f.title" class="feature-card">
        <div class="feat-icon">{{ f.icon }}</div>
        <div class="feat-title">{{ f.title }}</div>
        <div class="feat-desc">{{ f.desc }}</div>
      </div>
    </div>

    <!-- Stats bar -->
    <div class="stats-bar">
      <div v-for="s in stats" :key="s.label" class="stat-item">
        <span class="stat-n">{{ s.n }}</span>
        <span class="stat-l">{{ s.label }}</span>
      </div>
    </div>

    <!-- How it works -->
    <div class="how-section">
      <div class="section-label">How it works</div>
      <div class="steps">
        <div v-for="(step, i) in steps" :key="i" class="step">
          <div class="step-num">{{ i + 1 }}</div>
          <div class="step-body">
            <div class="step-title">{{ step.title }}</div>
            <div class="step-desc">{{ step.desc }}</div>
          </div>
          <div v-if="i < steps.length - 1" class="step-arrow">→</div>
        </div>
      </div>
    </div>

    <!-- Sample addresses -->
    <div class="samples-section">
      <div class="section-label">Try an example</div>
      <div class="samples">
        <button
          v-for="s in samples"
          :key="s"
          class="sample-chip"
          @click="$emit('pick', s)"
        >
          {{ s }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({ error: String })
defineEmits(['pick'])

const features = [
  { icon: '🏫', title: 'Education Access',   desc: 'Counts schools and universities within your chosen radius using live Google Places data.' },
  { icon: '🌳', title: 'Green Space',         desc: 'Parks and recreational areas affect both quality of life and long-term property values.' },
  { icon: '🚌', title: 'Transit Coverage',    desc: 'Subway, bus, and rail stops scored so you know exactly how car-dependent the area is.' },
  { icon: '🛒', title: 'Daily Errands',       desc: 'Supermarkets and grocery stores — because running out of milk at midnight matters.' },
  { icon: '🧮', title: 'Weighted Score',      desc: 'A transparent C# algorithm combines all factors into a single 0–100 neighbourhood score.' },
  { icon: '🤖', title: 'AI Advisor',          desc: 'GPT-4o-mini synthesises the data into a plain-English evaluation written from a buyer\'s perspective.' },
]

const stats = [
  { n: '5',    label: 'Amenity categories' },
  { n: '< 3s', label: 'Average response time' },
  { n: '100',  label: 'Point scoring scale' },
  { n: '5 km', label: 'Max search radius' },
]

const steps = [
  { title: 'Enter address',   desc: 'Type any address — autocomplete suggests real locations as you type.' },
  { title: 'Data is fetched', desc: 'The backend geocodes the address and queries Google Places for each category.' },
  { title: 'Score computed',  desc: 'A weighted formula converts the counts into a 0–100 neighbourhood score.' },
  { title: 'AI evaluates',    desc: 'Hit "Analyze Location" and GPT writes a buyer-focused summary in seconds.' },
]

const samples = [
  '123 King St W, Toronto',
  'Central Park, New York',
  '221B Baker St, London',
  'Rue de Rivoli, Paris',
]
</script>

<style scoped>
.hero { padding-bottom: 1rem; }

.hero-header {
  text-align: center;
  margin-bottom: 2.5rem;
}

.badge {
  display: inline-block;
  background: rgba(108, 99, 255, 0.15);
  border: 1px solid rgba(108, 99, 255, 0.35);
  color: #a89cff;
  font-size: 0.75rem;
  font-weight: 600;
  letter-spacing: 0.07em;
  text-transform: uppercase;
  padding: 0.3rem 0.9rem;
  border-radius: 99px;
  margin-bottom: 1.25rem;
}

.hero-title {
  font-size: clamp(1.8rem, 4vw, 2.8rem);
  font-weight: 800;
  line-height: 1.2;
  margin-bottom: 1rem;
}

.gradient-text {
  background: linear-gradient(135deg, var(--accent), var(--accent2));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.hero-sub {
  color: var(--muted);
  font-size: 1rem;
  max-width: 560px;
  margin: 0 auto;
  line-height: 1.7;
}

/* Search slot */
.search-slot {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  padding: 1.25rem 1.5rem;
  margin-bottom: 2rem;
}

.error-msg {
  color: var(--danger);
  margin-top: 0.75rem;
  font-size: 0.9rem;
}

/* Feature grid */
.feature-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(230px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.feature-card {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  padding: 1.25rem;
  transition: border-color 0.2s, transform 0.2s;
}

.feature-card:hover {
  border-color: rgba(108, 99, 255, 0.5);
  transform: translateY(-2px);
}

.feat-icon { font-size: 1.5rem; margin-bottom: 0.6rem; }
.feat-title { font-weight: 700; font-size: 0.95rem; margin-bottom: 0.35rem; }
.feat-desc  { color: var(--muted); font-size: 0.82rem; line-height: 1.6; }

/* Stats bar */
.stats-bar {
  display: flex;
  justify-content: center;
  gap: 0;
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  overflow: hidden;
  margin-bottom: 2rem;
}

.stat-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.1rem 0.5rem;
  border-right: 1px solid var(--border);
}

.stat-item:last-child { border-right: none; }

.stat-n {
  font-size: 1.5rem;
  font-weight: 800;
  background: linear-gradient(135deg, var(--accent), var(--accent2));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  line-height: 1;
  margin-bottom: 0.3rem;
}

.stat-l { font-size: 0.75rem; color: var(--muted); text-align: center; }

/* How it works */
.how-section { margin-bottom: 2rem; }

.section-label {
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: var(--muted);
  margin-bottom: 1rem;
}

.steps {
  display: flex;
  align-items: flex-start;
  gap: 0;
  flex-wrap: wrap;
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  overflow: hidden;
}

.step {
  flex: 1;
  min-width: 160px;
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 1.25rem;
  position: relative;
}

.step-num {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--accent), var(--accent2));
  color: #fff;
  font-weight: 700;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.step-title { font-weight: 700; font-size: 0.88rem; margin-bottom: 0.25rem; }
.step-desc  { color: var(--muted); font-size: 0.8rem; line-height: 1.5; }

.step-arrow {
  position: absolute;
  right: -8px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--muted);
  font-size: 1rem;
  z-index: 1;
}

/* Samples */
.samples-section { margin-bottom: 1rem; }

.samples {
  display: flex;
  flex-wrap: wrap;
  gap: 0.6rem;
}

.sample-chip {
  background: var(--surface);
  border: 1px solid var(--border);
  color: var(--muted);
  border-radius: 99px;
  padding: 0.4rem 1rem;
  font-size: 0.82rem;
  cursor: pointer;
  transition: border-color 0.2s, color 0.2s;
  font-family: inherit;
}

.sample-chip:hover {
  border-color: var(--accent);
  color: var(--text);
}
</style>
