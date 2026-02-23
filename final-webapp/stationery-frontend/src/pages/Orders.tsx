import { useState, useEffect } from 'react'
import apiService from '../services/api'
import type { Order } from '../types'

export default function Orders() {
  const [orders, setOrders] = useState<Order[]>([])
  const [loading, setLoading] = useState(true)
  const [isArabic, setIsArabic] = useState(false)

  useEffect(() => {
    fetchOrders()
  }, [])

  const fetchOrders = async () => {
    try {
      const response = await apiService.getOrders()
      if (response.success && response.data) {
        setOrders(response.data)
      }
    } catch (error) {
      console.error('Error fetching orders:', error)
    } finally {
      setLoading(false)
    }
  }

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, string> = {
      Pending: 'bg-warning',
      Processing: 'bg-info',
      Completed: 'bg-success',
      Cancelled: 'bg-danger',
    }
    return statusMap[status] || 'bg-secondary'
  }

  const getPaymentStatusBadge = (status: string) => {
    const statusMap: Record<string, string> = {
      Pending: 'bg-warning',
      Paid: 'bg-success',
      Partial: 'bg-info',
      Refunded: 'bg-danger',
    }
    return statusMap[status] || 'bg-secondary'
  }

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
          <h2>{isArabic ? 'الطلبات' : 'Orders'}</h2>
          <p className="text-muted mb-0">
            {isArabic ? 'إدارة طلبات العملاء' : 'Manage customer orders'}
          </p>
        </div>
        <div className="d-flex gap-2">
          <button className="btn btn-outline-secondary" onClick={() => setIsArabic(!isArabic)}>
            <i className="bi bi-translate me-1"></i>
            {isArabic ? 'English' : 'العربية'}
          </button>
          <button className="btn btn-primary">
            <i className="bi bi-plus-lg me-1"></i>
            {isArabic ? 'طلب جديد' : 'New Order'}
          </button>
        </div>
      </div>

      <div className="table-card">
        <div className="table-card-header">
          <span className="text-muted">
            {orders.length} {isArabic ? 'طلب' : 'orders'}
          </span>
        </div>

        <div className="table-card-body">
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>{isArabic ? 'رقم الطلب' : 'Order #'}</th>
                  <th>{isArabic ? 'العميل' : 'Customer'}</th>
                  <th>{isArabic ? 'التاريخ' : 'Date'}</th>
                  <th>{isArabic ? 'الحالة' : 'Status'}</th>
                  <th>{isArabic ? 'الدفع' : 'Payment'}</th>
                  <th className="text-end">{isArabic ? 'الإجمالي' : 'Total'}</th>
                  <th>{isArabic ? 'إجراءات' : 'Actions'}</th>
                </tr>
              </thead>
              <tbody>
                {orders.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center py-4 text-muted">
                      <i className="bi bi-inbox display-4 d-block mb-2"></i>
                      {isArabic ? 'لا توجد طلبات' : 'No orders found'}
                    </td>
                  </tr>
                ) : (
                  orders.map((order) => (
                    <tr key={order.id}>
                      <td>
                        <code className="fw-semibold">{order.orderNumber}</code>
                      </td>
                      <td>{order.customerName}</td>
                      <td>{new Date(order.orderDate).toLocaleDateString()}</td>
                      <td>
                        <span className={`badge ${getStatusBadge(order.status)}`}>{order.status}</span>
                      </td>
                      <td>
                        <span className={`badge ${getPaymentStatusBadge(order.paymentStatus)}`}>
                          {order.paymentStatus}
                        </span>
                      </td>
                      <td className="text-end fw-semibold">
                        ج.م {order.netAmount.toFixed(3)}
                      </td>
                      <td>
                        <button className="btn btn-sm btn-outline-primary me-1">
                          <i className="bi bi-eye"></i>
                        </button>
                        <button className="btn btn-sm btn-outline-secondary">
                          <i className="bi bi-printer"></i>
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  )
}
