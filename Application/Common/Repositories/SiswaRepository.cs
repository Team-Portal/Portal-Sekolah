using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public class SiswaRepository : GenericRepository<Siswa>
    {
        public SiswaRepository(string connectionString, string tableName) : base(connectionString, tableName) { }
        
    }
}
