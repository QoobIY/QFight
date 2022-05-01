import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/name': {
        ws:true,
        target: 'ws://localhost:8080',
      },
      '/greeting': {
        changeOrigin: true,
        target: 'http://localhost:8080',
      },
    }
  },
  define: {
    global: {},
  }
})
