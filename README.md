# JCarrollOnlineV2

A modern ASP.NET MVC web application with comprehensive user authentication and management features.

## Overview

JCarrollOnlineV2 is a full-featured web application built on ASP.NET MVC 5 and .NET Framework 4.8, featuring a robust authentication system using ASP.NET Identity with extensive customization and wrapper interfaces for enhanced testability.

## Key Features

### Community & Social Features

#### Forum System
- **Discussion Forums** - Full-featured forum system for community discussions
- **Topics & Threads** - Organized discussion topics with threaded replies
- **Post Management** - Create, edit, and delete forum posts
- **Forum Moderation** - Administrative tools for managing discussions and content
- **User Interactions** - Reply to threads, quote posts, and engage in discussions
- **Search Functionality** - Find discussions and posts across the forum system

#### Blog System
- **Blog Posts** - Create and publish long-form content and articles
- **Rich Text Editor** - Full HTML editing capabilities for blog content
- **Categories & Tags** - Organize blog posts by topic and tag for easy discovery
- **Comments** - User engagement through blog post comments and discussions
- **Blog Management** - Author dashboard for managing posts and content
- **Publication Controls** - Draft, publish, and schedule blog posts

#### Microposts (Social Feed)
- **Twitter-like Microblogging** - Share short updates, thoughts, and quick messages
- **User Feed** - Timeline of microposts from followed users and personal updates
- **Follow System** - Follow and unfollow other users to customize your feed
- **Real-time Updates** - Live feed updates as new microposts are created
- **Likes & Interactions** - Engage with microposts through likes, shares, and replies
- **Media Attachments** - Share images, links, and multimedia in microposts
- **User Mentions** - Tag and mention other users in microposts

#### Real-Time Chat
- **Live Chat Rooms** - Real-time communication using SignalR technology
- **Private Messaging** - Direct one-on-one conversations with other users
- **Group Chat** - Create and participate in group conversations
- **Online Status** - See who's currently online and available
- **Message History** - Persistent chat history and conversation archives
- **Typing Indicators** - Real-time typing notifications for active conversations
- **Push Notifications** - Receive instant notifications for new messages
- **File Sharing** - Share files and images in chat conversations
- 
### Authentication & Authorization
- **User Registration** - Complete user registration workflow with email confirmation
- **Email Confirmation** - Secure email verification for new accounts
- **Login/Logout** - Standard username/password authentication
- **Two-Factor Authentication (2FA)** - Enhanced security with two-factor sign-in
- **Password Reset** - Self-service password recovery via email
- **External Login Support** - Integration points for external authentication providers (Google, Facebook, etc.)
- **Account Lockout Protection** - Brute force attack prevention

### User Management
- **User Profile Management** - Users can manage their account settings
- **Phone Number Verification** - Optional phone number addition and verification
- **Password Change** - Secure password update functionality
- **External Login Management** - Link/unlink external authentication providers
- **Administrator Functions** - User deletion and management (role-based)

### Email System
- **Welcome Emails** - Automated welcome emails with confirmation links for new users
- **Password Reset Emails** - Professional HTML emails for password recovery
- **Handlebars Email Templates** - Customizable email templates using Handlebars.NET
- **SMTP Integration** - Configurable SMTP server support (currently configured for HostGator)

### Security Features
- **XSRF/CSRF Protection** - Anti-forgery token validation on all state-changing operations
- **SSL/TLS Support** - Secure communication with certificate validation
- **Role-Based Authorization** - Administrator and user role management
- **Account Lockout** - Configurable account lockout policies
- **Secure Cookie Authentication** - Browser remember me functionality

## Architecture

### Design Patterns
- **Wrapper Pattern** - All ASP.NET Identity managers wrapped in testable interfaces
- **Dependency Injection** - Constructor injection for improved testability
- **Repository Pattern** - Clean separation of data access concerns
- **MVC Pattern** - Standard Model-View-Controller architecture

### Testability Features
This application is designed with testability as a first-class concern:

- **Wrapper Interfaces** - All non-virtual ASP.NET Identity methods wrapped in mockable interfaces:
  - `IUserManagerWrapper` - Wraps `ApplicationUserManager`
  - `ISignInManagerWrapper` - Wraps `ApplicationSignInManager`
  - `IAuthenticationManagerWrapper` - Wraps `IAuthenticationManager`
  - `IHttpContextWrapper` - Wraps `HttpContext`
  - `IUrlHelperWrapper` - Wraps `UrlHelper`

- **Multiple Constructor Overloads** - Controllers support multiple constructor patterns:
  - Production constructors with concrete dependencies
  - Test constructors with wrapper interfaces (2-parameter)
  - Comprehensive test constructors with all wrappers (5-parameter)

