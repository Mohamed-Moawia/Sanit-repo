import { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import apiService from '../services/api'
import type { User, LoginRequest } from '../types'

interface AuthContextType {
  user: User | null
  token: string | null
  isAuthenticated: boolean
  isLoading: boolean
  login: (request: LoginRequest) => Promise<void>
  logout: () => Promise<void>
  error: string | null
  clearError: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const storedToken = localStorage.getItem('token')
    const storedUser = localStorage.getItem('user')

    if (storedToken && storedUser) {
      setToken(storedToken)
      setUser(JSON.parse(storedUser))
    }
    setIsLoading(false)
  }, [])

  const login = async (request: LoginRequest) => {
    try {
      setError(null)
      const response = await apiService.login(request)

      if (response.success && response.data) {
        const { token: newToken, ...userData } = response.data
        const user: User = {
          id: userData.userId,
          username: userData.username,
          email: userData.email,
          fullNameAr: userData.fullNameAr,
          fullNameEn: userData.fullNameEn,
          role: userData.role,
          roleId: userData.roleId,
          defaultBranchId: userData.defaultBranchId,
          language: userData.language,
        }
        localStorage.setItem('token', newToken)
        localStorage.setItem('user', JSON.stringify(user))
        setToken(newToken)
        setUser(user)
      } else {
        throw new Error(response.message || 'Login failed')
      }
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Login failed'
      setError(message)
      throw err
    }
  }

  const logout = async () => {
    try {
      await apiService.logout()
    } catch {
      // Ignore logout errors
    } finally {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      setToken(null)
      setUser(null)
    }
  }

  const clearError = () => setError(null)

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token,
        isLoading,
        login,
        logout,
        error,
        clearError,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
