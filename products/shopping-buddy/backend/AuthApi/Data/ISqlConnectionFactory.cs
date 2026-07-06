using Microsoft.Data.SqlClient;

namespace AuthApi.Data;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}
