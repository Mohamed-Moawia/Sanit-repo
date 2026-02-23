import { useState, useEffect } from 'react'
import apiService from '../services/api'
import type { Product } from '../types'

export default function Products() {
  const [products, setProducts] = useState<Product[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [isArabic, setIsArabic] = useState(false)

  useEffect(() => {
    fetchProducts()
  }, [])

  const fetchProducts = async () => {
    try {
      const response = await apiService.getProducts()
      if (response.success && response.data) {
        setProducts(response.data)
      }
    } catch (error) {
      console.error('Error fetching products:', error)
    } finally {
      setLoading(false)
    }
  }

  const filteredProducts = products.filter(
    (product) =>
      product.nameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.nameAr.includes(searchTerm) ||
      product.sku.toLowerCase().includes(searchTerm.toLowerCase())
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
          <h2>{isArabic ? 'المنتجات' : 'Products'}</h2>
          <p className="text-muted mb-0">
            {isArabic ? 'إدارة منتجات المتجر' : 'Manage store products'}
          </p>
        </div>
        <div className="d-flex gap-2">
          <button className="btn btn-outline-secondary" onClick={() => setIsArabic(!isArabic)}>
            <i className="bi bi-translate me-1"></i>
            {isArabic ? 'English' : 'العربية'}
          </button>
          <button className="btn btn-primary">
            <i className="bi bi-plus-lg me-1"></i>
            {isArabic ? 'إضافة منتج' : 'Add Product'}
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
            {filteredProducts.length} {isArabic ? 'منتج' : 'products'}
          </span>
        </div>

        <div className="table-card-body">
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>{isArabic ? 'الاسم' : 'Name'}</th>
                  <th>{isArabic ? 'SKU' : 'SKU'}</th>
                  <th>{isArabic ? 'السعر' : 'Price'}</th>
                  <th>{isArabic ? 'المخزون' : 'Stock'}</th>
                  <th>{isArabic ? 'الحالة' : 'Status'}</th>
                  <th>{isArabic ? 'إجراءات' : 'Actions'}</th>
                </tr>
              </thead>
              <tbody>
                {filteredProducts.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="text-center py-4 text-muted">
                      <i className="bi bi-inbox display-4 d-block mb-2"></i>
                      {isArabic ? 'لا توجد منتجات' : 'No products found'}
                    </td>
                  </tr>
                ) : (
                  filteredProducts.map((product) => (
                    <tr key={product.id}>
                      <td>
                        <div>
                          <div className="fw-semibold">{product.nameEn}</div>
                          <small className="text-muted">{product.nameAr}</small>
                        </div>
                      </td>
                      <td>
                        <code>{product.sku}</code>
                      </td>
                      <td>
                        <span className="fw-semibold">ج.م {product.unitPrice.toFixed(3)}</span>
                      </td>
                      <td>
                        <span
                          className={`badge ${
                            product.stockQuantity <= product.reorderLevel
                              ? 'bg-danger'
                              : 'bg-success'
                          }`}
                        >
                          {product.stockQuantity}
                        </span>
                      </td>
                      <td>
                        {product.isActive ? (
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
