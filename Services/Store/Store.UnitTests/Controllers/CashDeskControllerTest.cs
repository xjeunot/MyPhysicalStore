using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XJeunot.PhysicalStoreApps.Services.Store.API.Controllers;
using XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;
using Xunit;
using static MongoDB.Driver.DeleteResult;

namespace XJeunot.PhysicalStoreApps.Services.Store.UnitTests.Controllers
{
    public class CashDeskControllerTest
    {
        private readonly Mock<ICashDeskServices> _cashDeskServicesMock;
        private readonly Mock<ICashDeskFlowValid> _cashDeskFlowValidMock;

        public CashDeskControllerTest()
        {
            _cashDeskServicesMock = new Mock<ICashDeskServices>();
            _cashDeskFlowValidMock = new Mock<ICashDeskFlowValid>();
        }

        [Fact]
        public void Get_All_Success()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            IEnumerable<CashDeskItem> items = new List<CashDeskItem>() { item };
            _cashDeskServicesMock.Setup(x => x.GetCashDeskList()).Returns(Task.FromResult(items));

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Get().Result;

            // Assert.
            Assert.IsType<OkObjectResult>(actionResult);
            OkObjectResult actionResultType = actionResult as OkObjectResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal( ((List<CashDeskItem>)actionResultType.Value) , ((List<CashDeskItem>)items.ToList()) );
        }

        [Fact]
        public void Get_Id_IdSpace()
        {
            // Arrange
            string fakeId = "       ";
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
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
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
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
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Get(fakeId).Result;

            // Assert.
            Assert.IsType<OkObjectResult>(actionResult);
            OkObjectResult actionResultType = actionResult as OkObjectResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CashDeskItem)actionResultType.Value), item);
        }

        [Fact]
        public void Post_ObjectNull()
        {
            // Arrange

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
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
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
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
            CashDeskItem item = new CashDeskItem()
            {
                Id = "",
                Name = "Caisse_01"
            };
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Create, It.IsAny<CashDeskItem>(), null))
                .Returns(false)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Post(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Post_Success()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "",
                Name = "Caisse_01"
            };
            _cashDeskServicesMock.Setup(x => x.AddCashDesk(item))
                .Verifiable();
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Create, It.IsAny<CashDeskItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Post(item);

            // Assert.
            Assert.IsType<CreatedResult>(actionResult);
            CreatedResult actionResultType = actionResult as CreatedResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.Created);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Put_ObjectNull()
        {
            // Arrange

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
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
            CashDeskItem item = new CashDeskItem()
            {
                Id = "",
                Name = "Caisse_01"
            };

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
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
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));
            _cashDeskServicesMock.Setup(x => x.UpdateCashDesk(item.Id, item))
                .ReturnsAsync(false)
                .Verifiable();
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CashDeskItem>(), It.IsAny<CashDeskItem>()))
                .Returns(true)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Put_FlowValidError()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CashDeskItem>(), It.IsAny<CashDeskItem>()))
                .Returns(false)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Put_Success()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));
            _cashDeskServicesMock.Setup(x => x.UpdateCashDesk(item.Id, item))
                .ReturnsAsync(true)
                .Verifiable();
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Update, It.IsAny<CashDeskItem>(), It.IsAny<CashDeskItem>()))
                .Returns(true)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Put(item);

            // Assert.
            Assert.IsType<AcceptedResult>(actionResult);
            AcceptedResult actionResultType = actionResult as AcceptedResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.Accepted);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_IdNull()
        {
            // Arrange

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(null);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _cashDeskServicesMock.Verify();
        }

        [Fact]
        public void Delete_IdEmpty()
        {
            // Arrange

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(string.Empty);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _cashDeskServicesMock.Verify();
        }

        [Fact]
        public void Delete_IsAcknowledgedFail()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Unacknowledged deleteResult = Unacknowledged.Instance;
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));
            _cashDeskServicesMock.Setup(x => x.DeleteCashDesk(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CashDeskItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_CountFail()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Acknowledged deleteResult = new Acknowledged(0);
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));
            _cashDeskServicesMock.Setup(x => x.DeleteCashDesk(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CashDeskItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NotFoundResult>(actionResult);
            NotFoundResult actionResultType = actionResult as NotFoundResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NotFound);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_FlowValidError()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Acknowledged deleteResult = new Acknowledged(1);
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CashDeskItem>(), null))
                .Returns(false)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<BadRequestResult>(actionResult);
            BadRequestResult actionResultType = actionResult as BadRequestResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }

        [Fact]
        public void Delete_Success()
        {
            // Arrange
            CashDeskItem item = new CashDeskItem()
            {
                Id = "5c012842f8e2708cf041e247",
                Name = "Caisse_01"
            };
            Acknowledged deleteResult = new Acknowledged(1);
            _cashDeskServicesMock.Setup(x => x.GetCashDesk(item.Id)).Returns(Task.FromResult(item));
            _cashDeskServicesMock.Setup(x => x.DeleteCashDesk(item.Id))
                .ReturnsAsync(deleteResult)
                .Verifiable();
            _cashDeskFlowValidMock
                .Setup(x => x.IsValidOperation(BaseValidatorType.Delete, It.IsAny<CashDeskItem>(), null))
                .Returns(true)
                .Verifiable();

            // Act
            CashDeskController controller = new CashDeskController(_cashDeskServicesMock.Object, _cashDeskFlowValidMock.Object);
            ActionResult actionResult = controller.Delete(item.Id);

            // Assert.
            Assert.IsType<NoContentResult>(actionResult);
            NoContentResult actionResultType = actionResult as NoContentResult;
            Assert.Equal(actionResultType.StatusCode, (int)System.Net.HttpStatusCode.NoContent);
            _cashDeskServicesMock.Verify();
            _cashDeskFlowValidMock.Verify();
        }
    }
}
