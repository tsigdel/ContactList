using ContactList.Web.Models;
using ContactList.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ContactList.Web.Controllers
{
    [Authorize] // Apply authorization globally to ensure user is authenticated
    public class ContactsController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        // Index: List all contacts with optional search
        public IActionResult Index(string? search)
        {
            try
            {
                // Get the UserId from the ClaimsPrincipal (user identity)
                var userId = User.FindFirstValue("UserId");
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to Contacts Index.");
                    return RedirectToAction("Login", "Account");
                }

                var contacts = _contactService.GetContacts(int.Parse(userId), search);
                return View(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving contacts.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Create: Show form to create a new contact
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contact contact)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(contact); // Re-display form with validation messages
                }

                // Get the UserId from the ClaimsPrincipal (user identity)
                var userId = User.FindFirstValue("UserId");
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to Create Contact.");
                    return RedirectToAction("Login", "Account");
                }

                contact.UserId = int.Parse(userId);
                _contactService.AddContact(contact);
                _logger.LogInformation("Contact successfully created for UserId: {UserId}", userId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating contact.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Edit: Show form to edit a contact
        public IActionResult Edit(int id)
        {
            try
            {
                // Get the UserId from the ClaimsPrincipal (user identity)
                var userId = User.FindFirstValue("UserId");
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to Edit Contact.");
                    return RedirectToAction("Login", "Account");
                }

                var contact = _contactService.GetContact(id);
                if (contact == null || contact.UserId != int.Parse(userId))
                {
                    _logger.LogWarning("Contact not found or unauthorized access. ContactId: {ContactId}, UserId: {UserId}", id, userId);
                    return NotFound("Contact not found.");
                }

                return View(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving contact for editing.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contact contact)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(contact); // Re-display form with validation messages
                }

                // Get the UserId from the ClaimsPrincipal (user identity)
                var userId = User.FindFirstValue("UserId");
                if (userId == null || contact.UserId != int.Parse(userId))
                {
                    _logger.LogWarning("Unauthorized attempt to Edit Contact. ContactId: {ContactId}, UserId: {UserId}", contact.ContactId, userId);
                    return RedirectToAction("Login", "Account");
                }

                _contactService.UpdateContact(contact);
                _logger.LogInformation("Contact successfully updated. ContactId: {ContactId}, UserId: {UserId}", contact.ContactId, userId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating contact.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Delete: Show confirmation for deleting a contact
        public IActionResult Delete(int id)
        {
            try
            {
                // Get the UserId from the ClaimsPrincipal (user identity)
                var userId = User.FindFirstValue("UserId");
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized access attempt to Delete Contact.");
                    return RedirectToAction("Login", "Account");
                }

                var contact = _contactService.GetContact(id);
                if (contact == null || contact.UserId != int.Parse(userId))
                {
                    _logger.LogWarning("Contact not found or unauthorized access. ContactId: {ContactId}, UserId: {UserId}", id, userId);
                    return NotFound("Contact not found.");
                }

                return View(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving contact for deletion.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Get the UserId from the ClaimsPrincipal (user identity)
                var userId = User.FindFirstValue("UserId");
                if (userId == null)
                {
                    _logger.LogWarning("Unauthorized attempt to confirm deletion of Contact.");
                    return RedirectToAction("Login", "Account");
                }

                var contact = _contactService.GetContact(id);
                if (contact == null || contact.UserId != int.Parse(userId))
                {
                    _logger.LogWarning("Contact not found or unauthorized access for deletion. ContactId: {ContactId}, UserId: {UserId}", id, userId);
                    return NotFound("Contact not found.");
                }

                _contactService.DeleteContact(id);
                _logger.LogInformation("Contact successfully deleted. ContactId: {ContactId}, UserId: {UserId}", id, userId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting contact.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
