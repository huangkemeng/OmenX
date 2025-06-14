import './assets/main.css'
import EngineBus from '@/engines/EngineBus'
import Ant from 'ant-design-vue'
import '@/utils/promiseExtends.ts'
import {createPinia} from 'pinia'
import {createApp} from 'vue'
import piniaPersistedstatePlugin from 'pinia-plugin-persistedstate'
import App from './App.vue'

const app = createApp(App)
app.use(Ant)
const pinia = createPinia();
pinia.use(piniaPersistedstatePlugin)
app.use(pinia)
EngineBus.RunEngine(app)
app.mount('#app')

