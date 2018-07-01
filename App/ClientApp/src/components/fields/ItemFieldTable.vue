<template>
  <b-card no-body>
    <div slot="header">
      <b-button-toolbar>
        <b-button-group>
          <b-btn @click.prevent="onAddClick">Add</b-btn>
        </b-button-group>
      </b-button-toolbar>
    </div>

    <b-table
      :items="item[field.name]"
      striped
      hover
    />

    <b-modal
      ref="itemModal"
      title="Add item"
      @ok="addItem"
      @cancel="cancelModal"
    >
      <ItemField
        v-for="field in field.complexType.fields"
        :key="field.name"
        :item="modalItem"
        :field="field"
      />
    </b-modal>
  </b-card>
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
  data() {
    return {
      modalItem: {}
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
  },
  methods: {
    onAddClick() {
      this.$refs.itemModal.show()
    },
    addItem() {
      this.item[this.field.name].push(this.modalItem)
      this.modalItem = {}
    },
    cancelModal() {
      this.modalItem = {}
    }
  }
}
</script>
