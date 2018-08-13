<template>
  <b-form-checkbox
    v-model="value"
    :name="field.name"
    :disabled="field.attributes.readonly"
    :state="state"
  />
</template>

<script>
export default {
  props: {
    item: {
      type: Object,
      required: true
    },
    origItem: {
      type: Object,
      required: true
    },
    field: {
      type: Object,
      required: true
    },
    scope: {
      type: String,
      default: undefined
    }
  },
  computed: {
    value: {
      get() {
        return this.item[this.field.name]
      },
      set(value) {
        this.item[this.field.name] = value
      }
    },
    origValue() {
      return this.origItem[this.field.name]
    },
    state() {
      if (this.isModified) {
        return true
      }
      return null
    },
    isModified() {
      if (!this.value && !this.origValue) {
        return false
      }
      return this.value != this.origValue
    }
  }
}
</script>
