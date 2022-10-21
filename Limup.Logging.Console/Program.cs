using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Limup.Logging.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //SocketException ex = new SocketException();
            try
            {
                //Erro intencional
                string teste2 = "10.5";
                double teste = Convert.ToInt32(teste2);
            }
            catch (Exception ex)
            {
                ManagerLogs.LogWrite(LocationLogEntry.EventFile, System.Diagnostics.EventLogEntryType.Error, "Program()", ex.Message, ex);   
            }
        }
    }
}
