﻿using CustomExternalListWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Threading.Tasks;

namespace CustomExternalListWebApi.Controllers
{
    [Route("api/contacts")]
    public class ContactsController : Controller
    {
        private readonly ContactsService contactsService;

        public ContactsController(ContactsService contactsService)
        {
            this.contactsService = contactsService;
        }

        public IActionResult Get()
        {
            return Json(this.contactsService.GetAllContacts());
        }

        [Authorize]
        [HttpPost]
        [RequiredScope("access_as_user")]
        public async Task<IActionResult> Post()
        {
            try
            {
                var entities = await this.contactsService.HandleCustomExternalListRequest(Request.Body);
                return Json(entities);
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(ex);
            }
            
        }
    }
}
