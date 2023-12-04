using CustomExternalListWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CustomExternalListWebApi.Controllers
{
    [Route("api/contactsAnonymous")]
    public class ContactsAnonymousController : Controller
    {
        private readonly ContactsService contactsService;

        public ContactsAnonymousController(ContactsService contactsService)
        {
            this.contactsService = contactsService;
        }

        public IActionResult Get()
        {
            return Json(this.contactsService.GetAllContacts());
        }

        [HttpPost]
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
