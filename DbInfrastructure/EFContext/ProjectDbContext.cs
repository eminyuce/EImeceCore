using DbInfrastructure.Entities;
using DbInfrastructure.Repositories;
using EFGenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbInfrastructure.EFContext
{
    public class ProjectDbContext : EntitiesContext, IProjectDbContext
    {
        public DbSet<Product> Products { get; set; }
        private readonly string _connectionString;

        public ProjectDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public ProjectDbContext():base()
        {
        }


    }
}
