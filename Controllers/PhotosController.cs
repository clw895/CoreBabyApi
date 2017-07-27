using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;

namespace CoreBabyApi.Controllers
{
    [Route("api/[controller]")]
    public class PhotosController:Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var message = new Message();
            message.Body("Enjoy the photo. I hope it makes you smile");

            var urlTask =  await GetPhotoUrl();
            message.Media(urlTask);

            var response = new MessagingResponse();
            response.Message(message);

            return Content(response.ToString(), "application/xml");
        }
        private async Task<string> GetPhotoUrl()
        {
            var backupUrl = @"https://cdn.pixabay.com/photo/2016/04/01/09/26/emote-1299362_1280.png";

            // GET DROPBOX FOLDERS
            var photoUrl = string.Empty;
            try
            {
                // CREATE DROPBOX HELPER
                var helper = new DropboxHelper();

                // GET ALL PHOTO PATHS
                var filePaths = await helper.GetFilePaths();

                // SELECT RANDOM FILE
                var index = new Random().Next(filePaths.Count);

                // GET PUBLICLY ACCESSIBLE URI
                photoUrl = await helper.GetFileUri(filePaths[index]);
             }
            catch
            {
                // If anything fails, send a sad face
                photoUrl = backupUrl;
            }

            return photoUrl;
        }
    }
}