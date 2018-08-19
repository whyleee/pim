import Vue from 'vue'
import Router from 'vue-router'
import config from '@/config'

import Home from '@/views/Home.vue'
import ProductList from '@/views/ProductList.vue'
import ProductEdit from '@/views/ProductEdit.vue'

Vue.use(Router)

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home
  }
]

config.backends.forEach((backend) => {
  routes.push({
    path: `/${backend.key}`,
    name: `${backend.key}-list`,
    component: ProductList,
    props: {
      backend
    }
  }, {
    path: `/${backend.key}/:id`,
    name: `${backend.key}-edit`,
    component: ProductEdit,
    props: {
      backend
    }
  })
})

export default new Router({
  mode: 'history',
  routes
})
