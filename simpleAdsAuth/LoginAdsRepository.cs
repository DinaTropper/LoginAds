using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace simpleAdsAuth
{
    public class LoginAdsRepository
    {
        private readonly string _connectionString;

        public LoginAdsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User u)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(u.PasswordHash);
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Email, Password) " +
                "VALUES (@email, @hash) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@email", u.Email);
            cmd.Parameters.AddWithValue("@hash", hash);
            connection.Open();
            u.Id = (int)(decimal)cmd.ExecuteScalar();
        }
        public void AddAd(Ad a)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads (Name, Date, PhoneNumber, Details, UserId) " +
                "VALUES (@name, @date, @phoneNumber, @details, @userId) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", a.Name);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@phoneNumber", a.PhoneNumber);
            cmd.Parameters.AddWithValue("@details", a.Details);
            cmd.Parameters.AddWithValue("@userId", a.UserId);
            connection.Open();
            a.Id = (int)(decimal)cmd.ExecuteScalar();

        }

        public List<Ad> GetAdsForId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads WHERE UserId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            List<Ad> ads = new();
            var reader = cmd.ExecuteReader();
          
            while (reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Details = (string)reader["Details"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }
        //public List<User> GetUsers()
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    using var cmd = connection.CreateCommand();
        //    cmd.CommandText = "SELECT * FROM Users";
        //    connection.Open();
        //    var reader = cmd.ExecuteReader();
        //    List<User> users = new();
        //    if (!reader.Read())
        //    {
        //        return null;
        //    }
        //    while (reader.Read())
        //    {
        //        users.Add(new()
        //        {
        //            Id = (int)reader["Id"],
        //            Email = (string)reader["Email"],
        //            PasswordHash = (string)reader["Password"],
        //        });
        //    }
        //    return users;
        //}
        public List<Ad> GetAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads ORDER BY Date DESC";
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Ad> ads = new();
            if (!reader.Read())
            {
                return null;
            }
            while (reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Details = (string)reader["Details"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }
        public User GetByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            if(email == null)
            {
                return null;
            }
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["Password"]
            };
        }

        public User Login(string email, string password)
        {
            User user = GetByEmail(email);

            if (user == null)
            {
                return null;
            }
            bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isMatch)
            {
                return null;
            }
            return user;
        }
        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
       
    }
}
