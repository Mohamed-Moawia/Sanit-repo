import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios'
import type { ApiResponse, LoginRequest, LoginResponse, Product, Order, Customer, User } from '../types'

const API_BASE_URL = '/api'

class ApiService {
  private api: AxiosInstance

  constructor() {
    this.api = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    this.api.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = localStorage.getItem('token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    this.api.interceptors.response.use(
      (response) => response,
      (error: AxiosError<ApiResponse<unknown>>) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token')
          localStorage.removeItem('user')
          window.location.href = '/login'
        }
        return Promise.reject(error)
      }
    )
  }

  async login(request: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await this.api.post<ApiResponse<LoginResponse>>('/auth/login', request)
    return response.data
  }

  async logout(): Promise<void> {
    await this.api.post('/auth/logout')
  }

  async getCurrentUser(): Promise<User | null> {
    try {
      const response = await this.api.get<ApiResponse<User>>('/auth/me')
      return response.data.data
    } catch {
      return null
    }
  }

  async getProducts(): Promise<ApiResponse<Product[]>> {
    const response = await this.api.get<ApiResponse<Product[]>>('/products')
    return response.data
  }

  async getOrders(): Promise<ApiResponse<Order[]>> {
    const response = await this.api.get<ApiResponse<Order[]>>('/orders')
    return response.data
  }

  async getCustomers(): Promise<ApiResponse<Customer[]>> {
    const response = await this.api.get<ApiResponse<Customer[]>>('/customers')
    return response.data
  }

  async getEgyptInfo(): Promise<unknown> {
    const response = await this.api.get('/egypt/info')
    return response.data
  }
}

export const apiService = new ApiService()
export default apiService
