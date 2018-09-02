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

function getKeyName(meta) {
  const keyField = meta.fields.find(field =>
    Object.entries(field.attributes)
      .some(([key, value]) => key == 'key' && value))

  return keyField ? keyField.name : 'id'
}

function createBackend(config) {
  const api = apiFactory.create(config)
  const apiKey = apiKeyStore.getApiKey(config.key)

  if (apiKey) {
    api.setApiKey(apiKey)
  }

  return {
    config,
    meta: null,
    keyName: null,
    listItems: null,

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
        this.keyName = getKeyName(this.meta)
      }
    },
    async fetchListItems() {
      this.listItems = await catchError(() => api.data.get())
    },
    getItem(id) {
      return catchError(() => api.data.getById(id))
    },
    createItem(data) {
      return catchError(() => api.data.post(data))
    },
    updateItem(id, data) {
      return catchError(() => api.data.put(id, data))
    },
    async deleteItem(id) {
      await catchError(() => api.data.delete(id))

      const index = this.listItems.findIndex(i => i[this.keyName] == id)
      this.listItems.splice(index, 1)
    }
  }
}

export default store
