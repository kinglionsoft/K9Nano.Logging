<template>
  <section class="log-action-bar">
    <el-select
      v-if="isTag1"
      v-model="tag2Calc"
      placeholder="tag2"
      :size="size"
      class="log-action-bar-select-item log-action-bar-item"
      ref="tagB"
      @change="
        change({ type: 'changeTag', value: { type: 'tagB', value: tag2 } })
      "
    >
      <el-option :value="tag2" disabled>
        <el-input
          v-model.trim="tag2"
          placeholder="请输入"
          size="mini"
          style="width:140px"
        >
          <el-button
            slot="append"
            @click="
              change({
                type: 'changeTag',
                value: { type: 'tagB', value: tag2 }
              })
            "
            >确认</el-button
          >
        </el-input>
      </el-option>
      <el-option label="全部" value></el-option>
      <el-option
        v-for="item of tagBList"
        :key="item"
        :label="item"
        :value="item"
      ></el-option>
    </el-select>

    <el-select
      v-if="isTag2"
      v-model="tag3Calc"
      placeholder="tag3"
      :size="size"
      class="log-action-bar-select-item log-action-bar-item"
      ref="tagC"
      @change="
        change({ type: 'changeTag', value: { type: 'tagC', value: tag3 } })
      "
    >
      <el-option :value="tag3" disabled>
        <el-input
          v-model.trim="tag3"
          placeholder="请输入"
          size="mini"
          style="width:140px"
        >
          <el-button
            slot="append"
            @click="
              change({
                type: 'changeTag',
                value: { type: 'tagC', value: tag3 }
              })
            "
            >确认</el-button
          >
        </el-input>
      </el-option>
      <el-option label="全部" value></el-option>
      <el-option
        v-for="item of tagCList"
        :key="item"
        :label="item"
        :value="item"
      ></el-option>
    </el-select>

    <el-select
      v-if="isTag3"
      v-model="tag1Calc"
      placeholder="tag1"
      :size="size"
      class="log-action-bar-select-item log-action-bar-item"
      ref="tagA"
      @change="
        change({ type: 'changeTag', value: { type: 'tagA', value: tag1 } })
      "
    >
      <el-option :value="tag1" disabled>
        <el-input
          v-model.trim="tag1"
          placeholder="请输入"
          size="mini"
          style="width:140px"
        >
          <el-button
            slot="append"
            @click="
              change({
                type: 'changeTag',
                value: { type: 'tagA', value: tag1 }
              })
            "
            >确认</el-button
          >
        </el-input>
      </el-option>
      <el-option label="全部" value></el-option>
      <el-option
        v-for="item of tagAList"
        :key="item"
        :label="item"
        :value="item"
      ></el-option>
    </el-select>

    <el-select
      v-if="isScreen"
      v-model="screenVlueCalc"
      placeholder="切屏"
      :size="size"
      class="log-action-bar-select-item log-action-bar-item"
      @change="change({ type: 'changeScree', value: screenVlue })"
    >
      <el-option
        v-for="item in screenList"
        :key="item.value"
        :label="item.label"
        :value="item.value"
      ></el-option>
    </el-select>

    <el-button
      v-if="isReset"
      type="primary"
      icon="el-icon-refresh"
      circle
      :size="size"
      @click="click('reset')"
    ></el-button>

    <el-button
      v-if="isDownload"
      type="primary"
      icon="el-icon-download"
      circle
      :size="size"
      @click="click('download')"
    ></el-button>

    <el-button
      v-if="isZoom"
      type="primary"
      :icon="zoomCalc | filterIcon"
      circle
      :size="size"
      @click="click('zoom')"
      class="log-action-bar-item"
    ></el-button>

    <el-button
      v-if="isGoPage"
      type="primary"
      :size="size"
      @click="goPage"
      class="log-action-bar-item"
      >前往jaeger</el-button
    >

    <el-switch
      v-if="isWrap"
      v-model="wrapCalc"
      @change="switchWrap"
      class="log-action-bar-item"
    ></el-switch>

    <el-select
      v-model="level"
      multiple
      collapse-tags
      placeholder="请选择日志等级"
      :size="size"
      class="log-action-bar-item"
      @change="changeLevel"
    >
      <el-option
        v-for="(value, key) in levelMap"
        :key="key"
        :label="key"
        :value="value"
      ></el-option>
    </el-select>

    <el-autocomplete
      v-if="isQueryId"
      v-model="id"
      :fetch-suggestions="querySearchAsync"
      @select="handleSelect"
      class="input"
      :size="size"
    ></el-autocomplete>
  </section>
</template>

