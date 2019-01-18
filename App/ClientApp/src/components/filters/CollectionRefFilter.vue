<template>
  <b-form-group
    :label="filter.name"
    :description="filter.description"
    horizontal
  >
    <multiselect
      v-if="hasAccessToApi"
      :value="selectedValue"
      :options="options"
      :custom-label="getLabel"
      :group-values="isGrouped ? 'items' : undefined"
      :group-label="isGrouped ? 'name' : undefined"
      :multiple="filter.multiple"
      label="text"
      @input="onInput"
    />

    <Authorize
      v-else
      :backend="backend"
      :button-text="`Authorize ${backend.config.title}`"
    />
  </b-form-group>
</template>

<script>
import store from '@/store'
import Authorize from '@/components/Authorize.vue'
import Multiselect from 'vue-multiselect'

export default {
  components: {
    Authorize,
    Multiselect
  },
  props: {
    filter: {
      type: Object,
      required: true
    },
    listFilterParams: {
      type: Object,
      required: true
    },
    value: {
      type: [String, Number, Array],
      default: ''
    }
  },
  data() {
    const refs = Array.isArray(this.filter.ref) ? this.filter.ref : [this.filter.ref]
    return {
      refs,
      backends: refs.map(ref => this.getBackend(ref)),
      // TODO: hacks to make `collections` and `groups` reactive on store changes
      resolvedListItems: [],
      origFilterParams: null
    }
  },
  computed: {
    notAuthorizedBackends() {
      return this.backends
        .filter(backend => backend.config.authHeader && !backend.apiKey)
    },
    hasAccessToApi() {
      return this.notAuthorizedBackends.length == 0
    },
    filterParams() {
      return this.refs.reduce((hash, ref) => {
        hash[this.getRefKey(ref)] = this.getFilterParams(ref.filters)
        return hash
      }, {})
    },
    allFiltersSet() {
      return Object.values(this.filterParams)
        .every(refParams => Object.values(refParams)
          .every(val => val && !val.startsWith('{') && !val.endsWith('}')))
    },
    filterParamsModified() {
      return JSON.stringify(this.filterParams) != JSON.stringify(this.origFilterParams)
    },
    collections() {
      return this.refs.reduce((hash, ref) => {
        const backend = this.getBackend(ref)
        hash[this.getRefKey(ref)] = backend.getCollection(ref.collectionKey)
        return hash
      }, {})
    },
    groups() {
      // eslint-disable-next-line no-unused-vars
      const reactivityTrigger = this.resolvedListItems.length

      return this.refs
        .map(ref => this.collections[this.getRefKey(ref)])
        .filter(collection => collection)
        .map(collection => ({
          name: collection.meta.name,
          getKey: collection.getKey,
          getTitle: collection.getTitle,
          items: collection.listItems || []
        }))
    },
    isGrouped() {
      return this.groups.length > 1
    },
    options() {
      if (this.groups.length == 0) {
        return []
      }

      let options

      if (!this.isGrouped) {
        options = this.groups[0].items
          .map(item => this.groups[0].getKey(item))
      } else {
        options = this.groups.map(group => ({
          name: group.name,
          items: group.items
            .map(item => group.getKey(item))
        }))
      }

      if (!this.filter.required && !this.filter.multiple) {
        options.unshift('')
      }

      return options
    },
    labels() {
      const labels = {}

      this.groups.forEach((group) => {
        group.items.forEach((item) => {
          labels[group.getKey(item)] = group.getTitle(item)
        })
      })

      return labels
    },
    selectedValue() {
      const values = Array.isArray(this.value) ? this.value : [this.value]
      const optionValues = this.isGrouped
        ? this.options.reduce((arr, group) => arr.concat(group.items), [])
        : this.options

      if (values.every(val => optionValues.some(opt => opt == val))) {
        return this.value
      }

      if (optionValues.length > 0 && !this.filter.multiple) {
        return optionValues[0]
      }

      return null
    }
  },
  watch: {
    hasAccessToApi(yes) {
      if (yes) {
        this.fetchDataIfNeeded()
      }
    },
    filterParams: {
      handler() {
        this.fetchDataIfNeeded()
      }
    }
  },
  created() {
    this.fetchDataIfNeeded()
  },
  methods: {
    async fetchDataIfNeeded() {
      if (!this.hasAccessToApi) {
        return
      }
      if (!this.allFiltersSet || !this.filterParamsModified) {
        return
      }

      this.origFilterParams = this.filterParams

      const backendMetaPromises = this.backends
        .filter(backend => !backend.meta && backend.config.key != store.backend.config.key)
        .map(backend => backend.fetchMeta())

      if (backendMetaPromises.length > 0) {
        await Promise.all(backendMetaPromises)
      }

      const collectionPromises = this.refs.map((ref) => {
        const backend = this.getBackend(ref)
        const collection = backend.getCollection(ref.collectionKey)

        return collection.fetchListItems(this.filterParams[this.getRefKey(ref)])
      })

      await Promise.all(collectionPromises)
      this.resolvedListItems.push(...collectionPromises)

      if (this.selectedValue != this.value) {
        this.$emit('input', this.selectedValue)
      }
    },
    getFilterParams(filters) {
      return Object.entries(filters)
        .reduce((hash, [key, value]) => {
          hash[key] = value

          Object.entries(this.listFilterParams)
            .filter(([, fpValue]) => fpValue)
            .forEach(([fpKey, fpValue]) => {
              hash[key] = hash[key].replace(`{${fpKey}}`, fpValue)
            })

          return hash
        }, {})
    },
    getBackend(ref) {
      return ref.backendKey ? store.getBackend(ref.backendKey) : store.backend
    },
    getRefKey(ref) {
      const backend = this.getBackend(ref)
      return `${backend.config.key}:${ref.collectionKey}`
    },
    getLabel(key) {
      return this.labels[key] || key
    },
    onInput(value) {
      this.$emit('input', value)
    }
  }
}
</script>
