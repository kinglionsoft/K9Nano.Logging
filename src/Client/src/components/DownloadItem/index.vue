<template>
  <div>
    <el-button type="text" @click.stop="openDialog">下载</el-button>
    <el-dialog
      title="时间间隔"
      :visible.sync="dialogVisible"
      width="600px"
      append-to-body
    >
      <el-form :model="form" :rules="rules" ref="form" label-width="70px">
        <el-form-item label="date" prop="date">
          <el-date-picker
            v-model="form.date"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            align="right"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-form-item>
        <el-form-item label="log">
          <el-select v-model="form.log" multiple>
            <el-option
              v-for="item in logTypeList"
              :key="item.id"
              :label="item.value"
              :value="item.id"
            >
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancelDialog">取 消</el-button>
        <el-button type="primary" @click="validate">确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
export default {
  data() {
    return {
      dialogVisible: false,
      logTypeList: [
        {
          value: "ComputeName",
          id: 2
        },
        {
          value: "ClientTag",
          id: 3
        },
        {
          value: "CategoryName",
          id: 4
        }
      ],
      form: {
        date: [],
        log: []
      },
      rules: {
        date: [{ required: true, message: "必填项", trigger: "change" }]
      }
    };
  },
  methods: {
    openDialog() {
      this.dialogVisible = true;
      const date = new Date().toLocaleDateString("zh-Ch").replace(/\//g, "-");
      this.form.date = [date, date];
    },
    cancelDialog() {
      this.dialogVisible = false;
      this.$refs["form"].resetFields();
    },
    validate() {
      this.$refs["form"].validate(async valid => {
        if (!valid) return;
        let [startDate, endDate] = this.form.date;
        startDate = `${startDate} 00:00:00`;
        endDate = `${endDate} 23:59:59.999`;
        // 如果没有选就传默认值
        if (!this.form.log.length) {
          this.form.log = [0];
        }
        this.$emit("ok", [[startDate, endDate], this.form.log]);
        this.cancelDialog();
        this.form.log = [];
      });
    }
  }
};
</script>

<style lang="scss" scoped></style>
