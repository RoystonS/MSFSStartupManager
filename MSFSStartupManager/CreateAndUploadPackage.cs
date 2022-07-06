using MSFSExeXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MSFSStartupManager
{

    public class NoLongerSupportedException : Exception
    {
    }

    public class StatusResponse
    {
        public bool supported { get; set; }
        public string requiredVersion { get; set; }
    }

    public class UploadRequest
    {
        public long size { get; set; }
    }

    public class UploadResponse
    {
        public string url { get; set; }
    }

    class CreateAndUploadPackage
    {
        private static readonly Lazy<string> Hostname = new Lazy<string>(() =>
        {
            // This is a CNAME to the real hostname, and
            // we need to use the real hostname for TLS shenanigans
            var entry = Dns.GetHostEntry("msfsstartup-prod.shufflebotham.org");
            var hostname = entry.HostName;

            if (hostname.Contains("amazonaws.com"))
            {
                // Looks good.
                return hostname;
            }

            // We've done the lookup, and the lookup succeeded, but it's not pointing to an AWS hostname any more.
            // The endpoint has been abandoned.
            throw new NoLongerSupportedException();
        });

        public async static Task<StatusResponse> GetStatus()
        {
            var client = new HttpClient();
            var url = $"https://{Hostname.Value}/status";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            var responseMessage = await client.SendAsync(requestMessage);
            var status = await GetResponse<StatusResponse>(responseMessage);
            return status;
        }

        public async static Task Run(MainWindowViewModel viewModel) {
            var client = new HttpClient();

            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                if (File.Exists(FlightSimulatorPaths.ExeXmlPath))
                {
                    archive.CreateEntryFromFile(FlightSimulatorPaths.ExeXmlPath, "exe.xml");
                }

                var reportEntry = archive.CreateEntry("Report.txt");
                using (var reportStream = reportEntry.Open())
                using (var writer = new StreamWriter(reportStream))
                {
                    writer.Write(GenerateReport.BuildReport(viewModel));
                }
            }


            var firstUrl = $"https://{Hostname.Value}/upload";
            var uploadData = new UploadRequest
            {
                size = memoryStream.Length
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, firstUrl)
            {
                Content = JsonContent.Create(uploadData, typeof(UploadRequest))
            };

            var responseMessage = await client.SendAsync(requestMessage);
            var upload = await GetResponse<UploadResponse>(responseMessage);

            memoryStream.Seek(0, SeekOrigin.Begin);
            using (var streamContent = new StreamContent(memoryStream))
            {
                var uploadMessage = new HttpRequestMessage(HttpMethod.Put, upload.url);
                uploadMessage.Content = streamContent;
                var uploadResponseMessage = await client.SendAsync(uploadMessage);

                uploadResponseMessage.EnsureSuccessStatusCode();
            }
        }

        private async static Task<T> GetResponse<T>(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                using (var contentStream = await responseMessage.Content.ReadAsStreamAsync())
                {
                    return await JsonSerializer.DeserializeAsync<T>(contentStream);
                }
            } else
            {
                throw new Exception("FAIL");
            }
        }
    }
}
