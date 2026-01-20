using DevExpress.XtraReports.UI;
using System.Reflection;
using System;

namespace APIServerNFC.Api_Report
{
    public static class CreateReportWithName
    {
        public static XtraReport CreateReportInstance(string reportName)
        {
            // Lấy Assembly hiện tại
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                // Tìm lớp báo cáo theo tên (Namespace.ReportClassName)
                var type = assembly.GetType($"APIServerNFC.Api_Report.{reportName}");

                if (type != null && typeof(XtraReport).IsAssignableFrom(type))
                {
                    return (XtraReport)Activator.CreateInstance(type);
                }
            }
            catch {
                return null;
            }
            return null;

        }
    }
}
