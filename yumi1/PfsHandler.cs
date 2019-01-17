using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace yumi1
{
    public class PfsHandler
    {

        public static void ReadReplaceRequest(string requestType, string transferFile, string[] code, string[] propertyField)
        {
            int i = 0;
            if (code.Length == propertyField.Length)
            {
                foreach (string x in code)
                {
                    ReadReplaceRequest(requestType, transferFile, x, propertyField[i]);
                    i++;
                }
            }
        }

        public static StringBuilder ReadReplaceRequest(string requestType, string transferFile, string code, string propertyField)
        {
            StringBuilder request = new StringBuilder();
            //var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "transfer_file_processed.txt").FirstOrDefault();
            if (!File.Exists("transfer_file_processed"))
            {
                using (StreamReader streamReader = new StreamReader("transfer_file_processed.txt"))
                {
                    var x = streamReader.ReadLine();
                    if (x.Contains(requestType))
                    {
                        try
                        {
                            using (StreamReader streamReader2 = new StreamReader("transfer_file_processed.txt"))
                            {
                                string fileContents;
                                fileContents = streamReader2.ReadToEnd();
                                fileContents.Trim();
                                // make sure the request's line returns are the correct format
                                fileContents = Regex.Replace(fileContents, @"(?<=\b" + propertyField + "=).*", code);
                                fileContents = Regex.Replace(fileContents, @"(\r*\n)+", "\r\n");
                                request.AppendLine(fileContents);
                                request.AppendLine(); // add a blank line at the end
                            }
                        }
                        catch (Exception ex)
                        {
                            StringBuilder errorMessage = new StringBuilder();
                            errorMessage.AppendLine("TestLink Error: Read from transfer file failed");
                            errorMessage.AppendLine(ex.ToString());
                            throw new Exception(errorMessage.ToString());
                        }
                    }
                    else
                    {
                        WriteNewFile(transferFile, code, propertyField, request);
                    }
                }
            }
            else
            {
                WriteNewFile(transferFile, code, propertyField, request);
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
                Console.WriteLine(ex.StackTrace);
                throw new Exception(errorMessage.ToString());

            }
            var response = Pfs4.ClassLibrary.SendRequest("127.0.0.1", request);
            using (StreamWriter writer = new StreamWriter("transfer_file.txt"))
            {
                writer.WriteLine(response);
                writer.WriteLine();
                writer.WriteLine();
            }
            return request;
        }

        private static void WriteNewFile(string transferFile, string code, string propertyField, StringBuilder request)
        {
            try
            {
                using (StringReader stringReader = new StringReader(transferFile))
                {
                    string fileContents;
                    fileContents = stringReader.ReadToEnd();
                    fileContents.Trim();
                    // make sure the request's line returns are the correct format
                    fileContents = Regex.Replace(fileContents, @"(?<=\b" + propertyField + "=).*", code);
                    fileContents = Regex.Replace(fileContents, @"(\r*\n)+", "\r\n");
                    request.AppendLine(fileContents);
                    request.AppendLine(); // add a blank line at the end
                }
            }
            catch (Exception ex)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine("TestLink Error: Read from transfer file failed");
                errorMessage.AppendLine(ex.ToString());
                throw new Exception(errorMessage.ToString());
            }
        }
        public static bool ResponseOK()
        {
            string var;
            string filename = "transfer_file.txt";
            if (File.Exists(filename))
            {
                using (StreamReader reader = new StreamReader(filename))
                {

                    var = reader.ReadToEnd();
                }

                    if (var.Contains("OK"))
                        return true;
                    else
                    {
                        ParseResponseError(var);
                    }
            }
            else
            {

                throw new Exception("There is no transfer file in the current folder.");
            }
            return false;
        }

        public static void ParseResponseError(string error)
        {
            MessageBox.Show(error);
        }

    }
}

