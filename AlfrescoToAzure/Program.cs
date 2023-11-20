using System;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using AlfrescoToAzure;
using AlfrescoToAzure.ServiceReference1;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AlfrescoAzureConsoleApp
{
    class Program
    {
        private static readonly string ruta_log = "c:\\Logs\\Avatar\\";
        static async Task Main(string[] args)
        {
            
            // Configuración de Alfresco
            var alfrescoBasePath = ConfigurationSettings.AppSettings.Get("alfrescoBasePath"); ;
            var username = ConfigurationSettings.AppSettings.Get("username"); ;
            var password = ConfigurationSettings.AppSettings.Get("password"); ;

            // Configuración de Azure Blob Storage
            var connectionString = ConfigurationSettings.AppSettings.Get("connectionString"); ;
            var containerName = ConfigurationSettings.AppSettings.Get("containerName"); ;

            // Crear cliente HTTP
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

            // Obtener datos de avatares
            DataTable data = new GetIdsAvatar();

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];
                
                // Accede a los valores de cada columna por su nombre o índice
                string str_id = (string)row["str_id_alfresco"];
                int int_ente = (int)row["int_ente"];

                try
                {
                    // Obtener información del archivo en Alfresco
                    var alfrescoFileUrl = $"{alfrescoBasePath}/api/-default-/public/alfresco/versions/1/nodes/{str_id}/content";
                    var response = await httpClient.GetAsync(alfrescoFileUrl);
                    response.EnsureSuccessStatusCode();
                    var fileContent = await response.Content.ReadAsByteArrayAsync();

                    // Obtener la extensión del archivo
                    var extension = response.Content.Headers.ContentType.MediaType;

                    // Crear cliente de Azure Blob Storage
                    var blobClient = new BlobServiceClient(connectionString);
                    var containerClient = blobClient.GetBlobContainerClient(containerName);

                    // Subir el archivo a Azure Blob Storage con el Content-Type(extensión) adecuado
                    var blobName = $"{str_id}";
                    var blob = containerClient.GetBlobClient(blobName);
                    await blob.UploadAsync(new MemoryStream(fileContent), new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = extension
                        }
                    });

                    Console.WriteLine("Archivo subido a Azure Blob Storage correctamente.");
                    Logs.RegistrarTramas("r> " + "ente: "+ int_ente + " Archivo subido a Azure Blob Storage correctamente", ruta_log);
                    
                    // Validar actualización en la base
                    int int_actualizado = new UpdateIdAvatar(str_id, int_ente);
                    if (int_actualizado == 0)
                    {
                        Console.WriteLine("ID se actualizó correctamente en la base");
                        Logs.RegistrarTramas("r> " + "ente: " + int_ente + " ID se actualizó correctamente en la base", ruta_log);
                    }
                    else
                    {
                        Console.WriteLine("ID NO se actualizó correctamente en la base");
                        Logs.RegistrarTramas("e> " + "ente: " + int_ente + " ID NO se actualizó correctamente en la base", ruta_log);
                    }
                }
                catch (HttpRequestException httpException)
                {
                    Console.WriteLine($"Error en la solicitud HTTP a Alfresco: {httpException.Message}");
                    Logs.RegistrarTramas("e> " + "ente: " + int_ente + " Error Alfresco", ruta_log);
                }
                //catch (RequestFailedException azureException)
                //{
                //    Console.WriteLine($"Error en la API de Azure Blob Storage: {azureException.Message}");
                //}
                catch (Exception ex)
                {
                    Console.WriteLine($"Error general: {ex.Message}");
                    Logs.RegistrarTramas("e> " + "ente: " + int_ente + " Error general", ruta_log);
                }
            }
        }
    }
}
