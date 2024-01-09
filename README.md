# Blazor Ecommerce Project


## 🚀 Quick start

1.  **Step 1.**
    Clone the project
    ```sh
    git clone https://github.com/zhengwuqingling28/BlazorEcommerce.git
    ```
1.  **Step 2.**
    change connection string in Server/appsettings.json
    ```sh
     "ConnectionStrings": {
    "DefaultConnection": "server=localhost\\sqlexpress;database=blazorecommerce;trusted_connection=true"
    },
    ```
 1. **Step 3.**
    import database
    ```sh
    add-migration InitialDatabase
    update-database
    ```
1.  **Step 4.**
    Run project

1. **Step 5.**
    run Stripe CLI
    ```she
    cd Development/Stripe
    ```
    Verifies your authentication with Stripe
    ```she
    stripe login
    ```
    Using Stripe API Version
    ```she
    stripe listen --forward-to https://localhost:7276/api/payment
    ```
