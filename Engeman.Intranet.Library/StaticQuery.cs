using Microsoft.AspNetCore.Http;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text;

namespace Engeman.Intranet.Library {
  public class StaticQuery : IDisposable {

    private DatabaseConnector _databaseConnector;
    bool disposed = false;
    SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

    public StaticQuery() {
      _databaseConnector = new DatabaseConnector();
    }

    public StaticQuery(DatabaseConnector databaseConnector) {
      _databaseConnector = databaseConnector;
    }

    public IDbTransaction Transaction {
      get;
      set;
    }

    //public void PutDbRegIni(string tipo, string topico, string variavel, object conteudo, string usuario = "*", byte[] binaryContent = null) {

    //  string sql = string.Format("DELETE FROM REGINI WHERE TOPICO='{0}' AND VARIAVEL='{1}' AND NOME='{2}'", topico, variavel, usuario);
    //  ExecuteCommand(sql);

    //  sql = string.Empty;
    //  bool blob = false;
    //  if (tipo.Equals("M")) {
    //    blob = true;
    //  }
    //  if (!blob) {
    //    //Não é blob
    //    if (tipo.Equals("T") || tipo.Equals("S")) {
    //      if (tipo.Equals("T"))
    //        sql = string.Format("INSERT INTO REGINI(TOPICO, VARIAVEL, NOME, CONTEUDO_TXT, TIPO) VALUES('{0}','{1}','{2}','{3}','{4}')", topico, variavel, usuario, conteudo.ToString(), tipo);
    //      else
    //        sql = string.Format("INSERT INTO REGINI(TOPICO, VARIAVEL, NOME, CONTEUDO_STR, TIPO) VALUES('{0}','{1}','{2}','{3}','{4}')", topico, variavel, usuario, conteudo.ToString(), tipo);
    //    } else if (tipo.Equals("I")) {
    //      sql = string.Format("INSERT INTO REGINI(TOPICO, VARIAVEL, NOME, CONTEUDO_FLT, TIPO) VALUES('{0}','{1}','{2}',{3},'{4}')", topico, variavel, usuario, decimal.Parse(conteudo.ToString()), tipo);
    //    } else if (tipo.Equals("D")) {

    //      conteudo = string.Format("CONVERT(DATETIME,'{0}',103)", conteudo.ToString());

    //      sql = string.Format("INSERT INTO REGINI(TOPICO, VARIAVEL, NOME, CONTEUDO_DTE, TIPO) VALUES('{0}','{1}','{2}',{3},'{4}')", topico, variavel, usuario, conteudo, tipo);

    //    } else if (tipo.Equals("B")) {
    //      sql = "INSERT INTO REGINI(TOPICO, VARIAVEL, NOME, CONTEUDO_BLN, TIPO) VALUES('{0}','{1}','{2}','{3}','{4}')";
    //      if ((bool)conteudo) {
    //        sql = string.Format(sql, topico, variavel, usuario, "T", tipo);
    //      } else {
    //        sql = string.Format(sql, topico, variavel, usuario, "F", tipo);
    //      }

    //    } else {
    //      throw new Exception(string.Format("O tipo '{0}' não existe;", tipo));
    //    }

    //    try {
    //      ExecuteCommand(sql);
    //    } catch (Exception ex) {

    //      throw new Exception(ex.Message);
    //    }

    //  } else {
    //    using (StaticQuery dacc = new StaticQuery()) {

    //      if (binaryContent == null) {
    //        throw new Exception("O parâmetro binaryContent é obrigatório para inserção de campo binário.");
    //      }
    //      sql = "INSERT INTO REGINIBLB(CONTEUDO_BLB, NOME, TIPO, TOPICO, VARIAVEL ) VALUES(#VALOR,#NOME,#TIPO,#TOPICO,#VARIAVEL)";

    //      sql = sql.Replace("#", "@");
    //      try {
    //        SqlConnection conn_sqlserver = new SqlConnection(DatabaseInfo.ConnectionString);
    //        using (SqlCommand cmd = new SqlCommand(sql, conn_sqlserver)) {

