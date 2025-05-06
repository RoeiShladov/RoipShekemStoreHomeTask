using Microsoft.IdentityModel.Tokens;
using RoipBackend.Models;

namespace RoipBackend
{
    //Constants class for hardcoded strings and numbers.
    public class C
    {
        // JWT and Authentication
        public const int JWT_EXPIRATION_TIME = 60; // 60 minutes
        public const string JWT_INVALID_STR = "JWT token is invalid";
        public const string JWT_VALID_STR = "JWT token is valid";
        public const string JWT_INVALID_DESC_STR = "JWT token is empty, invalid, or expired. Please log in again.";
        public const string JWT_MODEL_STATE_INVALID_STR = "User Needs to successfully authenticate to make further actions.";
        public const string JWT_SIGN_ALGORITHM_STR = SecurityAlgorithms.HmacSha256;
        public const string JWT_GENERATED_SUCCESSFULLY_STR = "JWT token generated successfully.";
        public const string JWT_GENERATION_FAILED_STR = "Failed to generate JWT token.";
        public const string JWT_GENERATION_FAILED_DESC_STR = "Failed to generate JWT token due to system problem. Please try again to login";
        public const string JWT_DETAILS_RETRIEVED_SUCCESSFULLY_STR = "JWT token details retrieved successfully.";
        public const string JWT_CONTENT_CHECK_FAILED_STR = "JWT token content check failed.";
        public const string JWT_CONTENT_CHECK_FAILED_DESC_STR = "JWT token content check failed. Please try again to login";

        // ID Ranges
        public const int ID_MINIMUM_RANGE = 1;
        public const int ID_MAXIMUM_RANGE = 100;

        // Logging
        public const int LOG_LEVEL_MAXIMUM_LENGTH = 20;
        public const int LOG_MESSAGE_MAXIMUM_LENGTH = 500;
        public const int LOG_EXCEPTION_MAXIMUM_LENGTH = 2000;
        public const int LOG_ADDITIONAL_DATA_MAXIMUM_LENGTH = 100;
        public const string LOG_INFO_STR = "Info";
        public const string LOG_ERROR_STR = "Error";
        public const string LOG_WARNING_STR = "Warning";
        public const string LOG_CRITICAL_STR = "Critical";
        public const string LOG_FATAL_STR = "Fatal";
        public const string LOG_DEBUG_STR = "Debug";
        public const string LOG_TRACE_STR = "Trace";
        public const string FAILED_ADDING_DATABASE_LOG_STR = "Failed to add log entry to the database.";
        public const string EXCEPTION_STR = "Exception";
        public const string MESSAGE_STR = "Message";

