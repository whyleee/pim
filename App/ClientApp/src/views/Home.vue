<template>
  <b-row>
    <b-col>
      <h1>Products</h1>
      <b-button-toolbar>
        <b-button-group>
          <b-btn :to="{ name: 'product', params: { id: 'new' }}">New</b-btn>
        </b-button-group>
      </b-button-toolbar>
      <ProductList/>
    </b-col>
  </b-row>
</template>

<script>
import ProductList from '@/components/ProductList.vue'

export default {
  components: {
    ProductList
  }
}
</script>
