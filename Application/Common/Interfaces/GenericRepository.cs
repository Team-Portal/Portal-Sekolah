using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        
        protected GenericRepository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        /// <summary>
        /// Generate new connection based on connection string.
        /// </summary>
        /// <returns>SqlConnection.</returns>
        private SqlConnection SqlConnection()
        {
            //return new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Open new connection and return it for use.
        /// </summary>
        /// <returns>DbConnection.</returns>
        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Get all data from table database. 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<T>($"SELECT * FROM {_tableName}");
                return result;
            }
        }
                
        /// <summary>
        /// Get data by id from table database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
               var result = await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id});
               if (result == null)
                    throw new KeyNotFoundException($"{_tableName} with id [{id}] could not be found");
                return result;
            }
        }

        public async Task InsertAsync(T t)
        {
            var insertQuery = GenerateInsertQuery();
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(insertQuery, t);
            }
        }

        public async Task<int> SaveRangeAsync(IEnumerable<T> list)
        {
            var inserted = 0;
            var query = GenerateInsertQuery();

            using (var connection = CreateConnection())
            {
                inserted += await connection.ExecuteAsync(query, list);
            }
            return inserted;
        }

        public async Task DeleteRowAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id=@id", new { Id = id});
            }
        }
        
        public async Task UpdateAsync(T t)
        {
            var updateQuery = GenerateUpdateQuery();

            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(updateQuery, t);
            }
        }      

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }

        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();
        
        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName}");
            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(prop =>
            {
                if (!prop.Equals("Id"))
                {
                    updateQuery.Append($"{prop}=@{prop},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(" WHERE Id=@id");

            return updateQuery.ToString();
        }
    }
}

