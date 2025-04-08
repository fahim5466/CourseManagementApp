# CourseManagementApp

## Setup Instructions

**Required:** Docker desktop

**Recommended (for ease of use):** Docker desktop, .NET SDK 9.0, Visual studio 2022

### Steps
1. Download the source code as zip and extract it.
2. We have to create a self-signed HTTPS certificate. It can be easily done if you have .NET SDK installed. Open command prompt and navigate inside the solution folder. Perform the following commands:
```
mkdir https_cert
dotnet dev-certs https -ep https_cert\myapp_https_cert.pfx -p my-secret-password-for-https
dotnet dev-certs https --trust
```
3. You can use any other method to generate a self-signed certificate. Whatever the approach, we have to do the following:
    - Create a self-signed HTTPS certificate and trust it. Use 'my-secret-password-for-https' as the secret key to generate the certificate.
    - Export the certificate as 'myapp_https_cert.pfx'.
    - Inside the solution folder, create a new folder named 'https_cert' and move the pfx file inside it.
4. Open docker desktop. Make sure you have an internet connection. Docker desktop should also have network permissions, firewall access, etc.
5. Inside docker desktop, open a terminal. Navigate inside the solution folder. Perform the following command:
```
docker compose up --build
```
6. It will take some time to download the required images, start the containers and make everything ready. You can also open and run the solution using visual studio 2022 for ease of use.
7. Open postman with our API collection. The database has been seeded with the following users. Log in as one of them and test out the APIs
```
User with admin role
---
Email: admin@test.com
Password: abc12345!

User with staff role
---
Email: staff@test.com
Password: abc12345!

User with student role
---
Email: student@test.com
Password: abc12345!
```
8. You can also access the papercut UI at 'http://localhost:5003'. You can open any email the application sends here.
9. You can also access the seq logging UI at 'http://localhost:5004'. You can easily view any log the application generates here.

## Entity-Relation Diagram of Database
![ERD_diagram](ERD_diagram.png)
