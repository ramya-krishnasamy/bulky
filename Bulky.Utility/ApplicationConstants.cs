using System;
namespace Bulky.Utility {
	public static class ApplicationConstants {
		public const string ROLE_ADMIN = "Admin";
        public const string ROLE_COMPANY = "Company";
        public const string ROLE_CUSTOMER = "Customer";
        public const string ROLE_EMPLOYEE = "Employee";

        public const string SessionShoppingCart = "session_cart";
    }

    public static class OrderStatus {
        public const string PENDING = "pending";
        public const string APPROVED = "approved";
        public const string IN_PROCESS = "in_process";
        public const string SHIPPED = "shipped";
        public const string CANCELLED = "cancelled";
        public const string REFUNDED = "refunded";
    }

    public static class PaymentStatus {
        public const string PENDING = "pending";
        public const string APPROVED = "approved";
        public const string DELAYED_PAYMENT = "delayed_payment";
        public const string REJECTED = "rejected";
    }
}

