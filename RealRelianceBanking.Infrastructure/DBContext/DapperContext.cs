using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.DBContext
{
    public class DapperContext
    {
        private readonly DapperSettings _settings;

        public DapperContext(IOptions<DapperSettings> DapperSettings)
        {
            _settings = DapperSettings.Value;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_settings.SqlServer);

    }
}
