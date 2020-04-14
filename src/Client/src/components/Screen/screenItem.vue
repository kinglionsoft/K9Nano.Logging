<template>
  <div
    class="log-screen-item"
    :style="`width:${calcWidth(item,total)}%;${calcSpacing(item,total)}`"
  >
    <slot/>
  </div>
</template>

<script>
export default {
  name: "log-screen-item",
  props: {
    item: {
      type: Number,
      required: true,
      validator(value) {
        return value >= 0;
      }
    },
    total: {
      type: Number,
      required: true,
      validator(value) {
        return value >= 0;
      }
    },
    zoom: Boolean
  },
  watch: {
    zoom(value) {
      this.zoomInOut(value);
    }
  },
  methods: {
    calcWidth(item, total) {
      if (total % 2 !== 0 && total === item) return 100;
      return 50;
    },
    calcSpacing(item, total) {
      if (total % 2 !== 0 && total === item) return;
      if (item % 2 === 1) return `padding-right:2px`;
      if (item % 2 === 0) return `padding-left:2px`;
    },
    zoomInOut(value) {
      if (value) {
        this.$el.style.cssText = `position: fixed;z-index: 999;width: 100%;background-color: #fff;`;
      } else {
        this.$el.style.cssText = `width:${this.calcWidth(
          this.item,
          this.total
        )}%;${this.calcSpacing(this.item, this.total)}`;
      }
    }
  }
};
</script>

<style lang="scss">
.log-screen-item {
  box-sizing: border-box;
}
</style>