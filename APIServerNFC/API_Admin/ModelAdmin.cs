using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace APIServerNFC.API_Admin
{
    public static class ModelAdmin
    {

       
        public static string pathapp = AppDomain.CurrentDomain.BaseDirectory;
        public static string pathappdocument = Path.Combine(pathapp, "Document");


        /*public static string userhost = "scansiap";
        public static string passwordhost = "ckgt5678jzmu";
        public static string pathhostsoft = "ftp://112.213.89.32/public_html/Document_App/Huynh/";
        public static string pathhostpublic = "ftp://112.213.89.32/public_html/Document_App/SupplierPublic/";
        public static string pathurlfilepublic = "https://scansiapacific.com/Document_App/SupplierPublic/";
        public static string pathhostfile = "ftp://112.213.89.32/public_ftp/Supplier/";
       
        public static string pathhostdocument = "ftp://112.213.89.32/public_ftp/Supplier/Document/";*/

        public static string userhost = "vuthaihuynh";
        public static string passwordhost = "@Vuthaihuynh@123";

        public static string IPServer = "123.31.41.25";
        //public static string IPServer = "192.168.1.155";

        public static string pathhostsoft = string.Format("ftp://{0}:220/app/", IPServer);
        public static string pathhostpublic = string.Format("ftp://{0}:220/",IPServer);
        public static string pathurlfilepublic = string.Format("http://{0}:8003/",IPServer);
      
        public static string pathhostfile = string.Format("ftp://{0}:220/document/", IPServer);
        public static string pathhostdocument = string.Format("ftp://{0}:220/document", IPServer);
        public static string split = ">>";
        public static string stringsplit = " - ";

       
    }
}
