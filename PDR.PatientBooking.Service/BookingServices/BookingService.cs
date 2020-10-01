using System;
using System.Collections.Generic;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;

namespace PDR.PatientBooking.Service.BookingServices
{
    ///<inheritdoc cref="IBookingService"/>
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _addBookingValidator;
        private readonly ICancelBookingRequestValidator _cancelBookingValidator;

        public BookingService(PatientBookingContext context, IAddBookingRequestValidator addBookingValidator, ICancelBookingRequestValidator cancelBookingValidator)
        {
            _context = context;
            _addBookingValidator = addBookingValidator;
            _cancelBookingValidator = cancelBookingValidator;
        }

        public void AddBooking(AddBookingRequest request)
        {
            var validationResult = _addBookingValidator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var bookingId = new Guid();
            var bookingStartTime = request.StartTime;
            var bookingEndTime = request.EndTime;
            var bookingPatientId = request.PatientId;
            var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == request.PatientId);
            var bookingDoctorId = request.DoctorId;
            var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == request.DoctorId);
            var bookingSurgeryType = _context.Patient.FirstOrDefault(x => x.Id == bookingPatientId).Clinic.SurgeryType;

            var myBooking = new Order
            {
                Id = bookingId,
                StartTime = bookingStartTime,
                EndTime = bookingEndTime,
                PatientId = bookingPatientId,
                DoctorId = bookingDoctorId,
                Patient = bookingPatient,
                Doctor = bookingDoctor,
                SurgeryType = (int)bookingSurgeryType,
                IsActive = true
            };

            _context.Order.AddRange(new List<Order> { myBooking });
            _context.SaveChanges();
        }

        public void CancelBooking(Guid id)
        {
            var validationResult = _cancelBookingValidator.ValidateRequest(id);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var order = _context.Order.Single(o => o.Id == id);
            order.IsActive = false;

            _context.SaveChanges();
        }
    }
}
