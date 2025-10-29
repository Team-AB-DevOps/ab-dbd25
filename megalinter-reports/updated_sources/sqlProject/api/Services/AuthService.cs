using api.ExceptionHandlers;
using api.Interfaces;
using api.Models;
using api.Models.DTOs.Auth;

namespace api.Services;

public class AuthService : IAuthService
{
    public IUserRepository UserRepository { get; set; }
    public IPrivilegesRepository PrivilegesRepository { get; set; }
    public IPasswordHasher PasswordHasher { get; set; }
    public IJwtGenerator JwtGenerator { get; set; }

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtGenerator jwtGenerator,
        IPrivilegesRepository privilegesRepository
    )
    {
        UserRepository = userRepository;
        PasswordHasher = passwordHasher;
        JwtGenerator = jwtGenerator;
        PrivilegesRepository = privilegesRepository;
    }

    public async Task<UserResponseDto> Register(RegisterRequestDto registerRequest)
    {
        if (!registerRequest.Password.Equals(registerRequest.Password2))
        {
            throw new BadRequestException("Passwords are not matching");
        }

        var existingMail = await UserRepository.GetByEmail(registerRequest.Email);

        if (existingMail != null)
        {
            throw new BadRequestException("Email is unavailable");
        }

        var hashedPassword = PasswordHasher.Hash(registerRequest.Password);

        var userPrivilege = await PrivilegesRepository.GetByName("USER");
        if (userPrivilege == null)
        {
            throw new BadRequestException("Default user privilege not found");
        }

        var newUser = new User
        {
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Email = registerRequest.Email,
            Password = hashedPassword,
            Privileges = new List<Privilege>() { userPrivilege },
        };

        await UserRepository.CreateUser(newUser);
        return new UserResponseDto(newUser.FirstName, newUser.LastName, newUser.Email, newUser.Id);
    }

    public async Task<TokenUserResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var userInDb = await UserRepository.GetByEmail(loginRequestDto.Email);
        if (userInDb == null)
        {
            throw new BadRequestException("Email and/or password is wrong");
        }

        var matchingPassword = PasswordHasher.Verify(userInDb.Password, loginRequestDto.Password);
        if (!matchingPassword)
        {
            throw new BadRequestException("Email and/or password is wrong");
        }

        var token = JwtGenerator.GenerateToken(userInDb);

        return new TokenUserResponseDto(
            token,
            new UserResponseDto(userInDb.FirstName, userInDb.LastName, userInDb.Email, userInDb.Id)
        );
    }
}
