using AutoMapper;
using Contracts.DTOs;
using Domain.Entities;
using Domain.Repository;
using MimeKit;
using Presentation;
using Services.ServiceInterfaces;

namespace Services.Implementation
{
    public class ReservationService : IReservationService
    {
        private readonly IMailKitEmailService _emailService;
        private readonly IAwsNotificationService _smsService;
        private readonly INexmoSmsService _nexmoSmsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IMapper _mapper;

        public ReservationService(IMailKitEmailService emailService, IMapper mapper, IUnitOfWork unitOfWork, IAwsNotificationService smsService, INexmoSmsService nexmoSmsService)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _reservationRepository = _unitOfWork.GetRepository<Reservation>();
            _mapper = mapper;
            _smsService = smsService;
            _nexmoSmsService = nexmoSmsService;
        }


        public async Task BookTable(ReservationRequestDto request)
        {
            var reservation = _mapper.Map<Reservation>(request);
            reservation.CreatedDate = DateTime.Now;
            reservation.Status = "Pending";

            await _reservationRepository.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{request.Name}", $"{request.Email}"));
            message.To.Add(new MailboxAddress("SOB-Foods", "bellosoliu12@gmail.com"));
            message.Subject = "Table Reservation";
            message.Body = new TextPart("plain")
            {
                Text = $"Name: {request.Name}\nEmail: {request.Email}\nPhone: {request.PhoneNumber}\nDate: {request.Date}\nTime: \nNo.ofGuests: {request.Guests}\nMenu:\n"
            };
            await _emailService.SendEmailAsync(message);
            await _smsService.SendSmsAsync(reservation.PhoneNumber, $"Your reservation is {reservation.Status}, Details are:\nDate: {reservation.Date}\nTime:\nNo.of.Guests: {reservation.Guests}\nMenu:\n");
            _nexmoSmsService.SendSms(reservation.PhoneNumber, $"Your reservation is {reservation.Status}, Details are:\nDate: {reservation.Date}\nTime:\nNo.of.Guests: {reservation.Guests}\nMenu:\n");
        }


        public async Task<IEnumerable<ReservationRequestDto>> GetReservationHistory()
        {
            var reservationsHistory = await _reservationRepository.GetAllAsync(orderBy: r => r.OrderByDescending(res => res.CreatedDate));
            var historyDto = _mapper.Map<IEnumerable<ReservationRequestDto>>(reservationsHistory);
            return historyDto;
        }


        public async Task ConfirmReservation(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                throw new ApplicationException("Reservation not found");
            }
            reservation.Status = "Confirmed";
            await _unitOfWork.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SOB-Foods", "sob-foods@gmail.com"));
            message.To.Add(new MailboxAddress($"{reservation.Name}", $"{reservation.Email}"));
            message.Subject = "Confirmed Table Reservation";
            message.Body = new TextPart("plain")
            {
                Text = $"Hi, {reservation.Name}!\nYour table reservation has been confirmed\nThe details of your reservation are: \nDate: {reservation.Date}\nTime:\nNo.of.Guests: {reservation.Guests}\nMenu: \nStatus: {reservation.Status}\n"
            };
            await _emailService.SendEmailAsync(message);

            await _smsService.SendSmsAsync(reservation.PhoneNumber, $"Your reservation has been {reservation.Status}, Details are:\nDate: {reservation.Date}\nTime:\nNo.of.Guests: {reservation.Guests}\nMenu:\n");
            _nexmoSmsService.SendSms(reservation.PhoneNumber, $"Your reservation has been {reservation.Status}, Details are:\nDate: {reservation.Date}\nTime:\nNo.of.Guests: {reservation.Guests}\nMenu:\n");
        }


        public async Task RejectReservation(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                throw new ApplicationException("Reservation not found");
            }
            reservation.Status = "Rejected";
            await _unitOfWork.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SOB-Foods", "sob-foods@gmail.com"));
            message.To.Add(new MailboxAddress($"{reservation.Name}", $"{reservation.Email}"));
            message.Subject = "Rejected Table Reservation";
            message.Body = new TextPart("plain")
            {
                Text = $"Hi, {reservation.Name}!\n We are sorry for any inconvinience this may cause\n Your table reservation has been rejected due to some reasons\nThe details of your supposed reservation are: \nDate: {reservation.Date}\nTime: \nNo.of.Guests: {reservation.Guests}\nMenu: \nStatus: {reservation.Status}\n"
            };
            await _emailService.SendEmailAsync(message);

            await _smsService.SendSmsAsync(reservation.PhoneNumber, $"Your reservation has been {reservation.Status}, Details are: \nDate: {reservation.Date}\nTime:\nNo.of.Guests: {reservation.Guests}\nMenu:\n");
            _nexmoSmsService.SendSms(reservation.PhoneNumber, $"Your reservation has been {reservation.Status}, Details are:\nDate: {reservation.Date}\nTime:\nNo.of.Guests: {reservation.Guests}\nMenu:\n");
        }
    }
}
