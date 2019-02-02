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
    public class ChatMessagesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ChatMessages
        public List<UserChatMessage> GetChatMessages(int chatId)
        {
            ChatModels chatModels = new ChatModels(db);
            return chatModels.GetUserChatMessages(chatId, null);
        }

        // GET: api/ChatMessages
        public List<UserChatMessage> GetChatMessages(int chatId, int chatMessageId)
        {
            ChatModels chatModels = new ChatModels(db);
            var model = chatModels.GetUserChatMessages(chatId, chatMessageId);
            return model;
        }

        // GET: api/ChatMessages/5
        public bool GetChatMessage(int chatId, string username, int year, int month, int day, int hour, int minute, int second)
        {
            ChatModels chatModels = new ChatModels(db);
            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
            
            return chatModels.HadNewMessage(chatId, username, dateTime);
        }

        // POST: api/ChatMessages
        public async Task<IHttpActionResult> PostChatMessage(int chatId, string username, string message)
        {
            ChatModels chatModels = new ChatModels(db);
            DateTime dateTime = DateTime.Now;
            chatModels.AddMessage(chatId, username, message, dateTime);

            await db.SaveChangesAsync();
            SendNotificationWithFirebaseProvider provider = new SendNotificationWithFirebaseProvider();
            string opponentUsername = chatModels.GetOpponentUsername(chatId, username);
            string deviceId = db.Users.Where(u => u.UserName == opponentUsername).FirstOrDefault().FirebaseDeviceToken;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChatMessageExists(int id)
        {
            return db.ChatMessages.Count(e => e.Id == id) > 0;
        }
    }
}