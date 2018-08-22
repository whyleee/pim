<template>
  <div>
    <b-button
      @click="onAuthClick"
    >
      Authorize
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
  methods: {
    onAuthClick() {
      this.authModalApiKey = this.store.apiKey
      this.$refs.authModal.show()
    },
    onAuthSubmit() {
      this.store.setApiKey(this.authModalApiKey)
      this.authModalApiKey = null
    }
  }
}
</script>
