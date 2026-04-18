using System.Data;

namespace ERPDesktop.Application.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection CreateOpenConnection();
}
