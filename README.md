# DotNetOrderService

A robust order processing microservice built with .NET, designed for cloud-native deployment on **Microsoft Azure**. It leverages the **Transactional Outbox Pattern** via **MassTransit** to ensure reliable, event-driven messaging and prevent data loss during distributed transactions.

## Key Features
* **Azure Service Bus:** Asynchronous message brokering and event publishing across microservices.
* **Azure SQL Database:** Relational data persistence for order lifecycle management.
* **Azure Key Vault:** Secure, centralized storage for connection strings and application secrets.
* **Transactional Outbox Pattern (MassTransit):** Guarantees atomicity between database writes and message dispatching, ensuring at-least-once delivery without phantom events.
* **Idempotency:** Ensures that repeated order calls with an identical request id will not create a new order.
