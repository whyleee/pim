<template>
  <div>
    <b-form-textarea
      v-validate="validators"
      :name="field.name"
      v-model="item[field.name]"
      :plaintext="readonly"
      :data-vv-as="field.attributes.displayName"
      :state="state"
      rows="3"
      no-resize
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
    readonly() {
      return this.field.attributes.readonly
    },
    validators() {
      return {
        required: !!this.field.attributes.required
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
