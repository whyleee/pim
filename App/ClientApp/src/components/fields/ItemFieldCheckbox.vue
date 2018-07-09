<template>
  <b-form-checkbox
    v-model="item[field.name]"
    :name="field.name"
    :disabled="field.attributes.readonly"
  />
</template>

<script>
export default {
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
  }
}
</script>
