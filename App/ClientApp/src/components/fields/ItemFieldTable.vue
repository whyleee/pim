<template>
  <b-card
    :border-variant="isModified ? 'success' : null"
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
      <div
        v-for="field in field.complexType.fields"
        slot-scope="{ value }"
        :key="field.name"
        :slot="field.name"
      >
        <a
          v-if="field.type == 'string' && field.kind == 'Url'"
          :href="value"
        >
          {{ value }}
        </a>

        <img
          v-else-if="field.type == 'string' && field.kind == 'ImageUrl'"
          :src="value"
          class="img-thumbnail"
        >

        <template v-else-if="field.type == 'bool'">
          <b-form-checkbox
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
      </div>

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

    <ItemFieldTableModal
      ref="itemModal"
      :title="modalTitle"
      :item="modalItem"
      :orig-item="origModalItem"
      :meta="field.complexType"
      @ok="saveModalItem"
      @cancel="resetEditingRow"
    />
  </b-card>
</template>

<script>
import { format as formatDate } from 'date-fns/esm'
import ItemFieldTableModal from './ItemFieldTableModal.vue'

export default {
  components: {
    ItemFieldTableModal
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
      this.modalItem = this.createEmptyItem()
      this.origModalItem = this.createEmptyItem()
      this.resetEditingRow()
      this.$refs.itemModal.show()
    },
    onEditClick(item, index) {
      this.modalItem = Object.assign({}, item)
      this.origModalItem = Object.assign({}, item)
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
    createEmptyItem() {
      return this.field.complexType.fields.reduce((item, field) => {
        item[field.name] = null
        return item
      }, {})
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
