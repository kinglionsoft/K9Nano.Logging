import Vue from "vue";
import Router from "vue-router";

Vue.use(Router);

export default new Router({
  mode: "history",
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/",
      name: "main",
      component: () => import(/* webpackChunkName: "main" */ "./views/Main.vue")
    },
    {
      path: "/home",
      name: "home",
      component: () => import(/* webpackChunkName: "home" */ "./views/Home.vue")
    },
    {
      path: "/trace/:id",
      component: () =>
        import(/* webpackChunkName: "detail" */ "./views/Detail.vue")
    }
  ]
});
