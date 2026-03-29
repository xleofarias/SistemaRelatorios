# 📊 Asynchronous Report Generator (Distributed System)

> An enterprise-grade distributed architecture demonstrating the **Asynchronous Request-Reply** pattern using .NET 8, RabbitMQ, and PostgreSQL.

## 🎯 The Problem Solved
Traditional synchronous APIs block the client and consume server resources when processing heavy tasks (like large data exports). This project solves that problem by decoupling the HTTP request from the heavy processing using a Message Broker.

## 🏗️ Architecture Overview
This is a Monorepo containing three main components:

* **Relatorios.API (Producer):** A high-performance Minimal API that receives the request, saves a "Pending" state in PostgreSQL, publishes an event to RabbitMQ, and immediately returns an `HTTP 202 Accepted`.
* **Relatorios.Worker (Consumer):** A background service that listens to the RabbitMQ queue, simulates the heavy data processing asynchronously, and updates the database state to "Completed".
* **Relatorios.Contracts:** A shared class library containing the message contracts (Events) for the MassTransit broker.

## 💻 Tech Stack
* **C# & .NET 8**
* **MassTransit & RabbitMQ** (Message Broker)
* **Entity Framework Core & PostgreSQL** (Persistence)
* **Minimal APIs** (RESTful Routing)

## 🚀 How to Run Locally

1. Clone this repository to your machine.
2. Ensure you have a PostgreSQL database and a RabbitMQ instance running (Local or Cloud).
3. Configure your connection strings using .NET User Secrets in both the API and Worker projects:
   `dotnet user-secrets set "ConnectionStrings:RelatoriosAPI" "Your_Postgres_String"`
   `dotnet user-secrets set "ConnectionStrings:ConnectionRabbit" "Your_RabbitMQ_String"`
4. Run `dotnet ef database update --project Relatorios.API/RelatoriosAPI/` 
from the repository root.
5. Start both the API and the Worker simultaneously using `dotnet run`.

## 🐳 How to Run with Docker

1. Clone this repository.
2. Copy the environment file and fill in your values:
   `cp .env.example .env`
3. Apply database migrations:
   `dotnet ef database update --project Relatorios.API/RelatoriosAPI/ --connection "Host=localhost;Port=5433;Database=RelatoriosDb;Username=...;Password=..."`
4. Start all services:
   `docker compose up --build`

The API will be available at `http://localhost:8080`.

## 📡 API Endpoints

* `POST /api/relatorios` 
  Triggers the background job. Returns `202 Accepted` and the tracking URL.
* `GET /api/relatorios/{id}/status` 
  Polls the current status of the job (Pending, Processing, Completed).
