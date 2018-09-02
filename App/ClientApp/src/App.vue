<template>
  <div id="app">
    <Navbar/>
    <b-container>
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
</style>
