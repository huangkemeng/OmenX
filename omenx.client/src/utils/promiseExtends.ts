import type {Ref} from "vue";
import {AxiosError} from "axios";
import {message} from 'ant-design-vue'

Promise.prototype.loadingRequest = async function <T>(this: Promise<T>, loading: Ref<boolean, boolean>): Promise<T> {
  try {
    loading.value = true;
    return await this;
  } catch (e) {
    throw e;
  } finally {
    loading.value = false;
  }
}


Promise.prototype.showErrorMessage = async function <T>(this: Promise<T>): Promise<T> {
  try {
    return await this;
  } catch (e) {
    if (e instanceof AxiosError) {
      if (e.status == 401) {
        message.error('你未登录或登录已过期')
      } else if (e.status == 403) {
        message.error('你没有权限进行此项操作')
      } else if (e.status == 400 && e.response && e.response.data.detail) {
        message.error(e.response.data.detail);
      }
    }
    throw e;
  }
}
