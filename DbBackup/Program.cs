using System;
using System.Collections.Generic;
using System.Linq;

namespace DbBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbBackups = new List<DbBackup>()
            {
                new DbBackup {Name="Games", Size = 10, Value = 7},
                new DbBackup {Name="Accounts", Size = 7, Value = 3},
                new DbBackup {Name="Messages", Size = 2, Value = 5},
                new DbBackup {Name="Profiles", Size = 10, Value = 4},
                new DbBackup {Name="Manuals", Size = 3, Value = 3}
            };

            var resultDbs = DbBackupSolverManager.Compose(10, dbBackups);

            Console.WriteLine(string.Format("{0,-10} {1,-5} {2,2}", "Name", "Size", "Value"));

            foreach (var i in resultDbs)
            {
                Console.WriteLine(string.Format("{0,-10} {1,-4} {2,2}", i.Name, i.Size, i.Value));
            }

            Console.WriteLine($"\nTotal value: {resultDbs.TotalValue()}");

        }
    }

    public class DbBackup
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Value { get; set; }
    }

    public static class DbBackupSolverManager
    {
        public static List<DbBackup> Compose(int avaliableBackupSpace, List<DbBackup> dbBackups)
        {
            var dbBackupCount = dbBackups.Count;

            List<DbBackup>[,] dbMatrix = new List<DbBackup>[dbBackupCount + 1, avaliableBackupSpace + 1];

            for (int dbBackupIndex = 0; dbBackupIndex <= dbBackupCount; dbBackupIndex++)
            {
                for (int sizeIndex = 0; sizeIndex <= avaliableBackupSpace; sizeIndex++)
                {
                    dbMatrix[dbBackupIndex, sizeIndex] = new List<DbBackup>();

                    if (dbBackupIndex == 0 || sizeIndex == 0)
                        continue;

                    var currentDbBackupIndex = dbBackupIndex - 1;
                    var currentDbBackup = dbBackups[currentDbBackupIndex];

                    if (currentDbBackup.Size <= sizeIndex)
                    {
                        if (currentDbBackup.Value + dbMatrix[dbBackupIndex - 1, sizeIndex - currentDbBackup.Size].TotalValue() > dbMatrix[dbBackupIndex - 1, sizeIndex].TotalValue())
                        {
                            dbMatrix[dbBackupIndex, sizeIndex].Add(currentDbBackup);
                            dbMatrix[dbBackupIndex, sizeIndex].AddRange(dbMatrix[dbBackupIndex - 1, sizeIndex - currentDbBackup.Size]);
                        }

                        if (currentDbBackup.Value + dbMatrix[dbBackupIndex - 1, sizeIndex - currentDbBackup.Size].TotalValue() <= dbMatrix[dbBackupIndex - 1, sizeIndex].TotalValue())
                        {
                            dbMatrix[dbBackupIndex, sizeIndex].AddRange(dbMatrix[dbBackupIndex - 1, sizeIndex]);
                        }
                    }
                    else
                    {
                        dbMatrix[dbBackupIndex, sizeIndex].AddRange(dbMatrix[dbBackupIndex - 1, sizeIndex]);
                    }
                }
            }

            return dbMatrix[dbBackupCount, avaliableBackupSpace];
        }

        public static int TotalValue(this List<DbBackup> dbBackups)
        {
            return dbBackups.Select(x => x.Value).Sum();
        }
    }
}
