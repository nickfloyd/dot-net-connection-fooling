using System;
using System.EnterpriseServices;

namespace arch_1_connection_pooling.CustomDataHijinx {
    /// <summary>
    /// Handles transactions using Service Without Components.
    /// </summary>
    public class TransactionUtil {
        /// <summary>
        /// Specifies and configures the services and then activates the domain. 
        /// </summary>
        public static void Enter() {
            ServiceConfig serviceConfig = new ServiceConfig();
            serviceConfig.Transaction = TransactionOption.Required;
            serviceConfig.IsolationLevel = TransactionIsolationLevel.ReadCommitted;
            ServiceDomain.Enter(serviceConfig);
        }


        public static void ReadCommitted() {
            ServiceConfig serviceConfig = new ServiceConfig();
            serviceConfig.Transaction = TransactionOption.Required;
            serviceConfig.IsolationLevel = TransactionIsolationLevel.ReadCommitted;
            ServiceDomain.Enter(serviceConfig);
        }

        public static void ReadUncommitted() {
            ServiceConfig serviceConfig = new ServiceConfig();
            serviceConfig.Transaction = TransactionOption.NotSupported;
            serviceConfig.IsolationLevel = TransactionIsolationLevel.ReadUncommitted;
            ServiceDomain.Enter(serviceConfig);
        }

        public static void Serializable() {
            ServiceConfig serviceConfig = new ServiceConfig();
            serviceConfig.Transaction = TransactionOption.Required;
            serviceConfig.IsolationLevel = TransactionIsolationLevel.Serializable;
            ServiceDomain.Enter(serviceConfig);
        }

        public static void RepeatableRead() {
            ServiceConfig serviceConfig = new ServiceConfig();
            serviceConfig.Transaction = TransactionOption.Required;
            serviceConfig.IsolationLevel = TransactionIsolationLevel.RepeatableRead;
            ServiceDomain.Enter(serviceConfig);
        }

        /// <summary>
        /// Commits the transaction and ends the current context.
        /// </summary>
        public static void SetComplete() {
            ContextUtil.SetComplete();
            ServiceDomain.Leave();
        }

        /// <summary>
        /// Aborts the transaction and ends the current context.
        /// </summary>
        public static void SetAbort() {
            ContextUtil.SetAbort();
            ServiceDomain.Leave();
        }
    }
}
