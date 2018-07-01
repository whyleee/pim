<template>
  <div class="b-datepicker">
    <datepicker
      v-validate="validators"
      :name="field.name"
      v-model="date"
      :data-vv-as="field.attributes.displayName"
      :state="state"
      :input-class="inputClass"
      format="MMMM d, yyyy"
    />
  </div>
</template>

<script>
import { format } from 'date-fns/esm'
import Datepicker from 'vuejs-datepicker'

export default {
  components: {
    Datepicker
  },
  inject: ['$validator'],
  props: {
    item: {
      type: Object,
      required: true
    },
    field: {
      type: Object,
      required: true
    }
  },
  computed: {
    date: {
      get() {
        return this.item[this.field.name]
      },
      set(value) {
        this.item[this.field.name] = format(value, 'YYYY-MM-DD')
      }
    },
    validators() {
      return {
        required: !!this.field.attributes.required
      }
    },
    state() {
      if (this.errors.has(this.field.name)) {
        return 'invalid'
      }
      return null
    },
    inputClass() {
      return `form-control${this.errors.has(this.field.name) ? ' is-invalid' : ''}`
    }
  }
}
</script>

<style scoped>
  .vdp-datepicker input[readonly] {
    background-color: #fff;
  }
  .vdp-datepicker[state="invalid"] ~ .invalid-feedback {
    display: block;
  }
  .vdp-datepicker__calendar header .next,
  .vdp-datepicker__calendar header .prev {
    text-indent: 0;
    font-size: 1.3rem;
  }
  .vdp-datepicker__calendar header .next:after,
  .vdp-datepicker__calendar header .prev:after {
    display: none
  }
</style>

