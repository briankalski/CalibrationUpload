using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace CalibrationUpload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("username", "username_DATA");
                nvc.Add("prevCal", "prevCal_DATA");
                nvc.Add("newCal", "newCal_DATA");
                nvc.Add("temperature", "10");
                nvc.Add("humidity", "50");
                nvc.Add("calComments", "test");
                nvc.Add("gageSN", "B4711016");
                nvc.Add("company", "AIT33302-3014");
                nvc.Add("tempType", "C");
                nvc.Add("hours", "10");
                nvc.Add("minutes", "30");
                nvc.Add("result", "pass");
                nvc.Add("conditionFound", "broke");
                nvc.Add("conditionLeft", "fixed");
                nvc.Add("totalCycles", "10");
                nvc.Add("targetTorque", "10");
                nvc.Add("fileName", "UploadedFile.txt");
                nvc.Add("performedBy", "JOHNSMITH");
                nvc.Add("currentLocation", "Loc");
                nvc.Add("resetCalibrationSchedule", "0");
                nvc.Add("isValidation", "0");
                nvc.Add("toolAttachment", "attachment");
                nvc.Add("freeSpeedPSI", "20");

                HttpUploadFile("http://demos.groupspin.com/ACWebServices.svc/SubmitCalibration", openFileDialog1.FileName, nvc); // "text/html"
            }  
        }

        public static void HttpUploadFile(string url, string file, NameValueCollection nvc)
        {
            var filePath = file.Split('\\');
            string fileName = filePath[filePath.Length - 1];
            string boundary = "-----------------------------41952539122868";
            byte[] boundarybytes = System.Text.Encoding.UTF8.GetBytes(boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            //wr.KeepAlive = true;
            //wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Stream rs = wr.GetRequestStream();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: image/png\r\n\r\n";
            string header = string.Format(headerTemplate, "image", fileName);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            //byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            byte[] trailer = System.Text.Encoding.UTF8.GetBytes(boundary + "--");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();
            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                string result = reader2.ReadToEnd();
                MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while converting file", "Error!");
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }
       
    }
}
