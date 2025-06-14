const engines = import.meta.glob('/src/engines/*.(ts|js)', {eager: true})
import type {App} from 'vue'

export default class EngineBus {
  static RunEngine(app: App) {
    const orderedEngines: { Priority: number; Run: (app: App) => void }[] = []
    for (const [, value] of Object.entries(engines)) {
      const engineType = value as {
        default: { prototype: { Run: () => void; Priority: () => void } }
      }
      if (engineType.default && engineType.default.prototype) {
        const engine: typeof engineType.default.prototype = Object.create(
          engineType.default.prototype
        )
        const run = engine.Run
        if (run && typeof run === 'function') {
          let priority = 0
          if (engine.Priority && typeof engine.Priority == 'function') {
            priority = engine.Priority() ?? 0
          }
          orderedEngines.push({
            Priority: priority,
            Run: run
          })
        }
      }
    }
    if (orderedEngines.length) {
      orderedEngines
        .sort((a, b) => a.Priority - b.Priority)
        .forEach((engine) => {
          engine.Run(app)
        })
    }
  }
}
