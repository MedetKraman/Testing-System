using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Reflection;
using System.Web;

namespace Hadis.Models.DBModels
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("HadisConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<TestThemaVersion> TestThemaVersions { get; set; }
        public DbSet<TestCategoryTestThema> TestCategoryTestThemas { get; set; }
        public DbSet<TestThema> TestThemas { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<TestAnswer> TestAnswers { get; set; }

        public DbSet<ClientTestHistory> ClientTestHistories { get; set; }
        public DbSet<ClientTestQuestion> ClientTestQuestions { get; set; }
        public DbSet<ClientSelectedAnswer> ClientSelectedAnswers { get; set; }

        public DbSet<ClientCallBack> ClientCallBacks { get; set; }
        public DbSet<CallBackMessage> CallBackMessages { get; set; }


        /* Chat */
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Video> Videos { get; set; }
    }


    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Здесь добавьте настраиваемые утверждения пользователя
            return userIdentity;
        }

        public string FirebaseDeviceToken { get; set; }

        public virtual Client Client { get; set; }
        
        public ICollection<ChatUser> ChatUsers { get; set; }
    }

    public class Client
    {

        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }
        public virtual ApplicationUser User { get; set; }

        public ICollection<ClientTestHistory> ClientTestHistories { get; set; }
        public ICollection<ClientCallBack> ClientCallBacks { get; set; }
    }

    public class TestCategory
    {
        public TestCategory()
        {
            TestCategoryTestThemas = new HashSet<TestCategoryTestThema>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Категория")]
        public string Category { get; set; }

        public ICollection<TestCategoryTestThema> TestCategoryTestThemas { get; set; }
    }

    public class TestCategoryTestThema
    {
        public int Id { get; set; }

        [Required]
        public int TestCategoryId { get; set; }
        public virtual TestCategory TestCategory { get; set; }

        [Required]
        public int TestThemaId { get; set; }
        public virtual TestThema TestThema { get; set; }
    }

    public class TestThema
    {
        public TestThema()
        {
            TestQuestions = new HashSet<TestQuestion>();
            TestCategoryTestThemas = new HashSet<TestCategoryTestThema>();
            ClientTestHistories = new HashSet<ClientTestHistory>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Называние")]
        public string Thema { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Дата совтавление")]
        public DateTime CreatedDateTime { get; set; }
        [Required]
        [Display(Name = "Актуальность")]
        public bool IsActual { get; set; }
        [Required]
        [Display(Name = "Общее количество очков")]
        public double TotalPoint { get; set; }

        [InverseProperty("TestThema")]
        public virtual TestThemaVersion TestThemaVersion { get; set; }

        [InverseProperty("ParentTestThema")]
        public ICollection<TestThemaVersion> ChildrenVersions { get; set; }
        public ICollection<TestQuestion> TestQuestions { get; set; }
        public ICollection<TestCategoryTestThema> TestCategoryTestThemas { get; set; }
        public ICollection<ClientTestHistory> ClientTestHistories { get; set; }
    }
    public class TestThemaVersion
    {
        [Key]
        [ForeignKey("TestThema")]
        public int Id { get; set; }
        [InverseProperty("TestThemaVersion")]
        public virtual TestThema TestThema { get; set; }

        public int? ParentTestThemaId { get; set; }
        [InverseProperty("ChildrenVersions")]
        public virtual TestThema ParentTestThema { get; set; }

        public string Description { get; set; }
    }


    public class TestQuestion
    {
        public TestQuestion()
        {
            TestAnswers = new HashSet<TestAnswer>();
            ClientTestQuestions = new HashSet<ClientTestQuestion>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Вопрос")]
        public string Question { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Весь вопроса")]
        public double ShareWeight { get; set; }

        [Required]
        public int TestThemaId { get; set; }
        public virtual TestThema TestThema { get; set; }

        public ICollection<TestAnswer> TestAnswers { get; set; }
        public ICollection<ClientTestQuestion> ClientTestQuestions { get; set; }
    }

    public class TestAnswer
    {
        public TestAnswer()
        {
            ClientSelectedAnswers = new HashSet<ClientSelectedAnswer>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Ответ")]
        public string Answer { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Весь ответа")]
        public double ShareWeight { get; set; }
        [Required]
        [Display(Name = "Правильный ответ")]
        public bool IsCurrect { get; set; }

        [Required]
        public int TestQuestionId { get; set; }
        public virtual TestQuestion TestQuestion { get; set; }

        public ICollection<ClientSelectedAnswer> ClientSelectedAnswers { get; set; }
    }


    public class ClientTestHistory
    {
        public ClientTestHistory()
        {
            ClientTestQuestions = new HashSet<ClientTestQuestion>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Дата тестирование")]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Набранное количество очков")]
        public double Point { get; set; }
        [Required]
        [Display(Name = "Общее количество очков")]
        public double TotalPoint { get; set; }
        [Display(Name = "Комментраии")]
        public string Comment { get; set; }

        [Required]
        public int TestThemaId { get; set; }
        public virtual TestThema TestThema { get; set; }

        [Required]
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }

        public ICollection<ClientTestQuestion> ClientTestQuestions { get; set; }
    }

    public class ClientTestQuestion
    {
        public ClientTestQuestion()
        {
            ClientSelectedAnswers = new HashSet<ClientSelectedAnswer>();
        }

        public int Id { get; set; }
        [Display(Name = "Комментраии")]
        public string Comment { get; set; }

        [Required]
        public int ClientTestHistoryId { get; set; }
        public virtual ClientTestHistory ClientTestHistory { get; set; }

        [Required]
        public int TestQuestionId { get; set; }
        public virtual TestQuestion TestQuestion { get; set; }

        public ICollection<ClientSelectedAnswer> ClientSelectedAnswers { get; set; }
    }

    public class ClientSelectedAnswer
    {
        public int Id { get; set; }
        [Display(Name = "Комментраии")]
        public string Comment { get; set; }

        [Required]
        public int ClientTestQuestionId { get; set; }
        public virtual ClientTestQuestion ClientTestQuestion { get; set; }

        [Required]
        public int TestAnswerId { get; set; }
        public virtual TestAnswer TestAnswer { get; set; }
    }


    public class ClientCallBack
    {
        public ClientCallBack()
        {
            CallBackMessages = new HashSet<CallBackMessage>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Дата и время")]
        public DateTime DateTime { get; set; }
        [Required]
        [Display(Name = "Тема")]
        public string Thema { get; set; }
        [Required]
        [Display(Name = "Статус")]
        public bool IsThemaClosed { get; set; }
        [Display(Name = "Оценка удовлетворенности ответом")]
        public SatisfactionScores SatisfactionScore { get; set; }

        [Required]
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }

        public ICollection<CallBackMessage> CallBackMessages { get; set; }
    }
    public enum SatisfactionScores
    {
        [Display(Name = "Не удовлетворены")]
        NotSatisfied,
        [Display(Name = "Не очень")]
        NotReally,
        [Display(Name = "Нормально")]
        Normally,
        [Display(Name = "Получил ответ")]
        GotAnAnswer,
        [Display(Name = "Довольный")]
        Satisfied,
        [Display(Name = "Не ответили")]
        DidNotAnswer
    }

    public class CallBackMessage
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Дата и время")]
        public DateTime DateTime { get; set; }
        [Required]
        [Display(Name = "Сообщение")]
        public string Message { get; set; }

        [Required]
        public string UserId { get; set; }
        [Display(Name = "От ")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int ClientCallBackId { get; set; }
        public virtual ClientCallBack ClientCallBack { get; set; }
    }

    public static class EnumHelperCustom
    {
        public static string GetDisplayName(this Enum value)
        {
            return value.GetType().GetMember(value.ToString()).First()
                .GetCustomAttributes<DisplayAttribute>().First().GetName();
        }
    }


    /* Chat models */
    public class ChatUser
    {
        public ChatUser()
        {
            ChatMessages = new HashSet<ChatMessage>();
        }

        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
    public class Chat
    {
        public Chat()
        {
            ChatUsers = new HashSet<ChatUser>();
            ChatMessages = new HashSet<ChatMessage>();
        }

        public int Id { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
    public class ChatMessage
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }

        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        public int ChatUserId { get; set; }
        public virtual ChatUser ChatUser { get; set; }
    }

    public class Video
    {
        public int Id { get; set; }
        public string Uploader { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
    }
}