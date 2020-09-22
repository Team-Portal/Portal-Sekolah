using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SiswaController : Controller
    {
        private string _options;
        //private readonly SiswaRepository _siswa;
        public SiswaController(IOptions<ConnectionStringList> options)
        {
            _options = options.Value.Sekolah;
            //_siswa = new SiswaRepository(_options, "Siswa");
        }

        /*
        [HttpGet]
        public string Connection()
        {
            return _options;
        }
        */

        /*
        [HttpGet]
        public async Task<IEnumerable<Siswa>> GetSiswaAsync()
        {
            var siswa = new SiswaRepository(_options, "Siswa");
            var result = await siswa.GetAllAsync();
            return result;
        }
        */
        
    }
}
