# API Reference - Stationery Store Egypt

Base URL: `http://localhost:8080/api`

## Authentication

### POST /auth/login
Login with username/email and password.

**Request:**
```json
{
  "usernameOrEmail": "admin",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "userId": "guid",
    "username": "admin",
    "fullNameAr": "مدير النظام",
    "fullNameEn": "Administrator",
    "email": "admin@store.com",
    "role": "Admin",
    "roleId": "guid",
    "defaultBranchId": "guid",
    "token": "eyJhbGc...",
    "refreshToken": "base64...",
    "expiresInMinutes": 480,
    "language": "ar"
  }
}
```

### POST /auth/refresh
Refresh access token.

**Request:**
```
Authorization: Bearer {refreshToken}
```

### POST /auth/change-password
Change user password.

**Request:**
```json
{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword123"
}
```

### POST /auth/logout
Logout and invalidate token.

---

## Products

### GET /products
Get all products with filtering and pagination.

**Query Parameters:**
- `page` (int, default: 1)
- `pageSize` (int, default: 20)
- `search` (string)
- `barcode` (string)
- `categoryId` (Guid)
- `branchId` (Guid)
- `lowStock` (bool)
- `isActive` (bool)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [...],
    "currentPage": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

### GET /products/{id}
Get product by ID.

### GET /products/barcode/{barcode}
Get product by barcode.

### POST /products
Create new product.

**Request:**
```json
{
  "sku": "PEN-001",
  "nameAr": "قلم أزرق",
  "nameEn": "Blue Pen",
  "categoryId": "guid",
  "unitId": "guid",
  "vatRateId": "guid",
  "branchId": "guid",
  "costPrice": 5.00,
  "salePrice": 7.50,
  "minimumStockLevel": 100,
  "isActive": true,
  "etaItemCode": "0",
  "etaUnitCode": "PCE"
}
```

### PATCH /products/{id}/stock
Update product stock quantity.

**Request:**
```json
{
  "quantity": 500
}
```

---

## Customers

### GET /customers
Get all customers with filtering.

**Query Parameters:**
- `page`, `pageSize`
- `search`
- `type` (B2C, B2B, Government, Export)
- `taxNumber`
- `isActive`

### GET /customers/{id}
Get customer by ID.

### POST /customers
Create new customer.

**Request:**
```json
{
  "nameAr": "شركة المكتب",
  "nameEn": "Office Company",
  "type": "B2B",
  "taxRegistrationNumber": "123-456-789",
  "email": "info@company.com",
  "phone": "+201234567890",
  "creditLimit": 50000.00
}
```

---

## Invoices

### GET /invoices
Get all invoices with filtering.

**Query Parameters:**
- `page`, `pageSize`
- `invoiceNumber`
- `customerId`
- `branchId`
- `type` (TaxInvoice, TaxNote, Receipt, ProformaInvoice)
- `status` (Draft, Pending, Submitted, Approved, Rejected, Cancelled, Paid)
- `fromDate`, `toDate`

### GET /invoices/{id}
Get invoice by ID.

### POST /invoices
Create new invoice (POS sale).

**Request:**
```json
{
  "type": "TaxInvoice",
  "branchId": "guid",
  "customerId": null,
  "lines": [
    {
      "productId": "guid",
      "quantity": 2,
      "discountAmount": 0,
      "discountPercentage": 0
    }
  ],
  "paymentMethod": "Cash",
  "paidAmount": 15.00
}
```

### PUT /invoices/{id}/cancel
Cancel/void invoice.

---

## Inventory

### GET /inventory/stock
Get stock levels.

### POST /inventory/stock/adjust
Adjust stock quantity.

### POST /inventory/stock/transfer
Transfer stock between branches.

### GET /inventory/alerts/low-stock
Get low stock alerts.

---

## Suppliers

### GET /suppliers
Get all suppliers.

### POST /suppliers
Create supplier.

### POST /suppliers/purchase-orders
Create purchase order.

---

## ETA E-Invoicing

### POST /eta/submit-pending
Submit pending invoices to ETA.

### POST /eta/invoices/{id}/submit
Submit single invoice to ETA.

### POST /eta/invoices/{etaUuid}/cancel
Cancel invoice in ETA.

### GET /eta/submissions/{id}/status
Check submission status.

### GET /eta/compliance-report
Get compliance report.

---

## Reports

### GET /reports/dashboard/sales
Get sales dashboard.

**Query Parameters:**
- `fromDate`
- `toDate`

### GET /reports/sales/by-product
Product sales report.

### GET /reports/inventory/valuation
Inventory valuation report.

### GET /reports/tax/vat-summary
VAT summary for tax filing.

---

## Error Responses

**Standard Error Format:**
```json
{
  "success": false,
  "message": "Error message",
  "messageAr": "رسالة الخطأ",
  "errors": []
}
```

**HTTP Status Codes:**
- `200 OK` - Success
- `201 Created` - Resource created
- `204 No Content` - Success (no content)
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

---

## Rate Limiting

- Authentication endpoints: 10 requests/minute
- Other endpoints: 100 requests/minute

---

For interactive documentation, visit: http://localhost:8080/api-docs
