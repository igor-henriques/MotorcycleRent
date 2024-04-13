namespace MotorcycleRent.Core;

public static class Constants
{
    public static class SwaggerTags
    {
        public const string User = nameof(User);
        public const string Rental = nameof(Rental);
        public const string Motorcycle = nameof(Motorcycle);
        public const string DriverLicense = nameof(DriverLicense);
        public const string DeliveryPartner = nameof(DeliveryPartner);
        public const string Order = nameof(Order);
    }

    public static class Roles
    {
        public const string Administrator = nameof(Administrator);
        public const string DeliveryPartner = nameof(DeliveryPartner);
    }

    public static class Routes
    {
        public const string Health = "/health";
        public const string Swagger = "/swagger";

        public static class Motorcycle
        {
            private const string BaseRoute = "api/motorcycle";
            public const string Create = $"{BaseRoute}/create";
            public const string List = $"{BaseRoute}/list";
            public const string UpdatePlate = $"{BaseRoute}/update-plate";
            public const string UpdateStatus = $"{BaseRoute}/update-status";
            public const string Delete = $"{BaseRoute}/delete/{{motorcyclePlate}}";
        }

        public static class User
        {
            private const string BaseRoute = "api/user";
            public const string CreateAdmin = $"{BaseRoute}/create-admin";
            public const string CreateDeliveryPartner = $"{BaseRoute}/create-delivery-partner";
            public const string Authenticate = $"{BaseRoute}/authenticate";
        }

        public static class Rental
        {
            private const string BaseRoute = "api/rental";
            public const string Rent = $"{BaseRoute}/rent";
            public const string PeekPrice = $"{BaseRoute}/peek-price";
            public const string Return = $"{BaseRoute}/return";
        }

        public static class DriverLicense
        {
            private const string BaseRoute = "api/driver-license";
            public const string Create = $"{BaseRoute}/create";
            public const string Update = $"{BaseRoute}/update";
        }

        public static class Order
        {
            private const string BaseRoute = "api/order";
            public const string Create = $"{BaseRoute}/create";
            public const string CheckAvailability = $"{BaseRoute}/check-availability";
            public const string UpdateStatus = $"{BaseRoute}/update-status";
            public const string NotifiedPartners = $"{BaseRoute}/notified-partners/{{publicOrderId}}";
        }
    }

    public static class Messages
    {
        public const string InternalServerError = "An internal error occured while processing your request. Try again later.";
        public const string CalculatorServiceNotFound = "Calculator service unavailable at the moment.";

        public const string InvalidUserOperation = "Invalid user operation.";
        public const string InvalidDeliveryPartner = "Invalid delivery partner.";
        public const string DriverLicenseExists = "Driver license already exists.";
        public const string InvalidDriverLicense = "Delivery partner does not exist or does not have driver license registered.";
        public const string NoOngoingRentalForPartner = "No on going rental was found for the current partner.";

        public const string InvalidPlateOperation = "Invalid plate operation.";
        public const string InvalidMotorcyclePlate = "Invalid motorcycle plate.";
        public const string InvalidMotorcycleUpdate = "An error occurred while updating a motorcycle.";
        public const string MotorcycleRentedOnce = "An error occurred while deleting a motorcycle.";
        public const string InvalidRentalPlan = "Invalid rental plan.";

        public const string InvalidOrderUpdate = "An error occurred while updating an order.";
        public const string ForbiddenOrderUpdate = "Order update not allowed.";        
        public const string InvalidOrder = "Order does not exist.";
    }
}
