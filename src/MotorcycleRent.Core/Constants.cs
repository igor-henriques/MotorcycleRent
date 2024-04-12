namespace MotorcycleRent.Core;

public static class Constants
{
    public static class SwaggerTags
    {
        public const string User = nameof(User);
        public const string MotorcycleRent = nameof(MotorcycleRent);
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
            public const string UpdateState = $"{BaseRoute}/update-state";
            public const string Delete = $"{BaseRoute}/delete/{{motorcyclePlate}}";
        }

        public static class User
        {
            private const string BaseRoute = "api/user";
            public const string CreateAdmin = $"{BaseRoute}/create-admin";
            public const string CreateDeliveryPartner = $"{BaseRoute}/create-delivery-partner";
            public const string Authenticate = $"{BaseRoute}/authenticate";
        }

        public static class Rent
        {
            private const string BaseRoute = "api/rent";
            public const string Create = $"{BaseRoute}/create";
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
            public const string Update = $"{BaseRoute}/update";
        }
    }

    public static class Messages
    {
        public const string InternalServerErrorMessage = "An internal error occured while processing your request. Try again later.";
    }
}
