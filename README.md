# Kallipr-Assessment

# Device Telemetry Service

A simple multi-tenant telemetry system built as a take-home assessment.

- **Backend:** ASP.NET Core 8 + SQLite  
- **Frontend:** React 18 + TypeScript (Vite)  
- **Tenant context:** `X-Customer-Id` HTTP header

## Prerequisites

- .NET SDK **8+**
- Node.js **18+**

## Run the Backend

From the backend project directory (where `Program.cs` lives):

```bash
dotnet restore
dotnet ef database update
dotnet run
```

API will run on:
```
http://localhost:7073
```

## Run the Frontend

```bash
npm install
npm run dev
```



