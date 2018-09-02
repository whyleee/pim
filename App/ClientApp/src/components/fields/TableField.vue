<template>
  <div>
    <b-card
      :border-variant="stateVariant"
      class="mb-3"
      no-body
    >
      <div slot="header">
        <b-btn
          size="sm"
          @click.prevent="onAddClick"
        >
          Add
        </b-btn>
      </div>

      <b-table
        v-if="model.length"
        :items="model"
        :fields="columns"
        striped
        hover
      >
        <template
          v-for="field in field.complexType.fields"
          slot-scope="{ value }"
          :slot="field.name"
        >
          <a
            v-if="field.type == 'string' && field.kind == 'Url'"
            :key="field.name"
            :href="value"
          >
            {{ value }}
          </a>

          <img
            v-else-if="field.type == 'string' && field.kind == 'ImageUrl'"
            :key="field.name"
            :src="value"
            class="img-thumbnail"
          >

          <template v-else-if="field.type == 'bool'">
            <b-form-checkbox
              :key="field.name"
              :checked="value"
              disabled
            />
          </template>

          <template v-else-if="field.type == 'datetime'">
            {{ formatDate(value, 'MMMM D, YYYY') }}
          </template>

          <template v-else-if="field.type == 'array'">
            {{ getSelectedOptions(field, value).map(x => x.text).join(', ') }}
          </template>

          <template v-else>{{ value }}</template>
        </template>

        <template
          slot="actions"
          slot-scope="row"
        >
          <div class="text-right">
            <b-btn
              size="sm"
              class="mr-1"
              @click.prevent="onEditClick(row.item, row.index)"
            >
              Edit
            </b-btn>
            <b-btn
              size="sm"
              @click.prevent="removeItem(row.index)"
            >
              Remove
            </b-btn>
          </div>
        </template>
      </b-table>

      <TableFieldModal
        ref="itemModal"
        :title="modalTitle"
        :item="modalItem"
        :orig-item="origModalItem"
        :meta="field.complexType"
        @ok="saveModalItem"
        @cancel="resetEditingRow"
      />
    </b-card>

    <input
      v-validate="validators"
      v-model="model"
      :name="field.name"
      :data-vv-as="field.attributes.displayName"
      type="hidden"
    >
    <div
      v-if="errorMessage"
      class="invalid-feedback d-block"
    >
      {{ errorMessage }}
    </div>
  </div>
</template>

<script>
import { format as formatDate } from 'date-fns/esm'
import TableFieldModal from './TableFieldModal.vue'

export default {
  inject: ['$validator'],
  components: {
    TableFieldModal
  },
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
    }
  },
  data() {
    return {
      modalItem: {},
      origModalItem: {},
      editingRowIndex: -1
    }
  },
  computed: {
    model() {
      return this.item[this.field.name]
    },
    origModel() {
      return this.origItem[this.field.name]
    },
    columns() {
      return this.field.complexType.fields.map(field => ({
        key: field.name,
        label: field.attributes.displayName
      })).concat([{
        key: 'actions',
        label: ''
      }])
    },
    modalTitle() {
      return this.editingRowIndex >= 0 ? 'Edit' : 'Add'
    },
    validators() {
      return {
        required: !!this.field.attributes.required
      }
    },
    errorMessage() {
      return this.errors.first(this.field.name)
    },
    stateVariant() {
      if (this.errors.has(this.field.name)) {
        return 'danger'
      }
      if (this.isModified) {
        return 'success'
      }
      return null
    },
    isModified() {
      if (this.model.length != this.origModel.length) {
        return true
      }

      function isModified(value, origValue) {
        if (!value && !origValue) {
          return false
        }
        return value != origValue
      }

      for (let i = 0; i < this.model.length; i += 1) {
        const item = this.model[i]
        const origItem = this.origModel[i]
        const keys = Object.keys(item)

        for (let j = 0; j < keys.length; j += 1) {
          const key = keys[j]

          if (isModified(item[key], origItem[key])) {
            return true
          }
        }
      }

      return false
    }
  },
  methods: {
    formatDate,
    getSelectedOptions(field, value) {
      if (!value) {
        return []
      }

      const { selectOptions } = field.attributes
      return value
        .map(val => selectOptions.find(opt => opt.value == val))
    },
    onAddClick() {
      this.origModalItem = this.field.complexType.defaultItem
      this.modalItem = this.deepClone(this.origModalItem)
      this.resetEditingRow()
      this.$refs.itemModal.show()
    },
    onEditClick(item, index) {
      this.origModalItem = item
      this.modalItem = this.deepClone(item)
      this.editingRowIndex = index
      this.$refs.itemModal.show()
    },
    removeItem(index) {
      this.model.splice(index, 1)
    },
    saveModalItem() {
      if (this.editingRowIndex >= 0) {
        this.model.splice(this.editingRowIndex, 1, this.modalItem)
      } else {
        this.model.push(this.modalItem)
      }

      this.resetEditingRow()
    },
    resetEditingRow() {
      this.editingRowIndex = -1
    },
    deepClone(obj) {
      return JSON.parse(JSON.stringify(obj))
    }
  }
}
</script>

<style scoped>
.img-thumbnail {
  max-width: 100px;
  max-height: 100px;
}
</style>
