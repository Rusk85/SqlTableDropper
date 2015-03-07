using SmartFormat;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableDropper
{
    public class TableDropper
    {

        private string _connectionString;

        private TargetDb _database;

        private string _selectFkQuery = @"SELECT fks.name AS KeyName, tabs.name AS 'TableName' FROM sys.foreign_keys fks
                                        JOIN sys.tables tabs
                                        ON fks.parent_object_id = tabs.object_id";

        private string _selectPkQuery = @"SELECT keys.name as KeyName, tabs.name as 'TableName' FROM sys.key_constraints keys
                                        JOIN sys.tables as tabs
                                        ON keys.parent_object_id = tabs.object_id";

        private string _selectTablesQuery = @"SELECT tabs.name AS Name FROM sys.tables tabs";

        private string _dropFkQuery = @"ALTER TABLE {table} DROP {foreign_key}";

        private string _dropPkQuery = @"ALTER TABLE {table} DROP CONSTRAINT {primary_key}";

        private string _dropTableQuery = @"DROP TABLE {table}";


        public TableDropper(string connectionString)
        {
            _connectionString = connectionString;
            _database = buildDbObjectHierarchy();
        }

        public TableDropper()
            : this(@"Data Source=192.168.1.55;Initial Catalog=DbMigration;
                Integrated Security=False;User ID=DbMigration;Password=l1ICT6ViGChSlsNYMMA7;")
        {
        }

        private DbContext context()
        {
            return new DbContext(_connectionString);
        }

        private TargetDb buildDbObjectHierarchy()
        {
            var tabs = discoverTables();
            var pks = discoverPrimaryKeys();
            var fks = discoverForeignKeys();

            var pkTabs = tabs.Join(pks, t => t.Name, pk => pk.TableName,
                (t, pk) => new
                {
                    TableName = t.Name,
                    KeyName =
                        new PrimaryKey { KeyName = pk.KeyName, TableName = pk.TableName }
                });

            var grpPkTabs = pkTabs.GroupBy(t => t.TableName)
                .Select(t =>
                new Table
                {
                    Name = t.Key,
                    PrimaryKeys = t.Select(pk => pk.KeyName).ToList()
                }).ToList();

            var fkTabs = tabs.Join(fks, t => t.Name, fk => fk.TableName,
                (t, fk) => new
                {
                    TableName = t.Name,
                    ForeignKey =
                        new ForeignKey { KeyName = fk.KeyName, TableName = t.Name }
                });

            var grpFkTabs = fkTabs.GroupBy(t => t.TableName)
                .Select(t =>
                new Table
                {
                    Name = t.Key,
                    ForeignKeys = t.Select(fk => fk.ForeignKey).ToList()
                }).ToList();

            using (var ctx = context())
            {
                return new TargetDb
                {
                    Tables = grpPkTabs.Concat(grpFkTabs).GroupBy(t => t.Name)
                        .Select(t => new Table
                        {
                            Name = t.Key,
                            PrimaryKeys = t.Select(pk => pk.PrimaryKeys).SelectMany(_ => _).ToList(),
                            ForeignKeys = t.Select(fk => fk.ForeignKeys).SelectMany(_ => _).ToList()
                        }).ToList(),
                    DbName = ctx.Database.Connection.Database
                };
            }
        }


        private List<Table> discoverTables()
        {
            return sqlQuery<Table>(_selectTablesQuery).ToList();
        }

        private List<PrimaryKey> discoverPrimaryKeys()
        {
            return sqlQuery<PrimaryKey>(_selectPkQuery).ToList();
        }

        private List<ForeignKey> discoverForeignKeys()
        {
            return sqlQuery<ForeignKey>(_selectFkQuery).ToList();
        }


        private List<T> sqlQuery<T>(string query)
        {
            using (var db = context())
            {
                return db.Database.SqlQuery<T>(query).ToList();
            }
        }


        /// <summary>
        /// Drops specified tables along with foreign keys, primary keys and unique constraints.
        /// Foreign keys that reference any of the specified tables not part of the operation are
        /// not dropped and will therefore result in an error.
        /// </summary>
        /// <param name="tableNames">Names are case-insensitive.</param>
        public void DropTables(params string[] tableNames)
        {
            var tgtTables = _database.Tables.Where(t =>
                tableNames.Any(tn => tn.ToLower() == t.Name.ToLower()));

            foreach (var t in tgtTables)
            {
                dropForeignKey(t);
            }

            foreach (var t in tgtTables)
            {
                dropPrimaryKeys(t);
            }
            dropTables(tgtTables.ToList());
        }



        private void dropPrimaryKeys(Table table)
        {
            foreach (var pk in table.PrimaryKeys)
            {
                dropPrimaryKey(table.Name, pk.KeyName);
            }
        }


        private void dropPrimaryKey(string tableName, string primaryKeyName)
        {
            executeSqlCommand(Smart.Format(_dropPkQuery,
                new { table = tableName, primary_key = primaryKeyName }));
        }


        private void dropForeignKey(Table table)
        {
            foreach (var fk in table.ForeignKeys)
            {
                dropForeignKey(table.Name, fk.KeyName);
            }
        }


        private void dropForeignKey(string tableName, string foreignKeyName)
        {
            executeSqlCommand(Smart.Format(_dropFkQuery,
                new { table = tableName, foreign_key = foreignKeyName }));
        }

        private void dropTables(List<Table> tables)
        {
            foreach (var t in tables)
            {
                dropTable(t);
            }
        }

        private void dropTable(Table table)
        {
            dropTable(table.Name);
        }

        private void dropTable(string tableName)
        {
            executeSqlCommand(Smart.Format(_dropTableQuery,
                new { table = tableName }));
        }

        private void executeSqlCommand(string sql)
        {
            using (var db = context())
            {
                db.Database.ExecuteSqlCommand(sql);
            }
        }




    }


    public class TargetDb
    {
        public string DbName { get; set; }
        public List<Table> Tables { get; set; }
    }

    public class Table
    {
        public string Name { get; set; }
        public List<PrimaryKey> PrimaryKeys { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }

        public Table()
        {
            PrimaryKeys = new List<PrimaryKey>();
            ForeignKeys = new List<ForeignKey>();
        }
    }

    public class PrimaryKey
    {
        public string KeyName { get; set; }
        public string TableName { get; set; }
    }

    public class ForeignKey
    {
        public string KeyName { get; set; }
        public string TableName { get; set; }
    }
}
