<template>
  <b-form-group
    :label="filter.name"
    :description="filter.description"
    horizontal
  >
    <b-form-select
      :value="selectedValue"
      :options="options"
      @input="onInput"
    />
  </b-form-group>
</template>

<script>
export default {
  props: {
    store: {
      type: Object,
      required: true
    },
    filter: {
      type: Object,
      required: true
    },
    listFilterParams: {
      type: Object,
      required: true
    },
    value: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      collection: this.store.getCollection(this.filter.refCollectionKey)
    }
  },
  computed: {
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
    options() {
      const options = (this.collection.listItems || []).map(item => ({
        text: item.name,
        value: item[this.collection.keyName]
      }))

      if (!this.filter.required) {
        options.unshift({ text: '', value: '' })
      }

      return options
    },
    selectedValue() {
      if (this.options.some(o => o.value == this.value)) {
        return this.value
      }
      if (this.options.length > 0) {
        return this.options[0].value
      }
      return null
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
  async created() {
    if (this.allFiltersSet) {
      await this.fetchItems()

      if (this.selectedValue != this.value) {
        this.$emit('input', this.selectedValue)
      }
    }
  },
  methods: {
    fetchItems() {
      return this.collection.fetchListItems(this.filterParams)
    },
    onInput(value) {
      this.$emit('input', value)
    }
  }
}
</script>
