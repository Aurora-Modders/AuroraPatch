using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Lib
{
    public class DatabaseManager
    {
        private readonly Lib Lib;
        private SQLiteConnection Connection { get; set; } = null;
        private DateTime NextUpdate { get; set; } = DateTime.MinValue;

        internal DatabaseManager(Lib lib)
        {
            Lib = lib;
        }

        public DataTable ExecuteQuery(string query)
        {
            lock (this)
            {
                if (Connection == null)
                {
                    GenerateDatabase();
                }
            }

            lock (Connection)
            {
                if (DateTime.UtcNow > NextUpdate)
                {
                    Save();
                    NextUpdate = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                }
                
                using (var connection = new SQLiteConnection(Connection.ConnectionString))
                using (var adapter = new SQLiteDataAdapter(query, connection))
                {
                    connection.Open();

                    var data = new DataSet();
                    adapter.Fill(data, "RecordSet");

                    connection.Close();

                    return data.Tables["RecordSet"];
                }
            }
        }

        private void Save()
        {
            var functions = GetSaveFunctions();
            if (functions == null)
            {
                return;
            }

            var map = Lib.TacticalMap;
            if (map == null)
            {
                return;
            }

            var game = GetGameState(map);
            if (game == null)
            {
                return;
            }

            foreach (var function in functions.Split(','))
            {
                var method = game.GetType().GetMethods(AccessTools.all).Single(m =>
                {
                    if (m.Name != function)
                    {
                        return false;
                    }

                    var parameters = m.GetParameters();

                    if (parameters.Length != 1)
                    {
                        return false;
                    }

                    if (parameters[0].ParameterType.Name != "SQLiteConnection")
                    {
                        return false;
                    }

                    return true;
                });

                var type = method.GetParameters()[0].ParameterType;
                var par = Activator.CreateInstance(type, Connection.ConnectionString);
                par.GetType().GetMethod("Open").Invoke(par, new object[0]);

                method.Invoke(game, new object[] { par });

                par.GetType().GetMethod("Close").Invoke(par, new object[0]);

                Lib.Logger.LogInfo($"Called function {function}");
            }
        }

        private void GenerateDatabase()
        {
            var commands = new List<string>();

            Lib.Logger.LogInfo("Getting sql commands");
            using (var connection = new SQLiteConnection("Data Source=AuroraDB.db;Version=3;New=False;Compress=True;"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT sql FROM sqlite_master";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entry = reader.GetValue(0);

                        if (!(entry is DBNull))
                        {
                            var sql = (string)entry;
                            if (!sql.Contains("sqlite_"))
                            {

                                commands.Add(sql);
                            }
                        }
                    }
                }

                connection.Close();
            }

            Lib.Logger.LogInfo("Applying sql commands");
            Connection = new SQLiteConnection("FullUri=file::memory:?cache=shared;");
            Connection.Open();

            Lib.Logger.LogInfo($"Db Connection: {Connection.FileName}");

            foreach (var sql in commands)
            {
                Lib.Logger.LogInfo($"executing sql: {sql}");

                var command = Connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        private object GetGameState(Form map)
        {
            switch (Lib.AuroraChecksum)
            {
                case "chm1c7": return map.GetType().GetField("a", AccessTools.all).GetValue(map);
                default: return null;
            }
        }

        private string GetSaveFunctions()
        {
            switch (Lib.AuroraChecksum)
            {
                case "chm1c7": return "g2,g3,g4,hd,he,hf,hk,hl,hm,i5,hn,ho,hp,hg,hq,hs,hr,ht,hu,h5,hw,hx,hy,hz,h0,h1,h2,h3,h4,h6,h7,hh,h8,h9,ia,ib,ic,id,ig,ih,ii,ij,ik,il,im,in,hv,io,ip,iq,ir,is,it,iy,iu,iv,iw,ix,iz,i0,i1,i2,i3,i4,i7,i8,i9,i6,hc,hi,hb,ha,g9,g8,g7,hj,ie,g5,g6";
                default: return null;
            }
        }
    }
}
