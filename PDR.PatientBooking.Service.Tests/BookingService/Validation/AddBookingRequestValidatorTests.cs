using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingService.Validation;
using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBooking.Service.Tests.BookingService.Validation
{
    [TestFixture]
    public class AddBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private AddBookingRequestValidator _addBookingRequestValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _addBookingRequestValidator = new AddBookingRequestValidator(
                _context
            );
        }

        private void SetupMockDefaults()
        {
        }

        [Test]
        public void ValidateRequest_BookingTimeInThePast_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddHours(-5);
            request.EndTime = DateTime.UtcNow.AddHours(-4);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Appointment start time should be greater than current time");
        }

        [Test]
        public void ValidateRequest_StartTimeIsLessThanEndTime_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddHours(5);
            request.EndTime = DateTime.UtcNow.AddHours(4);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Appointment end time should be greater than start time");
        }

        [Test]
        public void ValidateRequest_DoctorHasConflictedBooking_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddHours(5);
            request.EndTime = DateTime.UtcNow.AddHours(6);

            var existingPatient = _fixture
                .Build<Patient>()
                .Create();

            var existingDoctorWithConflictedBooking = _fixture
                .Build<Doctor>()
                .With(x => x.Orders, new List<Order>()
                {
                    new Order()
                    {
                        StartTime = request.StartTime.AddMinutes(15),
                        EndTime = request.StartTime.AddMinutes(45),
                    }
                })
                .Create();

            _context.Add(existingPatient);
            _context.Add(existingDoctorWithConflictedBooking);
            _context.SaveChanges();

            request.PatientId = existingPatient.Id;
            request.DoctorId = existingDoctorWithConflictedBooking.Id;

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("A doctor is busy during selected time range");
        }

        [Test]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.UtcNow.AddHours(5);
            request.EndTime = DateTime.UtcNow.AddHours(6);

            var existingPatient = _fixture
                .Build<Patient>()
                .Create();

            var existingDoctor = _fixture
                .Build<Doctor>()
                .With(x => x.Orders, new List<Order>()
                {
                    new Order()
                    {
                        StartTime = request.StartTime.AddHours(2),
                        EndTime = request.StartTime.AddHours(2),
                    }
                })
                .Create();

            _context.Add(existingPatient);
            _context.Add(existingDoctor);
            _context.SaveChanges();

            request.PatientId = existingPatient.Id;
            request.DoctorId = existingDoctor.Id;

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        private AddBookingRequest GetValidRequest()
        {
            var request = _fixture.Create<AddBookingRequest>();
            return request;
        }
    }
}
