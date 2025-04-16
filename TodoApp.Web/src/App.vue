<template>
  <div id="app-container">
    <header>
      <h1>Todo Application</h1>
      <nav>
        <router-link to="/">Home</router-link>
      </nav>
    </header>
    <main>
      <router-view/>
    </main>
  </div>
</template>

<script setup>
</script>

<style src="@/assets/main.css"></style>