<script>
import { traces } from "@/server/api";
export default {
  name: "log-action-bar",
  data() {
    return {
      tag1: "",
      tag2: "",
      tag3: "",
      screenVlue: null,
      hasWrap: false,
      hasZoom: false,
      id: "",
      // 日志等级
      levelMap: {
        Trace: "T",
        Debug: "D",
        Information: "I",
        Warning: "W",
        Error: "E",
        Critical: "C"
      },
      level: ""
    };
  },
  props: {
    // 屏幕数量
    screen: Number,
    // 控件尺寸
    size: {
      type: String,
      default: "mini"
    },
    // 是否显示切屏控件
    isScreen: {
      type: Boolean,
      default: true
    },
    // 是否显示下载
    isDownload: {
      type: Boolean,
      default: true
    },
    // 是否显示重置
    isReset: {
      type: Boolean,
      default: true
    },
    // 是否显示换行控件
    isWrap: {
      type: Boolean,
      default: true
    },
    // 是否显示缩放
    isZoom: {
      type: Boolean,
      default: true
    },
    // 是否显示前往jaeger
    isGoPage: {
      type: Boolean,
      default: false
    },
    // 是否显示查询id
    isQueryId: {
      type: Boolean,
      default: false
    },
    // 是否显示tag1
    isTag1: {
      type: Boolean,
      default: true
    },
    // 是否显示tag2
    isTag2: {
      type: Boolean,
      default: true
    },
    // 是否显示tag3
    isTag3: {
      type: Boolean,
      default: true
    },
    zoomValue: Boolean,
    tagA: {
      type: String,
      default: ""
    },
    tagB: {
      type: String,
      default: ""
    },
    tagC: {
      type: String,
      default: ""
    },
    wrap: {
      type: Boolean,
      default: false
    },
    // 枚举列表
    tagAList: {
      type: Array | Set,
      default() {
        return [];
      }
    },
    tagBList: {
      type: Array | Set,
      default() {
        return [];
      }
    },
    tagCList: {
      type: Array | Set,
      default() {
        return [];
      }
    },
    screenList: {
      type: Array,
      default() {
        return [
          { value: 1, label: "一屏" },
          { value: 2, label: "二屏" },
          { value: 3, label: "三屏" },
          { value: 4, label: "四屏" }
        ];
      }
    }
  },
  // 利用计算属性来初始化/更新props的值
  computed: {
    screenVlueCalc: {
      get() {
        return this.screen === undefined ? this.screenVlue : this.screen;
      },
      set(newValue) {
        this.screenVlue = newValue;
      }
    },
    tag1Calc: {
      get() {
        return this.tagA === undefined ? this.tag1 : this.tagA;
      },
      set(newValue) {
        this.tag1 = newValue;
      }
    },
    tag2Calc: {
      get() {
        return this.tagB === undefined ? this.tag2 : this.tagB;
      },
      set(newValue) {
        this.tag2 = newValue;
      }
    },
    tag3Calc: {
      get() {
        return this.tagC === undefined ? this.tag3 : this.tagC;
      },
      set(newValue) {
        this.tag3 = newValue;
      }
    },
    wrapCalc: {
      get() {
        return this.wrap === undefined ? this.hasWrap : this.wrap;
      },
      set(newValue) {
        this.hasWrap = newValue;
      }
    },
    zoomCalc: {
      get() {
        return this.zoomValue === undefined ? this.hasZoom : this.zoomValue;
      },
      set(newValue) {
        this.hasZoom = newValue;
      }
    }
  },
  filters: {
    filterIcon(value) {
      return value ? "el-icon-zoom-out" : "el-icon-zoom-in";
    }
  },
  methods: {
    changeLevel(value) {
      this.$emit("change-level", value);
    },
    // select控件切换
    change({ type, value }) {
      this.$emit(type, value);
      this.$refs[value.type] && this.$refs[value.type].blur();
    },
    // button控件点击
    click(type) {
      this[type] && this[type]();
      this.$emit(type, type === "zoom" ? this.hasZoom : undefined);
    },
    // 缩放切换
    zoom() {
      this.zoomCalc = !this.zoomCalc;
    },
    // 刷新重置
    reset() {
      this.$emit("reset");
    },
    // 换行/不换行
    switchWrap() {
      this.$emit("switchWrap", this.hasWrap);
    },
    // 查询id
    async querySearchAsync(queryString, cb) {
      const { data } = await this.$axios.get(`${traces}/${queryString}`);
      cb(data.Data.map(item => ({ value: item })));
    },
    // 跳转详情
    handleSelect(item) {
      const { env, projectName } = this.$route.query;
      let routeData = this.$router.resolve({
        path: `/trace/${item.value}`,
        query: {
          env,
          projectName
        }
      });
      window.open(routeData.href, "_blank");
    },
    // 跳转外链
    goPage() {
      this.$emit("goPage", this.hasWrap);
    }
  }
};
</script>

<style lang="scss">
.log-action-bar {
  display: flex;
  align-items: center;
  box-sizing: border-box;
  height: 40px;
  padding: 0 5px;
  // 搜索项的间距和宽
  .log-action-bar-select-item {
    width: 100px;
  }
  .log-action-bar-item {
    margin-right: 10px;
  }
  .input {
    width: 140px;
    margin: 0 10px;
  }
}
// 修改下拉框高度
.el-scrollbar {
  max-height: calc(50vh - 80px);
}
.el-select-dropdown__wrap {
  max-height: calc(50vh - 80px);
}
</style>
