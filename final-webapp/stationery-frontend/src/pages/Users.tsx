import { useState } from 'react'

interface User {
  id: string
  username: string
  email: string
  fullNameAr: string
  fullNameEn: string
  role: string
  isActive: boolean
  createdAt: string
}

export default function Users() {
  const [isArabic, setIsArabic] = useState(false)

  // Mock users data - in real app, fetch from API
  const [users] = useState<User[]>([
    {
      id: '1',
      username: 'admin',
      email: 'admin@stationery.eg',
      fullNameAr: 'مدير النظام',
      fullNameEn: 'System Administrator',
      role: 'Admin',
      isActive: true,
      createdAt: new Date().toISOString(),
    },
    {
      id: '2',
      username: 'manager',
      email: 'manager@stationery.eg',
      fullNameAr: 'مدير المتجر',
      fullNameEn: 'Store Manager',
      role: 'Manager',
      isActive: true,
      createdAt: new Date().toISOString(),
    },
    {
      id: '3',
      username: 'cashier',
      email: 'cashier@stationery.eg',
      fullNameAr: 'أمين الصندوق',
      fullNameEn: 'Cashier',
      role: 'Cashier',
      isActive: true,
      createdAt: new Date().toISOString(),
    },
  ])

  const [searchTerm, setSearchTerm] = useState('')

  const filteredUsers = users.filter(
    (user) =>
      user.username.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.fullNameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.fullNameAr.includes(searchTerm) ||
      user.role.toLowerCase().includes(searchTerm.toLowerCase())
  )

  const getRoleBadgeClass = (role: string) => {
    const roleMap: Record<string, string> = {
      Admin: 'bg-danger',
      Manager: 'bg-primary',
      Cashier: 'bg-info',
    }
    return roleMap[role] || 'bg-secondary'
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2>{isArabic ? 'المستخدمين' : 'Users'}</h2>
          <p className="text-muted mb-0">
            {isArabic ? 'إدارة حسابات المستخدمين' : 'Manage user accounts'}
          </p>
        </div>
        <div className="d-flex gap-2">
          <button className="btn btn-outline-secondary" onClick={() => setIsArabic(!isArabic)}>
            <i className="bi bi-translate me-1"></i>
            {isArabic ? 'English' : 'العربية'}
          </button>
          <button className="btn btn-primary">
            <i className="bi bi-plus-lg me-1"></i>
            {isArabic ? 'إضافة مستخدم' : 'Add User'}
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
            {filteredUsers.length} {isArabic ? 'مستخدم' : 'users'}
          </span>
        </div>

        <div className="table-card-body">
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>{isArabic ? 'اسم المستخدم' : 'Username'}</th>
                  <th>{isArabic ? 'الاسم الكامل' : 'Full Name'}</th>
                  <th>{isArabic ? 'البريد' : 'Email'}</th>
                  <th>{isArabic ? 'الدور' : 'Role'}</th>
                  <th>{isArabic ? 'تاريخ الإنشاء' : 'Created'}</th>
                  <th>{isArabic ? 'الحالة' : 'Status'}</th>
                  <th>{isArabic ? 'إجراءات' : 'Actions'}</th>
                </tr>
              </thead>
              <tbody>
                {filteredUsers.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center py-4 text-muted">
                      <i className="bi bi-inbox display-4 d-block mb-2"></i>
                      {isArabic ? 'لا توجد مستخدمين' : 'No users found'}
                    </td>
                  </tr>
                ) : (
                  filteredUsers.map((user) => (
                    <tr key={user.id}>
                      <td>
                        <div className="d-flex align-items-center gap-2">
                          <div className="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center"
                               style={{ width: '32px', height: '32px' }}>
                            {user.username.charAt(0).toUpperCase()}
                          </div>
                          <span className="fw-semibold">{user.username}</span>
                        </div>
                      </td>
                      <td>
                        <div>
                          <div className="fw-semibold">{user.fullNameEn}</div>
                          <small className="text-muted">{user.fullNameAr}</small>
                        </div>
                      </td>
                      <td>
                        <a href={`mailto:${user.email}`}>{user.email}</a>
                      </td>
                      <td>
                        <span className={`badge ${getRoleBadgeClass(user.role)}`}>{user.role}</span>
                      </td>
                      <td>{new Date(user.createdAt).toLocaleDateString()}</td>
                      <td>
                        {user.isActive ? (
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
