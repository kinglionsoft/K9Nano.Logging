import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import "./plugins/element.js";
import "normalize.css";
import "./assets/global.scss";
import "./plugins/eventBus";
import Axios from "./plugins/Axios";

Vue.config.productionTip = false;

Vue.prototype.$axios = Axios;

new Vue({
  router,
  render: h => h(App)
}).$mount("#app");
