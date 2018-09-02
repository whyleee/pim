<template>
  <div class="d-inline-block ml-1">
    <b-button
      @click="onAuthClick"
    >
      {{ buttonText }}
    </b-button>

    <b-modal
      ref="authModal"
      :visible="authModalVisible"
      title="Authorize"
      @ok="onAuthSubmit"
    >
      <b-form @submit.prevent="onAuthSubmit">
        <b-form-group
          label="API key"
          horizontal
        >
          <b-form-input
            v-model="authModalApiKey"
            type="text"
            name="apiKey"
          />
        </b-form-group>
      </b-form>
    </b-modal>
  </div>
</template>

<script>
import store from '@/store'

export default {
  data() {
    return {
      store: store.backend,
      authModalVisible: false,
      authModalApiKey: null
    }
  },
  computed: {
    buttonText() {
      return this.store.apiKey ? 'Edit key' : 'Authorize'
    }
  },
  methods: {
    onAuthClick() {
      this.authModalApiKey = this.store.apiKey
      this.$refs.authModal.show()
    },
    onAuthSubmit() {
      this.store.apiKey = this.authModalApiKey
      this.authModalApiKey = null
    }
  }
}
</script>
