using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace arch_1_connection_pooling.CustomDataHijinx {
    public class Connection : IDisposable {
        private SqlConnection conn = null;
        /// <summary>
        /// True means that this connection is part of an ongoing transaction AND there is only 1
        /// persistenct storage which has enlisted till now, and hence this connection should not be disposed off,
        /// as it may be used by other nested transactions
        /// False means that that although this connection is a part of the ongoing transaction, but 
        /// there are more than one persistant storages in this transaction, so we can dispose this transaction safely
        /// </summary>
        public bool InTransaction = false;
        private bool IsDisposed = false;
        private string _ConnectionString = null;
        public Connection(string connectionString) {
            conn = new SqlConnection(connectionString);
            _ConnectionString = connectionString;
        }

        public void Open() {
            try {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
            }
            catch (Exception e) {

            }
        }

        public void Close() {
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }

        public ConnectionState State {
            get {
                return conn.State;
            }
        }

        public string ConnectionString {
            get {
                return _ConnectionString;
            }
        }

        public SqlConnection CurrentConnection {
            get {
                return conn;
            }
        }

        public SqlCommand CreateCommand() {
            return conn.CreateCommand();
        }

        #region IDisposable Members

        public void Dispose() {
            if (!IsDisposed) // If this connection is not already disposed
            {
                if (!InTransaction)
                    conn.Dispose();
                IsDisposed = true;

            }
        }

        #endregion
    }
}
