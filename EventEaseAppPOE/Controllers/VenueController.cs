using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseAppPOE.Data;
using EventEaseAppPOE.Models;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using Humanizer.Localisation;

namespace EventEaseAppPOE.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventeaseDbContext dbContext;

        //integrate blob storage
        private readonly BlobContainerClient containerClient;
        private readonly string containerName;

        public VenueController(EventeaseDbContext dbContext, IConfiguration config)
        {
            this.dbContext = dbContext;

            string connectionString = config["AzureBlobStorage:ConnectionString"]
                ?? throw new ArgumentNullException("AzureBlobStorage:ConnectionString");

            containerName = config["AzureBlobStorage:ContainerName"] ?? "venue-images";

            containerClient = new BlobContainerClient(connectionString, containerName);

            // Create the container if it doesn't exist 
            containerClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }


        //setup image upload method
        private async Task<string> UploadImageToBlob(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            //creates the contaner if it does not exist yet
            //var containerClient = GetContainerClient();
            //await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            //assigns the image a unique name and id
            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(blobName);

            //sends image to conatiner using blob client
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            // Save the image URL to the database
            //string imageUrl = blobClient.Uri.ToString();
            return blobClient.Uri.ToString();
        }

        //setup shared access signature token 
        //private string GenerateBlobSasUrl(string blobName)
        //{
        //    var blobUri = new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}/{blobName}");
        //    var sasBuilder = new BlobSasBuilder
        //    {
        //        BlobContainerName = containerName,
        //        BlobName = blobName,
        //        Resource = "b",
        //        ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
        //    };
        //    sasBuilder.SetPermissions(BlobSasPermissions.Read);

        //    var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(storageAccountName, storageAccountKey)).ToString;
        //    return $"{blobUri}?{sasToken}";
        //}


        // GET: Venue
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bookings = await dbContext.Bookings.ToListAsync();
            var venuesWithUrls = new List<Venue>();

            return View(await dbContext.Venues.ToListAsync());
        }

        // GET: Venue/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await dbContext.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity")] Venue venue, IFormFile VenueImage)
        {
            if (ModelState.IsValid)
            {
                // Check if an image was uploaded
                if (VenueImage != null && VenueImage.Length > 0)
                {
                    // Uploads the image to Azure Blob or local folder
                    var imageUrl = await UploadImageToBlob(VenueImage); // Or UploadImageToLocal(VenueImage)
                    venue.VenueImage = imageUrl;
                }

                // Saves the venue to database
                dbContext.Add(venue);
                await dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await dbContext.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,VenueImage")] Venue venue, IFormFile VenueImage)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (VenueImage != null && VenueImage.Length > 0)
                    {
                        var imageUrl = await UploadImageToBlob(VenueImage);
                        venue.VenueImage = imageUrl;
                    }

                    dbContext.Update(venue);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(venue);
        }

        // GET: Venue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await dbContext.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await dbContext.Venues.FindAsync(id);
            if (venue != null)
            {
                dbContext.Venues.Remove(venue);
            }

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return dbContext.Venues.Any(e => e.VenueId == id);
        }
    }
}