import type { App } from 'vue'

export default interface IEngine {
  Run(app: App): void
  Priority?(): number | undefined
}