    //          cmd.Parameters.Add("@VALOR", SqlDbType.VarBinary, binaryContent.Length).Value = binaryContent;
    //          cmd.Parameters.Add("@NOME", SqlDbType.VarChar).Value = usuario;
    //          cmd.Parameters.Add("@TOPICO", SqlDbType.VarChar).Value = topico;
    //          cmd.Parameters.Add("@VARIAVEL", SqlDbType.VarChar).Value = variavel;
    //          cmd.Parameters.Add("@TIPO", SqlDbType.VarChar).Value = tipo;
    //          cmd.ExecuteNonQuery();
    //        }
    //      } catch (Exception ex) {
    //        throw new Exception(ex.Message);
    //      }
    //    }
    //  }
    //}

    public DataSet GetDataSet(string query) {
      DataSet dsResult = new DataSet();

      try {
        IDbDataAdapter dataAdapter;


        dataAdapter = new SqlDataAdapter();


        dataAdapter.SelectCommand = _databaseConnector.Connection.CreateCommand();
        dataAdapter.SelectCommand.CommandText = query;

        if (Transaction != null)
          dataAdapter.SelectCommand.Transaction = Transaction;

        dataAdapter.Fill(dsResult);
        dataAdapter = null;
        return dsResult;
      } catch (Exception exp) {
        throw exp;
      } finally {
        if (dsResult != null)
          dsResult.Dispose();
      }
    }

    public DataSet GetDataSet(string query, string[] parametersNames, object[] parametersValues) {
      DataSet dsResult = new DataSet();

      try {
        IDbDataAdapter dataAdapter;

        dataAdapter = new SqlDataAdapter();

        dataAdapter.SelectCommand = _databaseConnector.Connection.CreateCommand();
        dataAdapter.SelectCommand.CommandText = query;

        if (Transaction != null)
          dataAdapter.SelectCommand.Transaction = Transaction;

        for (int i = 0; i < parametersNames.Length; i++) {
          var parameter = dataAdapter.SelectCommand.CreateParameter();
          parameter.ParameterName = parametersNames[i];
          parameter.Value = parametersValues[i];
          dataAdapter.SelectCommand.Parameters.Add(parameter);
        }

        dataAdapter.Fill(dsResult);
        dataAdapter = null;
        return dsResult;
      } catch (Exception exp) {
        throw exp;
      } finally {
        if (dsResult != null)
          dsResult.Dispose();
      }
    }

    public object GetDataToObject(string query) {
      object result = null;

      try {
        var command = _databaseConnector.Connection.CreateCommand();
        command.CommandText = query;

        if (Transaction != null)
          command.Transaction = Transaction;

        var resultObject = command.ExecuteScalar();

        if (resultObject != null)
          result = resultObject;

        if (command != null)
          command.Dispose();
        command = null;

        return result;
      } finally {

      }
    }

    public object GetDataToObject(string query, string[] parametersNames, object[] parametersValues) {
      object result = null;

      try {
        var command = _databaseConnector.Connection.CreateCommand();
        command.CommandText = query;

        if (Transaction != null)
          command.Transaction = Transaction;

        for (int i = 0; i < parametersNames.Length; i++) {
          var parameter = command.CreateParameter();
          parameter.ParameterName = parametersNames[i];
          parameter.Value = parametersValues[i];
          command.Parameters.Add(parameter);
        }

        var resultObject = command.ExecuteScalar();

        if (resultObject != null)
          result = resultObject;

        if (command != null)
          command.Dispose();
        command = null;

        return result;
      } finally {

      }
    }

    public string GetDataToString(string query) {
      try {
        var command = _databaseConnector.Connection.CreateCommand();
        command.CommandText = query;

        if (Transaction != null)
          command.Transaction = Transaction;

        var resultObject = command.ExecuteScalar();

        if (resultObject != null) {
          if (command != null)
            command.Dispose();
          command = null;

          if (resultObject.GetType() == typeof(byte[])) {
            return Encoding.UTF8.GetString(resultObject as byte[], 0, (resultObject as byte[]).Length);
          } else {
            return resultObject.ToString();
          }
        }

        return string.Empty;
      } catch (Exception exp) {
        throw exp;
      } finally {
        // Do Nothing...
      }
    }

