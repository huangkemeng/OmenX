<template>
  <div class="bg-gray-50 min-h-screen">
    <div class="container mx-auto px-4 py-8 max-w-5xl">
      <div class="text-center mb-10">
        <h1 class="text-4xl font-bold text-indigo-700 mb-2">OmenX</h1>
        <p class="text-gray-600">Run through all checks to ensure system is functioning
          properly</p>
      </div>
      <div class="flex flex-col md:flex-row gap-8">
        <div class="bg-white rounded-xl shadow-md overflow-hidden p-6 flex-1">
          <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-8">
            <h1 class="text-3xl font-bold text-gray-800 mb-4 md:mb-0">Checklist</h1>
            <button
              v-if="isCheckingAll || checklistItems.some(e=>e.Checking)"
              @click="cancelAllCheck"
              class="bg-red-600 hover:bg-red-700 text-white font-medium py-3 px-6 rounded-lg transition-colors duration-300 flex items-center justify-center"
            >
              Cancel All
            </button>
            <button
              v-else
              @click="startAllChecks"
              class="bg-green-600 hover:bg-green-700 text-white font-medium py-3 px-6 rounded-lg transition-colors duration-300 flex items-center justify-center"
            >
              Check All
            </button>
          </div>
          <loading-wrapper :loading="checkListLoading">
            <div class="space-y-4">
              <div
                v-for="item in checklistItems"
                :key="item.Url"
                class="check-item bg-white border border-gray-200 rounded-lg p-4 flex items-center justify-between hover:-translate-y-0.5 hover:shadow-sm transition-all duration-300"
              >
                <div class="flex-grow">
                  <h3 class="font-medium text-gray-800">{{ item.Title }}</h3>
                  <p class="text-sm text-gray-500 mt-1">{{ item.Description }}</p>
                </div>
                <div class="flex items-center space-x-3">
                  <button
                    v-if="!item.Checking"
                    @click="checkOne(item)"
                    class="bg-green-100 text-green-600 hover:bg-green-200 px-3 py-1 rounded-full text-sm font-medium transition-colors"
                  >
                    Check
                  </button>
                  <button
                    v-else
                    @click="cancelCheck(item)"
                    class="bg-red-100 text-red-600 hover:bg-red-200 px-3 py-1 rounded-full text-sm font-medium transition-colors"
                  >
                    Cancel
                  </button>

                  <a-popover title="Results" v-if="item.IsChecked && item.CheckResults.length > 0">
                    <template #content>
                      <p v-for="result in item.CheckResults" :key="result.Result"
                         :class="statusClasses[result.Result=='Error'?'error':result.Result=='Warning'?'warning':'success']">
                        <span>{{ result.Message }}</span>
                      </p>
                    </template>
                    <span
                      :class="[
                    'text-xs font-medium px-3 py-1 rounded-full flex items-center',
                    statusClasses[item.Status]
                  ]"
                    >
                  <i
                    :class="[statusIcons[item.Status], item.Status === 'checking' ? 'spin' : '']"></i>
                  {{ statusTexts[item.Status] }}
                </span>
                  </a-popover>
                  <span v-else
                        :class="[
                    'text-xs font-medium px-3 py-1 rounded-full flex items-center',
                    statusClasses[item.Status]
                  ]"
                  >
                  <i
                    :class="[statusIcons[item.Status], item.Status === 'checking' ? 'spin' : '']"></i>
                  {{ statusTexts[item.Status] }}
                </span>

                  <button
                    v-if="item.Status !== 'pending'"
                    @click="checkOne(item)"
                    class="text-gray-400 hover:text-blue-500 transition-colors"
                  >
                    <i class="fas fa-sync-alt"></i>
                  </button>
                </div>
              </div>
            </div>
          </loading-wrapper>
        </div>

        <div class="bg-white rounded-xl shadow-md overflow-hidden p-6 h-fit md:w-72 lg:w-80">
          <h2 class="text-xl font-semibold text-gray-700 mb-6 border-b pb-4">Check Summary</h2>
          <div class="grid grid-cols-1 gap-4">
            <div class="bg-green-50 p-4 rounded-lg flex items-center w-full">
              <div class="bg-green-100 p-3 rounded-full mr-4">
                <i class="fas fa-check-circle text-green-600 text-xl"></i>
              </div>
              <div class="flex-1">
                <p class="text-gray-500 text-sm">Success</p>
                <p class="text-xl font-bold text-gray-800">{{ successCount }}</p>
              </div>
            </div>
            <div class="bg-yellow-50 p-4 rounded-lg flex items-center w-full">
              <div class="bg-yellow-100 p-3 rounded-full mr-4">
                <i class="fas fa-exclamation-triangle text-yellow-600 text-xl"></i>
              </div>
              <div class="flex-1">
                <p class="text-gray-500 text-sm">Warnings</p>
                <p class="text-xl font-bold text-gray-800">{{ warningCount }}</p>
              </div>
            </div>
            <div class="bg-red-50 p-4 rounded-lg flex items-center w-full">
              <div class="bg-red-100 p-3 rounded-full mr-4">
                <i class="fas fa-times-circle text-red-600 text-xl"></i>
              </div>
              <div class="flex-1">
                <p class="text-gray-500 text-sm">Failures</p>
                <p class="text-xl font-bold text-gray-800">{{ failureCount }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref, computed, onMounted} from 'vue'
