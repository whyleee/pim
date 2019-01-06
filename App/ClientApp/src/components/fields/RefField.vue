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
        hash[ref.collectionKey] = store.backend.getCollection(ref.collectionKey)
        return hash
      }, {})
    },
    groups() {
      return this.refs
        .map(ref => this.collections[ref.collectionKey])
        .map(collection => ({
          name: collection.meta.name,
          keyName: collection.keyName,
          items: collection.listItems || []
        }))
    },
    isGrouped() {
      return this.groups.length > 1
    },
    options() {
      if (!this.isGrouped) {
        return this.groups[0].items.map(item => item[this.groups[0].keyName])
      }

      return this.groups.map(group => ({
        name: group.name,
        items: group.items.map(item => item[group.keyName])
      }))
    },
    labels() {
      const labels = {}

      this.groups.forEach((group) => {
        group.items.forEach((item) => {
          labels[item[group.keyName]] = item.name
        })
      })

      return labels
    },
    value: {
      get() {
        if (!this.dirty && this.readFrom && this.item[this.readFrom] !== undefined) {
          return this.item[this.readFrom]
        }
        return this.item[this.field.name]
      },
      set(value) {
        this.item[this.field.name] = value
        this.dirty = true
      }
    },
    origValue() {
      if (this.readFrom && this.origItem[this.readFrom] !== undefined) {
        return this.origItem[this.readFrom]
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
  created() {
    this.refs.forEach((ref) => {
      const filterParams = Object.entries(ref.filters)
        .reduce((hash, [key, value]) => {
          hash[key] = value

          Object.entries(this.itemFilterParams).forEach(([fpKey, fpValue]) => {
            hash[key] = hash[key].replace(`{${fpKey}}`, fpValue)
          })

          return hash
        }, {})

      this.collections[ref.collectionKey].fetchListItems(filterParams)
    })
  },
  methods: {
    getLabel(key) {
      return this.labels[key] || key
    }
  }
}
</script>
