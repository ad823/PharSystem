using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;


namespace Oracle_Lib
{
    public class ORCControl
    {
        public string Server { get; }
        public string TableName { get; }
        public string UserName { get; }
        public string Password { get; }
        public string ServiceName { get; }
        public string Owner { get; }
        public int Port { get; }

        public ORCControl(string server,string serviceName, string owner, string tableName, string userName, string password, int port)
        {
            Server = server;
            ServiceName = serviceName;
            Owner = owner;
            TableName = tableName;
            UserName = userName;
            Password = password;
            Port = port;
        }
        public ORCControl(string server, string serviceName, string owner, string userName, string password, int port)
        {
            Server = server;
            ServiceName = serviceName;
            Owner = owner;
            UserName = userName;
            Password = password;
            Port = port;
        }

        // 按你指定的格式組出連線字串
        public string conn_str =>
             $"Data Source={Server}:{Port}/{ServiceName};User ID={UserName};Password={Password};";

        public string[] GetAllColumn_Name()
        {
            string tableName = this.TableName;
            return GetAllColumn_Name(tableName);
        }
        public string[] GetAllColumn_Name(string tableName)
        {
            List<string> columnNames = new List<string>();

            string sql = @"
                SELECT COLUMN_NAME
                FROM ALL_TAB_COLUMNS
                WHERE OWNER = :OWNER
                  AND TABLE_NAME = :TABLE_NAME
                ORDER BY COLUMN_ID";

            using (var conn = new OracleConnection(conn_str))
            {
                conn.Open();

                using (var cmd = new OracleCommand(sql, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(new OracleParameter("OWNER", this.Owner.ToUpper()));
                    cmd.Parameters.Add(new OracleParameter("TABLE_NAME", tableName.ToUpper()));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columnNames.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return columnNames.ToArray();
        }
        public async Task<int> AddRowsAsync(List<object[]> values)
        {
            string table = this.TableName;
            return await AddRowsAsync(table, values);
        }
        public async Task<int> AddRowsAsync(string tableName, List<object[]> values)
        {
            if (values == null || values.Count == 0) return 0;

            // 取得欄位名稱（例如 PURRESD 有 20 欄）
            string[] allColumnNames = GetAllColumn_Name(tableName);
            if (values[0].Length == 0 || values[0].Length > allColumnNames.Length) return 0;

            int affected = 0;

            // 完整表名 = SCHEMA.TABLE
            string fullTableName = $"{this.Owner}.{tableName}";

            using (var conn = new OracleConnection(conn_str))
            {
                await conn.OpenAsync();

                using (var tx = conn.BeginTransaction())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.BindByName = true;

                    // --- 組 INSERT SQL ---
                    var sbCols = new StringBuilder();
                    var sbVals = new StringBuilder();

                    for (int k = 0; k < values[0].Length; k++)
                    {
                        string colName = allColumnNames[k];

                        if (k > 0)
                        {
                            sbCols.Append(",");
                            sbVals.Append(",");
                        }

                        sbCols.Append(colName);
                        sbVals.Append($":{colName}");

                        // 建立參數骨架
                        var p = cmd.CreateParameter();
                        p.ParameterName = colName;    // 無冒號
                        p.Value = DBNull.Value;
                        cmd.Parameters.Add(p);
                    }

                    // 不要加分號
                    cmd.CommandText = $"INSERT INTO {fullTableName} ({sbCols}) VALUES ({sbVals})";

                    try
                    {
                        // --- 實際塞資料 ---
                        foreach (var row in values)
                        {
                            for (int k = 0; k < row.Length; k++)
                            {
                                string colName = allColumnNames[k];
                                cmd.Parameters[colName].Value = row[k] ?? DBNull.Value;
                            }

                            affected += await cmd.ExecuteNonQueryAsync();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return affected;
        }
    }
}
