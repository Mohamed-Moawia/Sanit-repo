import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { useAuth } from '../context/AuthContext'
import type { LoginRequest } from '../types'

interface LoginForm extends LoginRequest {
  rememberMe: boolean
}

export default function Login() {
  const navigate = useNavigate()
  const { login, error, clearError } = useAuth()
  const [isLoading, setIsLoading] = useState(false)
  const [isArabic, setIsArabic] = useState(false)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>()

  const onSubmit = async (data: LoginForm) => {
    setIsLoading(true)
    clearError()
    try {
      await login({ usernameOrEmail: data.usernameOrEmail, password: data.password })
      navigate('/dashboard')
    } catch {
      // Error is handled by context
    } finally {
      setIsLoading(false)
    }
  }

  const toggleLanguage = () => {
    setIsArabic(!isArabic)
    document.documentElement.dir = isArabic ? 'ltr' : 'rtl'
    document.documentElement.lang = isArabic ? 'en' : 'ar'
  }

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          <i className="bi bi-store display-4"></i>
          <h3 className="mt-2 mb-0">{isArabic ? 'متجر القرطاسية' : 'Stationery Store'}</h3>
          <p className="mb-0 opacity-75">{isArabic ? 'تسجيل الدخول' : 'Sign In'}</p>
        </div>

        <div className="login-body">
          <button
            className="btn btn-outline-secondary btn-sm mb-3"
            onClick={toggleLanguage}
          >
            <i className="bi bi-translate me-1"></i>
            {isArabic ? 'English' : 'العربية'}
          </button>

          {error && (
            <div className="alert alert-danger alert-dismissible fade show" role="alert">
              <i className="bi bi-exclamation-triangle me-2"></i>
              {error}
              <button
                type="button"
                className="btn-close"
                onClick={clearError}
              ></button>
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="mb-3">
              <label className="form-label">
                {isArabic ? 'اسم المستخدم أو البريد الإلكتروني' : 'Username or Email'}
              </label>
              <input
                type="text"
                className={`form-control ${errors.usernameOrEmail ? 'is-invalid' : ''}`}
                placeholder={isArabic ? 'أدخل اسم المستخدم أو البريد' : 'Enter username or email'}
                {...register('usernameOrEmail', {
                  required: isArabic ? 'مطلوب' : 'This field is required',
                })}
              />
              {errors.usernameOrEmail && (
                <div className="invalid-feedback">{errors.usernameOrEmail.message}</div>
              )}
            </div>

            <div className="mb-3">
              <label className="form-label">{isArabic ? 'كلمة المرور' : 'Password'}</label>
              <input
                type="password"
                className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                placeholder={isArabic ? 'أدخل كلمة المرور' : 'Enter password'}
                {...register('password', {
                  required: isArabic ? 'مطلوب' : 'This field is required',
                  minLength: {
                    value: 6,
                    message: isArabic ? '6 أحرف على الأقل' : 'At least 6 characters',
                  },
                })}
              />
              {errors.password && <div className="invalid-feedback">{errors.password.message}</div>}
            </div>

            <div className="mb-3 form-check">
              <input
                type="checkbox"
                className="form-check-input"
                id="rememberMe"
                {...register('rememberMe')}
              />
              <label className="form-check-label" htmlFor="rememberMe">
                {isArabic ? 'تذكرني' : 'Remember me'}
              </label>
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100 py-2"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                  {isArabic ? 'جاري الدخول...' : 'Signing in...'}
                </>
              ) : (
                <>
                  <i className="bi bi-box-arrow-in-right me-2"></i>
                  {isArabic ? 'دخول' : 'Sign In'}
                </>
              )}
            </button>
          </form>

          <div className="text-center mt-3">
            <small className="text-muted">
              {isArabic ? 'تجربة: admin / password123' : 'Demo: admin / password123'}
            </small>
          </div>
        </div>
      </div>
    </div>
  )
}
