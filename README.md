# RapidOrders - Middleware Assignment

## Overview

The RapidOrders application is designed to streamline product ordering and is composed of four key services:

1. **ProductService:** A gRPC client service facilitating order placement and updates.
2. **GrpcOrderService:** A gRPC server allowing the creation and modification of orders.
3. **NotificationService1:** Listens for notifications related to order creation from a RabbitMQ exchange.
4. **NotificationService2:** Listens for notifications related to both order creation and updates from a RabbitMQ topic.

## Table of Contents

- [Installation](#installation)

## Installation

Follow these steps for a seamless installation:

### Prerequisites

1. **Windows Machine:**
   - Ensure you are using a Windows environment.

2. **Visual Studio 2022 Preview:**
   - If not already installed, download and install [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview/#download-preview) with the ASP.NET and web development workload.

3. **.NET SDK 8.0:**
   - If not already installed, download and install [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

4. **RabbitMQ:**
   - Install RabbitMQ from [rabbitmq.com/download.html](https://www.rabbitmq.com/download.html).

### Installation Steps

1. **Clone the Repository:**
   - Open a terminal and run the following command:
     ```sh
     git clone https://github.com/abhi6690/MiddlewareAssignment.git
     ```

2. **Update Configuration:**
   - Navigate to the following paths and update the `appsettings.json` file with RabbitMQ credentials in each project:
     - `.\MiddlewareAssignment\GrpcOrderService\appsettings.json`
     - `.\MiddlewareAssignment\NotificationService1\appsettings.json`
     - `.\MiddlewareAssignment\NotificationService2\appsettings.json`

     ```json
     "RabbitMQ": {
         "HostName": "localhost",
         "UserName": "guest",
         "Password": "guest"
     }
     ```

3. **Build and Run:**
   - Navigate to the root folder and execute the PowerShell script `Start_All_Services.ps1` to build and run all projects.

By following these steps, you'll have the RapidOrders application up and running, ready to handle your product ordering needs efficiently.
