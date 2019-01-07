<template>
  <div>
    <multiselect
      v-validate="validators"
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
  </div>
</template>

<script>
import Multiselect from 'vue-multiselect'
import store from '@/store'

export default {
  inject: ['$validator'],
  components: {
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
    return {
      // TODO: hacks to make `collections` and `groups` reactive on store changes
      resolvedBackendMetas: [],
      resolvedListItems: [],
      dirty: false
    }
  },
  computed: {
    itemFilterParams() {
      return this.$route.query
    },
    refs() {
      return Array.isArray(this.field.ref) ? this.field.ref : [this.field.ref]
    },
    collections() {
      return this.refs.reduce((hash, ref) => {
        // eslint-disable-next-line no-unused-vars
        const reactivityTrigger = this.resolvedBackendMetas.length

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
          titleName: collection.titleName,
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
        return this.groups[0].items.map(item => this.groups[0].getKey(item))
      }

      return this.groups.map(group => ({
        name: group.name,
        items: group.items.map(item => group.getKey(item))
      }))
    },
    labels() {
      const labels = {}

      this.groups.forEach((group) => {
        group.items.forEach((item) => {
          labels[group.getKey(item)] = item[group.titleName]
        })
      })

      return labels
    },
    value: {
      get() {
        if (!this.dirty && this.readFrom && this.item[this.readFrom] !== undefined) {
          return this.fixReadFromValue(this.item[this.readFrom])
        }
        return this.item[this.field.name]
      },
      set(value) {
        let converted = value

        if (value && this.field.type == 'string') {
          converted = value.toString()
        }
        if (value && this.multiple && this.field.kind == 'string') {
          converted = value.map(val => val.toString())
        }

        this.item[this.field.name] = converted
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
  async created() {
    const backendMetaPromises = this.refs
      .map(ref => this.getBackend(ref))
      .filter(backend => !backend.meta && backend.config.key != store.backend.config.key)
      .map(backend => backend.fetchMeta())

    if (backendMetaPromises.length > 0) {
      await Promise.all(backendMetaPromises)
      this.resolvedBackendMetas.push(...backendMetaPromises)
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
  methods: {
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
