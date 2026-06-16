using System;
using System.Collections.Generic;
using System.Text;

namespace PulseFlow.Interfaces
{
    public interface IDatabase<T>
    {
        List<T> Get();

        T? GetDetail(int? id);
        void Create(T entity);

        void Update(T entity);

        void Delete(int? id);



    }
}
