<template>
  <log-screen>
    <log-screen-item :item="1" :total="1">
      <log-action-bar
        :isDownload="false"
        :isScreen="false"
        :isReset="false"
        :isZoom="false"
        :isTag1="false"
        :isTag2="false"
        :isTag3="false"
        :isGoPage="true"
        :wrap="wrap"
        @switchWrap="switchWrap"
        @goPage="goPage"
        @change-level="changeLevel"
      />
      <log-list
        :list="screenList"
        :height="`calc(100vh - 40px)`"
        :isWrap="wrap"
      />
    </log-screen-item>
  </log-screen>
</template>

<script>
import ActionBar from "@/components/ActionBar";
import LogList from "@/components/LogList";
import Screen from "@/components/Screen/screen.vue";
import ScreenItem from "@/components/Screen/screenItem.vue";
import "@/plugins/signalr.min.js";
export default {
  name: "detail",
  components: {
    [ActionBar.name]: ActionBar,
    [LogList.name]: LogList,
    [Screen.name]: Screen,
    [ScreenItem.name]: ScreenItem
  },
  data() {
    return {
      wrap: false,
      screenList: []
    };
  },
  mounted() {
    this.getData();
  },
  methods: {
    changeLevel(value) {
      this.screenList = [];
      this.queryData(value);
    },
    switchWrap(val) {
      this.wrap = val;
    },
    goPage() {
      window.open(this.jaegerUrl + `/trace/${this.$route.params.id}`);
    },
    // 获取数据
    getData() {
      // 082e13
      this.screenList = [];
      this.guid = this.createGuid();
      // 创建
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(process.env.VUE_APP_SIGNALR_TRACE_URL)
        .build();
      // 查询详情
      this.connection.on("TraceLogs", (...rest) => {
        const [jaegerUrl, guid, messages] = rest;
        this.jaegerUrl = jaegerUrl;
        if (guid === this.guid) {
          this.screenList.unshift(...messages);
        }
      });
      // 启动
      this.connection
        .start()
        .then(() => {
          console.log("连接成功");
          this.queryData();
        })
        .catch(err => console.log("错误捕获", err.toString()));
    },
    queryData(level = []) {
      const { projectName, env } = this.$route.query;
      const { id } = this.$route.params;
      let arr = projectName
        ? [
            projectName.replace(/\"/g, ""),
            env.replace(/\"/g, "").replace(/\"/g, "")
          ]
        : [];
      this.connection.invoke("QueryTraceLogs", this.guid, id, arr, level);
    },
    // 生成guid
    createGuid() {
      return Date.now().toString();
    }
  }
};
</script>
