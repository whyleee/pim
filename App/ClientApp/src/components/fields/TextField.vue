<template>
  <b-form-input
    v-validate="validators"
    v-model="value"
    :type="type"
    :name="field.name"
    :step="step"
    :readonly="readonly"
    :data-vv-as="field.attributes.displayName"
    :state="state"
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
        let parsed = value

        if (this.type == 'number') {
          parsed = Number(value)
        }

        this.item[this.field.name] = parsed || value
      }
    },
    origValue() {
      return this.origItem[this.field.name]
    },
    type() {
      if (this.field.type == 'integer' || this.field.type == 'number') {
        return 'number'
      }
      return 'text'
    },
    step() {
      if (this.field.type == 'integer') {
        return 1
      }
      return undefined
    },
    readonly() {
      return this.field.attributes.readonly ||
        (this.field.attributes.constant && this.origValue != null)
    },
    validators() {
      const { range, required, regex } = this.field.attributes
      return {
        required: !!required,
        numeric: this.field.type == 'integer',
        decimal: this.field.type == 'number',
        regex,
        url: this.field.kind == 'Url' || this.field.kind == 'ImageUrl' ? [true] : false,
        between: range ? [range.min, range.max] : false
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
