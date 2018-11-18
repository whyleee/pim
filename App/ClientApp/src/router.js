import Vue from 'vue'
import Router from 'vue-router'
import config from '@/config.json'

import Home from '@/views/Home.vue'
import Backend from '@/views/Backend.vue'
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
    name: backend.key,
    component: Backend,
    props: {
      backend
    },
    children: [
      {
        path: ':collection',
        name: `${backend.key}-list`,
        component: ItemList,
        props(route) {
          return {
            backend,
            collectionKey: route.params.collection
          }
        }
      },
      {
        path: ':collection/:id',
        name: `${backend.key}-edit`,
        component: ItemEdit,
        props(route) {
          return {
            backend,
            collectionKey: route.params.collection,
            id: route.params.id
          }
        }
      }
    ]
  })
})

export default new Router({
  mode: 'history',
  routes
})
