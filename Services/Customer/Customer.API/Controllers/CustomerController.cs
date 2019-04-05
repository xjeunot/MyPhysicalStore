using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation;
using XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation.Impl;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _iCustomerServices;
        private readonly ICustomerFlowValid _iFlowValid;

        public CustomerController(ICustomerServices iCustomerServices,
            ICustomerFlowValid iFlowValid)
        {
            _iCustomerServices = iCustomerServices;
            _iFlowValid = iFlowValid;
        }

        // GET: api/v1/Customer
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CustomerItem>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<IEnumerable<CustomerItem>> Get()
        {
            IEnumerable<CustomerItem> customerItems = _iCustomerServices.GetCustomerList().Result;
            List<CustomerItem> customerItemsId = customerItems.ToList();
            return Ok(customerItemsId);
        }

        // GET: api/v1/Customer/5
        [HttpGet("{id}", Name = "GetCustomer")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CustomerItem))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<CustomerItem> Get(string id)
        {
            // Validation.
            if ((id == null) ||
                (id.Trim() == string.Empty)) return BadRequest();

            // Search Item.
            CustomerItem customerItem = _iCustomerServices.GetCustomer(id).Result;
            if (customerItem == null) return NotFound();

            // Return.
            return Ok(customerItem);
        }

        // POST: api/v1/Customer
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult Post([FromBody] CustomerItem value)
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
            _iCustomerServices.AddCustomer(value);

            // Return.
            return Created("GetCustomer", value);
        }

        // PUT: api/v1/Customer/
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult Put([FromBody] CustomerItem value)
        {
            // Validation.
            if ((value == null) ||
                (value.Id == string.Empty) ||
                (ModelState.IsValid == false))
                return BadRequest();

            // Object Flow Validation.
            CustomerItem customerItem = _iCustomerServices.GetCustomer(value.Id).Result;
            if (customerItem == null) return NotFound();
            bool isObjectFlowValid = _iFlowValid.IsValidOperation(BaseValidatorType.Update, customerItem, value);
            if (!isObjectFlowValid) return BadRequest();

            // Update.
            bool blnResultUpdate = _iCustomerServices.UpdateCustomer(value.Id, value).Result;

            // Return with case.
            if (blnResultUpdate)
                return Accepted();
            else
                return NotFound();
        }

        // DELETE: api/v1/Customer/5
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
            CustomerItem customerItem = _iCustomerServices.GetCustomer(id).Result;
            if (customerItem == null) return NotFound();
            bool isObjectFlowValid = _iFlowValid.IsValidOperation(BaseValidatorType.Delete, customerItem, null);
            if (!isObjectFlowValid) return BadRequest();

            // Delete.
            MongoDB.Driver.DeleteResult clsDeleteResult = _iCustomerServices.DeleteCustomer(id).Result;

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
