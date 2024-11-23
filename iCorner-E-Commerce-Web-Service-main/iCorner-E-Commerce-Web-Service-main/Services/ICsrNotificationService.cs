/****************************************************************************************
 * File: ICsrNotificationService.cs
 * Description: This file contains the ICsrNotificationService interface, which defines
 *              the contract for CSR notification services. The interface includes 
 *              methods for notifying customer service representatives about new customer 
 *              registrations.
 ****************************************************************************************/

using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Services
{
    public interface ICsrNotificationService
    {
        /// <summary>
        /// Notifies customer service representatives about a new customer registration.
        /// </summary>
        /// <param name="customerEmail">The email address of the newly registered customer.</param>
        /// <param name="csrEmails">A list of email addresses of the customer service representatives to notify.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task NotifyCsrsAboutNewCustomerAsync(string customerEmail, List<string> csrEmails);
    }
}
