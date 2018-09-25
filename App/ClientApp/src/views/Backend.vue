<template>
  <b-row>
    <b-col
      md="2"
      class="d-none d-md-block"
    >
      <b-nav
        v-if="store.meta"
        vertical
      >
        <b-nav-item
          v-for="collection in store.meta.collections"
          :key="collection.key"
          :to="{ name: `${backend.key}-list`, params: { collection: collection.key }}"
        >
          {{ collection.name }}
        </b-nav-item>
      </b-nav>
    </b-col>
    <b-col>
      <b-row align-v="center">
        <b-col>
          {{ backend.title }} [<i>{{ backend.data.baseUrl }}</i>]
        </b-col>
        <b-col class="text-right">
          <Authorize v-if="requiresApiKey"/>
        </b-col>
      </b-row>

      <b-row class="mt-3">
        <b-col>
          <router-view v-if="hasAccessToApi"/>
          <b-alert
            v-else
            show
            variant="dark"
          >
            Backend is secured, please authorize.
          </b-alert>
        </b-col>
      </b-row>
    </b-col>
  </b-row>
</template>

<script>
import store from '@/store'
import Authorize from '@/components/Authorize.vue'

export default {
  components: {
    Authorize
  },
  props: {
    backend: {
      type: Object,
      required: true
    }
  },
  data() {
    store.setBackend(this.backend)
    return {
      store: store.backend
    }
  },
  computed: {
    requiresApiKey() {
      return this.backend.authHeader
    },
    hasAccessToApi() {
      return !this.requiresApiKey || !!this.store.apiKey
    }
  },
  async created() {
    await this.store.fetchMeta()
  }
}
</script>