    public string GetDataToString(string query, string[] parametersNames, object[] parametersValues) {
      try {
        var command = _databaseConnector.Connection.CreateCommand();
        command.CommandText = query;

        if (Transaction != null)
          command.Transaction = Transaction;

        for (int i = 0; i < parametersNames.Length; i++) {
          var parameter = command.CreateParameter();
          parameter.ParameterName = parametersNames[i];
          parameter.Value = parametersValues[i];
          command.Parameters.Add(parameter);
        }

        var resultObject = command.ExecuteScalar();

        if (resultObject != null) {
          if (command != null)
            command.Dispose();
          command = null;

          if (resultObject.GetType() == typeof(byte[])) {
            return Encoding.UTF8.GetString(resultObject as byte[], 0, (resultObject as byte[]).Length);
          } else {
            return resultObject.ToString();
          }
        }

        return string.Empty;
      } catch (Exception exp) {
        throw exp;
      } finally {
        // Do Nothing...
      }
    }


    public byte[] GetByteArray(string query, string[] parametersNames, object[] parametersValues) {
      var command = _databaseConnector.Connection.CreateCommand();
      command.CommandText = query;

      if (Transaction != null)
        command.Transaction = Transaction;

      for (int i = 0; i < parametersNames.Length; i++) {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parametersNames[i];
        parameter.Value = parametersValues[i];
        command.Parameters.Add(parameter);
      }

      try {
        return command.ExecuteScalar() as byte[];
      } catch (Exception exp) {
        return null;
      } finally {
        command.Dispose();
        command = null;
      }
    }

    public double GetDataToDouble(string query) {
      string resultStr = GetDataToString(query);
      if (string.IsNullOrEmpty(resultStr)) {
        return 0;
      } else {
        return double.Parse(resultStr);
      }
    }

    public int GetDataToInt(string query) {
      string resultStr = GetDataToString(query);
      if (string.IsNullOrEmpty(resultStr)) {
        return 0;
      } else {
        return int.Parse(resultStr);
      }
    }

    public int GetDataToInt(string query, string[] parametersNames, object[] parametersValues) {
      string resultStr = GetDataToString(query, parametersNames, parametersValues);

      if (string.IsNullOrEmpty(resultStr)) {
        return 0;
      } else {
        return int.Parse(resultStr);
      }
    }

    public double GetDataToDouble(string query, string[] parametersNames, object[] parametersValues) {
      string resultStr = GetDataToString(query, parametersNames, parametersValues);

      if (string.IsNullOrEmpty(resultStr)) {
        return 0;
      } else {
        return double.Parse(resultStr);
      }
    }

    public int ExecuteCommand(string query) {
      var command = _databaseConnector.Connection.CreateCommand();
      try {
        command.CommandText = query;

        if (Transaction != null)
          command.Transaction = Transaction;

        return command.ExecuteNonQuery();
      } catch (Exception exp) {
        throw exp;
      } finally {
        if (command != null)
          command.Dispose();
      }
    }

    public int ExecuteCommand(string query, string[] parametersNames, object[] parametersValues) {
      var command = _databaseConnector.Connection.CreateCommand();
      try {
        command.CommandText = query;

        for (int i = 0; i < parametersNames.Length; i++) {
          string typeDb = string.Empty;
          string paramName = string.Empty;

          if (parametersNames[i].IndexOf(";") > -1) {
            typeDb = parametersNames[i].Substring(parametersNames[i].IndexOf(";") + 1);
            paramName = parametersNames[i].Substring(0, parametersNames[i].IndexOf(";"));
          } else {
            paramName = parametersNames[i];
          }

          var parameter = command.CreateParameter();

          if (typeDb.Trim().Length > 0) {
            if (typeDb.ToLower() == "byte") {
              parameter.DbType = DbType.Binary;
            }
          }

          parameter.ParameterName = paramName.Trim();
          parameter.Value = parametersValues[i];

          command.Parameters.Add(parameter);
        }

        if (Transaction != null)
          command.Transaction = Transaction;

        return command.ExecuteNonQuery();
      } catch (Exception exp) {
        throw exp;
      } finally {
        if (command != null)
          command.Dispose();
      }
    }

