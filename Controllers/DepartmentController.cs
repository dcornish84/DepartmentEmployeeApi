﻿using DepartmentApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, DeptName FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Department> departments = new List<Department>();

                    while (reader.Read())
                    {
                        Department department = new Department 
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DeptName = reader.GetString(reader.GetOrdinal("DeptName")),
                            
                        };

                        departments.Add(department);
                    }
                    reader.Close();

                    return Ok(departments);
                }
            }
        }

        [HttpGet("{id}", Name = "GetDepartments")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, DeptName
                        FROM Department
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Department department = null;

                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DeptName = reader.GetString(reader.GetOrdinal("Department Name")),
                        };
                    }
                    reader.Close();

                    return Ok(department);
                }
            }
        }

    }
}