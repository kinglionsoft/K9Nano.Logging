module.exports = {
  // publicPath: process.env.NODE_ENV === "production" ? "./" : "/",
  devServer: {
    host: "0.0.0.0", // ip
    port: 8000, // 端口
    proxy: {
      "/signalr": {
        target: "http://192.168.1.118:58961",
        pathRewrite: { "^/signalr": "" }
      },
      "/api": {
        target: "http://192.168.1.118:58961",
        pathRewrite: { "^/api": "api" }
      }
    }
  }
};
