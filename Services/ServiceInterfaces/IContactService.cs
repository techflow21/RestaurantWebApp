
using Contracts.DTOs;

namespace Services.ServiceInterfaces
{
    public interface IContactService
    {
        Task SubmitContactForm(ContactRequestDto request);
    }
}
