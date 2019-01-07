import apiFactory from '@/lib/api'
import globalConfig from '@/config.json'

const backends = {}
const store = {
  backend: null,
  error: null,

  getBackend(key) {
    const config = globalConfig.backends.find(b => b.key == key)

    if (!config) {
      throw new Error(`Unknown backend key '${key}'`)
    }

    if (!backends[key]) {
      // eslint-disable-next-line no-use-before-define
      backends[key] = createBackend(config)
    }

    return backends[key]
  },
  setBackend(config) {
    this.backend = this.getBackend(config.key)
  }
}

async function catchError(func) {
  try {
    const result = await func()
    return result
  } catch (e) {
    store.error = e
    throw e
  }
}

const apiKeyStore = {
  getApiKey(backendKey) {
    return localStorage.getItem(`${backendKey}-apiKey`)
  },
  setApiKey(backendKey, apiKey) {
    if (apiKey) {
      localStorage.setItem(`${backendKey}-apiKey`, apiKey)
    } else {
      localStorage.removeItem(`${backendKey}-apiKey`)
    }
  }
}

function createBackend(config) {
  const api = apiFactory.create(config)
  const collections = {}

  if (config.authHeader) {
    const apiKey = apiKeyStore.getApiKey(config.key)

    if (apiKey) {
      api.setApiKey(apiKey)
    }
  }

  return {
    config,
    meta: null,
    collection: null,

    get apiKey() {
      return apiKeyStore.getApiKey(this.config.key)
    },
    set apiKey(value) {
      apiKeyStore.setApiKey(this.config.key, value)
      api.setApiKey(value)
    },

    async fetchMeta() {
      if (!this.meta) {
        this.meta = await catchError(() => api.meta.get())
      }
    },
    getCollection(key) {
      if (!collections[key]) {
        if (!this.meta) {
          return null // TODO: this case must be handled with reactivity
        }
        const collectionMeta = this.meta.collections.find(c => c.key == key)
        const dataApi = api.createDataApi(collectionMeta)

        // eslint-disable-next-line no-use-before-define
        collections[key] = createCollection(collectionMeta, dataApi)
      }

      return collections[key]
    },
    setCollection(key) {
      this.collection = this.getCollection(key)
    }
  }
}

function createCollection(meta, dataApi) {
  return {
    meta,
    listItems: null,

    getKey(item) {
      const keyFields = meta.itemType.fields.filter(field => field.attributes.key)
      return keyFields.map(field => item[field.name]).join('~') || item.id
    },
    get titleName() {
      const titleField = meta.itemType.fields.find(field => field.attributes.title)
      return titleField ? titleField.name : 'name'
    },

    async fetchListItems(params = {}) {
      this.listItems = await catchError(() => dataApi.get(params))
    },
    getItem(id, params = {}) {
      return catchError(() => dataApi.getById(id, params))
    },
    createItem(data, params = {}) {
      return catchError(() => dataApi.post(data, params))
    },
    updateItem(id, data, params = {}) {
      return catchError(() => dataApi.put(id, data, params))
    },
    async deleteItem(id, params = {}) {
      await catchError(() => dataApi.delete(id, params))

      const index = this.listItems.findIndex(i => this.getKey(i) == id)
      this.listItems.splice(index, 1)
    }
  }
}

export default store
