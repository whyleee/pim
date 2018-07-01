<template>
  <b-row>
    <b-col>
      <ProductEdit :product-id="id != 'new' ? id : undefined"/>
    </b-col>
  </b-row>
</template>

<script>
import ProductEdit from '@/components/ProductEdit.vue'

export default {
  components: {
    ProductEdit
  },
  props: {
    id: {
      type: String,
      required: true
    }
  }
}
</script>
