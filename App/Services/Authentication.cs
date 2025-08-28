using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PriceTag.App.Services
{
    public static class Authentication
    {
        private static string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PriceTag.xml");
        public static void SaveAuthToken(string token)
        {
            XDocument doc = new(
                new XElement("PriceTag",
                    new XElement("GuidList",
                        new XElement("Guid",
                            new XAttribute("code", "PriceTag"),
                            new XAttribute("guid", token)
                        )
                    )
                )
            );
            doc.Save(xmlFilePath);
        }
        public static string LoadAuthToken()
        {
            if (!File.Exists(xmlFilePath))
                return "";
            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                XElement? guidElement = doc.Root?.Element("GuidList")?.Element("Guid");
                if (guidElement != null)
                {
                    string token = guidElement.Attribute("guid")?.Value ?? "";
                    return string.IsNullOrEmpty(token) ? "" : token;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        public static bool ValidateAuthToken(string token, string connectionString)
        {
            if (!Guid.TryParse(token, out Guid guid))
                return false;
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new("select count(*) from Collection where LocalGuid = @token", conn);
                cmd.Parameters.Add("@token", SqlDbType.UniqueIdentifier).Value = guid;
                object? result = cmd.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;
                return count > 0;
            }
        }
    }
}
