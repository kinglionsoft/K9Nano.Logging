<template>
  <div>
    <el-button type="primary" class="addBtn" icon="el-icon-plus" @click="openDialog"></el-button>
    <el-dialog title="添加" :visible.sync="dialogVisible" width="300px">
      <el-form :model="form" :rules="rules" ref="form" label-width="70px">
        <el-form-item label="tag1" prop="tag1">
          <el-input v-model="form.tag1" :placeholder="formPlaceholder.tag1"></el-input>
        </el-form-item>
        <el-form-item label="tag2" prop="tag2">
          <el-input v-model="form.tag2" :placeholder="formPlaceholder.tag2"></el-input>
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
      form: {
        tag1: "",
        tag2: ""
      },
      formPlaceholder: {
        tag1: "请输入tag1",
        tag2: "请输入tag2"
      },
      rules: {
        tag1: [{ required: true, message: "必填项", trigger: "blur" }],
        tag2: [{ required: true, message: "必填项", trigger: "blur" }]
      }
    };
  },
  methods: {
    openDialog() {
      this.dialogVisible = true;
    },
    cancelDialog() {
      this.dialogVisible = false;
      this.$refs["form"].resetFields();
    },
    validate() {
      this.$refs["form"].validate(async valid => {
        if (!valid) return;
        this.$emit("ok", this.form);
        this.cancelDialog();
      });
    }
  }
};
</script>