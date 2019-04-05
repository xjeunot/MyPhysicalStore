using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Abstractions;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Controllers;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation;
using XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation.Impl;
using XJeunot.PhysicalStoreApps.Services.Customer.API.IntegrationEvents.Events;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Model;
using Xunit;
using static MongoDB.Driver.DeleteResult;

namespace XJeunot.PhysicalStoreApps.Services.Customer.UnitTests.Controllers
{
    public class CustomerControllerTest
    {
        private readonly Mock<ICustomerServices> _customerServicesMock;
        private readonly Mock<ICustomerFlowValid> _customerFlowValidMock;

        public CustomerControllerTest()
        {
            _customerServicesMock = new Mock<ICustomerServices>();
            _customerFlowValidMock = new Mock<ICustomerFlowValid>();
        }

        [Fact]
        public void Get_All_Success()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            IEnumerable<CustomerItem> items = new List<CustomerItem>() { item };
            _customerServicesMock.Setup(x => x.GetCustomerList()).Returns(Task.FromResult(items));

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Get().Result;

            // Assert.
            Assert.IsType<OkObjectResult>(actionResult);
            OkObjectResult actionResultType = actionResult as OkObjectResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((List<CustomerItem>)actionResultType.Value), ((List<CustomerItem>)items.ToList()));
        }

        [Fact]
        public void Get_Id_IdSpace()
        {
            // Arrange
            string fakeId = "       ";
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
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
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
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
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Get(fakeId).Result;

            // Assert.
            Assert.IsType<OkObjectResult>(actionResult);
            OkObjectResult actionResultType = actionResult as OkObjectResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CustomerItem)actionResultType.Value), item);
        }

        [Fact]
        public void Post_ObjectNull()
        {
            // Arrange

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
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
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
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
            CustomerItem item = new CustomerItem()
            {
                Id = "",
                Name = "Caisse_01"
            };
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Create, It.IsAny<CustomerItem>(), null))
                .Returns(false)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Post(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Post_Success()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "",
                Name = "Caisse_01"
            };
            _customerServicesMock.Setup(x => x.AddCustomer(item))
                .Verifiable();
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Create, It.IsAny<CustomerItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Post(item);

            // Assert.
            Assert.IsType<CreatedResult>(actionResult);
            CreatedResult actionResultType = actionResult as CreatedResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.Created);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Put_ObjectNull()
        {
            // Arrange

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
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
            CustomerItem item = new CustomerItem()
            {
                Id = "",
                Name = "Caisse_01"
            };

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
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
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));
            _customerServicesMock.Setup(x => x.UpdateCustomer(item.Id, item))
                .ReturnsAsync(false)
                .Verifiable();
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CustomerItem>(), It.IsAny<CustomerItem>()))
                .Returns(true)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Put_FlowValidError()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CustomerItem>(), It.IsAny<CustomerItem>()))
                .Returns(false)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Put_Success()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));
            _customerServicesMock.Setup(x => x.UpdateCustomer(item.Id, item))
                .ReturnsAsync(true)
                .Verifiable();
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CustomerItem>(), It.IsAny<CustomerItem>()))
                .Returns(true)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<AcceptedResult>(actionResult);
            AcceptedResult actionResultType = actionResult as AcceptedResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.Accepted);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_IdNull()
        {
            // Arrange

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(null);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _customerServicesMock.Verify();
        }

        [Fact]
        public void Delete_IdEmpty()
        {
            // Arrange

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(string.Empty);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _customerServicesMock.Verify();
        }

        [Fact]
        public void Delete_IsAcknowledgedFail()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Unacknowledged deleteResult = Unacknowledged.Instance;
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));
            _customerServicesMock.Setup(x => x.DeleteCustomer(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CustomerItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_CountFail()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Acknowledged deleteResult = new Acknowledged(0);
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));
            _customerServicesMock.Setup(x => x.DeleteCustomer(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CustomerItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_FlowValidError()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Acknowledged deleteResult = new Acknowledged(1);
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CustomerItem>(), null))
                .Returns(false)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_Success()
        {
            // Arrange
            CustomerItem item = new CustomerItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Acknowledged deleteResult = new Acknowledged(1);
            _customerServicesMock.Setup(x => x.GetCustomer(item.Id)).Returns(Task.FromResult(item));
            _customerServicesMock.Setup(x => x.DeleteCustomer(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _customerFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CustomerItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CustomerController controller = new CustomerController(_customerServicesMock.Object, _customerFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NoContentResult>(actionResult);
            NoContentResult actionResultType = actionResult as NoContentResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NoContent);
            _customerServicesMock.Verify();
            _customerFlowValidMock.Verify();
        }
    }
}
