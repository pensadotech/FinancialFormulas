# Financial Formulas Service (API and Kafka)

By Armando Pensado

# Description

This project is a practical example of using **ASP.NET Core** and **Kafka** to build a robust, scalable, and versioned API. It demonstrates how to expose services via both HTTP APIs and Kafka messaging, making it an excellent learning resource for developers exploring these technologies.


## Requirements

- .NET 8 SDK
- A terminal, Visual Studio, or Visual Studio Code

---

## Overview

This service provides financial formula calculations through two interfaces:
1. **HTTP API**: Exposed using ASP.NET Core controllers.
2. **Kafka Messaging**: Allows clients to send requests and receive responses via Kafka topics.

The project also integrates **Swagger** for API documentation and **API versioning** to support backward compatibility.

---

## Architecture

### High-Level Diagram

```text
┌──────────────────────┐      HTTP / JSON / XML      ┌────────────────────────────┐
│ Client / Consumer    │ ──────────────────────────► │ ASP.NET Core API           │
│ Postman / Web App    │                             │ FinancialController        │
└──────────────────────┘                             │ /api/v{version}/financial  │
                                                     └──────────────┬─────────────┘
                                                                    │
                                                                    ▼
                                                     ┌────────────────────────────┐
                                                     │ FinancialService           │
                                                     │ Executes PTI and LTV       │
                                                     │ formula calculations       │
                                                     └──────────────┬─────────────┘
                                                                    │
                         ┌──────────────────────────────────────┼──────────────────────────────────────┐
                         │                                      │                                      │
                         ▼                                      ▼                                      ▼
          ┌──────────────────────┐              ┌──────────────────────┐              ┌──────────────────────┐
          │ Kafka Request Topic  │              │ Formula Logic        │              │ Kafka Response Topic │
          │ Clients send formula │              │ PTI / LTV engine     │              │ Results are returned │
          │ requests             │              │ Calculates values    │              │ to consumers         │
          └──────────────────────┘              └──────────────────────┘              └──────────────────────┘
```

This service accepts formula requests through the HTTP API, processes them in the financial service, and can also integrate with Kafka for asynchronous message-based communication.


## Features

### 1. HTTP API
The service exposes endpoints for financial calculations using ASP.NET Core controllers. These endpoints are documented using **Swagger**, making it easy to explore and test the API.

#### Swagger Integration
- Swagger UI is available at `/swagger/index.html`.
- It provides an interactive interface to test the API.
- Supports multiple API versions.

### 2. Kafka Messaging
The service integrates with Kafka to handle requests and responses via topics. This is useful for asynchronous communication and high-throughput scenarios.

#### Kafka Topics
- **RequestTopic**: Clients send calculation requests.
- **ResponseTopic**: The service sends back the results.

### 3. API Versioning
The service uses **ASP.NET API Versioning** to support multiple versions of the API. This ensures backward compatibility as new features are added.



## How to Run

1. **Clone the Repository**


2. **Configure Kafka**
   - Update the `appsettings.json` file with your Kafka configuration.

3. **Run the Application**

From this folder, run:

```bash
dotnet run

```

4. **Access Swagger**
- Navigate to `https://localhost:<port>/swagger/index.html` to explore the API.



## Learning Points

### ASP.NET Core
- **Controllers**: Learn how to create and expose endpoints.
- **Dependency Injection**: Understand how services are registered and used.
- **Middleware**: Explore how the HTTP pipeline is configured.

### Kafka
- **Producers and Consumers**: Learn how to send and receive messages.
- **Schema Registry**: Understand how Avro schemas are used for message serialization.

### Swagger
- **Interactive Documentation**: See how Swagger simplifies API testing and exploration.

### API Versioning
- **Backward Compatibility**: Learn how to support multiple API versions.



## References

- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Confluent Kafka Documentation](https://docs.confluent.io/platform/current/home.html)
- [Swagger Documentation](https://swagger.io/docs/)
- [ASP.NET API Versioning](https://github.com/dotnet/aspnet-api-versioning)

