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
              @click="startAllChecks"
              :disabled="isCheckingAll"
              class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-6 rounded-lg transition-colors duration-300 flex items-center justify-center"
            >
              <i
                :class="['mr-2', isCheckingAll ? 'fas fa-spinner spin' : 'fas fa-play-circle']"></i>
              {{ isCheckingAll ? 'Checking...' : 'Start Check' }}
            </button>
          </div>

          <div class="space-y-4">
            <div
              v-for="item in checklistItems"
              :key="item.id"
              class="check-item bg-white border border-gray-200 rounded-lg p-4 flex items-center justify-between hover:-translate-y-0.5 hover:shadow-sm transition-all duration-300"
            >
              <div class="flex-grow">
                <h3 class="font-medium text-gray-800">{{ item.title }}</h3>
                <p class="text-sm text-gray-500 mt-1">{{ item.subtitle }}</p>
              </div>
              <div class="flex items-center space-x-3">
                <button
                  v-if="item.status === 'pending'"
                  @click="checkItem(item.id)"
                  class="bg-blue-100 text-blue-600 hover:bg-blue-200 px-3 py-1 rounded-full text-sm font-medium transition-colors"
                >
                  <i class="fas fa-play mr-1"></i> Check
                </button>
                <span
                  :class="[
                    'text-xs font-medium px-3 py-1 rounded-full flex items-center',
                    statusClasses[item.status]
                  ]"
                >
                  <i
                    :class="[statusIcons[item.status], item.status === 'loading' ? 'spin' : '']"></i>
                  {{ statusTexts[item.status] }}
                </span>
                <button
                  v-if="item.status !== 'pending'"
                  @click="checkItem(item.id)"
                  class="text-gray-400 hover:text-blue-500 transition-colors"
                >
                  <i class="fas fa-sync-alt"></i>
                </button>
              </div>
            </div>
          </div>
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
import {ref, computed} from 'vue'

interface ChecklistItem {
  id: number
  title: string
  subtitle: string
  status: 'pending' | 'loading' | 'success' | 'failure' | 'warning'
}

const checklistItems = ref<ChecklistItem[]>([
  {
    id: 1,
    title: "Database Connection",
    subtitle: "Verify connection to primary database",
    status: "pending"
  },
  {id: 2, title: "API Endpoints", subtitle: "Check all critical API endpoints", status: "pending"},
  {
    id: 3,
    title: "File System Permissions",
    subtitle: "Validate write permissions in data directories",
    status: "pending"
  },
  {
    id: 4,
    title: "External Service Integration",
    subtitle: "Test connectivity to third-party services",
    status: "pending"
  },
  {
    id: 5,
    title: "Security Configuration",
    subtitle: "Verify TLS certificates and security headers",
    status: "pending"
  },
  {
    id: 6,
    title: "Backup System",
    subtitle: "Check recent backup completion status",
    status: "pending"
  }
])

const isCheckingAll = ref(false)

const statusClasses = {
  pending: 'bg-gray-200 text-gray-600',
  loading: 'bg-blue-100 text-blue-600',
  success: 'bg-green-100 text-green-600',
  failure: 'bg-red-100 text-red-600',
  warning: 'bg-yellow-100 text-yellow-600'
}

const statusIcons = {
  pending: 'far fa-clock',
  loading: 'fas fa-spinner',
  success: 'fas fa-check-circle',
  failure: 'fas fa-times-circle',
  warning: 'fas fa-exclamation-triangle'
}

const statusTexts = {
  pending: 'Pending',
  loading: 'Checking...',
  success: 'Success',
  failure: 'Failed',
  warning: 'Warning'
}

const successCount = computed(() => checklistItems.value.filter(item => item.status === 'success').length)
const warningCount = computed(() => checklistItems.value.filter(item => item.status === 'warning').length)
const failureCount = computed(() => checklistItems.value.filter(item => item.status === 'failure').length)

const checkItem = (itemId: number) => {
  const item = checklistItems.value.find(i => i.id === itemId)
  if (!item) return

  item.status = 'loading'

  const delay = 1000 + Math.random() * 2000

  setTimeout(() => {
    const random = Math.random()
    if (random < 0.6) {
      item.status = 'success'
    } else if (random < 0.8) {
      item.status = 'warning'
    } else {
      item.status = 'failure'
    }
  }, delay)
}

const startAllChecks = () => {
  isCheckingAll.value = true

  checklistItems.value.forEach(item => {
    if (item.status !== 'loading') {
      checkItem(item.id)
    }
  })

  const loadingItems = checklistItems.value.filter(item => item.status === 'loading')
  if (loadingItems.length === 0) {
    isCheckingAll.value = false
  } else {
    const checkInterval = setInterval(() => {
      const stillLoading = checklistItems.value.filter(item => item.status === 'loading')
      if (stillLoading.length === 0) {
        clearInterval(checkInterval)
        isCheckingAll.value = false
      }
    }, 500)
  }
}
</script>

<style>
.spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style>
