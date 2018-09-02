import Vue from 'vue'
import Router from 'vue-router'
import config from '@/config.json'

import Home from '@/views/Home.vue'
import ItemList from '@/views/ItemList.vue'
import ItemEdit from '@/views/ItemEdit.vue'

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
    component: ItemList,
    props: {
      backend
    }
  }, {
    path: `/${backend.key}/:id`,
    name: `${backend.key}-edit`,
    component: ItemEdit,
    props: {
      backend
    }
  })
})

export default new Router({
  mode: 'history',
  routes
})
