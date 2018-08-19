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
          v-for="product in products"
          :key="product.id"
        >
          <b-link :to="{ name: `${backend.key}-edit`, params: { id: product.id }}">
            {{ product.name }}
          </b-link>
          <b-link
            class="float-right"
            @click.stop="confirmDelete(product)"
          >
            Delete
          </b-link>
        </b-list-group-item>

        <b-modal
          :visible="!!selectedProduct"
          title="Delete item"
          @ok="onDeleteModalOk"
          @cancel="onDeleteModalCancel"
        >
          <p v-if="selectedProduct">
            Are you sure you want to delete product "{{ selectedProduct.name }}"?
          </p>
        </b-modal>
      </b-list-group>
    </b-card>
  </div>
</template>

<script>
import api from '@/lib/api'

export default {
  props: {
    backend: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      products: [],
      selectedProduct: null
    }
  },
  async created() {
    await this.fetchData()
  },
  methods: {
    async fetchData() {
      this.products = await api.data.get(this.backend.key)
    },
    confirmDelete(product) {
      this.selectedProduct = product
    },
    async onDeleteModalOk() {
      await api.data.delete(this.backend.key, this.selectedProduct.id)
      const index = this.products.findIndex(p => p.id == this.selectedProduct.id)
      this.products.splice(index, 1)
      this.selectedProduct = null
    },
    onDeleteModalCancel() {
      this.selectedProduct = null
    }
  }
}
</script>