import apiClient from "@/apis/ApiClient.ts";
import axios, {type CancelTokenSource} from "axios";
import {message} from "ant-design-vue";
import loadingWrapper from '@/components/loading-wrapper/index-view.vue'

interface ChecklistItem {
  Url: string
  Title: string
  Description: string
  IsChecked: boolean
  Checking: boolean
  CancelTokenSource: CancelTokenSource
  Status: 'pending' | 'checking' | 'success' | 'error' | 'warning',
  CheckResults: { Result: string, Message: string }[]
}

const checklistItems = ref<ChecklistItem[]>([])

const isCheckingAll = ref(false)

const statusClasses = {
  pending: 'bg-gray-200 text-gray-600',
  checking: 'bg-blue-100 text-blue-600',
  success: 'bg-green-100 text-green-600',
  error: 'bg-red-100 text-red-600',
  warning: 'bg-yellow-100 text-yellow-600'
}

const statusIcons = {
  pending: 'far fa-clock',
  checking: 'fas fa-spinner',
  success: 'fas fa-check-circle',
  error: 'fas fa-times-circle',
  warning: 'fas fa-exclamation-triangle'
}

const statusTexts = {
  pending: 'Pending',
  checking: 'Checking...',
  success: 'Success',
  error: 'Error',
  warning: 'Warning'
}

const successCount = computed(() => checklistItems.value.filter(item => item.Status === 'success').length)
const warningCount = computed(() => checklistItems.value.filter(item => item.Status === 'warning').length)
const failureCount = computed(() => checklistItems.value.filter(item => item.Status === 'error').length)

const cancelCheck = (item: ChecklistItem) => {
  if (item) {
    if (item.CancelTokenSource) {
      item.CancelTokenSource.cancel('Check cancelled')
    }
    item.Checking = false;
    if (!item.IsChecked) {
      item.Status = 'pending'
    }
  }
}

const checkOne = async (item: ChecklistItem) => {
  item.CancelTokenSource = axios.CancelToken.source()
  await checkItem(item);
}

const checkItem = async (item: ChecklistItem) => {
  const oldStatus = item.Status;
  item.Status = 'checking'
  item.Checking = true;
  try {
    item.IsChecked = true;
    const postRes = await axios.post(item.Url, {}, {
      cancelToken: item.CancelTokenSource.token
    });
    if (postRes.status == 200) {
      const data = postRes.data as { Message: string, Result: string }[];
      item.CheckResults = data;
      if (data == null || data.length == 0) {
        item.Status = 'success';
      } else {
        if (data.some(item => item.Result == "Error")) {
          item.Status = 'error';
        } else if (data.some(item => item.Result == "Warning")) {
          item.Status = 'warning';
        } else {
          item.Status = 'success';
        }
      }
    } else {
      item.Status = 'error';
      item.CheckResults = [{
        Message: 'Exception occurred during check,please check the internal error details of the checkpoint',
        Result: 'Error'
      }];
    }
  } catch (e) {
    if (!axios.isCancel(e)) {
      item.Status = 'error';
      item.CheckResults = [{
        Message: 'Exception occurred during check,please check the internal error details of the checkpoint',
        Result: 'Error'
      }];
    } else {
      item.Status = oldStatus;
    }
  } finally {
    item.Checking = false;
  }
}

function chunkArray(array: Array<string | number | object>, chunkSize: number) {
  const chunks = [];
  for (let i = 0; i < array.length; i += chunkSize) {
    chunks.push(array.slice(i, i + chunkSize));
  }
  return chunks;
}


const checkListLoading = ref(false);
const getAllChecks = async () => {
  try {
    const getRes = await apiClient.OmenX.Omen_xChecklists()
      .loadingRequest(checkListLoading);
    if (getRes.status == 200) {
      checklistItems.value = getRes.data.map<ChecklistItem>(e => ({
        ...(e as ChecklistItem),
        Status: 'pending',
        CheckResults: []
      }));
    }
  } catch {
    message.error('loading checklist failed,please check network')
  }
}

const cancelAllCheck = () => {
  checklistItems.value.forEach(item => {
    cancelCheck(item)
  })
  isCheckingAll.value = false
}
onMounted(async () => {
  await getAllChecks();
})


const startAllChecks = async () => {
  isCheckingAll.value = true
  try {
    const itemArrays = chunkArray(checklistItems.value, 5) as ChecklistItem[][]
    itemArrays.forEach(items => {
      items.forEach(item => {
        item.CancelTokenSource = axios.CancelToken.source()
      })
    })
    for (const items of itemArrays) {
      await Promise.allSettled(items.map(item => {
        return checkItem(item)
      }))
    }
  } finally {
    isCheckingAll.value = false
  }
}
</script>
