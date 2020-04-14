<template>
  <log-screen>
    <log-screen-item
      v-for="item in screen"
      :key="item"
      :item="item"
      :total="screen"
      :zoom="isShow[item - 1]"
    >
      <log-action-bar
        :tag-a="tagA[item - 1]"
        :tag-b="tagB[item - 1]"
        :tag-c="tagC[item - 1]"
        :tag-a-list="tagAList[item - 1]"
        :tag-b-list="tagBList[item - 1]"
        :tag-c-list="tagCList[item - 1]"
        :zoom-value="isShow[item - 1]"
        :wrap="wrap[item - 1]"
        :screen="screen"
        :isScreen="item === 1"
        :isDownload="false"
        :isQueryId="item === 1"
        @changeTag="changeTag($event,item - 1)"
        @changeScree="changeScreen"
        @switchWrap="switchWrap('wrap',$event,item - 1)"
        @zoom="zoom($event,item - 1)"
        @reset="reset(item - 1)"
        @change-level="changeLevel($event,item - 1)"
      />
      <log-list
        :list="screenList[item - 1]"
        :height="`calc(${calcHeight(screen)}vh - 40px)`"
        :isWrap="wrap[item - 1]"
        :zoom="isShow[item - 1]"
      />
    </log-screen-item>
  </log-screen>
</template>

<script>
import ActionBar from "@/components/ActionBar";
import LogList from "@/components/LogList";
import Screen from "@/components/Screen/screen.vue";
import ScreenItem from "@/components/Screen/screenItem.vue";
import { getQuery, decodeURIObj, objToSearchString } from "@/utils.js";
import "@/plugins/signalr.min.js";

