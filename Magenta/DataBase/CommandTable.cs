using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace Magenta.Core.Execution.DataBase;

public class CommandTable
{
    private readonly string connectionUrl = "Data Source=D:/sqllite/db/";
    private string fileName = "jarvis.db";

    public void CreateDatabase(string fileName)
    {
        var url = "Data Source=D:/sqllite/db/" + fileName;

        try
        {
            using (var conn = new SQLiteConnection(url))
            {
                conn.Open();
                Trace.WriteLine("The driver name is " + conn.ServerVersion);
                Trace.WriteLine("A new database has been created.");
                this.fileName = fileName;
            }
        }
        catch (Exception e)
        {
            Trace.WriteLine(e.Message);
        }
    }

    public void CreateTables()
    {
        using (var connection = new SQLiteConnection(connectionUrl))
        {
            connection.Open();
            Trace.WriteLine("Connected to database successfully.");

            using (var cmd = new SQLiteCommand(connection))
            {
                var sql = "CREATE TABLE IF NOT EXISTS COMMANDS " +
                          "(ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                          " NAME TEXT NOT NULL, " +
                          " COMMAND TEXT NOT NULL)";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void FillData(Command command)
    {
        using (var connection = new SQLiteConnection(connectionUrl))
        {
            connection.Open();
            Trace.WriteLine("Connected to database successfully.");

            using (var cmd = new SQLiteCommand(connection))
            {
                var sql = "INSERT INTO COMMANDS (NAME, COMMAND) VALUES (@Name, @Command)";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@Name", command.Name);
                cmd.Parameters.AddWithValue("@Command", command.CommandText);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Command> SelectAllData()
    {
        using (var connection = new SQLiteConnection(connectionUrl))
        {
            connection.Open();
            Trace.WriteLine("Connected to database successfully.");

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT * FROM COMMANDS";

                using (var reader = cmd.ExecuteReader())
                {
                    var commands = new List<Command>();

                    while (reader.Read())
                    {
                        var id = Convert.ToInt32(reader["ID"]);
                        var name = reader["NAME"].ToString();
                        var command = reader["COMMAND"].ToString();
                        commands.Add(new Command(id, name, command));
                    }

                    Trace.WriteLine(commands.ToString());
                    return commands;
                }
            }
        }
    }

    public Command SelectCommandByName(string fileName)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionUrl))
            {
                connection.Open();
                Trace.WriteLine("Connected to database successfully");

                using (var cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "SELECT * FROM COMMANDS WHERE NAME = @Name";
                    cmd.Parameters.AddWithValue("@Name", fileName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var commands = new List<Command>();

                        while (reader.Read())
                        {
                            var id = Convert.ToInt32(reader["ID"]);
                            var name = reader["NAME"].ToString();
                            var command = reader["COMMAND"].ToString();
                            commands.Add(new Command(id, name, command));
                        }

                        Trace.WriteLine(commands.ToString());

                        if (commands.Count == 0)
                            return new Command();
                        return commands[0];
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new Command();
        }
    }
}