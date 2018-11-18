<template>
  <b-form-group
    :label="filter.name"
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
    options() {
      return (this.collection.listItems || []).map(item => ({
        text: item.name,
        value: item[this.collection.keyName]
      }))
    },
    selectedValue() {
      return this.value || (this.options.length ? this.options[0].value : null)
    }
  },
  async created() {
    await this.collection.fetchListItems()

    if (this.selectedValue != this.value) {
      this.$emit('input', this.selectedValue)
    }
  },
  methods: {
    onInput(value) {
      this.$emit('input', value)
    }
  }
}
</script>
