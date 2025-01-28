using Microsoft.AspNetCore.Mvc;
using Npgsql;
using UserApi.Data;
using UserApi.Models;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        // Endpoint to insert a new user
        [HttpPost]
        public IActionResult InsertUser([FromBody] User user)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL insert_user(@username, @email, @password_hash)", conn))
                {
                    cmd.Parameters.AddWithValue("username", user.Username);
                    cmd.Parameters.AddWithValue("email", user.Email);
                    cmd.Parameters.AddWithValue("password_hash", user.PasswordHash);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        // Endpoint to get a user by ID
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM get_user_by_id(@user_id)", conn))
                {
                    cmd.Parameters.AddWithValue("user_id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3),
                                UpdatedAt = reader.GetDateTime(4),
                                Status = reader.GetString(5)
                            };
                            return Ok(user);
                        }
                    }
                }
            }
            return NotFound();
        }

        // Endpoint to get all users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = new List<User>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM get_all_users()", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3),
                                UpdatedAt = reader.GetDateTime(4),
                                Status = reader.GetString(5)
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return Ok(users);
        }

        // Endpoint to update a user
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL update_user(@user_id, @username, @email, @password_hash, @status)", conn))
                {
                    cmd.Parameters.AddWithValue("user_id", id);
                    cmd.Parameters.AddWithValue("username", user.Username);
                    cmd.Parameters.AddWithValue("email", user.Email);
                    cmd.Parameters.AddWithValue("password_hash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("status", user.Status);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        // Endpoint to delete a user
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL delete_user(@user_id)", conn))
                {
                    cmd.Parameters.AddWithValue("user_id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        // Endpoint for bulk blocking users
        [HttpPut("bulk-block")]
        public IActionResult BulkBlockUsers([FromBody] int[] userIds)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL bulk_block_users(@user_ids)", conn))
                {
                    cmd.Parameters.AddWithValue("user_ids", userIds);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        // Endpoint for bulk activating users
        [HttpPut("bulk-activate")]
        public IActionResult BulkActivateUsers([FromBody] int[] userIds)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL bulk_activate_users(@user_ids)", conn))
                {
                    cmd.Parameters.AddWithValue("user_ids", userIds);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok();
        }
    }
}
