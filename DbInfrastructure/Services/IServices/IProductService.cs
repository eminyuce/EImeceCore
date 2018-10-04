using DbInfrastructure.Repositories.IRepositories;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Text;
using DbInfrastructure.Entities;
using System.Threading.Tasks;

namespace DbInfrastructure.Services.IServices
{
    public interface IProductService : IBaseService<Product>, IDisposable
    {
        Task<List<Product>> GetAllFromCache();
        void RemoveCache();
    }

}
