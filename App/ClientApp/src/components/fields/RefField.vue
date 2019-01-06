<template>
  <div>
    <multiselect
      v-validate="validators"
      v-model="value"
      :options="options.map(opt => opt[collection.keyName])"
      :custom-label="getLabel"
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
      collection: store.backend.getCollection(this.field.ref.collectionKey),
      dirty: false
    }
  },
  computed: {
    itemFilterParams() {
      return this.$route.query
    },
    options() {
      return (this.collection.listItems || [])
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
    const filterParams = Object.entries(this.field.ref.filters)
      .reduce((hash, [key, value]) => {
        hash[key] = value

        Object.entries(this.itemFilterParams).forEach(([fpKey, fpValue]) => {
          hash[key] = hash[key].replace(`{${fpKey}}`, fpValue)
        })

        return hash
      }, {})

    this.collection.fetchListItems(filterParams)
  },
  methods: {
    getLabel(key) {
      const option = this.options
        .find(opt => opt[this.collection.keyName] == key)

      return option ? option.name : key
    }
  }
}
</script>
