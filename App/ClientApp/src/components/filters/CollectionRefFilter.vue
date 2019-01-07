<template>
  <b-form-group
    :label="filter.name"
    :description="filter.description"
    horizontal
  >
    <multiselect
      :value="selectedValue"
      :options="options.map(opt => opt.value)"
      :custom-label="val => options.find(opt => opt.value == val).text"
      :multiple="filter.multiple"
      label="text"
      @input="onInput"
    />
  </b-form-group>
</template>

<script>
import Multiselect from 'vue-multiselect'

export default {
  components: {
    Multiselect
  },
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
      type: [String, Number, Array],
      default: ''
    }
  },
  data() {
    return {
      collection: this.store.getCollection(this.filter.refCollectionKey),
      origFilterParams: null
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
    filterParams: {
      handler(value) {
        if (this.allFiltersSet && JSON.stringify(value) != JSON.stringify(this.origFilterParams)) {
          this.origFilterParams = value
          this.fetchItems()
        }
      }
    }
  },
  created() {
    if (this.allFiltersSet) {
      this.fetchItems()
    }
  },
  methods: {
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
