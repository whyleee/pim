<template>
  <div>
    <multiselect
      v-validate="validators"
      v-model="selectedOption"
      :options="options"
      :name="field.name"
      :disabled="readonly"
      :data-vv-as="field.attributes.displayName"
      :class="{
        'is-valid': state === true,
        'is-invalid': state === false
      }"
      label="name"
    />
  </div>
</template>

<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>

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
    filterParams() {
      return this.$route.query
    },
    options() {
      return (this.collection.listItems || [])
    },
    selectedOption: {
      get() {
        return (this.collection.listItems || [])
          .find(option => option[this.collection.keyName] == this.value)
      },
      set(option) {
        this.value = option[this.collection.keyName]
      }
    },
    value: {
      get() {
        return this.item[(!this.dirty && this.readFrom) || this.field.name]
      },
      set(value) {
        this.item[this.field.name] = value
        this.dirty = true
      }
    },
    origValue() {
      return this.origItem[this.readFrom || this.field.name]
    },
    readFrom() {
      return this.field.attributes.readFrom
    },
    readonly() {
      return this.field.attributes.readonly ||
        (this.field.attributes.constant && !!this.origValue)
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
      return this.value != this.origValue
    }
  },
  created() {
    const filters = Object.entries(this.field.ref.filters)
      .reduce((hash, [key, value]) => {
        hash[key] = value

        Object.entries(this.filterParams).forEach(([fpKey, fpValue]) => {
          hash[key] = hash[key].replace(`{${fpKey}}`, fpValue)
        })

        return hash
      }, {})

    this.collection.fetchListItems(filters)
  }
}
</script>

<style>
.multiselect .multiselect__tags {
  border-color: #ced4da;
}
.multiselect.is-valid .multiselect__tags {
  border-color: #28a745;
}
.multiselect.is-invalid .multiselect__tags {
  border-color: #dc3545;
}
</style>
