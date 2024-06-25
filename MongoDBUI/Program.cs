using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;

namespace MongoDBUI
{
    internal class Program
    {
        private static MongoDBDataAccess db;
        private static readonly string tableName = "Contacts";

        static void Main(string[] args)
        {
            db = new MongoDBDataAccess("MongoContactsDB", GetConnectionString());

            var user = new ContactModel
            {
                FirstName = "sue",
                LastName = "storm"
            };
            user.EmailAddresses.Add(new EmailAddressModel{EmailAddress = "sue@gmail.com"});
            user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "me@gmail.com" });
            user.PhoneNumbers.Add(new PhoneNumberModel{PhoneNumber = "55555555"});
            user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "40400404" });

            //CreateContact(user);
            //GetAllContacts();
            //GetContactById("88e3c01c-39dc-463f-8a84-b298be9880ce");

            //UpdateContactsFirstName("timmy", "storm", "d298ba51-c3b6-4635-bcad-c574d88b8a9d");
            //RemovePhoneNumberFromUser("40400404", "d298ba51-c3b6-4635-bcad-c574d88b8a9d");
            //RemoveUser("88e3c01c-39dc-463f-8a84-b298be9880ce");

            GetAllContacts();
            Console.WriteLine("done processing MongoDB");
            Console.ReadLine();
        }
        private static string GetConnectionString(string connectionStringName = "Default")
        {
            string output = "";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            output = config.GetConnectionString(connectionStringName);

            return output;
        }
        private static void CreateContact(ContactModel contact)
        {
            db.UpsertRecord(tableName, contact.Id, contact);
        }
        private static void GetAllContacts()
        {
            var contacts = db.LoadRecords<ContactModel>(tableName);

            foreach (var contact in contacts)
            {
                Console.WriteLine($"{contact.Id}: {contact.FirstName} {contact.LastName}");
            }
        }
        private static void GetContactById(string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);
            Console.WriteLine($"{contact.Id}: {contact.FirstName} {contact.LastName}");
        }
        private static void UpdateContactsFirstName(string firstName, string lastName, string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            contact.FirstName = firstName;
            contact.LastName = lastName;

            db.UpsertRecord(tableName, contact.Id, contact);
        }
        private static void RemovePhoneNumberFromUser(string phoneNumber, string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            contact.PhoneNumbers = contact.PhoneNumbers.Where(x => x.PhoneNumber != phoneNumber).ToList();

            db.UpsertRecord(tableName, contact.Id, contact);
        }
        private static void RemoveUser(string id)
        {
            Guid guid = new Guid(id);
            db.DeleteRecord<ContactModel>(tableName, guid);
        }
    }
}
