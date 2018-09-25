<template>
  <b-navbar
    toggleable="md"
    type="dark"
    variant="dark"
    fixed="top"
  >
    <b-navbar-toggle target="nav-collapse"/>

    <b-navbar-brand to="/">
      <img
        src="@/assets/logo.png"
        width="24"
        height="24"
      >
      pim
    </b-navbar-brand>

    <b-collapse
      id="nav-collapse"
      is-nav
    >
      <b-navbar-nav>
        <b-nav-item
          v-for="item in items"
          :key="item.title"
          :to="item.location"
        >
          {{ item.title }}
        </b-nav-item>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar>
</template>

<script>
import config from '@/config.json'

export default {
  data() {
    return {
      items: config.backends.map(backend => ({
        title: backend.title,
        location: {
          name: backend.key
        }
      }))
    }
  }
}
</script>
