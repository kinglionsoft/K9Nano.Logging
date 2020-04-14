<template>
  <textarea
    class="log-list"
    :style="`height:${height}`"
    :wrap="isWrap ? 'on' : 'off'"
    disabled
    :value="listValue"
    ref="textarea"
  ></textarea>
</template>

<script>
export default {
  name: "log-list",
  data() {
    return {
      scrollHeight: 0
    };
  },
  props: {
    // 列表
    list: {
      type: Array,
      default() {
        return [];
      }
    },
    // 控件高度
    height: {
      type: String,
      default: "500px"
    },
    // 是否沉底
    sink: {
      type: Boolean,
      default: true
    },
    // 是否换行
    isWrap: {
      type: Boolean,
      default: false
    },
    zoom: Boolean
  },
  watch: {
    list: {
      handler(val) {
        this.sink && this.down();
      },
      immediate: true
    },
    zoom(value) {
      this.zoomInOut(value);
    }
  },
  computed: {
    listValue() {
      return this.list.toString().replace(/\r\n,/g, "\r\n");
    }
  },
  methods: {
    down() {
      this.$nextTick(() => {
        const ele = this.$el;
        // console.log(
        //   Math.ceil(ele.scrollTop),
        //   ele.offsetHeight,
        //   this.scrollHeight
        // );
        if (
          Math.ceil(ele.scrollTop) + ele.offsetHeight >= this.scrollHeight ||
          Math.ceil(ele.scrollTop) + ele.offsetHeight - 10 >=
            this.scrollHeight ||
          (this.scrollHeight !== ele.scrollHeight && this.scrollHeight === 0)
        ) {
          ele.scrollTop = ele.scrollHeight;
        }
        this.scrollHeight = ele.scrollHeight;
      });
    },
    zoomInOut(value) {
      this.$el.style.height = value ? `calc(100vh - 40px)` : `${this.height}`;
    },
    calcWidth(list) {
      this.$nextTick(() => {
        let lens = list.map(item => item.length);
        let max = lens.length && Math.max(...lens);
        let ele = this.$refs[`textarea`];
        ele.style.width = this.isWrap ? `100%` : `${max * 13}px`;
      });
    }
  }
};
</script>

<style lang="scss">
.log-list {
  box-sizing: border-box;
  display: block;
  height: 100%;
  width: 100%;
  border: none;
  outline: none;
  resize: none;
  font-size: 14px;
  color: rgb(192, 192, 192);
  word-break: break-all;
  background-color: rgb(30, 30, 30);
  margin: 0;
  padding: 0 0 0 5px;
  font-family: "Consolas";
}
</style>

