using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using Windows.Storage;

namespace AdventOfCodeUWP
{
    public static class DataAccess
    {
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("aocdatabase.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "aocdatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var tableCommand = "CREATE TABLE IF NOT EXISTS " +
                    "TestData (Primary_Key INTEGER PRIMARY KEY, " +
                    "Year INT NOT NULL, " +
                    "Day INT NOT NULL, " +
                    "Ind INT NOT NULL, " +
                    "AplicableToPart1 INT NOT NULL, " +
                    "AplicableToPart2 INT NOT NULL, " +
                    "ExpectedValue TEXT NOT NULL, " +
                    "Content TEXT NOT NULL)";

                var createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT EXISTS " +
                    "LiveData (Primary_Key INTEGER PRIMARY KEY, " +
                    "Year INT NOT NULL, " +
                    "Day INT NOT NULL, " +
                    "Content TEXT NOT NULL)";

                createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();

                tableCommand = "CREATE TABLE IF NOT EXISTS " +
                    "SessionData (Primary_Key INTEGER PRIMARY KEY, " +
                    "Content TEXT NOT NULL)";

                createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();
            }
        }

        public static IEnumerable<TestDataModel> GetTestData(int year, int day)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "aocdatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var tableCommand = "SELECT Ind, AplicableToPart1, AplicableToPart2, ExpectedValue, Content FROM TestData "
                    +
                    $"WHERE Year='{year}' AND Day='{day}'"
                    ;
                using (var cmd = new SqliteCommand(tableCommand, db))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new TestDataModel
                        {
                            Year = year,
                            Day = day,
                            Index = (int)(long)reader[0],
                            ApplicableToPart1 = (long)reader[1] > 0,
                            ApplicableToPart2 = (long)reader[2] > 0,
                            ExpectedValue = BigInteger.Parse((string)reader[3]),
                            Content = (string)reader[4]
                        };
                    }
                }
            }
        }
        
        private static string GetSessionKey()
        {
            string dbpath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "aocdatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var tableCommand = "SELECT Content FROM SessionData "
                    ;
                using (var cmd = new SqliteCommand(tableCommand, db))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return (string)reader[0];
                    }
                }
            }
            return "";
        }
        // Note: If we are to run live data, download them from AoC. 
        // Huge thanks to Nick Kusters (https://www.youtube.com/@NKCSS) for pointing out that live data should not be kept on GitHub,
        // and allowing to copy his download code.
        private static string DownloadAndUpdateLiveData(int year, int day)
        {
            string session = GetSessionKey();// File.ReadAllText($"..\\..\\..\\session.txt");
            string url = $"https://adventofcode.com/{year}/day/{day}/input";
            var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "https://github.com/Luunoronti/AdventOfCode");
            wc.Headers.Add(HttpRequestHeader.Cookie, $"{nameof(session)}={session}");
            string contents = wc.DownloadString(url);

            string dbpath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "aocdatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var tableCommand = $"INSERT INTO LiveData VALUES (1, {year}, {day}, @content)";

                using (var cmd = new SqliteCommand(tableCommand, db))
                {
                    cmd.Parameters.Add(new SqliteParameter("@content", contents));
                    cmd.ExecuteNonQuery();
                }
            }
            return contents;
        }


        public static LiveDataModel GetLiveData(int year, int day)
        {
            var liveData = "";
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "aocdatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var tableCommand = "SELECT Content FROM LiveData "
                    +
                    $"WHERE Year='{year}' AND Day='{day}'"
                    ;
                using (var cmd = new SqliteCommand(tableCommand, db))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liveData = (string)reader[0];
                    }
                }
            }

            if (string.IsNullOrEmpty(liveData))
                liveData = DownloadAndUpdateLiveData(year, day);

            return new LiveDataModel { Content = liveData, Day = day, Year = year };
        }
    }


    public class TestDataModel
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public int Index { get; set; }
        public bool ApplicableToPart1 { get; set; }
        public bool ApplicableToPart2 { get; set; }
        public BigInteger ExpectedValue { get; set; }
        public string Content { get; set; }

    }
    public class LiveDataModel
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public string Content { get; set; }
    }
}