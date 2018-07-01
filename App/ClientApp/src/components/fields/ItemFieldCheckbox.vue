<template>
  <div>
    <b-form-checkbox
      :name="field.name"
      v-model="item[field.name]"
      :disabled="readonly"
    />
  </div>
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
    }
  },
  computed: {
    readonly() {
      return this.field.attributes.readonly
    }
  }
}
</script>