- **Comprehensive Unit Tests** - Full test coverage for controllers and services using Moq

### Technology Stack
- **Framework**: .NET Framework 4.8
- **Language**: C# 7.3
- **Web Framework**: ASP.NET MVC 5
- **Authentication**: ASP.NET Identity 2.x
- **ORM**: Entity Framework 6
- **Logging**: NLog
- **Email Templates**: Handlebars.NET
- **Dependency Injection**: Owin/OWIN
- **Object Mapping**: Omu.ValueInjecter
- **Testing**: MSTest with Moq

## Project Structure

```
JCarrollOnlineV2/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs    # Authentication and user account management
│   ├── ManageController.cs     # User profile and settings management
│   ├── HomeController.cs       # Home page and main navigation
│   └── UsersController.cs      # Administrator user management
├── Models/              # Data models and entities
├── ViewModels/          # View-specific models
├── Views/               # Razor views
├── Interfaces/          # Wrapper interfaces for testability
├── Infrastructure/      # Wrapper implementations
├── Helpers/            # Utility classes and helpers
├── App_Start/          # Application startup configuration
└── EmailViewModels/    # Email-specific view models

JCarrollOnlineV2.Test/
├── Controllers/        # Controller unit tests
├── Services/          # Service unit tests
└── Helpers/           # Test helper methods
```

## Getting Started

### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.8 SDK
- SQL Server (LocalDB, Express, or full version)
- SMTP server credentials (for email functionality)

### Configuration

1. **Database Connection**
   - Update the connection string in `Web.config`:
   ```xml
   <connectionStrings>
     <add name="DefaultConnection" 
          connectionString="Data Source=(LocalDb)\MSSQLLocalDB;..." 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

2. **SMTP Settings**
   - Configure SMTP settings in `Web.config` appSettings:
   ```xml
   <appSettings>
     <add key="SmtpHost" value="your-smtp-host.com" />
     <add key="SmtpPort" value="587" />
     <add key="SmtpUsername" value="your-email@domain.com" />
     <add key="SmtpPassword" value="your-password" />
     <add key="SmtpFromEmail" value="noreply@domain.com" />
     <add key="SmtpEnableSsl" value="true" />
   </appSettings>
   ```

3. **Database Migration**
   - Run Entity Framework migrations to create the database schema:
   ```powershell
   Update-Database
   ```

### Building and Running

1. Open `JCarrollOnlineV2.sln` in Visual Studio
2. Restore NuGet packages (automatic on build)
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5 for debug, Ctrl+F5 for release)

### Running Tests

```powershell
# Run all tests
dotnet test

# Run tests in Visual Studio
Test > Run All Tests
```

## Key Components

### AccountController
Handles all authentication-related operations:
- User registration with email confirmation
- Login/logout
- Two-factor authentication
- Password reset and recovery
- External authentication provider integration
- Email confirmation

### ManageController
Manages user profile and settings:
- Password changes
- Phone number management
- Two-factor authentication settings
- External login management
- User profile updates

### Email System
Professional email delivery system with:
- HTML email templates using Handlebars
- Automatic email generation for common scenarios
- SMTP configuration with SSL/TLS support
- Email logging and error handling

## Testing

The application includes comprehensive unit tests demonstrating:
- Mocking ASP.NET Identity components
- Testing authentication flows
- Validating email sending
- Testing authorization and security features

### Example Test Pattern
```csharp
// Arrange
Mock<IUserManagerWrapper> mockUserManager = CreateMockedUserManagerWrapper();
Mock<ISignInManagerWrapper> mockSignInManager = CreateMockedSignInManagerWrapper();
Mock<IUrlHelperWrapper> mockUrlHelper = CreateMockedUrlHelperWrapper();

AccountController controller = new AccountController(
    mockUserManager.Object, 
    mockSignInManager.Object,
    null, // Optional wrappers
    null,
    mockUrlHelper.Object);

// Act
ActionResult result = await controller.VerifyCode(model);

// Assert
Assert.IsInstanceOfType(result, typeof(RedirectResult));
```

## Security Considerations

- All passwords are hashed using ASP.NET Identity's secure hashing algorithm
- HTTPS should be enforced in production
- Anti-forgery tokens protect against CSRF attacks
- Account lockout prevents brute force attacks
- Email confirmation prevents fake account creation
- Two-factor authentication available for enhanced security

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is private and proprietary.

## Support

For issues, questions, or contributions, please open an issue on GitHub.

## Changelog

### Version 2.0
- Complete refactor with wrapper interfaces for testability
- Comprehensive unit test coverage
- Handlebars email templates
- Enhanced security features
- Improved user management
- Administrator dashboard

---

**Built with ❤️ using ASP.NET MVC**
