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
          const response = await http.get(`${this.url()}`)
          return response.data
        }
      },
      createDataApi(collection) {
        return {
          url(params = {}) {
            let url = `${backend.data.baseUrl}${collection.path}`

            Object.entries(params).forEach(([key, value]) => {
              url = url.replace(`{${key}}`, value)
            })

            return url
          },
          async get(params = {}) {
            const response = await http.get(this.url(params))

            return collection.itemsProperty
              ? response.data[collection.itemsProperty]
              : response.data
          },
          async getById(id, params = {}) {
            const response = await http.get(`${this.url(params)}/${id}`)
            return response.data
          },
          post(item, params = {}) {
            return http.post(this.url(params), item)
          },
          put(id, item, params = {}) {
            return http.put(`${this.url(params)}/${id}`, item)
          },
          delete(id, params = {}) {
            return http.delete(`${this.url(params)}/${id}`)
          }
        }
      }
    }
  }
}