        // User Validation
        public const int MINIMUM_USER_NAME_LENGTH = 2;
        public const int MINIMUM_PASSWORD_LENGTH = 8;
        public const int MAXIMUM_PASSWORD_LENGTH = 16;
        public const string USER_NAME_MANDATORY_STR = "Username is mandatory.";
        public const string USER_PASSWORD_MANDATORY_STR = "Password is mandatory.";
        public const string USER_EMAIL_MANDATORY_STR = "Email is mandatory.";
        public const string USER_PHONE_MANDATORY_STR = "Phone number is mandatory.";
        public const string USER_ADDRESS_MANDATORY_STR = "Address is mandatory.";
        public const string INVALID_EMAIL_FORMAT_STR = "Invalid email format.";
        public const string USER_PASSWORD_REQUIRED_STR = "Username and Password are required.";
        public const string USER_PROPERTIES_NOT_VALID_STR = "Some of User properties are not valid.";
        public const string USER_EMAIL_MODEL_STATE_INVALID_STR = "User Email text cannot exceed 100 characters.";
        public const string USER_PASSWORD_MODEL_STATE_INVALID_STR = "User Password text cannot exceed 16 characters. Please try again";
        public const string EMAIL_ALREADY_EXISTS_STR = "Email already exists. Please try again or contact Admin.";
        public const string EMAIL_ALREADY_EXISTS_DESC_STR = "Email already exists. Please try again or contact Admin.";
        public const string USER_NAME_MODEL_STATE_INVALID_STR = "User Name text cannot exceed 100 characters.";
        public const string USER_PHONE_MODEL_STATE_INVALID_STR = "User Phone text cannot exceed 100 characters.";
        public const string USER_ADDRESS_MODEL_STATE_INVALID_STR = "User Address text cannot exceed 100 characters.";
        public const string USER_NAME_MINIMUM_LENGTH_STR = "User Name must be at least 2 characters long.";
        public const string USER_NAME_MAXIMUM_LENGTH_STR = "User Name cannot exceed 100 characters.";
        public const string USER_PASSWORD_MINIMUM_LENGTH_STR = "User Password must be at least 8 characters long.";
        public const string USER_PASSWORD_MAXIMUM_LENGTH_STR = "User Password cannot exceed 16 characters.";
        public const string USER_PHONE_MINIMUM_LENGTH_STR = "User Phone must be at least 2 characters long.";
        public const string USER_PHONE_MAXIMUM_LENGTH_STR = "User Phone cannot exceed 100 characters.";
        public const string USER_ADDRESS_MINIMUM_LENGTH_STR = "User Address must be at least 2 characters long.";
        public const string USER_ADDRESS_MAXIMUM_LENGTH_STR = "User Address cannot exceed 100 characters.";
        public const string USER_NAME_ALREADY_EXISTS_STR = "User name already exists. Please try again or contact Admin.";
        public const string EMAIL_NOT_FOUND1_STR = "Email: ";
        public const string EMAIL_NOT_FOUND2_STR = " was not found. Please try again or contact Admin.";
        public const string FAILED_LOGIN_STR = "Failed to log in. Please try again or contact Admin.";
        public const string USER_NAME_ALREADY_EXISTS_DESC_STR = "User name already exists. Please try again or contact Admin.";
        public const string FAILED_LOGIN_DESC_STR = "Failed to log in. Please try again or contact Admin.";
        public const string FAILED_RETRIEVING_USERS_DESC_STR = "Failed to retrieve users. Please try again or contact Admin.";
        public const string NO_EMAIL_FOUND_STR = "No email found. Please try again or contact Admin.";
        public const string LOGIN_PASSWORD_FAILED_STR = "Login failed, Wrong password.";
        public const string MODEL_STATE_VALIDATION_FAILED_STR = "Model state validation failed. Please check the input data and try again.";
        public const string USERS_RETRIEVE_SUCCESS_STR = "Users retrieved successfully.";
        public const string NO_USERS_FOUND_STR = "No users found. Please try again or contact Admin.";
        public const string NO_USERS_FOUND_DESC_STR = "No users found. Please try again or contact Admin.";
        public const string REGISTRATION_FAILED_STR = "Registration failed. Please try again or contact Admin.";
        public const string REGISTRATION_FAILED_DESC_STR = "Registration failed. Please try again or contact Admin.";
        public const string USER_LOGGED_IN_SUCCESSFULLY_STR = "User logged in successfully.";
        public const string WRONG_PASSWORD_DESC_STR = "Wrong password. Please try again or contact Admin.";
        public const string MODEL_STATE_INVALID_STR = "Model state is invalid. Please check the input data and try again.";
        public const string USER_REGISTERED_SUCCESSFULLY_STR = "User registered successfully.";

