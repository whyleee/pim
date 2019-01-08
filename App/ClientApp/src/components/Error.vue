<template>
  <b-modal
    :visible="!!store.error"
    :title="title"
    ok-only
    @ok="onOk"
  >
    <p v-if="url">
      {{ httpMethod }} <b-link :href="url">{{ url }}</b-link>
    </p>

    <b-button
      variant="link"
      @click="toggleDetails"
    >
      {{ detailed ? 'Hide' : 'See' }} details
    </b-button>

    <p v-if="detailed">
      <pre>{{ errorDetails }}</pre>
    </p>
  </b-modal>
</template>

<script>
import store from '@/store'

export default {
  data() {
    return {
      store,
      detailed: false
    }
  },
  computed: {
    error() {
      return this.store.error || ''
    },
    title() {
      let title = 'API Error'

      if (this.error.message) {
        title += `: ${this.error.message}`
      }
      return title
    },
    url() {
      const { config } = this.error

      return config ? config.url : null
    },
    httpMethod() {
      const { config } = this.error

      if (config && config.method) {
        return config.method.toUpperCase()
      }

      return null
    },
    errorDetails() {
      if (this.error.response && this.error.response.data) {
        return this.error.response.data
      }

      return this.error
    }
  },
  methods: {
    onOk() {
      this.store.error = null
      this.detailed = false
    },
    toggleDetails() {
      this.detailed = !this.detailed
    }
  }
}
</script>
