import type IEngine from './IEngine'
import axios from "axios";

export default class AxiosEngine implements IEngine {
  Run(): void {
    axios.interceptors.request.use(async config => {
      return config;
    })
  }
}