        // Product Validation
        public const int MINIMUM_PRODUCT_NAME_LENGTH = 2;
        public const int MAXIMUM_PRODUCT_NAME_LENGTH = 30;
        public const int DESCRIBTION_MAXIMUM_LENGTH = 256;
        public const int PRODUCT_MIN_VALUE = 1;
        public const int QUANTITY_MINIMUM_VALUE = 0;
        public const int QUANTITY_MAXIMUM_VALUE = 10;
        public const string PRODUCT_NAME_MANDATORY_STR = "Product name is mandatory.";
        public const string PRODUCT_NAME_MODEL_STATE_INVALID_STR = "Product Name text cannot exceed 100 characters.";
        public const string PRODUCT_DESCRIBTION_STR = "Description cannot exceed 256 characters.";
        public const string PRODUCT_QUANTITY_MODEL_STATE_INVALID_STR = "Product Quantity cannot exceed 10 units. Please choose less";
        public const string PRICE_ABOVE_ZERO_STR = "Price must be greater than 0.";
        public const string QUANTITY_NOT_NEGATIVE_STR = "Quantity cannot be negative.";
        public const string FAILED_ADDING_PRODUCT_DESC_STR = "Failed to add product. Please try again or contact Admin.";
        public const string PRODUCT_NAME_MINIMUM_LENGTH_STR = "Product Name must be at least 2 characters long.";
        public const string PRODUCT_NAME_MAXIMUM_LENGTH_STR = "Product Name cannot exceed 30 characters.";
        public const string PRODUCT_PRICE_MINIMUM_VALUE_STR = "Product Price must be greater than 0.";
        public const string PRODUCT_PRICE_MAXIMUM_VALUE_STR = "Product Price cannot exceed 1000000.";
        public const string PRODUCT_QUANTITY_MINIMUM_VALUE_STR = "Product Quantity must be at least 0.";
        public const string PRODUCT_QUANTITY_MAXIMUM_VALUE_STR = "Product Quantity cannot exceed 10.";
        public const string FAILED_DELETING_PRODUCT_DESC_STR = "Failed to delete product. Please try again or contact Admin.";
        public const string PRODUCT_NAME_ALREADY_EXISTS_STR = "Product name already exists. Please try again or contact Admin.";
        public const string FAILED_UPDATE_PRODUCT_STR = "Failed to update product. Please try again or contact Admin.";
        public const string FAILED_UPDATE_PRODUCT_DESC_STR = "Failed to update product. Please try again or contact Admin.";
        public const string FILTER_MODEL_STATE_INVALID_STR = "Filter text cannot exceed 100 characters.";
        public const string MAXIMUM_PRICE_MODEL_STATE_INVALID_STR = "Maximum Price cannot exceed 100.";
        public const string MINIMUM_PRICE_MODEL_STATE_INVALID_STR = "Minimum Price cannot exceed 2.";
        public const string IMAGE_URL_VALIDATION_STR = "Image URL is not valid.";
        public const string INVALID_PAGINATION_PARAMETERS_STR = "Invalid pagination parameters. Page number and page size must be greater than 0.";
        public const string INVALID_PAGINATION_PARAMETERS_DESC_STR = "Invalid pagination parameters. Page number and page size must be greater than 0.";
        public const string HEALTH_CHECK_SUCCEEDED_STR = "Health check succeeded.";
        public const string HEALTH_CHECK_FAILED_STR = "Health check failed.";
        public const string LOGOUT_SUCCESSFULLY_STR = "User successfully logged out.";
        public const string PRODUCTS_RETRIEVE_SUCCESS_STR = "Products retrieved successfully.";
        public const string NO_PRODUCTS_FOUND_STR = "No products found. Please try again or contact Admin.";
        public const string NO_PRODUCTS_FOUND_DESC_STR = "No products found. Please try again or contact Admin.";
        public const string PRODUCT_DELETED_SUCCESSFULLY_STR = "Product deleted successfully.";

        // API Endpoints
        public const string GET_ALL_USERS_API_STR = "all-users";
        public const string GET_ALL_PRODUCTS_API_STR = "all-products";
        public const string REGISTER_API_STR = "register";
        public const string LOGIN_API_STR = "login";
        public const string LOGOUT_API_STR = "logout";
        public const string ADD_PRODUCT_API_STR = "add-product";
        public const string DELETE_PRODUCT_API_STR = "delete-product";
        public const string BUY_PRODUCT_API_STR = "buy-product";
        public const string SEARCH_PRODUCT_API_STR = "search-filter";
        public const string HEALTH_CHECK_API_STR = "health-check";

