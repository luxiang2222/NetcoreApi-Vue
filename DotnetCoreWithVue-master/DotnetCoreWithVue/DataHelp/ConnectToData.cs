using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DotnetCoreWithVue.DataHelp
{
  //获取数据
  //批量更新数据
  //实体转换
  //不同的返回类型
  public class ConnectToData
  {
    public static ConnectToData InstConnectToData
    {
      get;set;
    }

    static ConnectToData()
    {
      if(InstConnectToData==null)
      {
        InstConnectToData = new ConnectToData();
      }
    }

    public SqlConnection ConnectToMMSQL()
    {
      SqlConnection connect=new SqlConnection();
      try
      {
        SqlConnection Connect = new SqlConnection("Data source=.;user id=sa;password=sa;initial catalog=SOAP_LOG;");
        if(Connect.State == ConnectionState.Closed)
        {
          Connect.Open();
        }
        if (Connect.State == ConnectionState.Open)
        {
          connect = Connect;
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return connect;
    }

    public DataSet GetDataSet(string SqlStr, string Table = "Tmp")
    {
      DataSet Set = new DataSet();
      var Connect = ConnectToMMSQL();
      if (Connect == null)
      {
        throw new Exception("数据库连接出错,检查数据库服务是否正常开启/账户密码最近是否有改动。");
      }
      try
      {
        SqlCommand Command = new SqlCommand(SqlStr, Connect);
        Command.CommandTimeout = 150;
        SqlDataAdapter adap = new SqlDataAdapter(Command);
        adap.Fill(Set, Table);
      }
      catch(Exception ex)
      {
        throw new Exception($"【{SqlStr}】获取数据出错"+ex.Message);
      }
      finally
      {
        Connect.Close();
      }
      //Command.ExecuteScalar();//return 第一行第一列
      //Command.ExecuteNonQuery();//return 受影响行数
      return Set;
    }

    /// <summary>
    /// 返回受影响行数（事务的方式提交,支持失败回滚）
    /// </summary>
    /// <param name="sql_str"></param>
    /// <returns></returns>
    public int SQL_ExecuteNonQuery(string sql_str,string SQL_Connection= "data source=.;user id=sa;password=sa;initial catalog=SOAP_LOG")
    {
      SqlConnection Connect = new SqlConnection(SQL_Connection);
      //开启一个事务
      if(Connect.State==ConnectionState.Closed)
      {
        Connect.Open();
      }
      SqlTransaction transaction = Connect.BeginTransaction();//事务开启
      SqlCommand Command = new SqlCommand();
      Command.Connection = Connect;//连接字符串
      Command.Transaction = transaction;//Command事务开启
      Command.CommandTimeout = 150;//设置超时
      try
      {
        //返回受影响行数
        Command.CommandText = sql_str;
        var count= Command.ExecuteNonQuery();
        transaction.Commit();
        return count;
      }
      catch(Exception ex)
      {
        transaction.Rollback();
        throw new Exception($"执行{sql_str}出错"+ex.Message);
      }
      finally
      {
        Connect.Close();
      }
    }
    /// <summary>
    /// 返回第一行第一列
    /// </summary>
    /// <param name="sql_str"></param>
    /// <param name="SQL_Connection"></param>
    /// <returns></returns>
    public object SQL_ExecuteScalar(string sql_str, string SQL_Connection = "data source=.;user id=sa;password=sa;initial catalog=SOAP_LOG")
    {
      SqlConnection Connect = new SqlConnection(SQL_Connection);
      //开启一个事务
      if (Connect.State == ConnectionState.Closed)
      {
        Connect.Open();
      }
      SqlTransaction transaction = Connect.BeginTransaction();//事务开启
      SqlCommand Command = new SqlCommand();
      Command.Connection = Connect;//连接字符串
      Command.Transaction = transaction;//Command事务开启
      Command.CommandTimeout = 150;//设置超时
      try
      {
        //返回受影响行数
        Command.CommandText = sql_str;
        return Command.ExecuteScalar();
      }
      catch (Exception ex)
      {
        transaction.Rollback();
        throw new Exception($"执行{sql_str}出错" + ex.Message);
      }
      finally
      {
        Connect.Close();
      }
    }

    public DataTable GetDatatable(string SqlStr,string Table="Tmp")
    {
      return GetDataSet(SqlStr,Table).Tables[Table];
    }

    public List<T> SQL_GetListData<T>(string SqlStr) where T:new()
    {
      var data = GetDataSet(SqlStr, "TMP").Tables["TMP"];
      List<string> colum =new List<string>();
      List<T> result = new List<T>();
      foreach(DataColumn item in data.Columns)
      {
        colum.Add(item.ColumnName);
      }
      foreach(DataRow row in data.Rows)
      {
        result.Add(DatatableConverToModel<T>(row, colum));
      }
      return result;
    }

    public T DatatableConverToModel<T>(DataRow data, List<string> Columns) where T:new()
    {
      //table.Columns
      //Get the T perpoer
      T Mode = new T();
      Type type = typeof(T);
      //Get PropertyInfo array
      var properties=type.GetProperties();
      //Find the Model Map to Datarow
      foreach(PropertyInfo property in properties)
      {
        var Mode_ColumnName = Columns.Where(s => s.ToLower() == property.Name.ToString().ToLower()).FirstOrDefault();
        //property.Name   Get one Name from Model T
        //property.PropertyType Get Type from Model T
        //The Special deal with T  proprer
        if(!string.IsNullOrEmpty(Mode_ColumnName.ToString()))
        {
          if (property.PropertyType == typeof(Guid))
          {
            var value = Guid.Parse(data[Mode_ColumnName].ToString());
            property.SetValue(Mode, value,null);
          }
          else
          {
            var value = Convert.ChangeType(data[Mode_ColumnName], property.PropertyType);
            property.SetValue(Mode, value,null);
          }
        }
        //对值类型（例如：int）初始值为空的处理
      }
      return Mode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="TableName"></param>
    /// <param name="MapSelect">SQLSearcTypeEnum 类型 dic<字段名，value></param>
    /// <returns></returns>
    public string SQL_Select_Str(string TableName, Dictionary<SQLSearcTypeEnum, Dictionary<string, string>> MapSelect)
    {
      //获取表字段信息
      //获取表结构
      DataTable data = this.GetDatatable($"Select*From {TableName} where 1=2");
      List<string> Column = new List<string>();
      //读取字段名及值
      foreach (DataColumn row in data.Columns)
        Column.Add(row.ColumnName);
      if (Column.Count == 0)
      {
        throw new Exception($"表名{TableName}不存在任何字段，请检查表名{TableName}是否正确？");
      }

      StringBuilder SqlStrBuild = new StringBuilder();
      SqlStrBuild.Append($"Select*From {TableName} Where ");
      var Map = MapSelect.ToList();
      int i = 0;
      //拼接查询语句
      foreach (var item in MapSelect)
      {
        if(i!=0)
        {
          SqlStrBuild.Append("And");
        }
        i+=1;
        switch (item.Key)
        {
          case SQLSearcTypeEnum.Like:
            SqlStrBuild.Append($" {item.Value.Keys} Like '{item.Value.Values}%'");
            continue;
          case SQLSearcTypeEnum.Equal:
            SqlStrBuild.Append($" {item.Value.Keys}='{item.Value.Values}'");
            continue;
          case SQLSearcTypeEnum.Contain:
            SqlStrBuild.Append($" {item.Value.Keys} Like '%{item.Value.Values}%'");
            continue;
          case SQLSearcTypeEnum.GreaterThan:
            SqlStrBuild.Append($" {item.Value.Keys} > '{item.Value.Values}'");
            continue;
          case SQLSearcTypeEnum.LessThen:
            SqlStrBuild.Append($" {item.Value.Keys} < '{item.Value.Values}'");
            continue;
          case SQLSearcTypeEnum.BetWeen:
            SqlStrBuild.Append($" {item.Value.Keys} Between '{item.Value.Values.ToString().Split('|')[0]}' And '{item.Value.Values.ToString().Split('|')[1]}'");
            continue;
          default:
            continue;
        }
      }
      return SqlStrBuild.ToString();
    }

    /// <summary>
    /// 参数类型查询 返回List类型
    /// </summary>
    /// <param name="TableName"></param>
    /// <param name="MapSelect">字段名，查询的值（只支持 where MapSelect.key='MapSelect.value'）</param>
    /// <returns></returns>
    public DataTable SQL_Select_DataTable(string TableName, Dictionary<SQLSearcTypeEnum, Dictionary<string, string>> MapSelect)
    {
      var str = SQL_Select_Str(TableName, MapSelect);
      var ResultDatat = GetDatatable(str, TableName);
      return ResultDatat;
    }
    /// <summary>
    /// 参数类型查询 返回List类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="TableName"></param>
    /// <param name="MapSelect"></param>
    /// <returns></returns>
    public List<T> SQL_Select_List<T>(string TableName, Dictionary<SQLSearcTypeEnum, Dictionary<string, string>> MapSelect) where T:new()
    {
      var str= SQL_Select_Str(TableName, MapSelect);
      return SQL_GetListData<T>(str);
    }
  }

  public enum SQLSearcTypeEnum
  {
    Equal=0,//等于a='b'
    Like=1,//like 相似
    Contain=2,//包含
    BetWeen=3,//之间用|分割前后两个值
    GreaterThan =4,//大于
    LessThen=5//小于
    //OrderBy=4//
  }
}
