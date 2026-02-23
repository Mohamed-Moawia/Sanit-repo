import { useState } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import type { ReactNode } from 'react'

interface LayoutProps {
  children: ReactNode
}

interface NavItem {
  path: string
  label: string
  labelAr: string
  icon: string
}

const navItems: NavItem[] = [
  { path: '/dashboard', label: 'Dashboard', labelAr: 'لوحة التحكم', icon: 'bi-speedometer2' },
  { path: '/products', label: 'Products', labelAr: 'المنتجات', icon: 'bi-box' },
  { path: '/orders', label: 'Orders', labelAr: 'الطلبات', icon: 'bi-cart' },
  { path: '/customers', label: 'Customers', labelAr: 'العملاء', icon: 'bi-people' },
  { path: '/users', label: 'Users', labelAr: 'المستخدمين', icon: 'bi-person-badge' },
]

export default function Layout({ children }: LayoutProps) {
  const navigate = useNavigate()
  const location = useLocation()
  const { user, logout } = useAuth()
  const [isArabic, setIsArabic] = useState(false)

  const handleLogout = async () => {
    await logout()
    navigate('/login')
  }

  const toggleLanguage = () => {
    setIsArabic(!isArabic)
    document.documentElement.dir = isArabic ? 'ltr' : 'rtl'
    document.documentElement.lang = isArabic ? 'en' : 'ar'
  }

  return (
    <div className="app-layout">
      <aside className="sidebar">
        <div className="sidebar-header">
          <h4 className="mb-0">
            <i className="bi bi-store me-2"></i>
            {isArabic ? 'متجر القرطاسية' : 'Stationery Store'}
          </h4>
        </div>
        <nav className="sidebar-nav">
          {navItems.map((item) => (
            <button
              key={item.path}
              className={`nav-item ${location.pathname === item.path ? 'active' : ''}`}
              onClick={() => navigate(item.path)}
            >
              <i className={`bi ${item.icon}`}></i>
              {isArabic ? item.labelAr : item.label}
            </button>
          ))}
        </nav>
        <div className="sidebar-footer">
          <div className="d-flex align-items-center justify-content-between">
            <div className="text-truncate">
              <small className="d-block opacity-75">{isArabic ? 'مرحباً' : 'Welcome'}</small>
              <span className="fw-semibold">{user?.fullNameEn || user?.username}</span>
            </div>
          </div>
        </div>
      </aside>

      <main className="main-content">
        <header className="top-navbar">
          <div>
            <h5 className="mb-0">
              {navItems.find((item) => item.path === location.pathname)?.label || 'Dashboard'}
            </h5>
          </div>
          <div className="d-flex align-items-center gap-3">
            <button className="btn btn-outline-secondary btn-sm" onClick={toggleLanguage}>
              <i className="bi bi-translate me-1"></i>
              {isArabic ? 'English' : 'العربية'}
            </button>
            <button className="btn btn-outline-danger btn-sm" onClick={handleLogout}>
              <i className="bi bi-box-arrow-right me-1"></i>
              {isArabic ? 'تسجيل خروج' : 'Logout'}
            </button>
          </div>
        </header>

        <div className="content-area">{children}</div>
      </main>
    </div>
  )
}
