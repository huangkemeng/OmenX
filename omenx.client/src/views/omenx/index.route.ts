import IndexView from './index-view.vue'
import type {RouteRecordRaw} from "vue-router";

export default [
  {
    path: '/omenx-ui',
    name: 'omenx',
    component: IndexView
  },
] as RouteRecordRaw[]
