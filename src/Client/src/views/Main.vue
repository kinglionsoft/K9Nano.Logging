<template>
  <div class="index">
    <div class="main">
      <div class="title">
        <el-autocomplete
          v-model="id"
          :fetch-suggestions="querySearchAsync"
          @select="handleSelect"
          class="input"
          size="mini"
          placeholder="请输入trace id"
        ></el-autocomplete>
        <h1>日志系统</h1>
        <AddItem @ok="handelAdd" class="add-btn"></AddItem>
      </div>
      <el-table :data="list" style="width: 100%" max-height="570px" size="mini">
        <el-table-column prop="Name" label="名字"></el-table-column>
        <el-table-column
          prop="Count"
          label="总条数"
          width="100"
        ></el-table-column>
        <el-table-column label="操作" width="140">
          <template slot-scope="scope">
            <div class="btn-group">
              <el-button type="text" @click.stop="go(scope.row)" class="item"
                >前往</el-button
              >
              <DownloadItem
                @ok="handelDownload($event, scope.row)"
                class="item"
              ></DownloadItem>
              <el-button
                type="text"
                @click.stop="removeItem(scope.row)"
                v-show="$route.query.username"
                class="item"
                >删除</el-button
              >
            </div>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import { projects, DownLogs, traces } from "@/server/api";
import { objToSearchString } from "@/utils.js";
import AddItem from "@/components/AddItem";
import DownloadItem from "@/components/DownloadItem";
export default {
  name: "index",
  data() {
    return {
      list: [],
      id: ""
    };
  },
  components: {
    AddItem,
    DownloadItem
  },
  mounted() {
    this.getData();
  },
  methods: {
    async getData() {
      const { data } = await this.$axios.get(projects);
      this.list = data.Data;
    },
    go({ Tags }) {
      let query = {
        projectName: Tags[0],
        env: Tags[1]
      };
      this.$router.push(`home${encodeURI(objToSearchString(query))}`);
    },
    async handelAdd({ tag1, tag2 }) {
      const parmas = {
        tags: [tag1, tag2]
      };
      const { data } = await this.$axios.post(projects, parmas);
      if (data.IsSuccess) {
        this.$message.success(data.Msg);
        this.getData();
        return;
      }
      this.$message.error(data.Msg);
    },
    removeItem(item) {
      this.$confirm("确认删除?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(async () => {
          const { data } = await this.$axios({
            method: "delete",
            url: `${projects}/${item.Id}`,
            data: {
              UserName: this.$route.query.username
            }
          });
          if (data.IsSuccess) {
            this.$message.success(data.Msg);
            this.getData();
            return;
          }
          this.$message.error(data.Msg);
        })
        .catch(() => {});
    },
    async handelDownload([[StartTime, EndTime], log], item) {
      const params = {
        StartTime,
        EndTime,
        FileName: item.Name,
        tags: item.Tags,
        FormatType: log
      };
      const config = {
        responseType: "blob"
      };
      const res = await this.$axios
        .post(DownLogs, params, config)
        .catch(err => {
          let file = new FileReader();
          file.readAsText(err.response.data);
          file.onload = res => {
            let result = JSON.parse(res.currentTarget.result);
            this.$message.error(result.Msg);
          };
        });
      if (!res) return;
      const filename = this.getFileName(res.headers["content-disposition"]);
      const url = URL.createObjectURL(res.data);
      const a = document.createElement("a");
      a.href = url;
      document.body.appendChild(a);
      a.download = filename;
      a.click();
      document.body.removeChild(a);
    },
    getFileName(str) {
      let fliename = "";
      if (/filename\*/.test(str)) {
        //当存在 filename* 时，取filename* 并进行解码（为了解决中文乱码问题）
        let list = str.match(/(filename\*)=(.*)/);
        fliename = list[list.length - 1].replace(/.*''/, "");
      } else {
        let list = str.match(/(filename)=(.*)/);
        fliename = list[list.length - 1].replace(/;/, "");
      }
      return fliename;
    },
    // 查询id
    async querySearchAsync(queryString, cb) {
      const { data } = await this.$axios.get(`${traces}/${queryString}`);
      cb(data.Data.map(item => ({ value: item })));
    },
    // 跳转详情
    handleSelect(item) {
      let routeData = this.$router.resolve({
        path: `/trace/${item.value}`
      });
      window.open(routeData.href, "_blank");
    }
  }
};
</script>

<style lang="scss">
.index {
  display: flex;
  height: 100vh;
  justify-content: center;
  align-items: center;
  .main {
    box-sizing: border-box;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    width: 800px;
    height: 600px;
    border-radius: 4px;
    border: 1px solid #dcdfe6;
    padding: 0 60px;
    .title {
      display: flex;
      position: relative;
      height: 30px;
      width: 100%;
      justify-content: space-between;
      align-items: center;
      margin: 20px 0;
      h1 {
        font-size: 25px;
        height: 100%;
        margin: 0;
      }
    }
    .btn-group {
      display: flex;
      align-items: center;
      .item {
        margin-right: 10px;
      }
    }
  }
}
</style>
