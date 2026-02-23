import { useState, useEffect } from 'react'
import apiService from '../services/api'
import type { Customer } from '../types'

export default function Customers() {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [isArabic, setIsArabic] = useState(false)

  useEffect(() => {
    fetchCustomers()
  }, [])

  const fetchCustomers = async () => {
    try {
      const response = await apiService.getCustomers()
      if (response.success && response.data) {
        setCustomers(response.data)
      }
    } catch (error) {
      console.error('Error fetching customers:', error)
    } finally {
      setLoading(false)
    }
  }

  const filteredCustomers = customers.filter(
    (customer) =>
      customer.nameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      customer.nameAr.includes(searchTerm) ||
      customer.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
      customer.phone.includes(searchTerm)
  )

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
          <h2>{isArabic ? 'العملاء' : 'Customers'}</h2>
          <p className="text-muted mb-0">
            {isArabic ? 'إدارة حسابات العملاء' : 'Manage customer accounts'}
          </p>
        </div>
        <div className="d-flex gap-2">
          <button className="btn btn-outline-secondary" onClick={() => setIsArabic(!isArabic)}>
            <i className="bi bi-translate me-1"></i>
            {isArabic ? 'English' : 'العربية'}
          </button>
          <button className="btn btn-primary">
            <i className="bi bi-plus-lg me-1"></i>
            {isArabic ? 'إضافة عميل' : 'Add Customer'}
          </button>
        </div>
      </div>

      <div className="table-card">
        <div className="table-card-header">
          <div className="d-flex align-items-center gap-2">
            <div className="input-group" style={{ width: '300px' }}>
              <span className="input-group-text">
                <i className="bi bi-search"></i>
              </span>
              <input
                type="text"
                className="form-control"
                placeholder={isArabic ? 'بحث...' : 'Search...'}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
          </div>
          <span className="text-muted">
            {filteredCustomers.length} {isArabic ? 'عميل' : 'customers'}
          </span>
        </div>

        <div className="table-card-body">
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>{isArabic ? 'الاسم' : 'Name'}</th>
                  <th>{isArabic ? 'البريد' : 'Email'}</th>
                  <th>{isArabic ? 'الهاتف' : 'Phone'}</th>
                  <th>{isArabic ? 'المدينة' : 'City'}</th>
                  <th>{isArabic ? 'الرقم الضريبي' : 'Tax Number'}</th>
                  <th>{isArabic ? 'الحالة' : 'Status'}</th>
                  <th>{isArabic ? 'إجراءات' : 'Actions'}</th>
                </tr>
              </thead>
              <tbody>
                {filteredCustomers.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center py-4 text-muted">
                      <i className="bi bi-inbox display-4 d-block mb-2"></i>
                      {isArabic ? 'لا توجد عملاء' : 'No customers found'}
                    </td>
                  </tr>
                ) : (
                  filteredCustomers.map((customer) => (
                    <tr key={customer.id}>
                      <td>
                        <div>
                          <div className="fw-semibold">{customer.nameEn}</div>
                          <small className="text-muted">{customer.nameAr}</small>
                        </div>
                      </td>
                      <td>
                        <a href={`mailto:${customer.email}`}>{customer.email}</a>
                      </td>
                      <td>{customer.mobile || customer.phone}</td>
                      <td>{customer.city || '-'}</td>
                      <td>
                        <code>{customer.taxNumber || '-'}</code>
                      </td>
                      <td>
                        {customer.isActive ? (
                          <span className="badge bg-success">{isArabic ? 'نشط' : 'Active'}</span>
                        ) : (
                          <span className="badge bg-secondary">
                            {isArabic ? 'غير نشط' : 'Inactive'}
                          </span>
                        )}
                      </td>
                      <td>
                        <button className="btn btn-sm btn-outline-primary me-1">
                          <i className="bi bi-pencil"></i>
                        </button>
                        <button className="btn btn-sm btn-outline-danger">
                          <i className="bi bi-trash"></i>
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
