
using Contracts.DTOs;

namespace Services.ServiceInterfaces
{
    public interface IReservationService
    {
        Task BookTable(ReservationRequestDto request);
        Task<IEnumerable<ReservationRequestDto>> GetReservationHistory();
        Task ConfirmReservation(int reservationId);
        Task RejectReservation(int reservationId);
    }
}
