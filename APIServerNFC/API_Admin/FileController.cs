using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;

namespace APIServerNFC.API_Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FtpService _ftpService;

        public FileController(FtpService ftpService)
        {
            _ftpService = ftpService;
        }

        public class FtpRequest
        {
            public string FtpUrl { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("download")]
        public IActionResult DownloadFile([FromBody] FtpRequest request)
        {
            request.Username = ModelAdmin.userhost ;
            request.Password = ModelAdmin.passwordhost;
            var fileStream = _ftpService.DownloadFile(request.FtpUrl, request.Username, request.Password);
            return new FileStreamResult(fileStream, "application/octet-stream")
            {
                FileDownloadName = "downloadedfile"
            };
        }
    }

    public class FtpService
    {
        public Stream DownloadFile(string ftpUrl, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }
    }
}
