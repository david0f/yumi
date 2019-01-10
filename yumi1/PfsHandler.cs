using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace yumi1
{
    public class PfsHandler
    {
        public static StringBuilder ReadReplaceRequest(string transferFile, string code, string propertyField)
        {
            StringBuilder request = new StringBuilder();
            try
            {
                using (StreamReader streamReader = new StreamReader(transferFile))
                {
                    string fileContents;
                    fileContents = streamReader.ReadToEnd();
                    fileContents.Trim();
                    // make sure the request's line returns are the correct format
                    fileContents = Regex.Replace(fileContents, @"(?<=\b" + propertyField + "=).*", code);
                    fileContents = Regex.Replace(fileContents, @"(\r*\n)+", "\r\n");
                    request.AppendLine(fileContents);
                    request.AppendLine(); // add a blank line at the end
                }
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter("transfer_file_processed.txt", false))
                    {
                        streamWriter.Write(request);

                        streamWriter.WriteLine();
                        streamWriter.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    StringBuilder errorMessage = new StringBuilder();
                    errorMessage.AppendLine("TestLink Error: Write to transfer file failed");
                    errorMessage.AppendLine(ex.ToString());
                    throw new Exception(errorMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine("TestLink Error: Read from transfer file failed");
                errorMessage.AppendLine(ex.ToString());
                throw new Exception(errorMessage.ToString());
            }
            return request;
        }
    }
}
