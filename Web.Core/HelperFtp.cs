using System;
using System.IO;
using System.Net;
using System.Web;

namespace Web.Core
{
    public class HelperFtp
    {
        public static string GetIpAddress()
        {
            string ipAddress = null;
            try
            {
                if (HttpContext.Current != null &&
                    HttpContext.Current.Request.UserHostAddress != null
                    )
                {
                    ipAddress = HttpContext.Current.Request.UserHostAddress;
                }
            }
            catch (Exception)
            {
                ipAddress = null;
            }
            return ipAddress;
        }
        private string FtpServerIp { get; set; }
        private string FtpUser { get; set; }
        private string FtpPassword { get; set; }
        public HelperFtp(string server, string user, string pass)
        {
            FtpServerIp = server;
            FtpUser = user;
            FtpPassword = pass;
        }
        public bool UploadFile(string sourceFile, string desFile)
        {
            try
            {
                var fileInf = new FileInfo(sourceFile);
                var uri = string.Format("ftp://{0}{1}", FtpServerIp, desFile);
                // Create FtpWebRequest object from the Uri provided
                var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                // Provide the WebPermission Credintials
                reqFtp.Credentials = new NetworkCredential(FtpUser, FtpPassword);
                // By default KeepAlive is true, where the control connection is not closed
                // after a command is executed.
                reqFtp.KeepAlive = false;
                // Specify the command to be executed.
                reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
                // Specify the data transfer type.
                reqFtp.UseBinary = true;
                reqFtp.UsePassive = false;
                // Notify the server about the size of the uploaded file
                reqFtp.ContentLength = fileInf.Length;
                // The buffer size is set to 2kb
                const int buffLength = 2048;
                var buff = new byte[buffLength];
                // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
                var fs = fileInf.OpenRead();

                // Stream to which the file to be upload is written
                var strm = reqFtp.GetRequestStream();
                // Read from the file stream 2kb at a time
                var contentLen = fs.Read(buff, 0, buffLength);
                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteFile(string fileName)
        {
            try
            {
                var uri = string.Format("ftp://{0}/{1}", FtpServerIp, fileName);
                var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                reqFtp.Credentials = new NetworkCredential(FtpUser, FtpPassword);
                reqFtp.KeepAlive = false;
                reqFtp.Method = WebRequestMethods.Ftp.DeleteFile;
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var datastream = response.GetResponseStream();
                if (datastream != null)
                {
                    var sr = new StreamReader(datastream);
                    sr.Close();
                    datastream.Close();
                }
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public long GetFileSize(string filename)
        {
            long fileSize;
            try
            {
                var uri = string.Format("ftp://{0}/{1}", FtpServerIp, filename);
                var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(FtpUser, FtpPassword);
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                if (ftpStream != null) ftpStream.Close();
                response.Close();
            }
            catch (Exception)
            {
                fileSize = 0;
            }
            return fileSize;
        }
        public bool Rename(string currentFilename, string newFilename)
        {
            try
            {
                var uri = string.Format("ftp://{0}/{1}", FtpServerIp, currentFilename);
                var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                reqFtp.Method = WebRequestMethods.Ftp.Rename;
                reqFtp.RenameTo = newFilename;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(FtpUser, FtpPassword);
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                if (ftpStream != null) ftpStream.Close();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool MakeDir(string dirName)
        {
            try
            {
                // dirName = name of the directory to create.
                var uri = string.Format("ftp://{0}/{1}", FtpServerIp, dirName);
                var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(FtpUser, FtpPassword);
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                if (ftpStream != null) ftpStream.Close();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetIPHostAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
