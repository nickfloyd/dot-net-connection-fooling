using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using arch_1_connection_pooling.CustomDataHijinx;

namespace arch_1_connection_pooling {
    class Program {

        const string CONN = "server=localhost;" +
                                    "uid=test;" +
                                    "pwd=Pa$$w0rd;" +
                                    "database=AdventureWorks;";
        static int numTries = 0;
        const int MAX_TRIES = 3;


        static void Main(string[] args) {
            while (true) {
                try {
                    TransactionUtil.Enter();
                    for (int i = 0; i < 10000; i++) {
                        WriteDate();
                    }
                    TransactionUtil.SetComplete();
                    Console.WriteLine("Mash...\n");
                    Console.ReadLine();
                }
                catch (SqlException se) {
                    if (numTries < MAX_TRIES) {
                        Console.WriteLine("Trying again...\n");
                        numTries += 1;
                    }
                    else {
                        Console.WriteLine("Error: " + se.Message);
                        Console.ReadLine();
                    }
                }
            }


        }

        protected static void WriteDate() {

            SqlDataReader dr = null;


            string cmdText = "SELECT date=getdate()";
            SqlConnection cn = new SqlConnection(CONN);
            SqlCommand cmd = new SqlCommand(cmdText, cn);

            try {
                cn.Open();
                dr = cmd.ExecuteReader();
                if (dr != null) {
                    while (dr.Read()) {
                        Console.WriteLine(dr["date"].ToString());

                    }
                }
            }
            finally {
                if (dr != null) dr.Close();
                //cn.Close();
            }


        }

        protected static void WriteDateUsing() {

            try {
                using (SqlConnection connection = new SqlConnection(CONN)) {

                    using (SqlCommand cmd = new SqlCommand("SELECT date=getdate()", connection)) {
                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr != null) {
                            while (dr.Read()) {
                                Console.WriteLine(dr["date"].ToString());
                            }
                        }
                    }
                }
            }
            finally {
                //nothing;
            }



        }

        protected static void WriteDateUsingExecuteScalar() {
            //TransactionUtil.Enter();
            Console.WriteLine(ExecuteScalar().ToString());
            //TransactionUtil.SetComplete();
        }

        protected static string ExecuteScalar() {
            using (Connection connection = Scope.GetConnection(CONN)) {
                return SqlHelper.ExecuteScalar(connection.CurrentConnection, CommandType.Text, "SELECT date=getdate()").ToString();
            }

        }

        protected static void WriteDateUsingExecuteReader() {
           // TransactionUtil.Enter();

            using (IDataReader dr = ExecuteReader()) {
                if (dr.Read()) {
                    Console.WriteLine(dr["date"].ToString());
                }
            }
            
            //TransactionUtil.SetComplete();
        }

        protected static IDataReader ExecuteReader() {
            //using (Connection connection = Scope.GetConnection(CONN)) {
            return SqlHelper.ExecuteReader(CONN, CommandType.Text, "SELECT date=getdate()");
            //}

        }
         
    }
}

