<template>
  <div>
    <b-form @submit.prevent="onFormSubmit">
      <b-card no-body>
        <div slot="header">
          <b-row align-v="center">
            <b-col>
              <h1>{{ title }}</h1>
            </b-col>

            <b-col
              v-if="!hasAccessToApi"
              class="text-right"
            >
              <Authorize :backend="backend"/>
            </b-col>

            <b-col
              v-else
              class="text-right"
            >
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

        <b-card-body v-if="!hasAccessToApi">
          <div class="text-center lead">
            Backend is secured, please authorize.
          </div>
        </b-card-body>

        <b-card-body v-else-if="loading">
          <div
            class="text-center lead"
            v-html="loadingHtml"
          />
        </b-card-body>

        <template v-else>
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
            v-if="Object.keys(tabs).length > 0"
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
        </template>
      </b-card>

      <b-modal
        :visible="!!backendError"
        title="Backend error"
        ok-only
        @ok="onBackendErrorModalOk"
      >
        <div v-html="backendError"/>
      </b-modal>
    </b-form>
  </div>
</template>

<script>
import store from '@/store'
import Authorize from '@/components/Authorize.vue'
import Field from '@/components/fields/Field.vue'

export default {
  components: {
    Authorize,
    Field
  },
  $_veeValidate: {
    validator: 'new'
  },
  props: {
    backend: {
      type: Object,
      required: true
    }
  },
  data() {
    store.setBackend(this.backend)
    return {
      store: store.backend,
      item: null,
      form: null,
      origItem: null,
      loading: true,
      loadingHtml: '&nbsp;',
      errorSummaryVisible: false,
      backendError: null
    }
  },
  computed: {
    hasAccessToApi() {
      return !this.backend.authHeader || this.store.apiKey
    },
    itemId() {
      const { id } = this.$route.params
      return id != 'new' ? id : null
    },
    title() {
      if (!this.hasAccessToApi) {
        return this.backend.title
      }
      if (this.loading) {
        return ' '
      }
      if (this.item) {
        return this.item.name
      }
      return 'New Item'
    },
    submitButtonText() {
      return this.itemId ? 'Save' : 'Create'
    },
    headerFields() {
      return this.store.meta.fields
        .filter(f => f.attributes.groupName == 'Header' && !f.attributes.readonly)
    },
    tabs() {
      return this.store.meta.fields.reduce((tabs, field) => {
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
  watch: {
    hasAccessToApi(yes) {
      if (yes) {
        this.load()
      }
    }
  },
  async created() {
    if (this.hasAccessToApi) {
      this.load()
    }
  },
  methods: {
    async load() {
      setTimeout(() => {
        this.loadingHtml = 'Loading...'
      }, 50)

      const tasks = [this.store.fetchMeta()]

      if (this.itemId) {
        tasks.push(this.fetchItem())
      }

      await Promise.all(tasks)

      this.origItem = this.item || this.store.meta.defaultItem
      this.form = JSON.parse(JSON.stringify(this.origItem))

      this.loading = false

      window.onbeforeunload = () => this.hasPendingChanges || null
    },
    async fetchItem() {
      this.item = await this.store.getItem(this.itemId)
    },
    async onFormSubmit() {
      const ok = await this.$validator.validate()
      if (!ok) {
        return
      }

      try {
        if (this.itemId) {
          await this.store.updateItem(this.itemId, this.form)
        } else {
          await this.store.createItem(this.form)
        }

        this.origItem = this.form
        this.$router.push({ name: `${this.backend.key}-list` })
      } catch (error) {
        if (error.response && error.response.data) {
          const errorData = error.response.data

          if (typeof errorData == 'string') {
            this.backendError = errorData
          } else {
            this.backendError =
              `<ul>
                ${Object.values(errorData).map(message => `<li>${message}</li>`)}
              </ul>`
          }
        } else {
          this.backendError = error
        }
      }
    },
    toggleErrorSummary() {
      this.errorSummaryVisible = !this.errorSummaryVisible
    },
    onBackendErrorModalOk() {
      this.backendError = null
    }
  },
  beforeRouteLeave(to, from, next) {
    if (!this.hasPendingChanges) {
      next()
      return
    }

    // eslint-disable-next-line no-alert
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
