<template>
  <div :id="`date-container-${_uid}`">
    <b-form-input
      v-if="field.attributes.readonly"
      :value="prettyDateTime"
      plaintext
    />
    <b-input-group v-else>
      <b-input-group-prepend>
        <b-btn :id="`calendar-btn-${_uid}`">
          <font-awesome-icon :icon="calendarIcon"/>
        </b-btn>

        <b-popover
          :target="`calendar-btn-${_uid}`"
          :container="`date-container-${_uid}`"
          triggers="click blur"
          placement="bottomright"
        >
          <div>
            <datepicker
              v-model="dateTime"
              inline
            />

            <div class="text-center mt-2">
              <font-awesome-icon
                :icon="clockIcon"
                class="clock-icon mr-2"/>
              <timepicker
                v-model="time"
              />
            </div>
          </div>
        </b-popover>
      </b-input-group-prepend>

      <b-form-input
        v-validate="validators"
        v-model="inputValue"
        :name="field.name"
        :data-vv-as="field.attributes.displayName"
        :state="state"
        @change="onInputChange"
      />

      <b-input-group-append v-if="dateTime">
        <b-btn @click.prevent="onClearClick">
          Clear
        </b-btn>
      </b-input-group-append>
    </b-input-group>
  </div>
</template>

<script>
import {
  isValid as isValidDate,
  format as formatDate,
  parse as parseDate
} from 'date-fns/esm'
import Datepicker from 'vuejs-datepicker'
import Timepicker from 'vue2-timepicker'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import {
  faCalendarAlt,
  faClock
} from '@fortawesome/free-regular-svg-icons'
import { Validator } from 'vee-validate'

const DATE_FORMAT = 'MMMM d, yyyy \'at\' HH:mm'
const NOTIME_FORMAT = 'MMMM d, yyyy'

export default {
  components: {
    Datepicker,
    Timepicker,
    FontAwesomeIcon
  },
  inject: ['$validator'],
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
      inputValue: null
    }
  },
  computed: {
    calendarIcon() {
      return faCalendarAlt
    },
    clockIcon() {
      return faClock
    },
    value: {
      get() {
        return this.item[this.field.name]
      },
      set(value) {
        this.item[this.field.name] = value
      }
    },
    origValue() {
      return this.origItem[this.field.name]
    },
    time: {
      get() {
        if (!this.dateTime) {
          return null
        }
        return {
          HH: formatDate(this.dateTime, 'HH'),
          mm: formatDate(this.dateTime, 'mm')
        }
      },
      set(value) {
        const { dateTime } = this

        if (value.HH == dateTime.getHours() &&
          value.mm == dateTime.getMinutes()) {
          return
        }

        dateTime.setHours(value.HH)
        dateTime.setMinutes(value.mm)
        this.dateTime = dateTime
      }
    },
    dateTime: {
      get() {
        if (!this.value) {
          return null
        }
        return new Date(this.value)
      },
      set(value) {
        this.value = value ? this.getIsoDate(value) : null
      }
    },
    prettyDateTime() {
      return this.getPrettyDate(this.dateTime)
    },
    validators() {
      return {
        required: !!this.field.attributes.required,
        pretty_date_format: true
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
      return new Date(this.value).getTime() != new Date(this.origValue).getTime()
    }
  },
  watch: {
    dateTime(value) {
      this.inputValue = this.getPrettyDate(value)
    }
  },
  created() {
    this.registerDateFormatValidator()
    this.inputValue = this.getPrettyDate(this.value)
  },
  methods: {
    registerDateFormatValidator() {
      Validator.extend('pretty_date_format', {
        getMessage: field => `The ${field} field must be in format ${DATE_FORMAT}.`,
        validate: (value) => {
          if (!value) {
            return true
          }
          const parsedDate = this.parsePrettyDate(value)
          return isValidDate(parsedDate)
        }
      })
    },
    getPrettyDate(date) {
      if (!date) {
        return ''
      }
      return formatDate(date, DATE_FORMAT)
        .replace('at 00:00', '')
    },
    getIsoDate(date) {
      return `${date.toISOString().slice(0, -5)}Z`
    },
    parsePrettyDate(value, baseDate) {
      const parseFormat = value.indexOf(' at ') >= 0 ? DATE_FORMAT : NOTIME_FORMAT
      return parseDate(value, parseFormat, baseDate || new Date())
    },
    onClearClick() {
      this.dateTime = null
    },
    onInputChange(value) {
      if (!value) {
        this.dateTime = null
        return
      }

      const parsedDateTime = this.parsePrettyDate(value, this.dateTime)

      if (isValidDate(parsedDateTime)) {
        this.dateTime = parsedDateTime
      }
    }
  }
}
</script>

<style scoped>
  .clock-icon {
    vertical-align: -0.25em;
  }
</style>

<style>
  /* increase popover width */
  .popover {
    max-width: 100%;
  }

  /* open timepicker above the input */
  .time-picker .dropdown {
    top: calc(-10em - 5px);
    box-shadow: 0 1px 6px rgba(0,0,0,0.5);
  }

  /* set gray active color to timepicker */
  .time-picker .dropdown ul li.active,
  .time-picker .dropdown ul li.active:hover {
    background: #6c757d;
  }

  /* set gray colors to datepicker */
  .vdp-datepicker__calendar .cell:not(.blank):not(.disabled).day:hover,
  .vdp-datepicker__calendar .cell:not(.blank):not(.disabled).month:hover,
  .vdp-datepicker__calendar .cell:not(.blank):not(.disabled).year:hover {
    border: 1px solid #6c757d;
  }
  .vdp-datepicker__calendar .cell.selected,
  .vdp-datepicker__calendar .cell.selected:hover {
    background: #6c757d;
    color: #fff;
  }
</style>
