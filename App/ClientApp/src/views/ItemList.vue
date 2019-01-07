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
              v-if="!readonly"
              :to="{
                name: `${backend.key}-edit`,
                params: { collection: collectionKey, id: 'new' },
                query: itemEditQuery
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
            <CollectionQueryFilter
              v-if="filter.type == 'query'"
              :filter="filter"
              v-model="filterParams[filter.key]"
            />
            <CollectionRefFilter
              v-else-if="filter.type == 'ref'"
              :store="store"
              :filter="filter"
              :list-filter-params="filterParams"
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
          :key="collection.getKey(item)"
        >
          <template v-if="readonly || constant">
            {{ item[collection.titleName] || `[${collection.getKey(item)}]` }}
          </template>
          <b-link
            v-else
            :to="{
              name: `${backend.key}-edit`,
              params: { collection: collectionKey, id: collection.getKey(item) },
              query: itemEditQuery
            }"
          >
            {{ item[collection.titleName] || `[${collection.getKey(item)}]` }}
          </b-link>
          <b-link
            v-if="!readonly && !constant"
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
            Are you sure you want to delete item "
            {{ selectedItem[collection.titleName] || collection.getKey(selectedItem) }}"?
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
import CollectionQueryFilter from '@/components/filters/CollectionQueryFilter.vue'
import CollectionRefFilter from '@/components/filters/CollectionRefFilter.vue'

export default {
  components: {
    CollectionQueryFilter,
    CollectionRefFilter
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
    readonly() {
      if (!this.collection) {
        return true
      }
      return this.collection.meta.readonly
    },
    constant() {
      if (!this.collection) {
        return true
      }
      return this.collection.meta.constant
    },
    filters() {
      if (!this.collection) {
        return []
      }
      return this.collection.meta.filters
    },
    allFiltersSet() {
      return this.filters
        .filter(filter => filter.required)
        .map(filter => this.filterParams[filter.key])
        .every(param => param)
    },
    itemEditQuery() {
      return this.filters
        .filter(filter => filter.required)
        .reduce((query, filter) => {
          query[filter.key] = this.filterParams[filter.key]
          return query
        }, {})
    }
  },
  watch: {
    filterParams: {
      deep: true,
      handler() {
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
      const id = this.collection.getKey(this.selectedItem)
      await this.collection.deleteItem(id, this.filterParams)
      this.selectedItem = null
    },
    onDeleteModalCancel() {
      this.selectedItem = null
    }
  }
}
</script>
