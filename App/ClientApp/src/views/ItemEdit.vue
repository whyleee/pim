<template>
  <div>
    <b-form @submit.prevent="onFormSubmit">
      <b-card
        v-if="loading"
        no-body
      >
        <div slot="header">
          <h1>&nbsp;</h1>
        </div>
        <b-card-body>
          <div
            class="text-center lead"
            v-html="loadingHtml"
          />
        </b-card-body>
      </b-card>
      <b-card
        v-else
        no-body
      >
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
      </b-card>
    </b-form>
  </div>
</template>

<script>
import api from '@/lib/api'
import store from '@/store'
import Field from '@/components/fields/Field.vue'

export default {
  components: {
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
    return {
      meta: {
        fields: []
      },
      item: null,
      form: {},
      origItem: null,
      loading: true,
      loadingHtml: '&nbsp;',
      errorSummaryVisible: false
    }
  },
  computed: {
    itemId() {
      const { id } = this.$route.params
      return id != 'new' ? id : null
    },
    title() {
      return this.item ? this.item.name : 'New Item'
    },
    submitButtonText() {
      return this.itemId ? 'Save' : 'Create'
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
    setTimeout(() => {
      this.loadingHtml = 'Loading...'
    }, 50)

    const tasks = [this.fetchMeta()]

    if (this.itemId) {
      tasks.push(this.fetchData())
    }

    await Promise.all(tasks)

    this.origItem = this.item || this.meta.defaultItem
    this.form = JSON.parse(JSON.stringify(this.origItem))

    this.loading = false

    window.onbeforeunload = () => this.hasPendingChanges || null
  },
  methods: {
    async fetchMeta() {
      this.meta = await store.fetchMeta(this.backend.key, 'item')
    },
    async fetchData() {
      this.item = await api.data.getById(this.backend.key, this.itemId)
    },
    async onFormSubmit() {
      const ok = await this.$validator.validate()
      if (!ok) {
        return
      }

      if (this.itemId) {
        await api.data.put(this.backend.key, this.itemId, this.form)
      } else {
        await api.data.post(this.backend.key, this.form)
      }

      this.origItem = this.form
      this.$router.push({ name: `${this.backend.key}-list` })
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