        // Error Messages
        public const string INTERNAL_SERVER_ERROR_STR = "Status: 500, Internal server error. Please try again or contact Admin";
        public const string DATABASE_CONNECTION_ERROR_STR = "Database connection error";
        public const string DATABASE_CONNECTION_TIMEOUT_STR = "The Database connection exceeded the timeout limit. (30 Seconds).\n Please try again or contact Admin.";
        public const string REQUEST_TIME_OUT_STR = "Request timeout";
        public const string INVALID_OPERATION_STR = "Invalid operation";
        public const string INVALID_OPERATION_DESC_STR = "Invalid operation. Please contact Admin.";
        public const string ARGUMENT_NULL_STR = "Argument null exception";
        public const string ARGUMENT_NULL_DESC_STR = "Argument null exception. Please contact Admin.";
        public const string CONCURRENCY_ERROR_STR = "Concurrency error";
        public const string CONCURRENCY_ERROR_DESC_STR = "Please try again or contact Admin.";
        public const string FORMAT_EXCEPTION_STR = "Format exception";
        public const string INVALID_SIGNATURE_STR = "Invalid signature";
        public const string JWT_EXPIRED_STR = "JWT token expired";
        public const string SECURITY_TOKEN_EXCEPTION_STR = "Security token exception when trying to generate new JWT token.";
        public const string ERROR_CLEANING_DB_LOGS_STR = "Error cleaning database logs.";
        public const string ERROR_STOPPING_TIMER_STR = "Error stopping timer.";
        public const string DATABASE_LOG_CLEANUP_SERVICE_STOPPED_STR = "DatabaseLogCleanupService has stopped.";
        public const string ERROR_CONNECTING_HUB_STR = "Error in OnConnectedAsync:";
        public const string ERROR_DISCONNECTING_HUB_STR = "Error in OnDisconnectedAsync:";
        public const string ERROR_FETCHING_AUTHENTICATED_USER_STR = "Error in FetchAuthenticatedUser:";

        // Environment
        public const string PRODUCTION_ENV_STR = "Production";
        public const string DEVELOPMENT_ENV_STR = "Development";
        public const string ASPNETCORE_ENV_NAME_STR = "ASPNETCORE_ENVIRONMENT";

        // Roles
        public const string ADMIN_STR = "Admin";
        public const string CUSTOMER_STR = "Customer";
        public const string ANONYMOUS_STR = "Anonymous";

        // Miscellaneous
        public const int DATABASE_LOGS_CLEANUP_INTERVAL = 14; // 14 days
        public const int NEGATIVE_LOGS_CLEANUP_INTERVAL = -14; // 14 days
        public const int DB_REQUEST_TIMEOUT = 30; // 30 seconds
        public const int ADDITIONAL_FILE_LOGS_INTERVAL = 1800000; // 30 minutes
        public const string NAME_STR = "Name";
        public const string EMAIL_STR = "Email";
        public const string ROLE_STR = "Role";
        public const string CASHED_RETRY_LOGS_FILE_LOCATION_STR = "../CashedQuerylogs/fallback.log";
        public const string ADDITIONAL_EXCEPTION_MESSAGE_LOG_STR = "Additional Exception message from LoggerService";
        public const string ERROR_PROCESSING_FALLBACK_LOG_STR = "Error while processing fallback log";
        public const string HUB_LIVE_BROADCAST_ACTION_STR = "UpdateLiveConnectedUsers";
        public const string HUB_LIVE_BROADCAST_URL_STR = "/user-connection-hub";
        public const string HUB_LIVE_ON_DEMAND_BROADCAST_STR = "ReceiveConnectedUsers";
        public const string FILTERED_PRODUCTS_RETRIEVE_SUCCESS_STR = "Filtered products retrieved successfully.";
        public const string SOMETHING_WENT_WRONG_STR = "Action failed";
        public const string SOMETHING_WENT_WRONG_DESC_STR = "Something went wrong. Please try again or contact Admin.";
        public const string ACTION_CONNECTION_TIMEOUT_DESC_STR = "Action connection timeout(30 seconds). Please try again or contact Admin.";
        
