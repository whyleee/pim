<template>
  <b-form-group
    :label="filter.name"
    :description="filter.description"
    horizontal
  >
    <multiselect
      v-if="hasAccessToApi"
      :value="selectedValue"
      :options="options.map(opt => opt.value)"
      :custom-label="val => options.find(opt => opt.value == val).text"
      :multiple="filter.multiple"
      label="text"
      @input="onInput"
    />

    <Authorize
      v-else
      :backend="backend"
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
    const backend = this.filter.refBackendKey
      ? store.getBackend(this.filter.refBackendKey)
      : store.backend

    return {
      backend,
      collection: backend.getCollection(this.filter.refCollectionKey),
      origFilterParams: null
    }
  },
  computed: {
    requiresApiKey() {
      return this.backend.config.authHeader
    },
    hasAccessToApi() {
      return !this.requiresApiKey || !!this.backend.apiKey
    },
    filterParams() {
      return Object.entries(this.filter.filters)
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
    allFiltersSet() {
      return Object.values(this.filterParams)
        .every(val => val && !val.startsWith('{') && !val.endsWith('}'))
    },
    filterParamsModified() {
      return JSON.stringify(this.filterParams) != JSON.stringify(this.origFilterParams)
    },
    options() {
      if (this.collection == null) {
        return []
      }

      const options = (this.collection.listItems || []).map(item => ({
        text: item[this.collection.titleName] || this.collection.getKey(item),
        value: this.collection.getKey(item)
      }))

      if (!this.filter.required && !this.filter.multiple) {
        options.unshift({ text: '', value: '' })
      }

      return options
    },
    selectedValue() {
      const values = Array.isArray(this.value) ? this.value : [this.value]

      if (values.every(val => this.options.some(opt => opt.value == val))) {
        return this.value
      }

      if (this.options.length > 0 && !this.filter.multiple) {
        return this.options[0].value
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

      if (!this.backend.meta && this.backend.config.key != store.backend.config.key) {
        await this.backend.fetchMeta()
        this.collection = this.backend.getCollection(this.filter.refCollectionKey)
      }

      if (this.allFiltersSet && this.filterParamsModified) {
        this.origFilterParams = this.filterParams
        await this.fetchItems()
      }
    },
    async fetchItems() {
      await this.collection.fetchListItems(this.filterParams)

      if (this.selectedValue != this.value) {
        this.$emit('input', this.selectedValue)
      }
    },
    onInput(value) {
      this.$emit('input', value)
    }
  }
}
</script>
