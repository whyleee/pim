<template>
  <div>
    <h1>{{ backend.title }}</h1>
    <b-card no-body>
      <div slot="header">
        <b-button-toolbar>
          <b-button-group>
            <b-btn :to="{ name: `${backend.key}-edit`, params: { id: 'new' }}">New</b-btn>
          </b-button-group>
        </b-button-toolbar>
      </div>

      <b-list-group flush>
        <b-list-group-item
          v-for="item in items"
          :key="item[keyName]"
        >
          <b-link :to="{ name: `${backend.key}-edit`, params: { id: item[keyName] }}">
            {{ item.name }}
          </b-link>
          <b-link
            class="float-right"
            @click.stop="confirmDelete(item)"
          >
            Delete
          </b-link>
        </b-list-group-item>

        <b-modal
          :visible="!!selectedItem"
          title="Delete item"
          @ok="onDeleteModalOk"
          @cancel="onDeleteModalCancel"
        >
          <p v-if="selectedItem">
            Are you sure you want to delete item "{{ selectedItem.name }}"?
          </p>
        </b-modal>
      </b-list-group>
    </b-card>
  </div>
</template>

<script>
import api from '@/lib/api'
import store from '@/store'

export default {
  props: {
    backend: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      meta: {
        fields: []
      },
      items: [],
      selectedItem: null
    }
  },
  computed: {
    keyName() {
      const keyField = this.meta.fields.find(field =>
        Object.entries(field.attributes)
          .some(([key, value]) => key == 'key' && value))

      return keyField ? keyField.name : 'id'
    }
  },
  async created() {
    await Promise.all([
      this.fetchMeta(),
      this.fetchData()
    ])
  },
  methods: {
    async fetchMeta() {
      this.meta = await store.fetchMeta(this.backend.key, 'item')
    },
    async fetchData() {
      this.items = await api.data.get(this.backend.key)
    },
    confirmDelete(item) {
      this.selectedItem = item
    },
    async onDeleteModalOk() {
      await api.data.delete(this.backend.key, this.selectedItem[this.keyName])
      const index = this.items.findIndex(i =>
        i[this.keyName] == this.selectedItem[this.keyName])

      this.items.splice(index, 1)
      this.selectedItem = null
    },
    onDeleteModalCancel() {
      this.selectedItem = null
    }
  }
}
</script>
