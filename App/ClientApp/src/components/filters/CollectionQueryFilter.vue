<template>
  <b-form-group
    :label="filter.name"
    horizontal
  >
    <b-form-input
      :value="value"
      @change="onChange"
    />
  </b-form-group>
</template>

<script>
export default {
  props: {
    filter: {
      type: Object,
      required: true
    },
    value: {
      type: String,
      default: ''
    }
  },
  methods: {
    onChange(value) {
      this.$emit('input', value)
    }
  }
}
</script>
