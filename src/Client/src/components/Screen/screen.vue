<template>
  <section class="log-screen">
    <slot/>
  </section>
</template>

<script>
export default {
  name: "log-screen"
};
</script>

<style lang="scss">
.log-screen {
  display: flex;
  flex-wrap: wrap;
  height: 100vh;
  width: 100%;
}
</style>

