<template>
  <b-row>
    <b-col
      md="2"
      class="mb-3"
    >
      <h3>{{ backend.title }}</h3>
      <pre
        :title="backend.data.baseUrl"
        class="base-url small"
      >
        [Base URL: {{ backend.data.baseUrl }}]
      </pre>
      <hr>

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
      <hr>

      <Authorize
        v-if="requiresApiKey"
        :backend="store"
      />
    </b-col>

    <b-col>
      <b-alert
        v-if="!hasAccessToApi"
        show
        variant="dark"
      >
        Backend is secured, please authorize.
      </b-alert>

      <Welcome v-else-if="$route.name == backend.key">
        <h1>Welcome to {{ backend.title }}!</h1>
        <p class="lead"><i>Select collection in the sub-menu</i></p>
      </Welcome>

      <router-view v-else />
    </b-col>

    <b-col xl="2"/>
  </b-row>
</template>

<script>
import store from '@/store'
import Authorize from '@/components/Authorize.vue'
import Welcome from '@/components/Welcome.vue'

export default {
  components: {
    Authorize,
    Welcome
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
  watch: {
    hasAccessToApi(yes) {
      if (yes && !this.store.meta) {
        this.store.fetchMeta()
      }
    }
  },
  created() {
    if (this.hasAccessToApi && !this.store.meta) {
      this.store.fetchMeta()
    }
  }
}
</script>

<style scoped>
.nav .active {
  font-weight: bold;
}

.base-url {
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
}
</style>
