using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engeman.Intranet.Library {
  public static class DatabaseInfo {
    private static string _connectionString = string.Empty;
    private static bool _alwaysConnected = false;
    private static bool _useSequence = true;
    public static string DatabaseOwner { get; set; }
    private static IHttpContextAccessor _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor) {
      _httpContextAccessor = httpContextAccessor;
    }


    public static string DatabaseLogin {
      get {
        System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(_connectionString);

        if (!string.IsNullOrEmpty(builder.UserID))
          return builder.UserID;
        else
          return string.Empty;
      }
    }

    public static string DatabaseName {
      get {
        System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(_connectionString);

        if (!string.IsNullOrEmpty(builder.InitialCatalog))
          return builder.InitialCatalog;
        else
          return string.Empty;
      }
    }

    public static string ConnectionString {
      get {
        return _connectionString;
      }

      set {
        _connectionString = value;
      }
    }

    public static bool AlwaysConnected {
      get {
        return _alwaysConnected;
      }

      set {
        _alwaysConnected = value;
      }
    }

    public static bool UseSequence {
      get {
        return _useSequence;
      }

      set {
        _useSequence = value;
      }
    }

    public static string Concat() 
    {
      string concat = "+";
      return concat;
    }

    //public static void RegisterUserSession(IDbConnection conn) 
    //{
    //  if ((_httpContextAccessor != null) && (_httpContextAccessor.HttpContext != null) && (_httpContextAccessor.HttpContext.Session.GetString("USUARIO_ID") != null)) {
    //    InternalRegisterUserSession(conn, _httpContextAccessor.HttpContext.Session.GetString("USUARIO_ID").ToString());
    //  } else if ((_httpContextAccessor != null) && (_httpContextAccessor.HttpContext != null) && (_httpContextAccessor.HttpContext.Request != null) &&
    //      (_httpContextAccessor.HttpContext.Request.Headers != null) && (_httpContextAccessor.HttpContext.Request.Headers.Keys != null) &&
    //      (_httpContextAccessor.HttpContext.Request.Headers.Keys.Contains("USUARIO_ID"))) {
    //    InternalRegisterUserSession(conn, _httpContextAccessor.HttpContext.Request.Headers["USUARIO_ID"].ToString());
    //  }
    //}

    //private static void InternalRegisterUserSession(IDbConnection conn, string codCfgUsr) 
    //{
    //  try {
    //    var command = conn.CreateCommand();
    //    try {
    //      command.CommandText = "OPEN SYMMETRIC KEY KEYCRYPTPASS DECRYPTION BY PASSWORD = 'xdD6FLpk5APD396?$NL%'";
    //      command.ExecuteNonQuery();

    //      if (codCfgUsr.IndexOf(",") > -1)
    //        codCfgUsr = codCfgUsr.Substring(0, codCfgUsr.IndexOf(",") - 1);

    //      string qry = "DECLARE\r\n" +
    //        "@BinVar varbinary(128);\r\n" +
    //        "SET @BinVar = CAST(" + codCfgUsr + " AS varbinary(128));\r\n" +
    //        "SET CONTEXT_INFO @BinVar;";

    //      command.CommandText = qry;
    //      command.ExecuteNonQuery();
    //    } catch (Exception exp) {
    //      //throw exp;
    //    } finally {
    //      if (command != null)
    //        command.Dispose();
    //      command = null;
    //    }
    //  } catch {
    //    //Nothing.
    //  }
    //}
  }
}