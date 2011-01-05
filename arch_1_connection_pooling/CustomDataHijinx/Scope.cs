using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;

namespace arch_1_connection_pooling.CustomDataHijinx {
    public sealed class Scope : IDisposable {
        private bool _disposed, _ownsInstance;
        private TransactionScope _instance;
        private Scope _parent;
        [ThreadStatic]
        private static Scope _head;

        public string Name = "";
        /// <summary>
        /// True if there is only a single resource is being used
        /// False, if more that 1 resources are being used
        /// </summary>
        [ThreadStatic]
        private static bool SingleConnection;

        [ThreadStatic]
        private static Connection connection;

        /// <summary>
        /// The current connection string
        /// </summary>
        [ThreadStatic]
        private static string ConnectionString;
        [ThreadStatic]
        public static bool LastTransactionSuccess;

        [ThreadStatic]
        public static string Error;

        public Scope(TransactionScope instance) : this(instance, true) { }

        public Scope(TransactionScope instance, bool ownsInstance) {
            if (instance == null)
                throw new ArgumentNullException("instance");
            _instance = instance;
            _ownsInstance = ownsInstance;
            Thread.BeginThreadAffinity();
            _parent = _head;
            // If this is the Start of the outermost Scope, set SingleConnection to true.
            if (_head == null) {
                SingleConnection = true;
                ConnectionString = "";
                connection = null;
                LastTransactionSuccess = true;
                Error = "";
            }
            _head = this;
        }

        public static TransactionScope Current {
            get { return _head != null ? _head._instance : null; }
        }

        public static Scope CurrentScope {
            get { return _head; }
        }

        public static Connection GetConnection(string connectionString) {
            if (Current != null) {
                // We are in an existing scope
                // First find if there is a connection already present with the same ConnectionString
                if (connection != null) {
                    // We have a thread specific connection here, but find if it is using
                    // the same connection string
                    if (connectionString.Equals(connection.ConnectionString) && SingleConnection) {
                        // We have a connection which we can use, so return it
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        return connection;
                    }
                    else {
                        // There is already a connection in the current scope which is not
                        // connecting to the same DB, so the transaction is going to be upgraded to  
                        // a full blown DTC. Hence all the connection from this point on may use their own instance
                        SingleConnection = false;
                        return CreateNewConnection(connectionString, false);
                    }
                }
                else {
                    // The existing connection is null, so we create a new connection, and maintain that
                    // as our thread local connection
                    connection = CreateNewConnection(connectionString, true);
                    return connection;
                }
            }
            else {
                // We are not in an existing scope, so just create a new connection and give it back
                return CreateNewConnection(connectionString, false);
            }
        }

        private static Connection CreateNewConnection(string connectionString, bool IsInTransaction) {
            Connection conn = new Connection(connectionString);
            conn.Open();
            conn.InTransaction = IsInTransaction;
            return conn;
        }

        public void Dispose() {
            if (!_disposed) {
                _disposed = true;
                Debug.Assert(this == _head, "Disposed out of order.");
                _head = _parent;
                Thread.EndThreadAffinity();
                if (_ownsInstance) {
                    IDisposable disposable = _instance as IDisposable;
                    if (disposable != null) {
                        try {
                            disposable.Dispose();
                        }
                        catch (Exception ex) {
                            // If the outer scope commit, but not the inner one, then  the whole transaction will be aborted by throwing a TransactionAbortedException 
                            // as soon as the outer Scope goes out of scope. So although this is a legitimate exception, we just eat it.
                            LastTransactionSuccess = false;
                            Error = ex.Message;
                        }
                    }
                }
                if (_head == null) {
                    // Meaning that this is the outermost scope
                    if (connection != null)
                        connection.Dispose();
                }
            }
        }
    }
}
