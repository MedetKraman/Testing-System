using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hadis.Models;
using Hadis.Models.DBModels;
using Hadis.Providers;

namespace Hadis.Controllers
{
    [Authorize]
    public class ChatsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Chats
        public List<UserChat> GetChats(string username)
        {
            ChatModels chatModels = new ChatModels(db);
            var result = chatModels.GetUserChats(username);
            return result;
        }
        
        // POST: api/Chats
        public IHttpActionResult PostChat(string username, string clienUsername, string message)
        {
            ChatModels chatModels = new ChatModels(db);
            DateTime dateTime = DateTime.Now;
            int chatId = chatModels.AddChat(username, clienUsername, dateTime, message);

            SendNotificationWithFirebaseProvider provider = new SendNotificationWithFirebaseProvider();
            string deviceId = db.Users.Where(u => u.UserName == clienUsername).FirstOrDefault().FirebaseDeviceToken;
            if (deviceId != null && deviceId.Length > 0)
                provider.SendPush(new PushMessage
                {
                    to = deviceId,
                    notification = new PushMessageData
                    {
                        title = "Hadis diplome project",
                        text = "Новое сообщение"
                    },
                    data = new
                    {
                        chatId = chatId + ""
                    }
                });

            return Ok();
        }

        // DELETE: api/Chats/5
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> DeleteChat(int id)
        {
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            db.Chats.Remove(chat);
            await db.SaveChangesAsync();

            return Ok(chat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChatExists(int id)
        {
            return db.Chats.Count(e => e.Id == id) > 0;
        }
    }
}