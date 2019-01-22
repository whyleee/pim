<template>
  <div>
    <multiselect
      v-validate="validators"
      v-if="hasAccessToApi"
      v-model="value"
      :options="options"
      :custom-label="getLabel"
      :group-values="isGrouped ? 'items' : undefined"
      :group-label="isGrouped ? 'name' : undefined"
      :multiple="multiple"
      :name="field.name"
      :disabled="readonly"
      :data-vv-as="field.attributes.displayName"
      :class="{
        'is-valid': state === true,
        'is-invalid': state === false
      }"
    />

    <template v-else>
      <Authorize
        v-for="backend in notAuthorizedBackends"
        :key="backend.config.key"
        :backend="backend"
        :button-text="`Authorize ${backend.config.title}`"
      />
    </template>
  </div>
</template>

<script>
import store from '@/store'
import Authorize from '@/components/Authorize.vue'
import Multiselect from 'vue-multiselect'

export default {
  inject: ['$validator'],
  components: {
    Authorize,
    Multiselect
  },
  props: {
    item: {
      type: Object,
      required: true
    },
    origItem: {
      type: Object,
      required: true
    },
    field: {
      type: Object,
      required: true
    },
    scope: {
      type: String,
      default: undefined
    }
  },
  data() {
    const refs = Array.isArray(this.field.ref) ? this.field.ref : [this.field.ref]
    return {
      refs,
      backends: refs.map(ref => this.getBackend(ref)),
      // TODO: hacks to make `collections` and `groups` reactive on store changes
      resolvedListItems: [],
      dirty: false
    }
  },
  computed: {
    notAuthorizedBackends() {
      return this.backends
        .filter(backend => backend.config.authHeader && !backend.apiKey)
    },
    hasAccessToApi() {
      return this.notAuthorizedBackends.length == 0
    },
    itemFilterParams() {
      return this.$route.query
    },
    collections() {
      return this.refs.reduce((hash, ref) => {
        const backend = this.getBackend(ref)
        hash[this.getRefKey(ref)] = backend.getCollection(ref.collectionKey)
        return hash
      }, {})
    },
    groups() {
      // eslint-disable-next-line no-unused-vars
      const reactivityTrigger = this.resolvedListItems.length

      return this.refs
        .map(ref => this.collections[this.getRefKey(ref)])
        .filter(collection => collection)
        .map(collection => ({
          name: collection.meta.name,
          getKey: collection.getKey,
          getTitle: collection.getTitle,
          items: collection.listItems || []
        }))
    },
    isGrouped() {
      return this.groups.length > 1
    },
    options() {
      if (this.groups.length == 0) {
        return []
      }

      if (!this.isGrouped) {
        return this.groups[0].items
          .map(item => this.groups[0].getKey(item))
          .map(value => this.convertType(value))
      }

      return this.groups.map(group => ({
        name: group.name,
        items: group.items
          .map(item => group.getKey(item))
          .map(value => this.convertType(value))
      }))
    },
    labels() {
      const labels = {}

      this.groups.forEach((group) => {
        group.items.forEach((item) => {
          labels[group.getKey(item)] = group.getTitle(item)
        })
      })

      return labels
    },
    value: {
      get() {
        if (!this.dirty && this.readFrom && this.item[this.readFrom] !== undefined) {
          return this.convertType(this.fixReadFromValue(this.item[this.readFrom]))
        }
        return this.convertType(this.item[this.field.name])
      },
      set(value) {
        this.item[this.field.name] = this.convertType(value)
        this.dirty = true
      }
    },
    origValue() {
      if (this.readFrom && this.origItem[this.readFrom] !== undefined) {
        return this.fixReadFromValue(this.origItem[this.readFrom])
      }
      return this.origItem[this.field.name]
    },
    readFrom() {
      return this.field.attributes.readFrom
    },
    readonly() {
      return this.field.attributes.readonly ||
        (this.field.attributes.constant &&
          (this.multiple ? this.origValue.length > 0 : !!this.origValue))
    },
    multiple() {
      return this.field.type == 'array'
    },
    validators() {
      return {
        required: !!this.field.attributes.required
      }
    },
    errorSelector() {
      if (this.scope) {
        return `${this.scope}.${this.field.name}`
      }
      return this.field.name
    },
    state() {
      if (this.errors.has(this.errorSelector)) {
        return false
      }
      if (this.isModified) {
        return true
      }
      return null
    },
    isModified() {
      if (!this.value && !this.origValue) {
        return false
      }
      if (this.multiple && this.value && this.origValue) {
        return JSON.stringify(this.value) != JSON.stringify(this.origValue)
      }
      return this.value != this.origValue
    }
  },
  watch: {
    hasAccessToApi(yes) {
      if (yes) {
        this.fetchDataIfNeeded()
      }
    }
  },
  async created() {
    this.fetchDataIfNeeded()
  },
  methods: {
    async fetchDataIfNeeded() {
      if (!this.hasAccessToApi) {
        return
      }

      const backendMetaPromises = this.backends
        .filter(backend => !backend.meta && backend.config.key != store.backend.config.key)
        .map(backend => backend.fetchMeta())

      if (backendMetaPromises.length > 0) {
        await Promise.all(backendMetaPromises)
      }

      const collectionPromises = this.refs.map((ref) => {
        const filterParams = Object.entries(ref.filters)
          .reduce((hash, [key, value]) => {
            hash[key] = value

            Object.entries(this.itemFilterParams).forEach(([fpKey, fpValue]) => {
              hash[key] = hash[key].replace(`{${fpKey}}`, fpValue)
            })

            return hash
          }, {})

        const backend = this.getBackend(ref)
        const collection = backend.getCollection(ref.collectionKey)

        return collection.fetchListItems(filterParams)
      })

      await Promise.all(collectionPromises)
      this.resolvedListItems.push(...collectionPromises)
    },
    getBackend(ref) {
      return ref.backendKey ? store.getBackend(ref.backendKey) : store.backend
    },
    getRefKey(ref) {
      const backend = this.getBackend(ref)
      return `${backend.config.key}:${ref.collectionKey}`
    },
    getLabel(key) {
      return this.labels[key] || key
    },
    convertType(value) {
      if (value && this.field.type == 'string') {
        return value.toString()
      }
      if (value && this.multiple && this.field.kind == 'string') {
        if (Array.isArray(value)) {
          return value.map(val => val.toString())
        }
        return value.toString()
      }
      return value
    },
    // TODO: temp fix for parent id in path string [probably must be a plugin]
    fixReadFromValue(value) {
      if (value && value.startsWith('/') && value.endsWith('/')) {
        return value.split('/').filter(x => x).slice(-2)[0] || value
      }
      return value
    }
  }
}
</script>
