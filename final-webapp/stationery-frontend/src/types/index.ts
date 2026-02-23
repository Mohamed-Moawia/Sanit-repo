export interface User {
  id: string
  username: string
  email: string
  fullNameAr: string
  fullNameEn: string
  role: string
  roleId: string
  defaultBranchId: string | null
  language: string
}

export interface LoginRequest {
  usernameOrEmail: string
  password: string
}

export interface LoginResponse {
  userId: string
  username: string
  fullNameAr: string
  fullNameEn: string
  email: string
  role: string
  roleId: string
  defaultBranchId: string | null
  token: string
  refreshToken: string
  expiresInMinutes: number
  language: string
}

export interface Product {
  id: string
  nameAr: string
  nameEn: string
  description: string
  sku: string
  barcode: string
  unitPrice: number
  costPrice: number
  vatRate: number
  stockQuantity: number
  reorderLevel: number
  categoryId: string
  categoryName: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface Order {
  id: string
  orderNumber: string
  customerId: string
  customerName: string
  orderDate: string
  totalAmount: number
  vatAmount: number
  netAmount: number
  status: string
  paymentStatus: string
  items: OrderItem[]
}

export interface OrderItem {
  id: string
  productId: string
  productName: string
  quantity: number
  unitPrice: number
  vatRate: number
  totalAmount: number
}

export interface Customer {
  id: string
  nameAr: string
  nameEn: string
  email: string
  phone: string
  mobile: string
  address: string
  city: string
  taxNumber: string
  isActive: boolean
  createdAt: string
}

export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: Record<string, string[]> | null
}
