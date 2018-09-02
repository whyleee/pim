import axios from 'axios'

export default {
  create(backend) {
    const http = axios.create()

    return {
      getApiKey() {
        return http.defaults.headers.common[backend.authHeader]
      },
      setApiKey(apiKey) {
        if (!backend.authHeader) {
          throw new Error(`Backend ${backend.key} doesn't require api key`)
        }

        http.defaults.headers.common[backend.authHeader] = apiKey
      },
      meta: {
        url() {
          return backend.meta.url
        },
        async get() {
          const response = await http.get(`${this.url()}/item`)
          return response.data
        }
      },
      data: {
        url() {
          return backend.data.url
        },
        async get() {
          const response = await http.get(this.url())
          const { collectionItemsProperty } = backend.data

          return collectionItemsProperty
            ? response.data[collectionItemsProperty]
            : response.data
        },
        async getById(id) {
          const response = await http.get(`${this.url()}/${id}`)
          return response.data
        },
        post(item) {
          return http.post(this.url(), item)
        },
        put(id, item) {
          return http.put(`${this.url()}/${id}`, item)
        },
        delete(id) {
          return http.delete(`${this.url()}/${id}`)
        }
      }
    }
  }
}
