<template>
  <div>
    <b-card no-body>
      <div slot="header">
        <b-row align-v="center">
          <b-col>
            <h1>{{ backend.title }}</h1>
          </b-col>
          <b-col class="text-right">
            <b-button
              :to="{ name: `${backend.key}-edit`, params: { collection: collectionKey, id: 'new' }}"
              class="ml-1"
            >
              New
            </b-button>
          </b-col>
        </b-row>
      </div>

      <b-card-body v-if="loading">
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
          v-for="item in collection.listItems"
          :key="item[collection.keyName]"
        >
          <b-link :to="{ name: `${backend.key}-edit`, params: { collection: collectionKey, id: item[collection.keyName] }}">
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

export default {
  props: {
    backend: {
      type: Object,
      required: true
    },
    collectionKey: {
      type: String,
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
    collection() {
      return this.store.collection
    }
  },
  created() {
    this.load()
  },
  methods: {
    async load() {
      setTimeout(() => {
        this.loadingHtml = 'Loading...'
      }, 50)

      await this.store.fetchMeta()

      this.store.setCollection(this.collectionKey)
      await this.collection.fetchListItems()

      this.loading = false
    },
    confirmDelete(item) {
      this.selectedItem = item
    },
    async onDeleteModalOk() {
      const id = this.selectedItem[this.collection.keyName]
      await this.collection.deleteItem(id)
      this.selectedItem = null
    },
    onDeleteModalCancel() {
      this.selectedItem = null
    }
  }
}
</script>
