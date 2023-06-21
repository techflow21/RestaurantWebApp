﻿using AutoMapper;
using Contracts.DTOs;
using Domain.Entities;
using Domain.Repository;
using MimeKit;
using Presentation;
using Services.ServiceInterfaces;

namespace Services.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IMailKitEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IMailKitEmailService emailService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contactRepository = _unitOfWork.GetRepository<Contact>();
            _unitOfWork = unitOfWork;
        }

        public async Task SubmitContactForm(ContactRequestDto request)
        {
            var contact = _mapper.Map<Contact>(request);
            contact.CreatedDate = DateTime.Now;

            await _contactRepository.AddAsync(contact);
            await _unitOfWork.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress($"{request.Name}", $"{request.Email}"));
            message.To.Add(new MailboxAddress("SOB-Foods", "bellosoliu12@gmail.com"));
            message.Subject = "Contact Form";
            message.Body = new TextPart("plain")
            {
                Text = $"Name: {request.Name}\nEmail: {request.Email}\nMessage: {request.Message}"
            };
            await _emailService.SendEmailAsync(message);
        }
    }
}
