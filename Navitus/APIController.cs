using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using System.Linq;

namespace Navitus
{
    // Controllong all REST API calls
    public class APIController : NancyModule
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public APIController()
        {

            // Adding new customer into database
            Post("/", async args =>
            {
                log.Info("HTTP: Post '/' request received.");
                // Some checkings to make sure correct data is provided
                string req = this.Request.Body.AsString();
                var obj = JsonConvert.DeserializeObject<dynamic>(req);
                var name = (string)obj.Name;
                var comment = (string)obj.Comment;
                var age = (string)obj.Age;
                int age_int = -1;

                if (name == null) name = "";
                if (comment == null) comment = "";
                if (age != null && age.All(char.IsDigit)) age_int = int.Parse(age);

                // Getting data and passing to user.
                CustomerObj customer = new CustomerObj(name, age_int, comment);
                bool status = await DatabaseIO.AddCustomer(customer);

                // if all ok
                if (status)
                {
                    log.Info("HTTP: Post '/' Customer entry added into database successfully.");
                    return new ResponseMessage(true, "Customer entry added into database successfully.").Json();
                }
                // if something wrong
                else
                {
                    log.Info("HTTP: Post '/' Unable to add customer, maybe its missing name or json is in wrong format.");
                    return new ResponseMessage(false, "Unable to add customer, maybe its missing name or json is in wrong format.").Json();
                }


            });


            Post("/Edit", async args =>
            {
                log.Info("HTTP: Post '/Edit' request received.");
                // Some checkings to make sure correct data is provided
                string req = this.Request.Body.AsString();
                var obj = JsonConvert.DeserializeObject<dynamic>(req);
                var name = (string)obj.Name;
                var comment = (string)obj.Comment;
                var age = (string)obj.Age;
                int age_int = -1;
                var id = (string)obj.Id;
                int id_int = new int();


                if (name == null) name = "";
                if (comment == null) comment = "";
                if (age != null && age.All(char.IsDigit)) age_int = int.Parse(age);
                if (id != null && id.All(char.IsDigit)) id_int = int.Parse(id);

                // Getting data and passing to user.
                CustomerObj customer = new CustomerObj(name, age_int, comment, id_int);
                bool status = await DatabaseIO.UpdateCustomer(customer);
                // if all ok
                if (status)
                {
                    log.Info("HTTP: Post '/Edit' Customer entry updated successfully.");
                    return new ResponseMessage(true, "Customer entry updated successfully.").Json();
                }
                // if something wrong
                else
                {
                    log.Info("HTTP: Post '/Edit' unable to update customer entry, check json Id and if Name is not empty.");
                    return new ResponseMessage(false, "Unable to update customer entry, check json Id and if Name is not empty.").Json();
                }


            });


            Get("/", async args =>
            {
                log.Info("HTTP: Get '/' request received.");
                // Some checkings to make sure correct data is provided
                string req = this.Request.Body.AsString();
                var obj = JsonConvert.DeserializeObject<dynamic>(req);
                var name = (string)obj.Name;
                var comment = (string)obj.Comment;
                var age = (string)obj.Age;
                int age_int = -1;
                var id = (string)obj.Id;
                int id_int = new int();


                if (name == null) name = "";
                if (comment == null) comment = "";
                if (age != null && age.All(char.IsDigit)) age_int = int.Parse(age);
                if (id != null && id.All(char.IsDigit)) id_int = int.Parse(id);

                // Getting data and passing to user.
                CustomerObj customer = new CustomerObj(name, age_int, comment, id_int);
                var results = await DatabaseIO.GetCustomer(customer);
                // if all ok
                if (results != null)
                {
                    log.Info("HTTP: Get '/' customer details received from database and sent to user.");
                    string results_json = JsonConvert.SerializeObject(results);
                    return results_json;
                }
                // if something wrong
                else
                {
                    log.Info("HTTP: Get '/' exception, please check log file for more details.");
                    return new ResponseMessage(false, "Exception, please check log file for more details.");
                }


            });

            Get("/GetAll", async args =>
            {
                log.Info("HTTP: Get '/GetAll' request received.");
                // Getting data and passing to user.
                var results = await DatabaseIO.GetAllCustomers();
                // if all ok
                if (results != null)
                {
                    log.Info("HTTP: Get '/GetAll' all customer detals received and sent to user.");
                    string results_json = JsonConvert.SerializeObject(results);
                    return results_json;
                }
                // if something wrong
                else
                {
                    log.Info("HTTP: Get '/GetAll' Exception, please check log file for more details..");
                    return new ResponseMessage(false, "Exception, please check log file for more details.");
                }

            });

            Delete("/{Id}", async args =>
            {
                log.Info("HTTP: Delete '/" + args.Id + "' request received.");
                var id = args.Id;
                int id_int = new int();
                // Some checkings to make sure correct data is provided
                if (id != null) id_int = int.Parse(id);

                // Getting data and passing to user.
                bool results = await DatabaseIO.DeleteCustomer(id);
                // if all ok
                if (results != false)
                {
                    log.Info("HTTP: Delete '/" + args.Id + "' customer deleted successfully.");
                    return new ResponseMessage(true, "Customer deleted successfully.");
                }
                // if something wrong
                else
                {
                    log.Info("HTTP: Delete '/" + args.Id + "' customer does not exist or exception error accured. Please check logs for more information");
                    return new ResponseMessage(false, "Customer does not exist or exception error accured. Please check logs for more information");
                }


            });

            Get("/History/{Id}", async args =>
            {
                log.Info("HTTP: Get '/History/" + args.Id + "' request received.");
                // Some checkings to make sure correct data is provided
                var id = args.Id;
                int id_int = new int();
                if (id != null) id_int = int.Parse(id);
                // Getting data and passing to user.
                var results = await DatabaseIO.ShowHistory(id);
                // if all ok
                if (results != null)
                {
                    log.Info("HTTP: Get '/History/" + args.Id + "' customer history details send to user.");
                    string results_json = JsonConvert.SerializeObject(results);
                    return results_json;
                }
                // if something wrong
                else
                {
                    log.Info("HTTP: Get '/History/" + args.Id + "' exception, please check log file for more details or maybe there no history exists.");
                    return new ResponseMessage(false, "Exception, please check log file for more details or maybe there no history exists.");
                }


            });



        }
    }
}
