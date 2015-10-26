using System.Net.Mail;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Log
{
    /// <summary>
    /// Interface for email helpers
    /// </summary>
    public interface IEmailHelper
    {
        /// <summary>
        /// Sends notifications emails
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachment"></param>
        void SendNotificationEmail(string recipients, string subject, string body, Attachment attachment = null);
    }
}