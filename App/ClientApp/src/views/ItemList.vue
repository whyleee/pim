<template>
  <div>
    <b-card no-body>
      <div slot="header">
        <b-row align-v="center">
          <b-col>
            <h1 v-html="title"/>
          </b-col>
          <b-col class="text-right">
            <b-button
              :to="{
                name: `${backend.key}-edit`,
                params: { collection: collectionKey, id: 'new' },
                query: filterParams
              }"
              class="ml-1"
            >
              New
            </b-button>
          </b-col>
        </b-row>
      </div>

      <b-card-body v-if="filters.length">
        <b-row>
          <b-col
            v-for="filter in filters"
            :key="filter.key"
            md="6"
          >
            <CollectionFilter
              :store="store"
              :filter="filter"
              v-model="filterParams[filter.key]"
            />
          </b-col>
        </b-row>
      </b-card-body>

      <b-card-body v-if="loading">
        <div
          class="text-center lead"
          v-html="loadingHtml"
        />
      </b-card-body>

      <b-list-group
        v-else-if="collection.listItems.length"
        flush
      >
        <b-list-group-item
          v-for="item in collection.listItems"
          :key="item[collection.keyName]"
        >
          <b-link
            :to="{
              name: `${backend.key}-edit`,
              params: { collection: collectionKey, id: item[collection.keyName] },
              query: filterParams
            }"
          >
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

      <b-card-body v-else>
        <div class="text-center lead">No items</div>
      </b-card-body>
    </b-card>
  </div>
</template>

<script>
import store from '@/store'
import CollectionFilter from '@/components/CollectionFilter.vue'

export default {
  components: {
    CollectionFilter
  },
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
      loadingHtml: '&nbsp;',
      filterParams: {}
    }
  },
  computed: {
    collection() {
      return this.store.collection
    },
    title() {
      if (!this.collection) {
        return '&nbsp;'
      }
      return this.collection.meta.name
    },
    filters() {
      if (!this.collection) {
        return []
      }
      return this.collection.meta.filters
    },
    allFiltersSet() {
      return Object.values(this.filterParams).every(param => param)
    }
  },
  watch: {
    filterParams: {
      deep: true,
      async handler() {
        if (this.allFiltersSet) {
          this.fetchItems()
        }
      }
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

      this.filters.forEach((filter) => {
        this.$set(this.filterParams, filter.key, null)
      })

      if (!this.filters.length) {
        await this.fetchItems()
      }
    },
    async fetchItems() {
      this.loading = true
      await this.collection.fetchListItems(this.filterParams)
      this.loading = false
    },
    confirmDelete(item) {
      this.selectedItem = item
    },
    async onDeleteModalOk() {
      const id = this.selectedItem[this.collection.keyName]
      await this.collection.deleteItem(id, this.filterParams)
      this.selectedItem = null
    },
    onDeleteModalCancel() {
      this.selectedItem = null
    }
  }
}
</script>