    public void SaveUserLog(int userId, string userName, string description) {
      HttpContextAccessor _httpContextAccessor = new HttpContextAccessor();


      ExecuteCommand("INSERT INTO USERLOG (CODCFGUSR, NOME, DESCRICAO, DATA, COMPUTER, IP) VALUES (" + userId.ToString() + ", '" +
        userName + "', @DESCRICAO, " + SqlDateToString(DateTime.Now, true) + ", '" + Environment.MachineName + "', '" + _httpContextAccessor.HttpContext.Connection.RemoteIpAddress + "')", new string[] { "@DESCRICAO" }, new string[] { description });
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
        _databaseConnector.Dispose();
      }

      disposed = true;
    }

    public IDataReader GetDataReader(string query) {
      var command = _databaseConnector.Connection.CreateCommand();
      try {
        command.CommandText = query;

        if (Transaction != null)
          command.Transaction = Transaction;

        return command.ExecuteReader();
      } catch (Exception exp) {
        throw exp;
      } finally {
        if (command != null)
          command.Dispose();
      }
    }

    public DataTable GetDataTableFromDataReader(IDataReader reader) {
      DataTable tbSchema = reader.GetSchemaTable();
      DataTable tbReturn = new DataTable();

      foreach (DataRow r in tbSchema.Rows) {
        if (!tbReturn.Columns.Contains(r["ColumnName"].ToString())) {
          DataColumn col = new DataColumn() {
            ColumnName = r["ColumnName"].ToString(),
            Unique = Convert.ToBoolean(r["IsUnique"]),
            AllowDBNull = Convert.ToBoolean(r["AllowDBNull"]),
            ReadOnly = Convert.ToBoolean(r["IsReadOnly"])
          };
          tbReturn.Columns.Add(col);
        }
      }

      while (reader.Read()) {
        DataRow novaLinha = tbReturn.NewRow();
        for (int i = 0; i < tbReturn.Columns.Count; i++) {
          novaLinha[i] = reader.GetValue(i);
        }
        tbReturn.Rows.Add(novaLinha);
      }

      return tbReturn;
    }

    public string GetDataBaseOwner() {
      string sql = "SELECT COUNT(*) FROM (SELECT DISTINCT SCHEMAS.NAME AS SCHEMAS " +
                   "FROM SYS.TABLES AS TABLES_ " +
                   "INNER JOIN SYS.SCHEMAS AS SCHEMAS ON TABLES_.[SCHEMA_ID] = SCHEMAS.[SCHEMA_ID] " +
                   "WHERE LOWER(TABLES_.NAME) <> 'sysdiagrams') T";
      string result = GetDataToString(sql);

      if (int.Parse(result) == 0) {
        result = "dbo";
      } else if (int.Parse(result) == 1) {
        sql = "SELECT DISTINCT SCHEMAS.NAME AS SCHEMAS " +
             "FROM SYS.TABLES AS TABLES_ " +
             "INNER JOIN SYS.SCHEMAS AS SCHEMAS ON TABLES_.[SCHEMA_ID] = SCHEMAS.[SCHEMA_ID] " +
             "WHERE LOWER(TABLES_.NAME) <> 'sysdiagrams'";

        result = GetDataToString(sql);
      } else if (int.Parse(result) > 1) {
        sql = "SELECT DISTINCT SCHEMAS.NAME AS SCHEMAS " +
             "FROM SYS.TABLES AS TABLES_ " +
             "INNER JOIN SYS.SCHEMAS AS SCHEMAS ON TABLES_.[SCHEMA_ID] = SCHEMAS.[SCHEMA_ID] " +
             "WHERE LOWER(TABLES_.NAME) <> 'sysdiagrams' AND SCHEMAS.NAME = '" + DatabaseInfo.DatabaseLogin + "'";

        result = GetDataToString(sql);
      }

      if (string.IsNullOrEmpty(result)) {
        result = "dbo";
      }

      return result;
    }

    public static string GetFuntionConvertDate(string campo, bool includeHour) {
      if (includeHour) {
        return $"CONVERT(VARCHAR,{campo},121)";
      } else {
        return $"CONVERT(VARCHAR,{campo},103)";
      }

    }

