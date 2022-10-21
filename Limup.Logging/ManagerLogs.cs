// -----------------------------------------------------------------------
// <copyright file="LocationLogEntry.cs" company="Limup">
//      Empresa de tecnologia em desenvolvimento.
// </copyright>
// -----------------------------------------------------------------------

namespace Limup.Logging
{
    using System;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using System.Configuration;
    using System.Diagnostics;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    /// <summary>
    /// Classe que contém as funções para gerar log.
    /// Para o funcionamento desta classe é necessário adicionar o filesource 
    /// através do EnterpriseLibrary no projeto destino.
    /// </summary>
    public static class ManagerLogs
    {
        /// <summary> Nome do source que será pesquisa no arquivo de configuração. </summary>
        private static string configFile = "Limup.Logging";
        private static string eventSourceName = ConfigurationManager.AppSettings["ApplicationName"];

        /// <summary> Objeto com as informações para gravar no log. </summary>
        private static LogEntry logEntry = null;

        /// <summary>
        ///Inicializa o menbro statico <see cref="ManagerLogs"/>
        /// </summary>
        static ManagerLogs()
        {
            IConfigurationSource configLog = ConfigurationSourceFactory.Create(configFile);
            Logger.SetLogWriter(new LogWriterFactory(configLog).Create());

            IConfigurationSource config = ConfigurationSourceFactory.Create(configFile);
            ExceptionPolicyFactory factory = new ExceptionPolicyFactory(configLog);
            ExceptionManager exceptionManager = factory.CreateManager();
            ExceptionPolicy.SetExceptionManager(exceptionManager);
        }
        
        /// <summary>
        /// Inicia o processo para gerar o log.
        /// </summary>
        /// <param name="LocationLogEntry">Local para gravar o log</param>
        /// <param name="typeLog">Tipo de log</param>
        /// <param name="metodo">Método onde o erro ocorreu</param>
        /// <param name="message">Menssagem para gravar no log</param>
        /// <param name="exception">Exception para gravar no log de erro (Obrigatorio para logar erros)</param>
        public static void LogWrite(LocationLogEntry locationLogEntry, EventLogEntryType typeLog, string metodo, string message, Exception exception)
        {
            try
            {
                message = String.IsNullOrWhiteSpace(message) ? exception.Message : message;

                if (locationLogEntry == LocationLogEntry.EventViewer && !EventLog.SourceExists(eventSourceName))
                {
                    System.Diagnostics.EventLog.CreateEventSource(eventSourceName, eventSourceName);
                }

                switch (typeLog)
                {
                    case EventLogEntryType.Error:
                        if (exception != null)
                        {
                            LogWriteError(exception);
                        }
                        if (locationLogEntry == LocationLogEntry.EventFile)
                        {
                            BuildLogEntry(locationLogEntry, typeLog, metodo, message);
                            Logger.Write(logEntry);
                        }
                        break;
                    case EventLogEntryType.Information:
                        BuildLogEntry(locationLogEntry, typeLog, metodo, message);
                        Logger.Write(logEntry);
                        break;
                    case EventLogEntryType.Warning:
                        BuildLogEntry(locationLogEntry, typeLog, metodo, message);
                        Logger.Write(logEntry);
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "PolicyLogging");
            }
        }

        /// <summary>
        ///  Cria o objeto LogEntry usado para gravar o log
        /// </summary>
        /// <param name="LocationLogEntry">Local para gravar o log</param>
        /// <param name="typeLog">Tipo de log</param>
        /// <param name="metodo">Método onde o erro ocorreu</param>
        /// <param name="message">Menssagem para gravar no log</param>
        /// <returns>LogEntry</returns>
        private static LogEntry BuildLogEntry(LocationLogEntry LocationLog, EventLogEntryType typeLog, string metodo, string message)
        {
            try
            {
                logEntry = new LogEntry();

                logEntry.Message = message;
                logEntry.Categories.Add(LocationLog.ToString());
                logEntry.Title = string.Format("Gravado log do metodo {0}", metodo);
                logEntry.ProcessName = metodo;

                switch (typeLog)
                {
                    case EventLogEntryType.Error:
                        logEntry.Priority = 4;
                        logEntry.EventId = 400;
                        logEntry.Severity = TraceEventType.Error;
                        break;
                    case EventLogEntryType.Information:
                        logEntry.Priority = 2;
                        logEntry.EventId = 200;
                        logEntry.Severity = TraceEventType.Information;
                        break;
                    case EventLogEntryType.Warning:
                        logEntry.Priority = 3;
                        logEntry.EventId = 300;
                        logEntry.Severity = TraceEventType.Warning;
                        break;
                    default:
                        logEntry.Priority = 1;
                        logEntry.EventId = 100;
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "PolicyLogging");
            }
            return logEntry;
        }

        /// <summary>
        /// Grava o erro apartir da politica estipulada
        /// </summary>
        /// <param name="exception">Exception</param>
        private static void LogWriteError(Exception exception)
        {
            try
            {
                ExceptionPolicy.HandleException(exception, "PolicyLogging");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
