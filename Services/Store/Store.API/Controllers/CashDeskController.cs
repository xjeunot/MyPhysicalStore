using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation;
using XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl;
using Microsoft.AspNetCore.Authorization;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CashDeskController : ControllerBase
    {
        private readonly ICashDeskServices _iCashDeskServices;
        private readonly ICashDeskFlowValid _iFlowValid;

        public CashDeskController(ICashDeskServices iCashDeskServices,
            ICashDeskFlowValid iFlowValid)
        {
            _iCashDeskServices = iCashDeskServices;
            _iFlowValid = iFlowValid;
        }

        // GET: api/v1/CashDesk
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CashDeskItem>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<IEnumerable<CashDeskItem>> Get()
        {
            IEnumerable<CashDeskItem> cashDeskItems = _iCashDeskServices.GetCashDeskList().Result;
            List<CashDeskItem> cashDeskItemsId = cashDeskItems.ToList();
            return Ok(cashDeskItemsId);
        }

        // GET: api/v1/CashDesk/5
        [HttpGet("{id}", Name = "GetCashDesk")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CashDeskItem))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult<CashDeskItem> Get(string id)
        {
            // Validation.
            if ((id == null) ||
                (id.Trim() == string.Empty)) return BadRequest();

            // Search Item.
            CashDeskItem cashDeskItem = _iCashDeskServices.GetCashDesk(id).Result;
            if (cashDeskItem == null) return NotFound();

            // Return.
            return Ok(cashDeskItem);
        }

        // POST: api/v1/CashDesk
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult Post([FromBody] CashDeskItem value)
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
            _iCashDeskServices.AddCashDesk(value);

            // Return.
            return Created("GetCashDesk", value);
        }

        // PUT: api/v1/CashDesk/
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult Put([FromBody] CashDeskItem value)
        {
            // Validation.
            if ((value == null) ||
                (value.Id == string.Empty) ||
                (ModelState.IsValid == false))
                return BadRequest();

            // Object Flow Validation.
            CashDeskItem cashDeskItem = _iCashDeskServices.GetCashDesk(value.Id).Result;
            if (cashDeskItem == null) return NotFound();
            bool isObjectFlowValid = _iFlowValid.IsValidOperation(BaseValidatorType.Update, cashDeskItem, value);
            if (!isObjectFlowValid) return BadRequest();

            // Update.
            bool blnResultUpdate = _iCashDeskServices.UpdateCashDesk(value.Id, value).Result;

            // Return with case.
            if (blnResultUpdate)
                return Accepted();
            else
                return NotFound();
        }

        // DELETE: api/v1/CashDesk/5
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
            CashDeskItem cashDeskItem = _iCashDeskServices.GetCashDesk(id).Result;
            if (cashDeskItem == null) return NotFound();
            bool isObjectFlowValid = _iFlowValid.IsValidOperation(BaseValidatorType.Delete, cashDeskItem, null);
            if (!isObjectFlowValid) return BadRequest();

            // Delete.
            MongoDB.Driver.DeleteResult clsDeleteResult = _iCashDeskServices.DeleteCashDesk(id).Result;

            // Return with case.
            if ((clsDeleteResult != null)&&
                (clsDeleteResult.IsAcknowledged) &&
                (clsDeleteResult.DeletedCount > 0))
                return NoContent();
            else
                return NotFound();
        }
    }
}
