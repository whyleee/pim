<template>
  <b-form-textarea
    v-validate="validators"
    v-model="value"
    :name="field.name"
    :readonly="readonly"
    :data-vv-as="field.attributes.displayName"
    :state="state"
    rows="3"
    no-resize
  />
</template>

<script>
export default {
  inject: ['$validator'],
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
    readonly() {
      return this.field.attributes.readonly ||
        (this.field.attributes.constant && !!this.origValue)
    },
    validators() {
      return {
        required: !!this.field.attributes.required
      }
    },
    errorSelector() {
      if (this.scope) {
        return `${this.scope}.${this.field.name}`
      }
      return this.field.name
    },
    state() {
      if (this.errors.has(this.errorSelector)) {
        return false
      }
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
