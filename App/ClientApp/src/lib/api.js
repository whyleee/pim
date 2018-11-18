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
            const query = {}

            Object.entries(params).forEach(([key, value]) => {
              if (url.indexOf(`{${key}}`) >= 0) {
                url = url.replace(`{${key}}`, value)
              } else if (value) {
                query[key] = value
              }
            })

            if (Object.keys(query).length) {
              const qs = Object.entries(query)
                .map(entry => `${entry[0]}=${entry[1]}`)
                .join('&')

              url += (url.indexOf('?') >= 0 ? '&' : '?') + qs
            }

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