        public const string FAILED_RETRIEVING_PRODUCTS_STR = "Failed to retrieve products.";
        public const string FAILED_RETRIEVING_PRODUCTS_DESC_STR = "Failed to retrieve products. Please try again or contact Admin.";
        public const string FAILED_RETRIEVING_USERS_STR = "Failed to retrieve users.";
        public const string FAILED_RETRIEVING_PRODUCT_STR = "Failed to retrieve product.";
        public const string FAILED_RETRIEVING_USER_STR = "Failed to retrieve user.";
        public const string FAILED_ADDING_PRODUCT_STR = "Failed to add product.";
        public const string FAILED_ADDING_USER_STR = "Failed to add user.";
        public const string FAILED_DELETING_PRODUCT_STR = "Failed to delete product.";
        public const string FAILED_DELETING_USER_STR = "Failed to delete user.";
        public const string FAILED_BUYING_PRODUCT_STR = "Failed to buy product.";
        public const string FAILED_BUYING_PRODUCT_DESC_STR = "Failed to buy product. Please try again or contact Admin.";
        public const string FAILED_SEARCHING_PRODUCT_STR = "Failed to search product.";
        public const string FAILED_SEARCHING_PRODUCT_DESC_STR = "Failed to search product. Please try again or contact Admin.";
        public const string FAILED_LOGGING_IN_STR = "Failed to log in.";
        public const string FAILED_LOGGING_IN_DESC_STR = "Failed to log in. Please try again or contact Admin.";
        public const string FAILED_LOGGING_OUT_STR = "Failed to log out.";
        public const string FAILED_LOGGING_OUT_DESC_STR = "Failed to log out. Please try again or contact Admin.";
        public const string FAILED_REGISTERING_STR = "Failed to register.";
        public const string FAILED_REGISTERING_DESC_STR = "Failed to register. Please try again or contact Admin.";
        public const string FAILED_UPDATING_PRODUCT_STR = "Failed to update product.";
        public const string FAILED_UPDATING_PRODUCT_DESC_STR = "Failed to update product. Please try again or contact Admin.";
        public const string PRODUCT_NOT_FOUND1_STR = "Product name: ";
        public const string PRODUCT_NOT_FOUND2_STR = " was not found. Please try again or contact Admin.";
        public const string PRODUCT_NOT_FOUND_STR = "Product not found.";

        public const string INSUFFICIENT_QUANTITY_STR = "Insufficient quantity";
        public const string INSUFFICIENT_QUANTITY_DESC_STR = "Insufficient quantity. Please try again or contact Admin.";
        public const string PRODUCT_QUANTITY_SUCCESSFULLY_UPDATED_STR = "Product quantity successfully updated.";

        public const string ADDRESS_REGEX_STR = @"^[a-zA-Z0-9\s,.'-]{3,}$"; // Example regex for address validation
        public const string INVALID_ADDRESS_STR = "Invalid address format. Please try again.";
        public const string PLEASE_VALID_EMAIL_STR = "Please enter a valid email address.";
        public const string PLEASE_VALID_PHONE_STR = "Please enter a valid phone number.";
        public const string PLEASE_VALID_PRODUCT_NAME_STR = "Please enter a valid product name.";
        public const string PLEASE_VALID_PRODUCT_QUANTITY_STR = "Please enter a valid product quantity.";
        public const string PLEASE_VALID_PRODUCT_PRICE_STR = "Please enter a valid product price.";
        public const string PLEASE_VALID_PRODUCT_DESCRIPTION_STR = "Please enter a valid product description.";
        public const string PLEASE_VALID_PRODUCT_ID_STR = "Please enter a valid product ID.";
        public const string PLEASE_VALID_USER_ID_STR = "Please enter a valid user ID.";
        public const string PLEASE_VALID_USER_NAME_STR = "Please enter a valid user name.";
        public const string PLEASE_VALID_USER_PASSWORD_STR = "Please enter a valid user password.";
        public const string DATABASE_UPDATE_ERROR_STR = "Database update error";
        public const string DATABASE_UPDATE_ERROR_DESC_STR = "Database update error. Please try again or contact Admin.";
        public const string WRONG_PASSWORD_STR = "Wrong password";
        public const string USER_NOT_FOUND_STR = "User not found";
        public const string USER_NOT_FOUND_DESC_STR = "User not found. Please try again or contact Admin.";
        public const string PRODUCT_SUCCESSFULLY_ADDED_STR = "Product successfully added.";
        public const string USER_SUCCESSFULLY_ADDED_STR = "User successfully added.";
        public const string PRODUCT_ALREADY_EXISTS_DESC_STR = "Product already exists. Please try again or contact Admin.";
        public const string USER_ALREADY_EXISTS_DESC_STR = "User already exists. Please try again or contact Admin.";
        public const string USER_SUCCESSFULLY_LOGGED_IN_STR = "User successfully logged in.";
        public const string USER_SUCCESSFULLY_LOGGED_OUT_STR = "User successfully logged out.";
        public const string USER_SUCCESSFULLY_REGISTERED_STR = "User successfully registered.";
        public const string PRODUCT_ALREADY_EXISTS_STR = "Product already exists.";
        public const string USER_ALREADY_EXISTS_STR = "User already exists.";
    }
}