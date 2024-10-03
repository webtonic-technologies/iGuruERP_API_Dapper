﻿using Employee_API.DTOs.ServiceResponse;
using System.Net;
using System.Net.Mail;


    public class SendEmail
    {
        public async Task<ServiceResponse<string>> SendEmailWithAttachmentAsync(string recipientEmail, string token)
        {
            try
            {
                // Sender's email address and credentials
                string senderEmail = "vinita@webtonictechnologies.com";
                string senderPassword = "Vinita@1234";
                 
                // Mail message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(new MailAddress(recipientEmail));
                mail.Subject = "Forget Password Link";
                mail.Body = "http://localhost:53715/api/Accounts/ResetPassword/" + token;

                //if (!string.IsNullOrEmpty(attachmentPath))
                //{
                //    Attachment attachment = new Attachment(attachmentPath, MediaTypeNames.Application.Octet);
                //    mail.Attachments.Add(attachment);
                //}

                // SMTP client configuration
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // Replace with your SMTP server and port
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

                // Send the email asynchronously
                await smtpClient.SendMailAsync(mail);

                // Clean up resources
                mail.Dispose();
              
              
                return new ServiceResponse<string>(true, "Email sent successfully", "Email sent successfully", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Failed to send email", ex.Message, 500);
            }
        }
    }