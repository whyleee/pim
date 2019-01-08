<template>
  <div class="d-inline-block ml-1">
    <b-button
      @click="onAuthClick"
    >
      {{ buttonText || defaultButtonText }}
    </b-button>

    <b-modal
      ref="authModal"
      :visible="authModalVisible"
      :title="title"
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
export default {
  props: {
    backend: {
      type: Object,
      required: true
    },
    buttonText: {
      type: String,
      default: undefined
    }
  },
  data() {
    return {
      authModalVisible: false,
      authModalApiKey: null
    }
  },
  computed: {
    title() {
      return `Authorize ${this.backend.config.title}`
    },
    defaultButtonText() {
      return this.backend.apiKey ? 'Edit key' : 'Authorize'
    }
  },
  methods: {
    onAuthClick() {
      this.authModalApiKey = this.backend.apiKey
      this.$refs.authModal.show()
    },
    onAuthSubmit() {
      this.backend.apiKey = this.authModalApiKey
      this.authModalApiKey = null
    }
  }
}
</script>
