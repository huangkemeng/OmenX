import type IEngine from './IEngine'
import {useRouteStore} from '../stores/routeStore'
import {createRouter, createWebHistory, type RouteRecordRaw} from 'vue-router'
import type {App} from 'vue'


const routeModules = import.meta.glob('/src/views/**/**.route.(ts|js)', {
  eager: true
})

export default class RouteEngine implements IEngine {
  public Run(app: App): void {
    const routes: RouteRecordRaw[] = []
    const routeStore = useRouteStore()
    for (const [, value] of Object.entries(routeModules)) {
      const routeType = value as {
        default: RouteRecordRaw[]
      }
      if (routeType.default) {
        const pageRoutes: RouteRecordRaw[] = routeType.default
        if (pageRoutes && pageRoutes.length) {
          pageRoutes.forEach((pageRoute) => {
            pageRoute.beforeEnter = (to, from, next) => {
              routeStore.currentRoute = to
              let version = import.meta.env.VITE_APP_VERSION
              if (version) {
                version = `(${version})`
              } else {
                version = ''
              }
              let title = import.meta.env.VITE_APP_TITLE + version
              if (to.meta.title) {
                title = String(to.meta.title) + ' - ' + title
              }
              document.title = title
              next()
            }
            routes.push(pageRoute)
          })
        }
      }
    }
    const router = createRouter({
      history: createWebHistory(),
      routes: routes
    })
    router.beforeEach(async (to, from, next) => {
      return next();
    })
    app.use(router)
  }

  public Priority() {
    return -1
  }
}
