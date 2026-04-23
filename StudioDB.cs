using System;
using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=CreatorStudio.sqlite");
connection.Open();
SqliteCommand command = new SqliteCommand();
command.Connection = connection;
command.CommandText = """
                      CREATE TABLE IF NOT EXISTS table_performers(id INTEGER PRIMARY KEY AUTOINCREMENT,
                      alias TEXT NOT NULL,
                      registration_date TEXT NOT NULL)
                      """;
command.ExecuteNonQuery();

command.CommandText = """
                      CREATE TABLE IF NOT EXISTS table_personal_files(id INTEGER PRIMARY KEY AUTOINCREMENT, 
                      performer_id INTEGER NOT NULL,
                      pass_series INTEGER NOT NULL, 
                      pass_number INTEGER NOT NULL,
                      date_of_issue TEXT NOT NULL,
                      last_name TEXT NOT NULL,
                      first_name TEXT NOT NULL,
                      patronymic TEXT NULL,
                      address TEXT NOT NULL,
                      FOREIGN KEY (performer_id) REFERENCES table_performers(id))
                      """;
command.ExecuteNonQuery();
 
command.CommandText =
    """
    CREATE TABLE IF NOT EXISTS table_projects(id INTEGER PRIMARY KEY AUTOINCREMENT, 
    name TEXT NOT NULL, 
    deadline TEXT NOT NULL)
    """;
command.ExecuteNonQuery();

command.CommandText = """
                      CREATE TABLE IF NOT EXISTS table_assignments(performer_id INTEGER NOT NULL,
                      project_id INTEGER NOT NULL,
                      role TEXT NOT NULL,
                      PRIMARY KEY (performer_id, project_id),
                      FOREIGN KEY (performer_id) REFERENCES table_performers(id),
                      FOREIGN KEY (project_id) REFERENCES table_projects(id))
                      """;
command.ExecuteNonQuery();


command.CommandText = """
                      SELECT table_projects.name, 
                      table_projects.deadline, 
                      table_performers.alias, 
                      table_assignments.role,
                      table_personal_files.last_name,
                      table_personal_files.first_name,
                      table_personal_files.patronymic,
                      table_personal_files.pass_series,
                      table_personal_files.pass_number
                      FROM table_projects  
                      JOIN table_assignments  ON table_projects.id = table_assignments.project_id
                      JOIN table_performers  ON table_assignments.performer_id = table_performers.id
                      JOIN table_personal_files  ON table_performers.id = table_personal_files.performer_id
                      """;


using (SqliteDataReader reader = command.ExecuteReader())
{
 if (reader.HasRows)
 {
  while (reader.Read())
  {
   string projectName = reader.GetString(0);
   string deadline = reader.GetString(1);
   string alias = reader.GetString(2);
   string role = reader.GetString(3);
   string lastName = reader.GetString(4);
   string firstName = reader.GetString(5);
   string patronymic = reader.GetString(6);
   int passSeries = reader.GetInt32(7);
   int passNumber = reader.GetInt32(8);
   
   Console.WriteLine($"Project: {projectName} " +
                     $"Deadline: {deadline} " +
                     $"Performer {alias}, {role}, full name: {lastName}, {firstName}, {patronymic}, " +
                     $"Passport : {passSeries}, {passNumber}");
  }
 }
}
