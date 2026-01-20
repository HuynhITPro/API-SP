using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace APIServerNFC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoTPController : ControllerBase
    {
        [Route("UploadFileInfoKTP")]
        [HttpPost]
        //Lưu ý: public host tham chiếu thẳng đến Folder SupplierPublic string.Format("http://{0}:8003/", IPServer)
        public async Task<IActionResult> UploadImageProfile([FromForm] FileViewModel fileviewmodel)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                //if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic")))
                //{
                //    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic"));
                //}
                //if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic","KhoTP")))
                //{
                //    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic", "KhoTP"));
                //}
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "SupplierPublic", "NFC/Document/KhoTP");//lấy theo thư mục debug lấy ra
                                                                                                         //Tạo sẵn thư mục này trong wwwroot
                //if (!Directory.Exists(pathBuilt))
                //{
                //    Directory.CreateDirectory(pathBuilt);
                //}
                using (Model.NFCDBContext nFCDBContext = new Model.NFCDBContext())
                {
                    string pathsave = "";
                    Model.FileHoSo fileHoSo = JsonConvert.DeserializeObject<Model.FileHoSo>(fileviewmodel.name);
                    if (fileHoSo.TableName == "KhoTP_ScanCT")
                    {
                        var query = nFCDBContext.KhoTpScanCt.Where(p => p.Serial.Equals(fileHoSo.SerialLink.Value)).FirstOrDefault();
                        if (query == null)
                        {
                            return Ok("Cont không tồn tại trên hệ thống");

                        }
                        fileHoSo.DienGiai = query.Ngay.Value.ToString("yyyy.MM.dd");
                        pathsave = "Document/KhoTP/" + fileHoSo.DienGiai + "/" + fileHoSo.SerialLink.Value.ToString();

                    }
                    else //Nếu không phải chứng từ lên Cont thì tạo thêm folder ChungTu
                    {
                        var query = nFCDBContext.KhoTpChungTu.Where(p => p.MaCt.Equals(fileHoSo.SerialLink.Value)).FirstOrDefault();
                        if (query == null)
                        {
                            return Ok("Chứng từ không tồn tại trên hệ thống");

                        }
                        fileHoSo.DienGiai = query.Ngay.Value.ToString("yyyy.MM.dd");
                        pathsave = "Document/KhoTP/" + fileHoSo.DienGiai + "/ChungTu/" + fileHoSo.SerialLink.Value.ToString();
                    }
                    string pathngay = Path.Combine(pathBuilt, fileHoSo.DienGiai);
                    if (!Directory.Exists(pathngay))
                    {
                        Directory.CreateDirectory(pathngay);
                    }
                    string pathitem = "";

                    if (fileHoSo.TableName == "KhoTP_ScanCT")
                    {
                        pathitem = Path.Combine(pathngay, fileHoSo.SerialLink.Value.ToString());
                    }
                    else
                    {
                        string pathchungtu = Path.Combine(pathngay, "ChungTu");
                        if (!Directory.Exists(pathchungtu))
                        {
                            Directory.CreateDirectory(pathchungtu);
                        }
                        pathitem = Path.Combine(pathchungtu, fileHoSo.SerialLink.Value.ToString());
                    }
                    if (!Directory.Exists(pathitem))
                    {
                        Directory.CreateDirectory(pathitem);
                    }
                    fileHoSo.UrlFile =pathsave+'/' + fileviewmodel.file.FileName;
                    var filePath = Path.Combine(pathitem, fileviewmodel.file.FileName);// Path.GetTempFileName();
                    fileHoSo.Dvt = "MB";

                    var stream = new FileStream(filePath, FileMode.Create);
                    await fileviewmodel.file.CopyToAsync(stream);
                    nFCDBContext.FileHoSo.Add(fileHoSo);
                    nFCDBContext.SaveChanges();
                }
                return new JsonResult("done");
                //create a Document instance, and insert value into database.
                //return "done";
                // return Ok(new { length = fileviewmodel.file.Length, name = fileviewmodel.file.FileName });
            }
            catch(Exception ex)
            {
                return new JsonResult("Lỗi lưu ảnh "+ex.Message);
                //return BadRequest();
            }

            //}
            // return Ok("OK");

        }
        public class FileViewModel
        {
            public string name { get; set; }
            public IFormFile file { get; set; }
        }
    }
}
