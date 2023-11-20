using System;
using System.IO;


namespace AlfrescoToAzure
{
    public static class Logs
    {
        private static readonly object objetoBloqueo = new object();
       
        public static void RegistrarTramas(string strTramaRequest, string rutaArchivo)
        {
            try
            {
                lock (objetoBloqueo)
                {
                    var fileName = rutaArchivo + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    using (var fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(DateTime.Now.ToString("HHmmssff") + " " + strTramaRequest + " ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
