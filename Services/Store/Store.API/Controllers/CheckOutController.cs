using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Abstractions;
using XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl;
using XJeunot.PhysicalStoreApps.Services.Store.API.IntegrationEvents.Events;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckOutServices _iCheckOutServices;
        private readonly IEventBus _iEventBus;
        private readonly ICheckOutFlowValid _iFlowValid;

        public CheckOutController(ICheckOutServices iCheckOutServices,
            IEventBus iEventBus, ICheckOutFlowValid iFlowValid)
        {
            _iCheckOutServices = iCheckOutServices;
            _iEventBus = iEventBus;
            _iFlowValid = iFlowValid;
        }

        // GET: api/v1/CheckOut
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CheckOutItem>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<IEnumerable<CheckOutItem>> Get()
        {
            IEnumerable<CheckOutItem> checkOutItems = _iCheckOutServices.GetCheckOutList().Result;
            List<CheckOutItem> checkOutItemsId = checkOutItems.ToList();
            return Ok(checkOutItemsId);
        }

        // GET: api/v1/CheckOut/5
        [HttpGet("{id}", Name = "GetCheckOut")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CheckOutItem))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<CheckOutItem> Get(string id)
        {
            // Validation.
            if ((id == null) ||
                (id.Trim() == string.Empty)) return BadRequest();

            // Search Item.
            CheckOutItem checkOutItem = _iCheckOutServices.GetCheckOut(id).Result;
            if (checkOutItem == null) return NotFound();

            // Return.
            return Ok(checkOutItem);
        }

        // POST: api/v1/CheckOut
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult Post([FromBody] CheckOutItem value)
        {
            // Validation.
            if ((value == null) ||
                (value.Id != string.Empty) ||
                (ModelState.IsValid == false))
                return BadRequest();

            // Object Flow Validation.
            bool isObjectFlowValid = _iFlowValid.IsValidOperation(BaseValidatorType.Create, value, null);
            if (!isObjectFlowValid) return BadRequest();

            // Add.
            _iCheckOutServices.AddCheckOut(value);

            // Event Bus Publish.
            if (value.CurrentState == CheckOutItem.STATE_CLOSED)
            {
                CheckOutEvent checkOutEvent = CheckOutEvent.FromItem(value);
                _iEventBus.Publish(checkOutEvent);
            }

            // Return.
            return Created("GetCheckOut", value);
        }

        // PUT: api/v1/CheckOut/
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult Put([FromBody] CheckOutItem value)
        {
            // Validation.
            if ((value == null) ||
                (value.Id == string.Empty) ||
                (ModelState.IsValid == false))
                return BadRequest();

            // Object Flow Validation.
            CheckOutItem checkOutItem = _iCheckOutServices.GetCheckOut(value.Id).Result;
            if (checkOutItem == null) return NotFound();
            bool isObjectFlowValid = _iFlowValid.IsValidOperation(BaseValidatorType.Update, checkOutItem, value);
            if (!isObjectFlowValid) return BadRequest();

            // Update.
            bool blnResultUpdate = _iCheckOutServices.UpdateCheckOut(value.Id, value).Result;

            // Return with case.
            if (blnResultUpdate)
            {
                // Event Bus Publish.
                if (value.CurrentState == CheckOutItem.STATE_CLOSED)
                {
                    CheckOutItem checkOutItemUpdated = _iCheckOutServices.GetCheckOut(value.Id).Result;
                    CheckOutEvent checkOutEvent = CheckOutEvent.FromItem(checkOutItemUpdated);
                    _iEventBus.Publish(checkOutEvent);
                }

                return Accepted();
            }
            else
                return NotFound();
        }

        // DELETE: api/v1/CheckOut/5
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult Delete(string id)
        {
            // Validation.
            if ((id == null) ||
                (id.Trim() == string.Empty)) return BadRequest();

            // Object Flow Validation.
            CheckOutItem checkOutItem = _iCheckOutServices.GetCheckOut(id).Result;
            if (checkOutItem == null) return NotFound();
            bool isObjectFlowValid = _iFlowValid.IsValidOperation(BaseValidatorType.Delete, checkOutItem, null);
            if (!isObjectFlowValid) return BadRequest();

            // Delete.
            MongoDB.Driver.DeleteResult clsDeleteResult = _iCheckOutServices.DeleteCheckOut(id).Result;

            // Return with case.
            if ((clsDeleteResult != null) &&
                (clsDeleteResult.IsAcknowledged) &&
                (clsDeleteResult.DeletedCount > 0))
                return NoContent();
            else
                return NotFound();
        }
    }
}
