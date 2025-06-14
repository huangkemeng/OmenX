<script setup lang="ts">
import {v4} from 'uuid'
import zhCN from 'ant-design-vue/es/locale/zh_CN'

const nonce = v4();

</script>

<template>
  <a-config-provider :locale="zhCN" :csp="{ nonce: nonce }" :wave="{ disabled: false }">
    <router-view></router-view>
  </a-config-provider>
</template>

