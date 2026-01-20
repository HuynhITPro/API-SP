using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APIServerNFC.API_Admin
{
    public static class DirectoryFTP
    {
        public static string PathKhoTP = Path.Combine(ModelAdmin.pathhostdocument, "KhoTP");

        public static bool CreateFTPDirectory(string directory)
        {
            try
            {
                //create the directory
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(ModelAdmin.userhost, ModelAdmin.passwordhost);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }
        public static bool CreateFolder(string Foldername)
        {

            CreateFTPDirectory(Path.Combine(ModelAdmin.pathhostfile, Foldername));
            return true;
        }
        public static bool CreateFolderDocument(string Foldername)
        {
            CreateFTPDirectory(Path.Combine(ModelAdmin.pathhostdocument, Foldername));
            return true;
        }
        

    }
}
