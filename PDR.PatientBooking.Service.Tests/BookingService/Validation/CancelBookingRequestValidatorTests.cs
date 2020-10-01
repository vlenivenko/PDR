using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBooking.Service.Tests.BookingService.Validation
{
    [TestFixture]
    public class CancelBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private CancelBookingRequestValidator _cancelBookingRequestValidator;

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
            _cancelBookingRequestValidator = new CancelBookingRequestValidator(
                _context
            );
        }

        private void SetupMockDefaults()
        {
        }

        [Test]
        public void ValidateRequest_BookingWasNotFound_ReturnsFailedValidationResult()
        {
            //arrange
            var id = Guid.NewGuid();

            //act
            var res = _cancelBookingRequestValidator.ValidateRequest(id);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("A booking with provided id was not found in the system");
        }

        [Test]
        public void ValidateRequest_BookingWasAlreadyCancelled_ReturnsFailedValidationResult()
        {
            //arrange
            var existingBooking = _fixture
                .Build<Order>()
                .With(o => o.IsActive, false)
                .Create();

            _context.Add(existingBooking);
            _context.SaveChanges();

            var id = existingBooking.Id;

            //act
            var res = _cancelBookingRequestValidator.ValidateRequest(id);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("A booking with provided id was already cancelled");
        }

        [Test]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            //arrange
            var existingBooking = _fixture
                .Build<Order>()
                .Create();

            _context.Add(existingBooking);
            _context.SaveChanges();

            var id = existingBooking.Id;

            //act
            var res = _cancelBookingRequestValidator.ValidateRequest(id);

            //assert
            res.PassedValidation.Should().BeTrue();
        }
    }
}
