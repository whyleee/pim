import axios from 'axios'
import config from '@/config'

function getBackend(key) {
  return config.backends.find(b => b.key == key)
}

export default {
  meta: {
    url(backend) {
      return getBackend(backend).meta.url
    },
    async get(backend, itemType) {
      const response = await axios.get(`${this.url(backend)}/${itemType}`)
      return response.data
    }
  },
  data: {
    url(backend) {
      return getBackend(backend).data.url
    },
    async get(backend) {
      const response = await axios.get(this.url(backend))
      const { collectionItemsProperty } = getBackend(backend).data

      return collectionItemsProperty
        ? response.data[collectionItemsProperty]
        : response.data
    },
    async getById(backend, id) {
      const response = await axios.get(`${this.url(backend)}/${id}`)
      return response.data
    },
    post(backend, item) {
      return axios.post(this.url(backend), item)
    },
    put(backend, id, item) {
      return axios.put(`${this.url(backend)}/${id}`, item)
    },
    delete(backend, id) {
      return axios.delete(`${this.url(backend)}/${id}`)
    }
  }
}
