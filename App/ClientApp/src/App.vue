<template>
  <div id="app">
    <Navbar/>
    <b-container>
      <router-view :key="$route.path"/>
    </b-container>
  </div>
</template>

<script>
import Navbar from '@/components/Navbar.vue'

export default {
  components: {
    Navbar
  }
}
</script>

<style>
body {
  padding-top: 5rem;
}
</style>
