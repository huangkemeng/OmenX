import type {Ref} from "vue";

declare  global {
  interface Promise<T> {
    loadingRequest(loading: Ref<boolean, boolean>): Promise<T>;

    showErrorMessage(): Promise<T>;
  }
}



