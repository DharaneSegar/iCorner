/****************************************************************************************
 * File: ICustomerNotificationService.cs
 * Description: This file contains the ICustomerNotificationService interface, which defines
 *              the contract for customer notification services. The interface includes 
 *              methods for notifying customers about account activation and deactivation.
 ****************************************************************************************/

using System.Threading.Tasks;

namespace EAD.Services
{
    public interface ICustomerNotificationService
    {
        /// <summary>
        /// Notifies a customer about the successful activation of their account.
        /// </summary>
        /// <param name="customerEmail">The email address of the customer to notify.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task NotifyCustomerActivationAsync(string customerEmail);

        /// <summary>
        /// Notifies a customer about the successful deactivation of their account.
        /// </summary>
        /// <param name="customerEmail">The email address of the customer to notify.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task NotifyCustomerDeactivationAsync(string customerEmail);
    }
}