    public static string SqlDateToString(DateTime date, bool includeHour) {
      //Remove Milisegundos da data
      date = new DateTime(date.Ticks - (date.Ticks % TimeSpan.TicksPerSecond), date.Kind);
      string result = "";

      if (includeHour == true) {
        result = "CONVERT(DATETIME, '" +
                  date.Year.ToString() + "/" + date.Month.ToString().PadLeft(2, '0') + "/" + date.Day.ToString().PadLeft(2, '0') + " " +
                  date.Hour.ToString().PadLeft(2, '0') + ":" + date.Minute.ToString().PadLeft(2, '0') + ":" +
                  date.Second.ToString().PadLeft(2, '0') + ":" + date.Millisecond.ToString().PadLeft(3, '0') + "', 121)";

      } else {

        result = "CONVERT(DATETIME, '" +
                  date.Day.ToString().PadLeft(2, '0') + "/" + date.Month.ToString().PadLeft(2, '0') + "/" +
                  date.Year.ToString() + "', 103)";

      }
      return result;
    }

    public string MountDateToCompare(string fieldName, bool useTime) {
      if (useTime) {
        return string.Format("CONVERT(DATETIME, CAST(DATEPART(YEAR,{0})AS VARCHAR)+'-' + CAST(DATEPART(MONTH,{0})AS VARCHAR)+'-' + CAST(DATEPART(DAY,{0})AS VARCHAR)+' ' + RIGHT(REPLICATE('0', 2) + CAST(DATEPART(HOUR,{0})AS VARCHAR),2)+':' + RIGHT(REPLICATE('0', 2) + CAST(DATEPART(MINUTE,{0})AS VARCHAR),2)+':' + RIGHT(REPLICATE('0', 2) + CAST(DATEPART(SECOND,{0})AS VARCHAR),2),121)", fieldName);
      } else {
        return string.Format("CONVERT(DATETIME, CAST(DATEPART(DAY,{0})AS VARCHAR)+'/'+ CAST(DATEPART(MONTH,{0})AS VARCHAR)+'/'+CAST(DATEPART(YEAR,{0})AS VARCHAR),103)", fieldName);
      }
    }

    //public object GetDbRegIni(string tipo, string topico, string variavel, object defaultResult, string usuario = "*") {
    //  string fieldResult;
    //  //Verifica o tipo para definir qual campo buscar no banco
    //  if (tipo.Equals("T")) {
    //    fieldResult = "CONTEUDO_TXT";
    //  } else if (tipo.Equals("S")) {
    //    fieldResult = "CONTEUDO_STR";
    //  } else if (tipo.Equals("D")) {
    //    fieldResult = "CONTEUDO_DTE";
    //  } else if (tipo.Equals("B")) {
    //    fieldResult = "CONTEUDO_BLN";
    //  } else {
    //    fieldResult = "CONTEUDO_FLT";
    //  }
    //  //Cria e aplica a consulta no banco de dados
    //  string sql = string.Format("SELECT {0} FROM REGINI WHERE TIPO='{1}' AND TOPICO='{2}' AND VARIAVEL='{3}' AND NOME='{4}'", fieldResult, tipo, topico, variavel, usuario);
    //  string sqlResult = GetDataToString(sql);

    //  //Determinando retorno
    //  object result = null;
    //  //Se não obteve retorno do banco, retorna ao usuário o defaultResult informando na função
    //  if (string.IsNullOrEmpty(sqlResult)) {
    //    result = defaultResult;
    //  } else {
    //    //Verifica o tipo e retorna o valor no seu tipo correto para poder converter implicitamente na chamada da função
    //    if ((tipo.Equals("T")) || (tipo.Equals("S"))) {
    //      result = sqlResult;
    //    } else if (tipo.Equals("D")) {
    //      result = DateTime.Parse(sqlResult);
    //    } else if (tipo.Equals("B")) {
    //      if (sqlResult.Equals("T")) {
    //        result = true;
    //      } else {
    //        result = false;
    //      }
    //    } else {
    //      result = double.Parse(sqlResult);
    //    }
    //  }
    //  return result;
    //}

