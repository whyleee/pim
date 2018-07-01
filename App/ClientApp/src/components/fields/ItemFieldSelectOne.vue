<template>
  <div>
    <b-form-select
      v-validate="validators"
      :name="field.name"
      v-model="item[field.name]"
      :options="options"
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
    options() {
      return this.field.attributes.selectOptions
    },
    validators() {
      return {
        required: !!this.field.attributes.required
      }
    }
  }
}
</script>
