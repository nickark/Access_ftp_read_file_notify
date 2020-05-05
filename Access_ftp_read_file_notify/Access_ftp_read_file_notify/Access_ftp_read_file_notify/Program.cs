using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Collections;

namespace Access_ftp_read_file_notify
{
    class Program
    {
        static void Main(string[] args)
        {

            //reading the name of the file we would like to search for in the ftp. We assume that everyday a new file is created with a usique datestamp

            string[] name_of_the_file = System.IO.File.ReadAllLines(@"name_of_the_file.config");
            string filename_read = name_of_the_file[0];



            DateTime thisday = DateTime.Today;
            string day = thisday.ToString("dd");
            string month = thisday.ToString("MM");
            string year = thisday.ToString("yyyy");
            string filename = filename_read + year + "_" + month + "_" + day + "*.*";

            //reading the ftp server name and credentials

            string[] name_of_the_ftp = System.IO.File.ReadAllLines(@"name_of_the_ftp.config");
            string ftpname_read = name_of_the_ftp[0];
            string ftpusername_read = name_of_the_ftp[1];
            string ftppass_read = name_of_the_ftp[2];



            //reading the email client and the password of it in lines 1,2 of the file. Then for the rest lines reading the email addresse to send the alert

            string[] lines = System.IO.File.ReadAllLines(@"mailcredentials.config");


            string MailTo;

            string MailFrom = lines[0];
            string MailFromPass = lines[1];



            //accessing ftp and searching for the specific file 

            int file_found = 0;

            try
            {
                WebClient request = new WebClient();
                string url = ftpname_read + filename;
                request.Credentials = new NetworkCredential(ftpusername_read, ftppass_read);
                string[] dirs = Directory.GetFiles(ftpname_read, filename);
                foreach (string dir in dirs)
                {
                    file_found = 1;
                }
                Console.WriteLine(file_found);
                Console.WriteLine(filename);
                if (file_found == 1)
                {
                    Console.WriteLine("The file is found");
                }

                else
                {
                    Console.WriteLine("The file is not found");
                }
            }
            catch (Exception e)
            {


                Console.WriteLine(e);


            }




            string Subject;
            string Message;
            
            if (file_found == 0)
            {
                Subject = filename + "is not found";
                Message = "Dear all" + "\n\n" + "The file is not found in the ftp.";
            }
            else
            {
                Subject = filename + "is found";
                Message = "Dear all" + "\n\n" + "The file is found in the ftp.";
            }

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(MailFrom);


                for (int a = 2; a < lines.Length; a = a + 1)
                {

                    MailTo = lines[a];
                    mail.To.Add(new MailAddress(MailTo));


                }


                
                mail.Body = Message;
                mail.Subject = Subject;

                SmtpClient Smtp_Client = new SmtpClient();




                Smtp_Client = new SmtpClient("smtp.office365.com", 587);
                Smtp_Client.EnableSsl = true;



                Smtp_Client.Credentials = new NetworkCredential(MailFrom, MailFromPass);

                Smtp_Client.Send(mail);


                Smtp_Client.SendAsync(mail, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }






        }
    }
}
