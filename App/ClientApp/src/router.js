import Vue from 'vue'
import Router from 'vue-router'
import ProductList from '@/views/ProductList.vue'
import ProductEdit from '@/views/ProductEdit.vue'

Vue.use(Router)

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      name: 'home',
      component: ProductList
    },
    {
      path: '/:id',
      name: 'product',
      component: ProductEdit
    }
  ]
})
