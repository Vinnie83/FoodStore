using FoodStore.Controllers;
using FoodStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests
{
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> loggerMock;
        private HomeController controller;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<HomeController>>();
            controller = new HomeController(loggerMock.Object);
        }

        [TearDown] public void Teardown()
        {
            controller.Dispose();
        }

        [Test]
        public void Index_ReturnsViewResult()
        {

            var result = controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Privacy_ReturnsViewResult()
        {
            
            var result = controller.Privacy();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Error_ReturnsViewResultWithErrorViewModel_UsingActivityId()
        {

            var activity = new Activity("TestActivity");
            activity.Start(); 
            Activity.Current = activity;

            var result = controller.Error();

            activity.Stop(); 


            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.That(model.RequestId, Is.EqualTo(activity.Id));
        }

        [Test]
        public void Error_ReturnsViewResultWithErrorViewModel_UsingTraceIdentifier()
        {
          
            Activity.Current = null;

            var context = new DefaultHttpContext();
            context.TraceIdentifier = "trace-456";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

        
            var result = controller.Error();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.That(model.RequestId, Is.EqualTo("trace-456"));
        }
    }
}
