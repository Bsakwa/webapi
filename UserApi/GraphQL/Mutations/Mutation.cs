using Dapper;
using HotChocolate;
using UserApi.Data;
using UserApi.Models;
using UserApi.GraphQL.Types.Inputs;

namespace UserApi.GraphQL.Mutations
{
    public class Mutation
    {
        private readonly DatabaseContext _context;

        public Mutation(DatabaseContext context)
        {
            _context = context;
        }

        [GraphQLDescription("Create a new user")]
        public async Task<User> CreateUser(CreateUserInput input)
        {
            using var conn = _context.GetConnection();
            
            await conn.ExecuteAsync(
                "CALL insert_user(@Username, @Email, @PasswordHash)",
                new { input.Username, input.Email, input.PasswordHash }
            );

            // Fetch the created user
            return await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM get_user_by_email(@Email)",
                new { input.Email }
            ) ?? throw new GraphQLException("Failed to create user");
        }

        [GraphQLDescription("Update an existing user")]
        public async Task<User> UpdateUser(UpdateUserInput input)
        {
            using var conn = _context.GetConnection();
            
            await conn.ExecuteAsync(
                "CALL update_user(@UserId, @Username, @Email, @PasswordHash)",
                new { 
                    input.UserId,
                    input.Username,
                    input.Email,
                    input.PasswordHash
                }
            );

            return await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM get_user_by_id(@UserId)",
                new { input.UserId }
            ) ?? throw new GraphQLException("User not found");
        }

        [GraphQLDescription("Delete a user")]
        public async Task<bool> DeleteUser(int id)
        {
            using var conn = _context.GetConnection();
            await conn.ExecuteAsync(
                "CALL delete_user(@UserId)",
                new { UserId = id }
            );
            return true;
        }
    }
}
