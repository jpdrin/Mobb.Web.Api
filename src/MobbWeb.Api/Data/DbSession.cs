using System.Data;
using System.Data.SqlClient;

namespace MobbWeb.Api.Data
{
  public class DbSession : IDisposable
  {
    public IDbConnection Connection { get; }

    public DbSession(IConfiguration configuration)
    {
      Connection = new SqlConnection(configuration.GetConnectionString("ConnectionString"));

      Connection.Open();
    }

    public void Dispose()
    {
      Connection.Close();
      Connection?.Dispose();
    }
  }
}