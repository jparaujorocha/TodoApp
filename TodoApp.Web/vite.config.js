import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [
    vue(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: './tests/unit/setup.js'
  },
  server: {
    port: 8080,
    proxy: {
      '/api': {
        target: 'https://localhost:44359',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})