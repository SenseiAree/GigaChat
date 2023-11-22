using GigaChatDALCrossPlatform;
using GigaChatDALCrossPlatform.Models;
using System.Diagnostics;

namespace GigaChatTestApplication
{
    public class Program
    {

        static GigaChatDBContext DBContext { get; set; }
        static UserRepository UserRepo { get; set; }
        static AdminRepository AdminRepo { get; set; }
        static Program()
        {
            DBContext = new GigaChatDBContext();
            UserRepo = new UserRepository(DBContext);
            AdminRepo = new AdminRepository(DBContext);
        }

        static void Main(string[] args)
        {
            //TestRegister("testuser2@test.com","passsword@123", "Test User 2", new DateTime(2009,10,10));
            //TestValidateCredentials("testuser2@test.com", "passsword@123");
            //TestLogout("testuser2@test.com");
            //TestAddChat(1005,1002);
            //TestGetChats(1005);

            Console.WriteLine(UserRepo.AvgFeedbackRating());

        }
        //static void TestRegister(string emailId, string password, string displayName, DateTime dOB)
        //{
        //    int userId;
        //    var result = UserRepo.RegisterUser(emailId , password, displayName, dOB, out userId);
        //    Console.WriteLine(result+" "+ userId );

        //}
        //static void TestValidateCredentials(string emailId, string password)
        //{
        //    var result = UserRepo.ValidateCredentials(emailId, password);
        //    Console.WriteLine(result);
        //}
        //static void TestLogout(string emailId)
        //{
        //    var result = UserRepo.Logout(emailId);
        //    Console.WriteLine(result);
        //}
        //static void TestAddChat(int initiator, int recipient) 
        //{
        //    var result = UserRepo.AddChat(initiator, recipient);
        //    Console.WriteLine(result);
        //}
        //static void TestGetChats(int userId)
        //{
        //    var result = UserRepo.GetChats(userId);
        //    foreach(var i in result)
        //    {
        //        Console.WriteLine(i.ChatId);
        //    }
        }
        
    }