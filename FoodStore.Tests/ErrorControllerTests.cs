using FoodStore.Controllers;
using FoodStore.Services.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests
{
    public class ErrorControllerTests
    {
        private ErrorController _controller;

        [SetUp]
        public void Setup()
        {
            // No dependencies, just instantiate the controller
            _controller = new ErrorController();
        }

        [TearDown]
        public void Teardown()
        {
            _controller.Dispose();
        }
        [Test]
        public void NotFoundPage_ReturnsNotFoundView()
        {

            var result = _controller.NotFoundPage() as ViewResult;


            Assert.IsNotNull(result);
            Assert.That(result.ViewName, Is.EqualTo("NotFound"));
        }

        [Test]
        public void ServerError_ReturnsServerErrorView()
        {

            var result = _controller.ServerError() as ViewResult;

            Assert.IsNotNull(result);
            Assert.That(result.ViewName, Is.EqualTo("ServerError"));
        }

    }

        
}
