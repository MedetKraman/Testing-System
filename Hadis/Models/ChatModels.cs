using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hadis.Models.DBModels;

namespace Hadis.Models
{
    public class ChatModels
    {
        private ApplicationDbContext db;
        public ChatModels(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool HadUser(string username)
        {
            return db.Users.Where(u => u.UserName == username).Count() > 0;
        }
        public bool HadNewMessage(int chatId, string username, DateTime dateTime)
        {
            return (from cm in db.ChatMessages
                    where cm.ChatId == chatId
                    where cm.DateTime > dateTime
                    where cm.ChatUser.User.UserName != username
                    select cm).Count() > 0;
        }

        public List<UserChat> GetUserChats(string username)
        {
            ApplicationUser user = db.Users.Where(u => u.UserName == username).FirstOrDefault();
            if (user == null) throw new Exception("Пользователь нге найден");

            var userChats = (from cu in db.ChatUsers
                             from cu2 in db.ChatUsers

                             where cu.User.UserName == username
                             where cu2.User.UserName != username
                             where cu.ChatId == cu2.ChatId

                             select new UserChat
                             {
                                 ChatId = cu.ChatId,
                                 ChatName = cu2.User.UserName
                             }).ToList();
            return userChats;
        }
        public List<UserChatMessage> GetUserChatMessages(int chatId, int? chatMessageId)
        {
            if (chatMessageId == null)
            {
                return (from cm in db.ChatMessages
                        where cm.ChatId == chatId
                        orderby cm.DateTime descending
                        select cm).Take(30).OrderBy(u => u.DateTime)
                        .Select(cm => new UserChatMessage
                        {
                            Id = cm.Id,

                            Username = cm.ChatUser.User.UserName,
                            Message = cm.Message,
                            Year = cm.DateTime.Year,
                            Month = cm.DateTime.Month,
                            Day = cm.DateTime.Day,
                            Hour = cm.DateTime.Hour,
                            Minute = cm.DateTime.Minute,
                            Second = cm.DateTime.Second
                        }).ToList();
            }
            return (from cm in db.ChatMessages
                    where cm.ChatId == chatId
                    where cm.Id > chatMessageId
                    orderby cm.DateTime descending
                    select cm).Take(30).OrderBy(u => u.DateTime)
                        .Select(cm => new UserChatMessage
                        {
                            Username = cm.ChatUser.User.UserName,
                            Message = cm.Message,
                            Year = cm.DateTime.Year,
                            Month = cm.DateTime.Month,
                            Day = cm.DateTime.Day,
                            Hour = cm.DateTime.Hour,
                            Minute = cm.DateTime.Minute,
                            Second = cm.DateTime.Second
                        }).ToList();
        }

        public void AddMessage(int chatId, string username, string message, DateTime dateTime)
        {
            int chatUserId = db.ChatUsers.Where(u => u.User.UserName == username).First().Id;
            db.ChatMessages.Add(new ChatMessage
            {
                ChatId = chatId,
                DateTime = dateTime,
                Message = message,
                ChatUserId = chatUserId
            });
            db.SaveChanges();
        }
        public int AddChat(string username, string clienUsername, DateTime dateTime, string message)
        {
            bool hadChat = (from cu in db.ChatUsers
                            from cu1 in db.ChatUsers

                            where cu.User.UserName == username
                            where cu1.User.UserName == clienUsername
                            where cu.ChatId == cu1.ChatId

                            select cu).Count() > 0;
            if (hadChat) return 0;

            ChatUser chatUser = new ChatUser
            {
                UserId = db.Users.Where(u => u.UserName == username).First().Id
            };
            ChatUser chatClientUser = new ChatUser
            {
                UserId = db.Users.Where(u => u.UserName == clienUsername).First().Id
            };
            Chat chat = new Chat
            {
                ChatUsers = new List<ChatUser>
                {
                    chatUser,
                    chatClientUser
                }
            };
            db.Chats.Add(chat);
            db.SaveChanges();

            AddMessage(chat.Id, username, message, dateTime);

            return chat.Id;
        }

        public List<ApplicationUser> GetNewChatUsers(string ownUsername)
        {
            var chats = db.ChatUsers.Where(u => u.User.UserName == ownUsername).Select(u => u.Chat);
            var chatUsers = chats.SelectMany(u => u.ChatUsers);
            var users = chatUsers.Select(u => u.User);
            return db.Users.Where(u => u.UserName != ownUsername)
                .Except(users).ToList();
        }

        public void UpdateUserFirebaseToken(string username, string token)
        {
            var user = db.Users.Where(u => u.UserName == username).FirstOrDefault();
            user.FirebaseDeviceToken = token;
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        public string GetOpponentUsername(int chatId, string username)
        {
            return (from cu in db.ChatUsers
                    where cu.ChatId == chatId
                    where cu.User.UserName != username
                    select cu.User.UserName).First();
        }

    }

    public class UserChat
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
    }

    public class UserChatMessage
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }
}