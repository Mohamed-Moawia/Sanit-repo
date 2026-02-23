import { useState, useEffect } from 'react'
import apiService from '../services/api'
import { useAuth } from '../context/AuthContext'

export default function Dashboard() {
  const { user } = useAuth()
  const [stats, setStats] = useState({
    totalProducts: 0,
    totalOrders: 0,
    totalCustomers: 0,
    lowStockItems: 0,
  })
  const [loading, setLoading] = useState(true)
  const [isArabic, setIsArabic] = useState(user?.language === 'ar')

  useEffect(() => {
    // Fetch dashboard stats
    const fetchStats = async () => {
      try {
        const [products, orders, customers] = await Promise.all([
          apiService.getProducts().catch(() => ({ data: [] })),
          apiService.getOrders().catch(() => ({ data: [] })),
          apiService.getCustomers().catch(() => ({ data: [] })),
        ])

        setStats({
          totalProducts: products.data?.length || 0,
          totalOrders: orders.data?.length || 0,
          totalCustomers: customers.data?.length || 0,
          lowStockItems: products.data?.filter((p) => p.stockQuantity <= p.reorderLevel).length || 0,
        })
      } catch (error) {
        console.error('Error fetching stats:', error)
      } finally {
        setLoading(false)
      }
    }

    fetchStats()
  }, [])

  const statCards = [
    {
      title: isArabic ? 'إجمالي المنتجات' : 'Total Products',
      value: stats.totalProducts,
      icon: 'bi-box',
      color: 'primary',
      bgColor: 'bg-primary bg-opacity-10',
    },
    {
      title: isArabic ? 'إجمالي الطلبات' : 'Total Orders',
      value: stats.totalOrders,
      icon: 'bi-cart',
      color: 'success',
      bgColor: 'bg-success bg-opacity-10',
    },
    {
      title: isArabic ? 'إجمالي العملاء' : 'Total Customers',
      value: stats.totalCustomers,
      icon: 'bi-people',
      color: 'info',
      bgColor: 'bg-info bg-opacity-10',
    },
    {
      title: isArabic ? 'منتجات منخفضة المخزون' : 'Low Stock Items',
      value: stats.lowStockItems,
      icon: 'bi-exclamation-triangle',
      color: 'warning',
      bgColor: 'bg-warning bg-opacity-10',
    },
  ]

  if (loading) {
    return (
      <div className="loading-spinner">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    )
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2>{isArabic ? 'لوحة التحكم' : 'Dashboard'}</h2>
          <p className="text-muted mb-0">
            {isArabic ? 'مرحباً بك في نظام إدارة المتجر' : 'Welcome to the Store Management System'}
          </p>
        </div>
        <button className="btn btn-outline-secondary" onClick={() => setIsArabic(!isArabic)}>
          <i className="bi bi-translate me-1"></i>
          {isArabic ? 'English' : 'العربية'}
        </button>
      </div>

      <div className="row g-4 mb-4">
        {statCards.map((stat) => (
          <div className="col-md-3" key={stat.title}>
            <div className="stat-card h-100">
              <div className="d-flex align-items-center justify-content-between">
                <div>
                  <p className="text-muted mb-1">{stat.title}</p>
                  <h3 className="mb-0 fw-bold">{stat.value}</h3>
                </div>
                <div className={`stat-card-icon ${stat.bgColor} text-${stat.color}`}>
                  <i className={`bi ${stat.icon}`}></i>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="row g-4">
        <div className="col-md-6">
          <div className="stat-card h-100">
            <h5 className="mb-3">{isArabic ? 'آخر النشاطات' : 'Recent Activity'}</h5>
            <div className="text-muted text-center py-4">
              <i className="bi bi-clock-history display-4 d-block mb-2"></i>
              {isArabic ? 'لا توجد نشاطات حديثة' : 'No recent activity'}
            </div>
          </div>
        </div>

        <div className="col-md-6">
          <div className="stat-card h-100">
            <h5 className="mb-3">{isArabic ? 'تنبيهات المخزون' : 'Stock Alerts'}</h5>
            {stats.lowStockItems > 0 ? (
              <div className="alert alert-warning">
                <i className="bi bi-exclamation-triangle me-2"></i>
                {isArabic
                  ? `${stats.lowStockItems} منتجات منخفضة المخزون`
                  : `${stats.lowStockItems} items with low stock`}
              </div>
            ) : (
              <div className="alert alert-success">
                <i className="bi bi-check-circle me-2"></i>
                {isArabic ? 'جميع المنتجات متوفرة' : 'All products in stock'}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
