using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using Hadis.Models;
using Hadis.Models.DBModels;

namespace Hadis.Controllers
{
    public class ChatUsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ChatUsers
        public object GetApplicationUsers(string ownUsername)
        {
            ChatModels chatModels = new ChatModels(db);
            return chatModels.GetNewChatUsers(ownUsername).Select(u => new { UserId = u.Id, UserName = u.UserName }).ToList();
        }

        // POST: api/ChatUsers
        public IHttpActionResult PostChatUsers(string username, string token)
        {
            //fWkFWfF4GyE
            ChatModels chatModels = new ChatModels(db);
            chatModels.UpdateUserFirebaseToken(username, token);
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
    }
}