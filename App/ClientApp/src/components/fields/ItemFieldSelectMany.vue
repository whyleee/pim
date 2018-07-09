<template>
  <b-form-checkbox-group
    v-validate="validators"
    v-model="item[field.name]"
    :name="field.name"
    :options="options"
    :disabled="field.attributes.readonly"
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
    options() {
      return this.field.attributes.selectOptions
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
      return null
    }
  }
}
</script>
