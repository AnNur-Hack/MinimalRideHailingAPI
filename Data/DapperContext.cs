using System.Data;
using Microsoft.Data.SqlClient;

namespace MinimalRideHailingAPI.Data;

public class DapperContext(IConfigurationRoot configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));
    }
}