    public double HtoN(string hora) {
      double result = 0;
      string auxhora, xhora = "", xmin = "", xseg = "";
      auxhora = hora.Trim();
      if (string.IsNullOrEmpty(auxhora))
        return 0;

      while (auxhora.IndexOf(':') > -1) {
        if (string.IsNullOrEmpty(xhora)) {
          xhora = auxhora.Substring(0, auxhora.IndexOf(':'));
          auxhora = auxhora.Substring(auxhora.IndexOf(':') + 1);
        } else if (string.IsNullOrEmpty(xmin)) {
          xmin = auxhora.Substring(0, auxhora.IndexOf(':'));
          auxhora = auxhora.Substring(auxhora.IndexOf(':') + 1);
        } else if (string.IsNullOrEmpty(xseg)) {
          xseg = auxhora.Substring(0, auxhora.IndexOf(':'));
          auxhora = auxhora.Substring(auxhora.IndexOf(':') + 1);

        } else
          auxhora = auxhora.Substring(auxhora.IndexOf(':') + 1);

      }

      if (!string.IsNullOrEmpty(auxhora) && string.IsNullOrEmpty(xhora))
        xhora = auxhora;
      else if (!string.IsNullOrEmpty(auxhora) && string.IsNullOrEmpty(xmin))
        xmin = auxhora;

      if (string.IsNullOrEmpty(xhora))
        xhora = "0";

      if (string.IsNullOrEmpty(xmin))
        xmin = "0";

      if (string.IsNullOrEmpty(xseg))
        xseg = "0";


      result = int.Parse(xhora) + int.Parse(xmin) / (double)(60 + int.Parse(xseg) / 3600);
      return result;
    }

    public string Ntoh(double number) {
      string sql = string.Empty;
      sql = string.Format("SELECT [{0}].FNTOH({1}) FROM CFGGERAL", GetDataBaseOwnerByFunction("ntoh"), number.ToString().Replace(",", "."));

      string result = GetDataToString(sql);
      if (string.IsNullOrEmpty(result)) {
        result = "00:00:00";
      }
      return result;
    }

    public string GetDataBaseOwnerByFunction(string function) {
      string sql = string.Format(@"SELECT S.NAME, P.NAME FROM sys.all_objects P
                                  INNER JOIN sys.schemas S ON S.schema_id = P.schema_id
                                  WHERE ([type] IN ('P','X','PC','FN')) AND UPPER(P.NAME) = '{0}'", function);

      string result = GetDataToString(sql);

      return result;
    }

    public string PrepareCommandAndReplaceGetOwnerByFunction(string command) {
      try {
        var query = command;

        var arr = query.Split("[");
        var dictionaryFunctions = new Dictionary<string, string>();

        for (int i = 0; i < arr.Length; i++) {
          if (arr[i].ToLower().IndexOf("getowner") > -1) {
            var temp = arr[i].Split("]")[0];
            var func = temp.Split("(")[1].Split(")")[0];

            if (!dictionaryFunctions.ContainsKey(temp)) {
              dictionaryFunctions.Add(temp, GetDataBaseOwnerByFunction(func));
            }
          }
        }

        foreach (var item in dictionaryFunctions) {
          query = query.Replace(item.Key, item.Value);
        }

        return query;
      } catch {
        return command;
      }
    }

    public bool TableExists(string table) {
      string sql = "SELECT 1 AS FIELD FROM " + table + " WHERE 1=0";

      try {
        GetDataToString(sql);
        return true;
      } catch {
        return false;
      }
    }

    public static ArrayList ParseDataTableToJson(DataSet ds) {
      ArrayList lista = new ArrayList();
      foreach (DataRow dtrow in ds.Tables[0].Rows) {
        lista.Add(dtrow.Table);
      }
      return lista;
    }

    public static ArrayList ParseDictionaryToJson(string count, string pkField,
                                string tableSize, string lastUpdate,
                                string lastId, string dependencies) {
      Dictionary<string, string> dic = new Dictionary<string, string>();
      dic.Add("count", count.ToString());
      dic.Add("primary_key_name", pkField.ToString());
      dic.Add("table_size", tableSize.Trim().Length > 0 ? tableSize.ToString() : "0");
      dic.Add("last_update", lastUpdate.ToString());
      dic.Add("last_id", lastId.ToString());
      dic.Add("relationships", dependencies.Trim().Length > 0 ? dependencies.ToString() : "0");

      ArrayList list = new ArrayList();
      list.Add(dic);

      return list;
    }
  }
}
