import apiFactory from '@/lib/api'

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
        this.meta = await api.meta.get()
        this.keyName = getKeyName(this.meta)
      }
    },
    async fetchListItems() {
      this.listItems = await api.data.get()
    },
    getItem(id) {
      return api.data.getById(id)
    },
    createItem(data) {
      return api.data.post(data)
    },
    updateItem(id, data) {
      return api.data.put(id, data)
    },
    async deleteItem(id) {
      await api.data.delete(id)

      const index = this.listItems.findIndex(i => i[this.keyName] == id)
      this.listItems.splice(index, 1)
    }
  }
}

const backends = {}

export default {
  backend: null,

  setBackend(config) {
    if (!backends[config.key]) {
      backends[config.key] = createBackend(config)
    }

    this.backend = backends[config.key]
  }
}
