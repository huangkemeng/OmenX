import type IEngine from './IEngine'

import type {App} from 'vue'

const compModules = import.meta.glob('/src/components/**/**.comp.vue', {
  eager: true
})

export default class ComponentEngine implements IEngine {
  public Run(app: App): void {
    for (const [path, value] of Object.entries(compModules)) {
      const compType = value as {
        default: { name: string }
      }
      const comp = compType.default
      if (!comp.name) {
        const paths = path.split('/')
        if (paths.length > 1) {
          comp.name = paths[paths.length - 2]
        } else {
          comp.name = paths[paths.length - 1].replace('.comp.vue', '').replace('.vue', '')
        }
      }
      const compInstall = {
        install(_app: App) {
          _app.component(comp.name, comp)
        }
      }
      app.use(compInstall)
    }
  }

  public Priority() {
    return 10
  }
}
