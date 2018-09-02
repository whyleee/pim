<template>
  <b-form-checkbox-group
    v-validate="validators"
    v-model="model"
    :name="field.name"
    :options="options"
    :disabled="readonly"
    :data-vv-as="field.attributes.displayName"
    :state="state"
    stacked
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
    model: {
      get() {
        return this.item[this.field.name]
      },
      set(value) {
        this.item[this.field.name] = value
      }
    },
    origModel() {
      return this.origItem[this.field.name]
    },
    options() {
      return this.field.attributes.selectOptions
    },
    readonly() {
      return this.field.attributes.readonly ||
        (this.field.attributes.constant && this.origModel.length > 0)
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
      if (this.model.length != this.origModel.length) {
        return true
      }

      function isModified(value, origValue) {
        if (!value && !origValue) {
          return false
        }
        return value != origValue
      }

      for (let i = 0; i < this.model.length; i += 1) {
        if (isModified(this.model[i], this.origModel[i])) {
          return true
        }
      }

      return false
    }
  }
}
</script>
