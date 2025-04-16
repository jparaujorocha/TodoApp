import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/views/Home/Home.vue' // Updated path
import TodoDetail from '@/views/TodoDetail/TodoDetail.vue' // Updated path

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/todo/:id',
    name: 'TodoDetail',
    component: TodoDetail,
    props: true
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

export default router