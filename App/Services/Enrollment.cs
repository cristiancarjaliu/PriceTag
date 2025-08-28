using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PriceTag.App.Services
{
    public static class Enrollment
    {
        private static string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PriceTag.xml");
        public static void Enroll(string collectionEmail, string connectionString)
        {
            string localGuid = GetLocalGuidFromDB(collectionEmail, connectionString);
            if (string.IsNullOrEmpty(localGuid))
            {
                localGuid = Guid.NewGuid().ToString();
                UpdateLocalGuidInDB(collectionEmail, localGuid, connectionString);
            }
            if (!File.Exists(xmlPath))
            {
                XDocument doc = new(
                    new XElement("PriceTag",
                        new XElement("GuidList",
                            new XElement("Guid",
                                new XAttribute("code", "PriceTag"),
                                new XAttribute("guid", localGuid)
                            )
                        )
                    )
                );
                doc.Save(xmlPath);
            }
        }
        private static string GetLocalGuidFromDB(string email, string connectionString)
        {
            string guid = "";
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new("select LocalGuid from Collection where Email=@Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                object? result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    guid = result.ToString() ?? "";
            }
            return guid;
        }
        private static void UpdateLocalGuidInDB(string email, string guid, string connectionString)
        {
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new("update Collection set LocalGuid=@Guid where Email=@Email", conn);
                cmd.Parameters.AddWithValue("@Guid", guid);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
