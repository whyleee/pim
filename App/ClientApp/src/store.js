import apiFactory from '@/lib/api'

const backends = {}
const store = {
  backend: null,
  error: null,

  setBackend(config) {
    if (!backends[config.key]) {
      // eslint-disable-next-line no-use-before-define
      backends[config.key] = createBackend(config)
    }

    this.backend = backends[config.key]
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
    setCollection(key) {
      if (!collections[key]) {
        const collectionMeta = this.meta.collections.find(c => c.key == key)
        const dataApi = api.createDataApi(collectionMeta)

        // eslint-disable-next-line no-use-before-define
        collections[key] = createCollection(collectionMeta, dataApi)
      }

      this.collection = collections[key]
    }
  }
}

function createCollection(meta, dataApi) {
  return {
    meta,
    listItems: null,

    get keyName() {
      const keyField = meta.itemType.fields.find(field =>
        Object.entries(field.attributes)
          .some(([key, value]) => key == 'key' && value))

      return keyField ? keyField.name : 'id'
    },

    async fetchListItems() {
      this.listItems = await catchError(() => dataApi.get())
    },
    getItem(id) {
      return catchError(() => dataApi.getById(id))
    },
    createItem(data) {
      return catchError(() => dataApi.post(data))
    },
    updateItem(id, data) {
      return catchError(() => dataApi.put(id, data))
    },
    async deleteItem(id) {
      await catchError(() => dataApi.delete(id))

      const index = this.listItems.findIndex(i => i[this.keyName] == id)
      this.listItems.splice(index, 1)
    }
  }
}

export default store
