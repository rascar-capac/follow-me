using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Dapper;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public class DAL : Singleton<DAL>
{
    public string DatabaseName => "FollowMe";
    public string DatabasePath => Path.Combine(Application.dataPath, "Database");
    public string FullPath => Path.Combine(DatabasePath, DatabaseName + ".sqlite");
    public string ConnectionString => $"Data Source={ FullPath };version=3";

    #region Database check and creation
    public async Task CheckDatabase()
    {
        System.Threading.Thread.Sleep(2000);
        if (!File.Exists(FullPath))
        {
            if (!Directory.Exists(DatabasePath))
                Directory.CreateDirectory(DatabasePath);
            await CreateDatabase();
            await InitialLoad();
            Debug.Log($"{FullPath} Database created.");
        }
        else
            Debug.Log($"{FullPath} Database exists.");
    }
    public async Task CreateDatabase()
    {
        using (SQLiteConnection connection = GetConnection())
        {
            connection.Open();

            //HighScores
            string sql = @"CREATE TABLE HighScores (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                playerName varchar(20),
                score int
            )";
            await CreateTable(sql, connection);

            connection.Close();
        }
    }
    public async Task InitialLoad()
    {
        using (SQLiteConnection connection = GetConnection())
        {
            connection.Open();

            //HighScores
            HighScore h = new HighScore() { playerName = "Olivier", score = 10 };
            h.id = await SaveAsync(h);

            connection.Close();
        }
    }
    public async Task CreateTable(string sql, SQLiteConnection connection)
    {
        await connection.ExecuteAsync(sql);
    }
    #endregion

    #region Connection
    public SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(ConnectionString);
    }
    #endregion

    #region Queries 
    public async Task<T> GetFromIDAsync<T>(string tableName, int id)
    {
        T o = default(T);
        using (SQLiteConnection connection = GetConnection())
        {
            o = await connection.QueryFirstOrDefaultAsync<T>($"select * from {tableName} where id = @id", new { id });
        }
        return o;
    }
    public async Task<IEnumerable<T>> GetAsync<T>(string tableName, object parameters)
    {
        IEnumerable<T> o = null;
        using (SQLiteConnection connection = GetConnection())
        {
            string sql = $"select* from { tableName } where 1=1 ";
            foreach (PropertyInfo pi in parameters.GetType().GetProperties())
            {
                sql += $" and {pi.Name}=@{pi.Name}";
            }
            o = await connection.QueryAsync<T>(sql, parameters);
        }
        return o;
    }
    public async Task<T> GetFirstAsync<T>(string tableName, object parameters)
    {
        T o = default(T);
        using (SQLiteConnection connection = GetConnection())
        {
            string sql = $"select* from { tableName } where 1=1 ";
            foreach (PropertyInfo pi in parameters.GetType().GetProperties())
            {
                sql += $" and {pi.Name}=@{pi.Name}";
            }
            o = await connection.QueryFirstAsync<T>(sql, parameters);
        }
        return o;
    }
    public async Task<int> SaveAsync(BaseModel model, SQLiteConnection connection = null)
    {
        if (model == null)
            return -1;

        bool closeConnection = connection == null;
        connection = connection ?? DAL.I.GetConnection();
        if (closeConnection)
            connection.Open();

        string sql = "";
        int result = model.id;

        if (model.id <= 0)
        {
            string columns = "";
            string parameters = "";
            foreach (PropertyInfo pi in model.GetType().GetProperties())
            {
                if (pi.Name == "id")
                    continue;

                if (columns.Length > 0)
                    columns += ",";
                columns += $"{ pi.Name}";
                if (parameters.Length > 0)
                    parameters += ",";

                parameters += $"@{ pi.Name }";
            }
            sql = $@"INSERT INTO {model.TableName} ({columns}) values ({parameters});";
            sql += "select last_insert_rowid()";
            result = await connection.QueryFirstOrDefaultAsync<int>(sql, model);
            model.id = result;
        }
        else
        {
            string columns = "";
            foreach (PropertyInfo pi in model.GetType().GetProperties())
            {
                if (pi.Name == "id")
                    continue;

                if (columns.Length > 0)
                    columns += ",";
                columns += $"{ pi.Name } = @{ pi.Name }";
            }
            sql = $@"UPDATE {model.TableName} set {columns} where id=@id";
            await connection.ExecuteAsync(sql, model);
        }
        if (closeConnection)
        {
            connection.Close();
            connection.Dispose();
        }
        return result;
    }
    public async Task SaveMultipleAsync(List<BaseModel> models)
    {
        if (models == null || models.Count <= 0)
            return;

        using (SQLiteConnection connection = GetConnection())
        {
            foreach (BaseModel model in models)
            {
                await SaveAsync(model, connection);
            }
        }
    }
    public T GetFromID<T>(string tableName, int id)
    {
        return GetFromIDAsync<T>(tableName, id).Result;
    }
    public IEnumerable<T> Get<T>(string tableName, object parameters)
    {
        return GetAsync<T>(tableName, parameters).Result ;
    }
    public T GetFirst<T>(string tableName, object parameters)
    {
        return GetFirstAsync<T>(tableName, parameters).Result;
    }
    public int Save(BaseModel model, SQLiteConnection connection = null)
    {
        return SaveAsync(model, connection).Result;
    }
    public void SaveMultiple(List<BaseModel> models)
    {
        _ = SaveMultipleAsync(models);
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _ = CheckDatabase();
        Debug.Log("awake");
    }

}
