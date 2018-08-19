import api from '@/lib/api'

const backends = {}

function createState() {
  return {
    meta: {}
  }
}

export default {
  async fetchMeta(backend, itemType) {
    if (!backends[backend]) {
      backends[backend] = createState()
    }

    let meta = backends[backend].meta[itemType]

    if (!meta) {
      meta = await api.meta.get(backend, itemType)
      backends[backend].meta[itemType] = meta
    }

    return meta
  }
}
