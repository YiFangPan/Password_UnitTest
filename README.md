## [Unit Test] Valid one-time password 

### Purpose 
1. How could you write unit test before you get the web api?
2. How could you write unit test before you wirte sql command?

***

### 

#### Naming
 * Class name: AuthenticationService
 * Method: IsValid(String account, String password)
 * Return value: true / false

#### Feature Requirement 
 1. Get account's password from database
 2. Get account's OTP(one-time password) from third-party APIs
 3. Valid => user input password = password from database + OTP password

***

#### You can learn more about...
 1. Use NSubstitute Package for mock object
 2. Use Dependency injection(DI) - Constructor Injection for pass mock object
