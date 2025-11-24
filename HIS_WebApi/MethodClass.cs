using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLUI;
using HIS_DB_Lib;
using MySql.Data;
using Basic;
using System.ComponentModel;
using System.Reflection;
namespace HIS_WebApi
{
    public class MethodClass
    {
        static public Table CheckCreatTable(sys_serverSettingClass serverSettingClass, Enum Enum)
        {
            Table table = new Table(Enum);

            string Server = serverSettingClass.Server;
            string DB = serverSettingClass.DBName;
            string UserName = serverSettingClass.User;
            string Password = serverSettingClass.Password;
            uint Port = (uint)serverSettingClass.Port.StringToInt32();
            table.Server = Server;
            table.DBName = DB;
            table.Username = UserName;
            table.Password = Password;
            table.Port = Port.ToString();

            SQLControl sQLControl = new SQLControl(Server, DB, table.TableName, UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);

            if (!sQLControl.IsTableCreat()) sQLControl.CreatTable(table);
            else sQLControl.CheckAllColumnName(table, true);
            return table;
        }
        static public Table CheckCreatTable(sys_serverSettingClass serverSettingClass, Enum Enum, string tableName)
        {
            Table table = new Table(Enum);

            string Server = serverSettingClass.Server;
            string DB = serverSettingClass.DBName;
            string UserName = serverSettingClass.User;
            string Password = serverSettingClass.Password;
            uint Port = (uint)serverSettingClass.Port.StringToInt32();
            table.Server = Server;
            table.DBName = DB;
            table.Username = UserName;
            table.Password = Password;
            table.Port = Port.ToString();
            table.TableName = tableName;

            SQLControl sQLControl = new SQLControl(Server, DB, tableName, UserName, Password, Port, MySql.Data.MySqlClient.MySqlSslMode.None);

            if (!sQLControl.IsTableCreat()) sQLControl.CreatTable(table);
            else sQLControl.CheckAllColumnName(table, true);
            return table;
        }

        static public Table CheckCreatTable<T>(sys_serverSettingClass serverSettingClass)
        {
            Type typeFromHandle = typeof(T);
            string text = typeFromHandle.GetCustomAttribute<DescriptionAttribute>()?.Description ?? typeFromHandle.Name;
            Table table = new Table(typeFromHandle);
            string Server = serverSettingClass.Server;
            string DB = serverSettingClass.DBName;
            string UserName = serverSettingClass.User;
            string Password = serverSettingClass.Password;
            uint Port = (uint)serverSettingClass.Port.StringToInt32();
            table.Server = Server;
            table.DBName = DB;
            table.Username = UserName;
            table.Password = Password;
            table.Port = Port.ToString();
            SQLControl sQLControl = new SQLControl(table);
            if (!sQLControl.IsTableCreat())
            {
               sQLControl.CreatTable(table);
            }
            else
            {
               sQLControl.CheckAllColumnName(table, autoAdd: true);
            }

            return table;
        }


    }
}
