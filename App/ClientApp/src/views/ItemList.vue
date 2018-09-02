<template>
  <div>
    <b-card no-body>
      <div slot="header">
        <b-row align-v="center">
          <b-col>
            <h1>{{ backend.title }}</h1>
          </b-col>
          <b-col class="text-right">
            <Authorize/>
            <b-button
              v-if="hasAccessToApi"
              :to="{ name: `${backend.key}-edit`, params: { id: 'new' }}"
              class="ml-1"
            >
              New
            </b-button>
          </b-col>
        </b-row>
      </div>

      <b-card-body v-if="!hasAccessToApi">
        <div class="text-center lead">
          Backend is secured, please authorize.
        </div>
      </b-card-body>

      <b-card-body v-else-if="loading">
        <div
          class="text-center lead"
          v-html="loadingHtml"
        />
      </b-card-body>

      <b-list-group
        v-else
        flush
      >
        <b-list-group-item
          v-for="item in store.listItems"
          :key="item[store.keyName]"
        >
          <b-link :to="{ name: `${backend.key}-edit`, params: { id: item[store.keyName] }}">
            {{ item.name }}
          </b-link>
          <b-link
            class="float-right"
            @click.stop="confirmDelete(item)"
          >
            Delete
          </b-link>
        </b-list-group-item>

        <b-modal
          :visible="!!selectedItem"
          title="Delete item"
          @ok="onDeleteModalOk"
          @cancel="onDeleteModalCancel"
        >
          <p v-if="selectedItem">
            Are you sure you want to delete item "{{ selectedItem.name }}"?
          </p>
        </b-modal>
      </b-list-group>
    </b-card>
  </div>
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
      store: store.backend,
      selectedItem: null,
      loading: true,
      loadingHtml: '&nbsp;'
    }
  },
  computed: {
    hasAccessToApi() {
      return !this.backend.authHeader || this.store.apiKey
    }
  },
  watch: {
    hasAccessToApi(yes) {
      if (yes) {
        this.load()
      }
    }
  },
  created() {
    if (this.hasAccessToApi) {
      this.load()
    }
  },
  methods: {
    async load() {
      if (!this.loading) {
        return
      }

      setTimeout(() => {
        this.loadingHtml = 'Loading...'
      }, 50)

      await Promise.all([
        this.store.fetchMeta(),
        this.store.fetchListItems()
      ])

      this.loading = false
    },
    confirmDelete(item) {
      this.selectedItem = item
    },
    async onDeleteModalOk() {
      await this.store.deleteItem(this.selectedItem[this.store.keyName])
      this.selectedItem = null
    },
    onDeleteModalCancel() {
      this.selectedItem = null
    }
  }
}
</script>
