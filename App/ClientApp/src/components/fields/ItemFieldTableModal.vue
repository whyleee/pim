<template>
  <b-modal
    ref="modal"
    :title="title"
    :lazy="true"
    @ok="onOk"
    @cancel="$emit('cancel')"
  >
    <b-form
      :data-vv-scope="vvScope"
      @submit.prevent="onOk"
    >
      <ItemField
        v-for="field in meta.fields"
        :key="field.name"
        :item="item"
        :orig-item="origItem"
        :field="field"
        :scope="vvScope"
      />
    </b-form>
  </b-modal>
</template>

<script>
export default {
  $_veeValidate: {
    validator: 'new'
  },
  props: {
    title: {
      type: String,
      required: true
    },
    item: {
      type: Object,
      required: true
    },
    origItem: {
      type: Object,
      required: true
    },
    meta: {
      type: Object,
      required: true
    }
  },
  computed: {
    vvScope() {
      return this.meta.name
    }
  },
  methods: {
    show() {
      this.$refs.modal.show()
    },
    hide() {
      this.$refs.modal.hide()
    },
    async onOk(event) {
      event.preventDefault()

      const ok = await this.$validator.validate(`${this.vvScope}.*`)
      if (!ok) {
        return
      }

      this.hide()
      this.$emit('ok')
    }
  }
}
</script>