export default {
  name: "home",
  data() {
    return {
      tagAList: [],
      tagBList: [],
      tagCList: [],
      tagA: [],
      tagB: [],
      tagC: [],
      wrap: [],
      screen: 1,
      screenList: [],
      list: [],
      isShow: [],
      connection: null,
      guid: null,
      maxLength: 2000,
      projectName: null,
      env: null,
      hash: {}
    };
  },
  components: {
    [ActionBar.name]: ActionBar,
    [LogList.name]: LogList,
    [Screen.name]: Screen,
    [ScreenItem.name]: ScreenItem
  },
  mounted() {
    this.init();
    this.getData();
    this.setTagTitle();
    // this.mockDataAsync();
    // this.mockDataSync();
  },
  methods: {
    // 测试异步数据
    mockDataAsync() {
      this.screenList = [...new Array(this.screen).keys()].map(_ => []);
      this.isShow = [...new Array(this.screen).keys()].map(_ => false);
      // 模拟数据
      let index = 0;
      setInterval(() => {
        let data = [
          `Lorem ipsum dolor sit amet, consectetur adipisicing elit. Sint autem officiis ipsam ipsa nesciunt voluptas et. Ab, repellendus quos dignissimos quibusdam est, eos enim similique quas labore nostrum inventore praesentium!${index++}!\r\n`,
          `Lorem ipsum dolor sit amet, consectetur adipisicing elit. Sint autem officiis ipsam ipsa nesciunt voluptas et. Ab, repellendus quos dignissimos quibusdam est, eos enim similique quas labore nostrum inventore praesentium!${index++}!\r\n`,
          `Lorem ipsum dolor sit amet, consectetur adipisicing elit. Sint autem officiis ipsam ipsa nesciunt voluptas et. Ab, repellendus quos dignissimos quibusdam est, eos enim similique quas labore nostrum inventore praesentium!${index++}!\r\n`
        ];
        this.screenList[0].push(...data);
      }, 1000);
    },
    // 测试同步数据
    mockDataSync() {
      let index = 0;
      let data = Array.from(
        { length: 100 },
        () =>
          `2019-06-12 04:49:27.470 I 1*    ctc - dev - tag1 - cn1 ${++index}\r\n`
      );
      this.tagCList = Array.from({ length: 100 }, () => ++index);
      this.screenList[0] = data;
      this.screenList[1] = data;
      this.screenList[2] = data;
      this.screenList[3] = data;
      document.addEventListener("keydown", e => {
        if (e.keyCode === 13) {
          this.$set(this.screenList, 0, [
            ...this.screenList[0],
            `测试数据${index++}\r\n`
          ]);
        }
      });
    },
    // 获取数据
    getData() {
      let tag1 = [];
      let tag2 = [];
      let tag3 = [];
      this.screenList = [...new Array(this.screen).keys()].map(_ => []);
      this.isShow = [...new Array(this.screen).keys()].map(_ => false);
      this.guid = [...new Array(this.screen).keys()].map(_ =>
        this.createGuid()
      );
      // 创建
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(process.env.VUE_APP_SIGNALR_URL)
        .build();
      // 历史消息
      this.connection.on("HistoryLogs", (...rest) => {
        const [guid, index, messages, tags] = rest;
        tag1[index] = [];
        tag2[index] = [];
        tag3[index] = [];
        tags.forEach(([temp1, temp2, tagA, tagB, tagC]) => {
          if (tagA !== "") tag1[index].push(tagA);
          if (tagB !== "") tag2[index].push(tagB);
          if (tagC !== "") tag3[index].push(tagC);
        });

        if (guid === this.guid[index]) {
          this.screenList[index].unshift(...messages.reverse());
          this.$set(this.tagAList, index, new Set(tag1[index]));
          this.$set(this.tagBList, index, new Set(tag2[index]));
          this.$set(this.tagCList, index, new Set(tag3[index]));
        }
      });

      // 消息通知
      this.connection.on("ReceiveLog", (...rest) => {
        const [guid, index, messages, tags] = rest;
        tags.forEach(([temp1, temp2, tagA, tagB, tagC]) => {
          tag1[index].push(tagA);
          tag2[index].push(tagB);
          tag3[index].push(tagC);
        });
        // 去重过滤条件
        if (guid === this.guid[index]) {
          this.screenList[index].push(...messages);
          this.$set(this.tagAList, index, new Set(tag1[index]));
          this.$set(this.tagBList, index, new Set(tag2[index]));
          this.$set(this.tagCList, index, new Set(tag3[index]));
        }

        if (this.screenList[index].length > this.maxLength) {
          // 超过最大值，删除最大数量的10%
          this.screenList[index].splice(0, this.maxLength*0.1);
        }
      });
      // 启动
      this.connection
        .start()
        .then(() => {
          console.log("连接成功");
          // 有几个屏触发几次
          this.guid.forEach((item, index) => {
            this.getHistory(item, index);
          });
        })
        .catch(err => console.log("错误捕获", err.toString()));
    },
    // tag控件切换
    changeTag({ type, value }, index) {
      let params = decodeURIObj(getQuery(location.href));
      params[type][index] = value;
      // if (type === "tagA") {
      //   params["tagB"][index] = "";
      //   params["tagC"][index] = "";
      // } else if (type === "tagB") {
      //   params["tagC"][index] = "";
      // }
      this.changeUrl(params);
      this.init();
      this.guid[index] = this.createGuid();
      this.screenList.splice(index, 1, []);
      this.getHistory(this.guid[index], index);
    },
    // 日志等级切换
    changeLevel(value, index) {
      this.screenList.splice(index, 1, []);
      this.getHistory(this.guid[index], index, value);
    },
    // 主动触发查询历史记录
    getHistory(guid, index, level = []) {
      this.connection.invoke(
        "QueryHistoryLogs",
        guid,
        index,
        [
          this.projectName,
          this.env,
          this.tagA[index],
          this.tagB[index],
          this.tagC[index]
        ],
        level
      );
    },
    // 清屏
    reset(index) {
      this.$set(this.screenList, index, []);
    },
    // 生成guid
    createGuid() {
      return Date.now().toString();
    },
    // 切屏控件切换
    changeScreen(value) {
      this.screen = value;
      let obj = this.transformObj(this.screen);
      this.changeUrl(obj);
      this.init();
      this.screenList = [...new Array(this.screen).keys()].map(_ => []);
      this.isShow = [...new Array(this.screen).keys()].map(_ => false);
      this.guid = [...new Array(this.screen).keys()].map(_ =>
        this.createGuid()
      );
      this.guid.forEach((item, index) => {
        this.getHistory(item, index);
      });
    },
    // 缩放
    zoom(value, index) {
      if (value) {
        // 放大
        this.isShow = [...new Array(this.screen).keys()].map(_ => false);
        this.isShow[index] = value;
      } else {
        // 缩小
        this.isShow = [...new Array(this.screen).keys()].map(_ => false);
      }
    },
    // 切换是否换行
    switchWrap(type, value, index) {
      let params = decodeURIObj(getQuery(location.href));
      params[type][index] = value;
      this.changeUrl(params);
      this.init();
    },
    // 计算高度
    calcHeight(total) {
      return 100 / Math.ceil(total / 2);
    },
    // 改变url
    changeUrl(obj) {
      let queryString = encodeURI(objToSearchString(obj));
      history.pushState(
        null,
        null,
        `${location.origin + location.pathname + queryString}`
      );
    },
    // 初始化
    init() {
      let params = decodeURIObj(getQuery(location.href));
      if (!params.screen) {
        let obj = this.transformObj(this.screen);
        this.changeUrl(obj);
        params = decodeURIObj(getQuery(location.href));
      }
      this.screen = +params.screen;
      this.tagA = params.tagA;
      this.tagB = params.tagB;
      this.tagC = params.tagC;
      this.wrap = params.wrap;
      this.projectName = params.projectName;
      this.env = params.env;
    },
    // 根据屏幕数量转载装载对象
    transformObj(num) {
      let { projectName, env } = decodeURIObj(getQuery(location.href));
      let result = {
        screen: num,
        wrap: [...new Array(num).keys()].map(_ => false),
        tagA: [...new Array(num).keys()].map(_ => ""),
        tagB: [...new Array(num).keys()].map(_ => ""),
        tagC: [...new Array(num).keys()].map(_ => ""),
        projectName,
        env
      };
      return result;
    },
    // 设置title
    setTagTitle() {
      document.title = `log-${decodeURIObj(getQuery(location.href)).env}`;
    }
  }
};
</script>