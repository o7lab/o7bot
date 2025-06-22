using System;
using System.IO;
using System.Net;
using System.Text;

public class WebhookSender
{
    public static void SendFile(string webhookUrl, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found: " + filePath);
            return;
        }

        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        string contentType = "multipart/form-data; boundary=" + boundary;

        byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
        byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webhookUrl);
            request.Method = "POST";
            request.ContentType = contentType;
            request.KeepAlive = true;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Write multipart form boundary
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                // Create headers
                string header = "Content-Disposition: form-data; name=\"file\"; filename=\"" +
                                Path.GetFileName(filePath) + "\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                byte[] headerBytes = Encoding.UTF8.GetBytes(header);
                requestStream.Write(headerBytes, 0, headerBytes.Length);

                // Write file content
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }
                }

                // Write closing boundary
                requestStream.Write(trailer, 0, trailer.Length);
            }

            // Get response (optional logging)
            using (WebResponse response = request.GetResponse())
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                string resp = sr.ReadToEnd();
                Console.WriteLine("Upload success: " + resp);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Upload failed: " + ex.Message);
        }
    }
}
