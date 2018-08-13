<template>
  <b-list-group flush>
    <b-list-group-item
      v-for="product in products"
      :key="product.id"
    >
      <b-link :to="{ name: 'product', params: { id: product.id }}">
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
</template>

<script>
import api from '@/lib/api'

export default {
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
      const response = await api.products.get()
      this.products = response.data
    },
    confirmDelete(product) {
      this.selectedProduct = product
    },
    async onDeleteModalOk() {
      await api.products.delete(this.selectedProduct.id)
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
