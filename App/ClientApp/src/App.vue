<template>
  <div id="app">
    <Navbar/>
    <b-container fluid>
      <router-view :key="$route.path"/>
    </b-container>
    <Error/>
  </div>
</template>

<script>
import Navbar from '@/components/Navbar.vue'
import Error from '@/components/Error.vue'

export default {
  components: {
    Navbar,
    Error
  }
}
</script>

<style>
body {
  padding-top: 5rem;
}

/* fix overlapping other absolute elements like multiselect list */
.input-group-prepend button.btn,
.input-group-append button.btn {
  z-index: 1; /* reduced from 2 */
}

/* vue-multiselect bootstrap style overrides */

/* align input size */
.multiselect .multiselect__tags {
  border-radius: 0.25rem;
}
.multiselect .multiselect__placeholder {
  margin-bottom: 7px;
  padding-top: 0;
}

/* align input state colors */
.multiselect .multiselect__tags {
  border-color: #ced4da;
}
.multiselect.is-valid .multiselect__tags {
  border-color: #28a745;
}
.multiselect.is-invalid .multiselect__tags {
  border-color: #dc3545;
}

/* align list hover and select styles */
.multiselect .multiselect__option--highlight {
  background-color: #f8f9fa;
  color: inherit;
}
.multiselect .multiselect__option--highlight:after {
  background-color: #f8f9fa;
  color: #6c757d;
  font-weight: normal;
}
</style>
