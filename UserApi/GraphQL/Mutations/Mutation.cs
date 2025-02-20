using Dapper;
using HotChocolate;
using UserApi.Data;
using UserApi.Models;
using UserApi.GraphQL.Types.Inputs;
using System;
using System.Threading.Tasks;
using Npgsql; // Use Npgsql for PostgreSQL

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
            try
            {
                using var conn = _context.GetConnection();
                
                await conn.ExecuteAsync(
                    "CALL insert_user(@Username, @Email, @PasswordHash)",
                    new { input.Username, input.Email, input.PasswordHash }
                );
                
                var user = await conn.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM get_user_by_email(@Email)",
                    new { input.Email }
                );
                
                if (user == null)
                {
                    throw new GraphQLException("Failed to create user");
                }
                    
                return user;
            }
            catch (PostgresException pgEx)
            {
                // Handle specific PostgreSQL exceptions
                if (pgEx.SqlState == "23505") // Unique violation
                {
                    throw new GraphQLException("A user with this email or username already exists");
                }
                
                throw new GraphQLException($"Database error during user creation: {pgEx.Message}");
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"User creation failed: {ex.Message}");
            }
        }

        [GraphQLDescription("Update an existing user")]
        public async Task<User> UpdateUser(UpdateUserInput input)
        {
            try
            {
                using var conn = _context.GetConnection();
                
                var result = await conn.ExecuteAsync(
                    "CALL update_user(@UserId, @Username, @Email, @PasswordHash, @Status)",
                    new { 
                        input.UserId,
                        input.Username,
                        input.Email,
                        input.PasswordHash,
                        input.Status
                    }
                );
                
                if (result == 0)
                {
                    throw new GraphQLException($"No user found with ID {input.UserId}");
                }
                
                var user = await conn.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM get_user_by_id(@UserId)",
                    new { input.UserId }
                );
                
                if (user == null)
                {
                    throw new GraphQLException($"User with ID {input.UserId} not found after update");
                }
                    
                return user;
            }
            catch (PostgresException pgEx)
            {
                // Handle specific PostgreSQL exceptions
                if (pgEx.SqlState == "23505") // Unique violation
                {
                    throw new GraphQLException("A user with this email or username already exists");
                }
                
                throw new GraphQLException($"Database error during user update: {pgEx.Message}");
            }
            catch (GraphQLException)
            {
                // Re-throw GraphQL exceptions as they're already properly formatted
                throw;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Failed to update user: {ex.Message}");
            }
        }

        [GraphQLDescription("Delete a user")]
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                using var conn = _context.GetConnection();
                var result = await conn.ExecuteAsync(
                    "CALL delete_user(@UserId)",
                    new { UserId = id }
                );
                
                if (result == 0)
                {
                    throw new GraphQLException($"No user found with ID {id}");
                }
                
                return true;
            }
            catch (PostgresException pgEx)
            {
                throw new GraphQLException($"Database error during user deletion: {pgEx.Message}");
            }
            catch (GraphQLException)
            {
                // Re-throw GraphQL exceptions
                throw;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Failed to delete user: {ex.Message}");
            }
        }

        [GraphQLDescription("Bulk block users")]
        public async Task<bool> BulkBlockUsers(int[] userIds)
        {
            try
            {
                if (userIds == null || userIds.Length == 0)
                {
                    throw new GraphQLException("No user IDs provided for bulk block operation");
                }
                
                using var conn = _context.GetConnection();
                await conn.ExecuteAsync(
                    "CALL bulk_block_users(@UserIds)",
                    new { UserIds = userIds }
                );
                
                return true;
            }
            catch (PostgresException pgEx)
            {
                throw new GraphQLException($"Database error during bulk block operation: {pgEx.Message}");
            }
            catch (GraphQLException)
            {
                // Re-throw GraphQL exceptions
                throw;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Failed to block users: {ex.Message}");
            }
        }

        [GraphQLDescription("Bulk activate users")]
        public async Task<bool> BulkActivateUsers(int[] userIds)
        {
            try
            {
                if (userIds == null || userIds.Length == 0)
                {
                    throw new GraphQLException("No user IDs provided for bulk activation operation");
                }
                
                using var conn = _context.GetConnection();
                await conn.ExecuteAsync(
                    "CALL bulk_activate_users(@UserIds)",
                    new { UserIds = userIds }
                );
                
                return true;
            }
            catch (PostgresException pgEx)
            {
                throw new GraphQLException($"Database error during bulk activation operation: {pgEx.Message}");
            }
            catch (GraphQLException)
            {
                // Re-throw GraphQL exceptions
                throw;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Failed to activate users: {ex.Message}");
            }
        }
    }
}
