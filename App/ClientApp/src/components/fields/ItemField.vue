<template>
  <b-form-group
    :horizontal="true"
    :label="field.attributes.displayName"
    :feedback="errors.first(field.name)"
    :state="state"
  >
    <component
      :is="fieldComponent"
      :item="item"
      :field="field"
    />
  </b-form-group>
</template>

<script>
import ItemFieldCheckbox from './ItemFieldCheckbox.vue'
import ItemFieldDate from './ItemFieldDate.vue'
import ItemFieldSelectMany from './ItemFieldSelectMany.vue'
import ItemFieldText from './ItemFieldText.vue'
import ItemFieldTextarea from './ItemFieldTextarea.vue'

export default {
  components: {
    ItemFieldCheckbox,
    ItemFieldDate,
    ItemFieldSelectMany,
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
    }
  },
  computed: {
    state() {
      if (this.errors.has(this.field.name)) {
        return 'invalid'
      }
      return null
    },
    fieldComponent() {
      if (this.field.type == 'bool') return ItemFieldCheckbox
      if (this.field.type == 'datetime') return ItemFieldDate
      if (this.field.type == 'array') return ItemFieldSelectMany
      if (this.field.kind == 'MultilineText') return ItemFieldTextarea
      return ItemFieldText
    }
  }
}
</script>
