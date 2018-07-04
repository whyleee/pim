<template>
  <div>
    <div v-if="field.complexType && field.type == 'array'">
      <h5>{{ field.attributes.displayName }}</h5>
      <ItemFieldTable
        :item="item"
        :field="field"
      />
    </div>

    <div v-else-if="field.complexType">
      <h4>{{ field.attributes.displayName }}</h4>
      <ItemField
        v-for="field in field.complexType.fields"
        :key="field.name"
        :item="item"
        :field="field"
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
        :field="field"
        :scope="scope"
      />
    </b-form-group>
  </div>
</template>

<script>
import Vue from 'vue'
import ItemFieldCheckbox from './ItemFieldCheckbox.vue'
import ItemFieldDate from './ItemFieldDate.vue'
import ItemFieldSelectMany from './ItemFieldSelectMany.vue'
import ItemFieldSelectOne from './ItemFieldSelectOne.vue'
import ItemFieldTable from './ItemFieldTable.vue'
import ItemFieldText from './ItemFieldText.vue'
import ItemFieldTextarea from './ItemFieldTextarea.vue'

export default Vue.component('ItemField', {
  components: {
    ItemFieldCheckbox,
    ItemFieldDate,
    ItemFieldSelectMany,
    ItemFieldSelectOne,
    ItemFieldTable,
    ItemFieldText,
    ItemFieldTextarea
  },
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
      if (this.field.type == 'bool') {
        return ItemFieldCheckbox
      }
      if (this.field.type == 'datetime') {
        return ItemFieldDate
      }
      if (this.field.type == 'array') {
        return ItemFieldSelectMany
      }
      if (this.field.type == 'string') {
        if (this.field.kind == 'MultilineText') {
          return ItemFieldTextarea
        }
        if (this.field.attributes.selectOptions) {
          return ItemFieldSelectOne
        }
      }

      return ItemFieldText
    }
  }
})
</script>
