namespace MotorcycleRent.Api.Swagger;

public static class SwaggerApiDescriber
{
    public static Attribute[] CreateDriverLicense()
    {
        return
        [
            new SwaggerOperationAttribute("CreateDriverLicense", "Creates a new driver license for a delivery partner."),
            new SwaggerResponseAttribute(204, "Driver license created successfully. No content returned."),
            new SwaggerResponseAttribute(400, "Invalid data or request format."),
            new SwaggerResponseAttribute(403, "Authorization required or access denied."),
            new SwaggerResponseAttribute(500, "Server error or unable to process the request.")
        ];
    }

    public static Attribute[] UpdateDriverLicense()
    {
        return
        [
            new SwaggerOperationAttribute("UpdateDriverLicense", "Updates an existing driver license for a delivery partner."),
            new SwaggerResponseAttribute(204, "Driver license updated successfully. No content returned."),
            new SwaggerResponseAttribute(400, "Invalid driver license ID or data."),
            new SwaggerResponseAttribute(403, "Authorization required or access denied."),
            new SwaggerResponseAttribute(404, "Driver license not found."),
            new SwaggerResponseAttribute(500, "Server error or unable to process the request.")
        ];
    }

    public static Attribute[] CreateMotorcycle()
    {
        return
        [
            new SwaggerOperationAttribute("CreateMotorcycle", "Creates a new motorcycle in the system."),
            new SwaggerResponseAttribute(200, "Returns the ID of the newly created motorcycle."),
            new SwaggerResponseAttribute(400, "Bad request if the data is invalid."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(400, "Conflict if there is a duplicate plate."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] ListMotorcycles()
    {
        return
        [
            new SwaggerOperationAttribute("ListMotorcycles", "Lists motorcycles based on search criteria."),
            new SwaggerResponseAttribute(200, "Successful response with a list of motorcycles."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] UpdateMotorcyclePlate()
    {
        return
        [
            new SwaggerOperationAttribute("UpdateMotorcyclePlate", "Updates the plate number of an existing motorcycle."),
            new SwaggerResponseAttribute(204, "No content if the update is successful."),
            new SwaggerResponseAttribute(400, "Bad request if the plate data is invalid."),
            new SwaggerResponseAttribute(404, "Not found if the motorcycle does not exist."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(400, "Conflict if the new plate already exists."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] UpdateMotorcycleStatus()
    {
        return
        [
            new SwaggerOperationAttribute("UpdateMotorcycleStatus", "Updates the status of an existing motorcycle."),
            new SwaggerResponseAttribute(204, "No content if the update is successful."),
            new SwaggerResponseAttribute(400, "Bad request if the status data is invalid."),
            new SwaggerResponseAttribute(404, "Not found if the motorcycle does not exist."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] DeleteMotorcycle()
    {
        return
        [
            new SwaggerOperationAttribute("DeleteMotorcycle", "Deletes a motorcycle by its plate number."),
            new SwaggerResponseAttribute(204, "No content if the deletion is successful."),
            new SwaggerResponseAttribute(404, "Not found if the motorcycle does not exist."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs or if the motorcycle cannot be deleted due to existing records.")
        ];
    }

    public static Attribute[] CreateOrder()
    {
        return
        [
            new SwaggerOperationAttribute("CreateOrder", "Creates a new order in the system."),
            new SwaggerResponseAttribute(200, "Returns the details of the newly created order."),
            new SwaggerResponseAttribute(400, "Bad request if the data is invalid."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] GetNotifiedPartners()
    {
        return
        [
            new SwaggerOperationAttribute("GetNotifiedPartners", "Retrieves a list of delivery partners notified about an order."),
            new SwaggerResponseAttribute(200, "Successful response with a list of notified delivery partners."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(404, "Not found if the order does not exist."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] CheckOrderAvailability()
    {
        return
        [
            new SwaggerOperationAttribute("CheckOrderAvailability", "Checks the availability of an order for delivery."),
            new SwaggerResponseAttribute(204, "No content if the order is available for delivery."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(404, "Not found if the order does not exist."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] UpdateOrderStatus()
    {
        return
        [
            new SwaggerOperationAttribute("UpdateOrderStatus", "Updates the status of an existing order."),
            new SwaggerResponseAttribute(204, "No content if the update is successful."),
            new SwaggerResponseAttribute(400, "Bad request if the update data is invalid."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(404, "Not found if the order does not exist."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] RentMotorcycle()
    {
        return
        [
            new SwaggerOperationAttribute("RentMotorcycle", "Rents a motorcycle for a specified period."),
            new SwaggerResponseAttribute(200, "Returns the cost and details of the motorcycle rental."),
            new SwaggerResponseAttribute(400, "Bad request if the rental data is invalid."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(400, "Conflict if there is an ongoing rental that conflicts."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs during the rental process.")
        ];
    }

    public static Attribute[] PeekRentalPrice()
    {
        return
        [
            new SwaggerOperationAttribute("PeekRentalPrice", "Calculates the potential rental price for a motorcycle without creating a rental record."),
            new SwaggerResponseAttribute(200, "Successful response with the calculated rental price."),
            new SwaggerResponseAttribute(400, "Bad request if the input data is invalid."),
            new SwaggerResponseAttribute(500, "Internal server error if an error occurs in calculating the price.")
        ];
    }

    public static Attribute[] ReturnMotorcycleRental()
    {
        return
        [
            new SwaggerOperationAttribute("ReturnMotorcycleRental", "Finalizes the ongoing motorcycle rental for the current user."),
            new SwaggerResponseAttribute(204, "No content if the return is successfully processed."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(404, "Not found if there is no ongoing rental to finalize."),
            new SwaggerResponseAttribute(500, "Internal server error if an error occurs during the return process.")
        ];
    }

    public static Attribute[] CreateAdministrator()
    {
        return
        [
            new SwaggerOperationAttribute("CreateAdministrator", "Creates a new administrator in the system."),
            new SwaggerResponseAttribute(204, "No content if the administrator is successfully created."),
            new SwaggerResponseAttribute(400, "Bad request if the administrator data is invalid."),
            new SwaggerResponseAttribute(403, "Forbidden if the user is not authorized."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] CreateDeliveryPartner()
    {
        return
        [
            new SwaggerOperationAttribute("CreateDeliveryPartner", "Creates a new delivery partner in the system."),
            new SwaggerResponseAttribute(204, "No content if the delivery partner is successfully created."),
            new SwaggerResponseAttribute(400, "Bad request if the delivery partner data is invalid."),
            new SwaggerResponseAttribute(400, "Conflict if a delivery partner with the same National ID already exists."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs.")
        ];
    }

    public static Attribute[] Authenticate()
    {
        return
        [
            new SwaggerOperationAttribute("Authenticate", "Authenticates a user and returns a JWT token if successful."),
            new SwaggerResponseAttribute(200, "Returns the JWT token for the authenticated user."),
            new SwaggerResponseAttribute(400, "Bad request if the login credentials are invalid."),
            new SwaggerResponseAttribute(401, "Unauthorized if the credentials do not match any user."),
            new SwaggerResponseAttribute(500, "Internal server error if an exception occurs during authentication.")
        ];
    }
}
