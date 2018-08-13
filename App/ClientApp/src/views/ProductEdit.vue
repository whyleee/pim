<template>
  <div
    v-if="loading"
    class="text-center lead"
  >
    Loading...
  </div>
  <div v-else>
    <b-form @submit.prevent="onFormSubmit">
      <b-card no-body>
        <div slot="header">
          <b-row align-v="center">
            <b-col>
              <h1>{{ title }}</h1>
            </b-col>
            <b-col class="text-right">
              <b-button
                v-if="hasErrors"
                variant="link"
                class="text-danger mr-3"
                @click="toggleErrorSummary"
              >
                {{ errorSummaryVisible ? 'Hide' : 'Show' }} errors
              </b-button>

              <span
                v-if="hasPendingChanges && !hasErrors"
                class="text-success align-middle mr-3"
              >
                Pending changes
              </span>

              <b-button
                :disabled="saveButtonDisabled"
                :variant="saveButtonVariant"
                type="submit"
              >
                {{ submitButtonText }}
              </b-button>
            </b-col>
          </b-row>

          <b-alert
            :show="errorSummaryVisible"
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
        </div>

        <b-card-body class="header-fields">
          <Field
            v-for="field in headerFields"
            :key="field.name"
            :item="form"
            :orig-item="origItem"
            :field="field"
          />
        </b-card-body>

        <b-tabs
          card
          no-fade
        >
          <b-tab
            v-for="[tabName, fields] in Object.entries(tabs)"
            :key="tabName"
            :title="tabName"
          >
            <Field
              v-for="field in fields"
              :key="field.name"
              :item="form"
              :orig-item="origItem"
              :field="field"
            />
          </b-tab>
        </b-tabs>
      </b-card>
    </b-form>
  </div>
</template>

<script>
import api from '@/lib/api'
import Field from '@/components/fields/Field.vue'

export default {
  components: {
    Field
  },
  $_veeValidate: {
    validator: 'new'
  },
  data() {
    return {
      meta: {
        fields: []
      },
      product: null,
      form: {},
      origItem: null,
      loading: true,
      errorSummaryVisible: false
    }
  },
  computed: {
    productId() {
      const { id } = this.$route.params
      return id != 'new' ? id : null
    },
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
    },
    hasErrors() {
      return this.summaryErrors.length > 0
    },
    hasPendingChanges() {
      return JSON.stringify(this.form) != JSON.stringify(this.origItem)
    },
    saveButtonVariant() {
      if (this.hasErrors) {
        return 'danger'
      } else if (this.hasPendingChanges) {
        return 'success'
      }
      return undefined
    },
    saveButtonDisabled() {
      return this.hasErrors || !this.hasPendingChanges
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

    window.onbeforeunload = () => this.hasPendingChanges || null
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
    },
    toggleErrorSummary() {
      this.errorSummaryVisible = !this.errorSummaryVisible
    }
  },
  beforeRouteLeave(to, from, next) {
    if (!this.hasPendingChanges) {
      next()
      return
    }

    const answer = window.confirm('Changes you made may not be saved.')
    if (answer) {
      next()
    } else {
      next(false)
    }
  }
}
</script>

<style scoped>
.header-fields {
  background-color: rgba(0, 0, 0, 0.03);
  padding-bottom: 0;
}
</style>
