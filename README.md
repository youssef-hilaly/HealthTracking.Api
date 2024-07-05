## HealthTracking API

This project implements a Web API for a health tracking application.

### Database Structure

The application utilizes three database tables:

* **Users:**
    * IdentityId (string)
    * FirstName (string)
    * LastName (string)
    * Email (string)
    * PhoneNumber (string)
    * DateOfBirth (DateTime)
    * Country (string)
    * Address (string)
    * Sex (string)
* **HealthData:**
    * Id (int) (Primary Key)
    * Height (int)
    * Weight (int)
    * BloodPressure (string)
    * BloodSugarLevel (int)
    * BloodType (string)
    * Race (string)
    * UseGlasses (bool)
    * UserId (string) (Foreign Key - Users.IdentityId)
* **RefreshTokens:**
    * Id (int) (Primary Key)
    * UserId (string) (Foreign Key - Users.IdentityId)
    * Token (string)
    * JwtId (string)
    * IsUsed (bool)
    * IsRevoked (bool)
    * ExpiryDate (DateTime)

### API Endpoints

The API utilizes versioning and is segmented into four main areas:

**1. Accounts (api/v1/accounts)**

* **Register:** Creates a new user and returns an authentication token.
* **Login:** Authenticates a user and returns an authentication token.
* **RefreshToken:** Refreshes an expired authentication token.

**2. Profile (api/v1/profile)** - Requires Authentication

* **GetProfile:** Retrieves the current user's profile information.
* **UpdateProfile:** Updates the current user's profile information.

**3. Users (api/v1/users)** - Requires Admin Authentication

* **CRUD** operations (Create, Read, Update, Delete) on User data.

**4. Health (api/v1/health)** - Requires Authentication

* **CRUD** operations (Create, Read, Update, Delete) on HealthData for the authenticated user.
* **GetReport:** Generates a PDF report containing a chart of the user's health data history.

### Project Structure

The project is split into four class libraries:

* **HealthTracking.Api:** Contains controllers and the Program.cs file.
* **HealthTracking.Authentications:** Contains DTOs related to authentication.
* **HealthTracking.Configuration:** Contains static strings used throughout the project.
* **HealthTracking.DataService:** Implements the Repository, Unit of Work, and Singleton design patterns. Also includes the database context class (Entity Framework) and migrations.
* **HealthTracking.Entity:** Defines database entities and project-wide DTOs.

### Technologies Used

* **Authentication.JwtBearer:** Package for JWT-based authentication.
* **EntityFrameworkCore:** Provides data access with Entity Framework Core.
* **IdentityUser:** Uses ASP.NET Identity for user management.
* **Versioning:** Implements API versioning for future compatibility.
* **XlsIO.Net.Core & XlsIORenderer.Net.Core:** Used for generating the health report PDF with charts.

### Getting Started

1. Clone the repository.
2. Install the required NuGet packages.
3. Configure the database connection string in appsettings.json.
4. Run migrations to create the database schema (if not already done).
5. Start the application.

**Note:** This readme provides a high-level overview. Refer to the code for detailed implementation specifics.
