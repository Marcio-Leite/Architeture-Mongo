using System;
using System.Threading.Tasks;

namespace MongoCoreDbRepository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit();
    }
}