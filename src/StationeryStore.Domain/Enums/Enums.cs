namespace StationeryStore.Domain.Enums;

/// <summary>
/// Customer types for Egyptian market segmentation
/// </summary>
public enum CustomerType
{
    B2C = 0,           // Individual consumer
    B2B = 1,           // Business customer
    Government = 2,    // Government entity
    Export = 3         // Export customer
}

/// <summary>
/// Invoice types aligned with Egyptian ETA requirements
/// </summary>
public enum InvoiceType
{
    TaxInvoice = 0,      // Regular tax invoice (فاتورة ضريبية)
    TaxNote = 1,         // Credit/Debit note (إشعار ضريبي)
    Receipt = 2,         // Simple receipt (إيصال استلام)
    ProformaInvoice = 3  // Proforma invoice (فاتورة مبدئية)
}

/// <summary>
/// Invoice status tracking for ETA submission workflow
/// </summary>
public enum InvoiceStatus
{
    Draft = 0,
    Pending = 1,
    Submitted = 2,       // Submitted to ETA
    Approved = 3,        // Approved by ETA
    Rejected = 4,        // Rejected by ETA
    Cancelled = 5,
    Paid = 6,
    PartiallyPaid = 7
}

/// <summary>
/// Payment methods commonly used in Egypt
/// </summary>
public enum PaymentMethod
{
    Cash = 0,
    CreditCard = 1,
    DebitCard = 2,
    BankTransfer = 3,
    Cheque = 4,
    Installments = 5,
    MobilePayment = 6,   // Vodafone Cash, Orange Money, etc.
    Credit = 7           // On account
}

/// <summary>
/// Payment status tracking
/// </summary>
public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3,
    Cancelled = 4
}

/// <summary>
/// Purchase order workflow status
/// </summary>
public enum PurchaseOrderStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Sent = 3,
    PartiallyReceived = 4,
    Completed = 5,
    Cancelled = 6,
    Rejected = 7
}

/// <summary>
/// Inventory movement types
/// </summary>
public enum MovementType
{
    Purchase = 0,
    Sale = 1,
    Return = 2,
    Adjustment = 3,
    Transfer = 4,
    WriteOff = 5,
    Count = 6
}

/// <summary>
/// ETA submission status for e-invoicing
/// </summary>
public enum EtaSubmissionStatus
{
    NotSubmitted = 0,
    Pending = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5
}

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRole
{
    Admin = 0,
    Manager = 1,
    Cashier = 2,
    InventoryManager = 3,
    Accountant = 4,
    SalesRepresentative = 5
}
