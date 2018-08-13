import axios from 'axios'

const apiUrl = '/api'

export default {
  meta: {
    url: `${apiUrl}/meta`,
    get(itemType) {
      return axios.get(`${this.url}/${itemType}`)
    }
  },
  products: {
    baseUrl: `${apiUrl}/products`,
    get() {
      return axios.get(this.baseUrl)
    },
    getById(id) {
      return axios.get(`${this.baseUrl}/${id}`)
    },
    post(product) {
      return axios.post(this.baseUrl, product)
    },
    put(id, product) {
      return axios.put(`${this.baseUrl}/${id}`, product)
    },
    delete(id) {
      return axios.delete(`${this.baseUrl}/${id}`)
    }
  }
}
