<template>
  <div
    v-if="loading"
    class="text-center lead"
  >
    Loading...
  </div>
  <div v-else>
    <h1>{{ title }}</h1>
    <b-form @submit.prevent="onFormSubmit">
      <b-card no-body>
        <div slot="header">
          <ItemField
            v-for="field in headerFields"
            :key="field.name"
            :item="form"
            :orig-item="origItem"
            :field="field"
          />
        </div>

        <b-tabs
          card
          no-fade
        >
          <b-tab
            v-for="[tabName, fields] in Object.entries(tabs)"
            :key="tabName"
            :title="tabName"
          >
            <ItemField
              v-for="field in fields"
              :key="field.name"
              :item="form"
              :orig-item="origItem"
              :field="field"
            />
          </b-tab>
        </b-tabs>

        <div slot="footer">
          <b-alert
            :show="summaryErrors.length > 0"
            :fade="false"
            variant="danger"
          >
            <ul>
              <li
                v-for="error in summaryErrors"
                :key="error.id"
              >
                {{ error.msg }}
              </li>
            </ul>
          </b-alert>

          <b-button
            type="submit"
            variant="dark"
          >
            {{ submitButtonText }}
          </b-button>
        </div>
      </b-card>
    </b-form>
  </div>
</template>

<script>
import api from '@/lib/api'
import ItemField from '@/components/fields/ItemField.vue'

export default {
  components: {
    ItemField
  },
  $_veeValidate: {
    validator: 'new'
  },
  props: {
    productId: {
      type: String,
      default: undefined
    }
  },
  data() {
    return {
      meta: {
        fields: []
      },
      product: null,
      form: {},
      origItem: null,
      loading: true
    }
  },
  computed: {
    title() {
      return this.product ? this.product.name : 'New Product'
    },
    submitButtonText() {
      return this.productId ? 'Save' : 'Create'
    },
    headerFields() {
      return this.meta.fields
        .filter(f => f.attributes.groupName == 'Header' && !f.attributes.readonly)
    },
    tabs() {
      return this.meta.fields.reduce((tabs, field) => {
        const groupName = field.attributes.groupName || 'Content'

        if (groupName != 'Header') {
          tabs[groupName] = tabs[groupName] || []
          tabs[groupName].push(field)
        }

        return tabs
      }, {})
    },
    summaryErrors() {
      return this.errors.items.filter(err => !err.scope)
    }
  },
  async created() {
    await this.fetchMeta()

    if (this.productId) {
      await this.fetchData()
    }

    this.origItem = this.product || this.meta.defaultItem
    this.form = JSON.parse(JSON.stringify(this.origItem))

    this.loading = false
  },
  methods: {
    async fetchMeta() {
      const response = await api.meta.get('product')
      this.meta = response.data
    },
    async fetchData() {
      const response = await api.products.getById(this.productId)
      this.product = response.data
    },
    async onFormSubmit() {
      const ok = await this.$validator.validate()
      if (!ok) {
        return
      }

      if (this.productId) {
        await api.products.put(this.productId, this.form)
      } else {
        await api.products.post(this.form)
      }

      this.$router.push({ name: 'home' })
    }
  }
}
</script>
