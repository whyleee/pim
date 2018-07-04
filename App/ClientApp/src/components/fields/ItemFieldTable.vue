<template>
  <b-card
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
      :meta="field.complexType"
      @ok="saveModalItem"
      @cancel="resetEditingRow"
    />
  </b-card>
</template>

<script>
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
    field: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      modalItem: {},
      editingRowIndex: -1
    }
  },
  computed: {
    model() {
      return this.item[this.field.name]
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
    }
  },
  methods: {
    onAddClick() {
      this.modalItem = {}
      this.resetEditingRow()
      this.$refs.itemModal.show()
    },
    onEditClick(item, index) {
      this.modalItem = Object.assign({}, item)
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
    }
  }
}
</script>
