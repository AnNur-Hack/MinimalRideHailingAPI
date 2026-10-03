using MinimalRideHailingAPI.Domain.Entities;

namespace MinimalRideHailingAPI.Repositories.Interface;

public interface IUserRepository
{
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User?> GetUserByIdAsync(int id);
    public Task<User?> GetUserByUserIdAsync(string userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    public Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
    
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}