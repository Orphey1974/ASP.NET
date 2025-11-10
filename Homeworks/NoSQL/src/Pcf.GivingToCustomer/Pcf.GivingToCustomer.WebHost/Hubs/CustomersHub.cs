using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Pcf.GivingToCustomer.WebHost.Hubs
{
    /// <summary>
    /// SignalR Hub для уведомлений о клиентах
    /// </summary>
    public class CustomersHub : Hub
    {
        /// <summary>
        /// Отправить уведомление о создании клиента
        /// </summary>
        public async Task CustomerCreated(object customerData)
        {
            await Clients.All.SendAsync("CustomerCreated", customerData);
        }

        /// <summary>
        /// Отправить уведомление об обновлении клиента
        /// </summary>
        public async Task CustomerUpdated(object customerData)
        {
            await Clients.All.SendAsync("CustomerUpdated", customerData);
        }

        /// <summary>
        /// Отправить уведомление об удалении клиента
        /// </summary>
        public async Task CustomerDeleted(Guid customerId)
        {
            await Clients.All.SendAsync("CustomerDeleted", customerId);
        }

        /// <summary>
        /// Отправить уведомление о получении списка клиентов
        /// </summary>
        public async Task CustomersListUpdated(object customersData)
        {
            await Clients.All.SendAsync("CustomersListUpdated", customersData);
        }
    }
}

