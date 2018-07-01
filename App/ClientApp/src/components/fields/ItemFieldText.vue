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
      :type="type"
      :name="field.name"
      v-model="item[field.name]"
      :step="step"
      :readonly="readonly"
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
    readonly() {
      return this.field.attributes.readonly
    },
    validators() {
      const { range } = this.field.attributes
      return {
        required: !!this.field.attributes.required,
        url: this.field.kind == 'Url' || this.field.kind == 'ImageUrl' ? [true] : false,
        between: range ? [range.min, range.max] : false
      }
    },
    state() {
      if (this.errors.has(this.field.name)) {
        return 'invalid'
      }
      return null
    }
  }
}
</script>
