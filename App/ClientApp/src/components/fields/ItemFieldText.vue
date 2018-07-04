<template>
  <div class="input-group">
    <span
      v-if="field.kind == 'Currency'"
      class="input-group-addon"
    >
      $
    </span>
    <b-form-input
      v-validate="validators"
      v-model="item[field.name]"
      :type="type"
      :name="field.name"
      :step="step"
      :readonly="field.attributes.readonly"
      :data-vv-as="field.attributes.displayName"
      :state="state"
    />
  </div>
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
      if (this.field.kind == 'Currency') {
        return 0.01
      }
      return undefined
    },
    validators() {
      const { range, required, regex } = this.field.attributes
      return {
        required: !!required,
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
      return null
    }
  }
}
</script>
