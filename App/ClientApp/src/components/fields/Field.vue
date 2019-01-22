<template>
  <div v-if="field.complexType && field.type == 'array'">
    <h5>{{ field.attributes.displayName }}</h5>
    <TableField
      :item="item"
      :orig-item="origItem"
      :field="field"
    />
  </div>

  <div v-else-if="field.complexType">
    <h5>{{ field.attributes.displayName }}</h5>
    <Field
      v-for="innerField in field.complexType.fields"
      :key="innerField.name"
      :item="item[field.name]"
      :orig-item="origItem[field.name]"
      :field="innerField"
      :scope="scope"
    />
  </div>

  <b-form-group
    v-else
    :label="field.attributes.displayName"
    :feedback="errors.first(errorSelector)"
    :state="state"
    horizontal
  >
    <component
      :is="fieldComponent"
      :item="item"
      :orig-item="origItem"
      :field="field"
      :scope="scope"
    />
  </b-form-group>
</template>

<script>
import Vue from 'vue'
import CheckboxField from './CheckboxField.vue'
import DateField from './DateField.vue'
import RefField from './RefField.vue'
import SelectManyField from './SelectManyField.vue'
import SelectOneField from './SelectOneField.vue'
import TableField from './TableField.vue'
import TextField from './TextField.vue'
import TextareaField from './TextareaField.vue'

export default Vue.component('Field', {
  components: {
    CheckboxField,
    DateField,
    RefField,
    SelectManyField,
    SelectOneField,
    TableField,
    TextField,
    TextareaField
  },
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
    },
    fieldComponent() {
      if (this.field.ref) {
        return RefField
      }
      if (this.field.type == 'bool') {
        return CheckboxField
      }
      if (this.field.type == 'datetime') {
        return DateField
      }
      if (this.field.type == 'array'
        && this.field.attributes.selectOptions) {
        return SelectManyField
      }
      if (this.field.type == 'string') {
        if (this.field.kind == 'MultilineText') {
          return TextareaField
        }
        if (this.field.attributes.selectOptions) {
          return SelectOneField
        }
      }

      return TextField
    }
  }
})
</script>
