<template>
  <b-modal
    :visible="!!store.error"
    title="Error"
    ok-only
    @ok="onOk"
  >
    {{ errorMessage }}
  </b-modal>
</template>

<script>
import store from '@/store'

export default {
  data() {
    return {
      store
    }
  },
  computed: {
    errorMessage() {
      const { error } = this.store

      if (!error) {
        return ''
      }
      if (error.response && error.response.data) {
        return error.response.data
      }

      return error
    }
  },
  methods: {
    onOk() {
      this.store.error = null
    }
  }
}
</script>
