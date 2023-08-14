using Microsoft.AspNetCore.Mvc;
using ContactApi.Data;
using ContactApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactApi.Controllers
{
    [ApiController] //API controller anotation
    [Route("api/[controller]")] //for routing
    public class ContactsController : Controller
    {
        private readonly ContactAPIDbContext dbContext; //use this to talk with the in memory database
        public ContactsController(ContactAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            return Ok(await dbContext.Contacts.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetContact([FromRoute] Guid id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);

            if(contact!=null)
            {
                return Ok(contact);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(AddContactRequest addContactRequest) //since we use async IActionResult should be made inside the task
        {
            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                Address = addContactRequest.Address,
                Email = addContactRequest.Email,
                Phone = addContactRequest.Phone,
                FullName = addContactRequest.FullName,
            };

            await dbContext.Contacts.AddAsync(contact);
            await dbContext.SaveChangesAsync();

            return Ok(contact);
        }

        [HttpPut]
        [Route("{id:guid}")] //since we should update we should be having a separate routing url for updation 

        public async Task<IActionResult> UpdateContact([FromRoute] Guid id, UpdateContactRequest updateContactRequest) //since in Route we defined id so the name of id only should be mentioned here
        {
            //first check for the value present in database or not
            var contact = await dbContext.Contacts.FindAsync(id);

            if(contact!=null) //for checking exists or not
            {
                contact.FullName = updateContactRequest.FullName;
                contact.Address = updateContactRequest.Address;
                contact.Phone = updateContactRequest.Phone;
                contact.Email = updateContactRequest.Email;

                await dbContext.SaveChangesAsync();

                return Ok(contact); // for returning IActionResult we need OK function to be called
            }

            return NotFound();
    
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteContact([FromRoute] Guid id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);
            if(contact!=null)
            {
                dbContext.Remove(contact);
                await dbContext.SaveChangesAsync();
                return Ok("Contact Deleted");
            }

            return NotFound();

        }
    }

}
