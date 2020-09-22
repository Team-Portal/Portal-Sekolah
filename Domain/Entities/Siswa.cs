using Domain.Common;
using System;

namespace Domain.Entities
{
    public class Siswa : Entity
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public Siswa(string name, string alamat)
        {
            Id = Guid.NewGuid();
            Name = name;
            Address = alamat;
        }
    }
}
