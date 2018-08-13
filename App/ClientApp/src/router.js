import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/views/Home.vue'
import ProductEdit from '@/components/ProductEdit.vue'

Vue.use(Router)

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/:id',
      name: 'product',
      component: ProductEdit
    }
  ]
})
