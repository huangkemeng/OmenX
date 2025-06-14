import {defineStore} from 'pinia'
import type {RouteLocationNormalized} from 'vue-router'

export const useRouteStore = defineStore('routeStore', {
    state: () => {
        return {
            currentRoute: {} as RouteLocationNormalized
        }
    }
})
