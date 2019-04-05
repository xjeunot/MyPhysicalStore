using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Abstractions;
using XJeunot.PhysicalStoreApps.Services.Store.API.Controllers;
using XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl;
using XJeunot.PhysicalStoreApps.Services.Store.API.IntegrationEvents.Events;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;
using Xunit;
using static MongoDB.Driver.DeleteResult;

namespace XJeunot.PhysicalStoreApps.Services.Store.UnitTests.Controllers
{
    public class CheckOutControllerTest
    {
        private readonly Mock<ICheckOutServices> _checkOutServicesMock;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<ICheckOutFlowValid> _checkOutFlowValidMock;

        public CheckOutControllerTest()
        {
            _checkOutServicesMock = new Mock<ICheckOutServices>();
            _eventBusMock = new Mock<IEventBus>();
            _checkOutFlowValidMock = new Mock<ICheckOutFlowValid>();
        }

        [Fact]
        public void Get_All_Success()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            IEnumerable<CheckOutItem> items = new List<CheckOutItem>() { item };
            _checkOutServicesMock.Setup(x => x.GetCheckOutList()).Returns(Task.FromResult(items));

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Get().Result;

            // Assert.
            Assert.IsType<OkObjectResult>(actionResult);
            OkObjectResult actionResultType = actionResult as OkObjectResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((List<CheckOutItem>)actionResultType.Value), ((List<CheckOutItem>)items.ToList()));
        }

        [Fact]
        public void Get_Id_IdSpace()
        {
            // Arrange
            string fakeId = "       ";
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Get(fakeId).Result;

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Get_Id_IdError()
        {
            // Arrange
            string fakeId = "this_is_not_a_id";
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Get(fakeId).Result;

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public void Get_Id_Success()
        {
            // Arrange
            string fakeId = "5c012842f8e2708cf041e247";
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Get(fakeId).Result;

            // Assert.
            Assert.IsType<OkObjectResult>(actionResult);
            OkObjectResult actionResultType = actionResult as OkObjectResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CheckOutItem)actionResultType.Value), item);
        }

        [Fact]
        public void Post_ObjectNull()
        {
            // Arrange

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Post(null);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Post_ObjectIdNotEmpty()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Post(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Post_FlowValidError()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = ""
            };
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Create, It.IsAny<CheckOutItem>(), null))
                .Returns(false)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Post(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
            _checkOutFlowValidMock.Verify();
        }

        [Fact]
        public void Post_Success()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = ""
            };
            _checkOutServicesMock.Setup(x => x.AddCheckOut(item))
                .Verifiable();
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Create, It.IsAny<CheckOutItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Post(item);

            // Assert.
            Assert.IsType<CreatedResult>(actionResult);
            CreatedResult actionResultType = actionResult as CreatedResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.Created);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
            _checkOutFlowValidMock.Verify();
        }

        [Fact]
        public void Put_ObjectNull()
        {
            // Arrange

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Put(null);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Put_ObjectIdEmpty()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = ""
            };

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Put_ObjectNotExist()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));
            _checkOutServicesMock.Setup(x => x.UpdateCheckOut(item.Id, item))
                .ReturnsAsync(false)
                .Verifiable();
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CheckOutItem>(), It.IsAny<CheckOutItem>()))
                .Returns(true)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
        }

        [Fact]
        public void Put_FlowValidError()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CheckOutItem>(), It.IsAny<CheckOutItem>()))
                .Returns(false)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
        }

        [Fact]
        public void Put_Success()
        {
            // Arrange
            CheckOutItem itemFrom = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247",
                CurrentState = CheckOutItem.STATE_PAID
            };
            CheckOutItem itemTo = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247",
                CurrentState = CheckOutItem.STATE_CLOSED
            };
            _checkOutServicesMock.Setup(x => x.GetCheckOut(itemFrom.Id)).Returns(Task.FromResult(itemFrom));
            _checkOutServicesMock.Setup(x => x.UpdateCheckOut(itemTo.Id, itemTo))
                .ReturnsAsync(true)
                .Verifiable();
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CheckOutItem>(), It.IsAny<CheckOutItem>()))
                .Returns(true)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Put(itemTo);

            // Assert.
            Assert.IsType<AcceptedResult>(actionResult);
            AcceptedResult actionResultType = actionResult as AcceptedResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.Accepted);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();

            // Assert Bus.
            _eventBusMock.Verify(x => x.Publish(It.IsAny<CheckOutEvent>()));
        }

        [Fact]
        public void Delete_IdNull()
        {
            // Arrange

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(null);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _checkOutServicesMock.Verify();
        }

        [Fact]
        public void Delete_IdEmpty()
        {
            // Arrange

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(string.Empty);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _checkOutServicesMock.Verify();
        }

        [Fact]
        public void Delete_IsAcknowledgedFail()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            Unacknowledged deleteResult = Unacknowledged.Instance;
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));
            _checkOutServicesMock.Setup(x => x.DeleteCheckOut(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CheckOutItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_CountFail()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            Acknowledged deleteResult = new Acknowledged(0);
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));
            _checkOutServicesMock.Setup(x => x.DeleteCheckOut(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CheckOutItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_FlowValidError()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            Acknowledged deleteResult = new Acknowledged(1);
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CheckOutItem>(), null))
                .Returns(false)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_Success()
        {
            // Arrange
            CheckOutItem item = new CheckOutItem()
            {
                Id = "5c012842f8e2708cf041e247"
            };
            Acknowledged deleteResult = new Acknowledged(1);
            _checkOutServicesMock.Setup(x => x.GetCheckOut(item.Id)).Returns(Task.FromResult(item));
            _checkOutServicesMock.Setup(x => x.DeleteCheckOut(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _checkOutFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CheckOutItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CheckOutController controller = new CheckOutController(_checkOutServicesMock.Object, _eventBusMock.Object, _checkOutFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NoContentResult>(actionResult);
            NoContentResult actionResultType = actionResult as NoContentResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NoContent);
            _checkOutServicesMock.Verify();
            _checkOutFlowValidMock.Verify();
        }
    }
}
