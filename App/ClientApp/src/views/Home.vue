<template>
  <b-row>
    <b-col>
      <h1>Products</h1>
      <b-card no-body>
        <div slot="header">
          <b-button-toolbar>
            <b-button-group>
              <b-btn :to="{ name: 'product', params: { id: 'new' }}">New</b-btn>
            </b-button-group>
          </b-button-toolbar>
        </div>

        <ProductList/>
      </b-card>
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
