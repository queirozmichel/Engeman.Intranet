using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engeman.Intranet.Library {
  public class DatabaseConnector : IDisposable {
    private IDbConnection _connection;
    bool disposed = false;
    SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
    string _codCfgUsr = string.Empty;
    string _sessionId = string.Empty;

    public DatabaseConnector() {
      InitializeConnection();
    }

    public IDbConnection Connection {
      get {
        return _connection;
      }
    }

    private IDbConnection InitializeConnection() {
      if (Connection == null) {

        _connection = new SqlConnection(DatabaseInfo.ConnectionString);


        try {
          _connection.Open();
        } catch (Exception exp) {
          throw exp;
        }

        //SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
        return _connection;
      } else {
        throw new Exception("Unknown connection parameters.");
      }
    }

    private void CloseConnection() {
      if (Connection != null) {
        try {
          //Base.Database.DatabaseInfo.RemoveUserSession(Connection);
          Connection.Close();
          Connection.Dispose();
        } catch (Exception exp) {
          throw exp;
        }
      }
    }

    public string GetDataBaseOwnerByFunction(string function) {
      string sql = string.Format(@"SELECT S.NAME, P.NAME FROM sys.all_objects P
      INNER JOIN sys.schemas S ON S.schema_id = P.schema_id
      WHERE ([type] = 'P' OR [type] = 'X' OR [type] = 'PC' OR [type] = 'FN') AND UPPER(P.NAME) = '{0}'", function);

      string result = string.Empty;

      using (StaticQuery sq = new StaticQuery()) {
        result = sq.GetDataToString(sql);
      }

      if (result.Trim().Length == 0) {
        result = "dbo";
      }

      return result;
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (disposed)
        return;

      if (disposing) {
        handle.Dispose();
        _connection.Close();
        _connection.Dispose();
      }

      disposed = true;
    }
  }
}
