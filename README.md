# Coding Challenge Solution

This project provides an efficient solution to interact with the Frankfurter API without overloading it, ensuring a smooth and responsive experience for users. By caching API responses, we minimize the frequency of requests sent to Frankfurter, balancing the load and improving performance.

## Key Features

- **Caching Mechanism**: Responses from the Frankfurter API are cached in memory. This reduces the number of requests sent to the API, preventing overload and improving response time for subsequent requests.
- **Swagger UI Integration**: Easily test the API endpoints through a user-friendly interface provided by Swagger UI.
- **Support for Multiple Request Methods**: The API can be tested using Swagger UI, Postman, or a simple curl command.

## Assumptions

The rate in which the data changes will not that frequent, that's why 5 minutes were used as a value for the refresh rate.

## Getting Started

### Prerequisites

- .NET SDK installed on your machine.

### Running the Project

1. Clone this repository to your local machine.
2. Navigate to the `CurrencyApp` folder in your terminal.
3. Run the following command to start the project:
```
dotnet build
dotnet run
```

### Testing the API

You can test the API using one of the following methods:

- **Swagger UI**: Navigate to `http://localhost:5090/swagger` in your browser to access the Swagger UI interface.
- **Postman**: Send a GET request to `http://localhost:5090/api/currency/convert?from=usd&to=eur&amount=30000`.
- **Curl Command**: Use the following curl command:
```bash
curl -X 'GET' \
  'http://localhost:5090/api/currency/convert?from=usd&to=eur&amount=30000' \
 -H 'accept: */*'
```
## Future Enhancements

- **Dockerize the Application**: Containerize the application to simplify deployment and ensure consistency across different environments.

- **Add Code Comments**: Improve code readability and maintenance by adding comments throughout the codebase.

- **Expand Testing**: Create a new project within the solution that contains unit tests and integration tests to ensure code quality and reliability.
