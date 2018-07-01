<template>
  <div>
    <b-form-checkbox-group
      :name="field.name"
      v-model="item[field.name]"
      :options="options"
      stacked
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
    }
  },
  created() {
    this.attachValidators()
  },
  methods: {
    attachValidators() {
      this.$validator.attach({
        name: this.field.name,
        alias: this.field.attributes.displayName,
        getter: () => this.item[this.field.name],
        rules: {
          required: !!this.field.attributes.required
        }
      })
    }
  }
}
</script>
