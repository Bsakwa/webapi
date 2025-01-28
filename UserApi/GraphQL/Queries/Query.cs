// GraphQL/Queries/Query.cs
using Dapper;
using HotChocolate;
using UserApi.Data;
using UserApi.Models;

namespace UserApi.GraphQL.Queries
{
    public class Query
    {
        private readonly DatabaseContext _context;

        public Query(DatabaseContext context)
        {
            _context = context;
        }

        [GraphQLDescription("Get all users")]
        public async Task<IEnumerable<User>> GetUsers()
        {
            using var conn = _context.GetConnection();
            return await conn.QueryAsync<User>(@"
                SELECT 
                    user_id AS UserId, 
                    username AS Username, 
                    email AS Email, 
                    created_at AS CreatedAt, 
                    updated_at AS UpdatedAt,
                    status AS Status
                FROM get_all_users()");
        }

        [GraphQLDescription("Get user by ID")]
        public async Task<User?> GetUserById(int id)
        {
            using var conn = _context.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(@"
                SELECT 
                    user_id AS UserId, 
                    username AS Username, 
                    email AS Email, 
                    created_at AS CreatedAt, 
                    updated_at AS UpdatedAt,
                    status AS Status
                FROM get_user_by_id(@UserId)",
                new { UserId = id });
        }
    }
}
