using System;
using System.Text.RegularExpressions;

namespace Navitus
{
    // Storing temporaly structured customer details into this object
    public class CustomerObj
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Comment { get; set; }



        public CustomerObj(string name = "", int age = -1, string comment = "", int id = 0)
        {
            if (id == 0)
            {
                log.Info(String.Format("Assigning user to the CustomerObj: Name:{0}, Age: {1}, Comment: {2}, Id:{3}", name, age, comment, id));
            }
            else
            {
                log.Info(String.Format("Creating new CustomerObj: Name:{0}, Age: {1}, Comment: {2}, Id:{3}", name, age, comment, id));
            }

            try
            {
                // First letter uppercase for each word and rest of the letters lowercase, removing double spaces
                this.Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Regex.Replace(name, @"\s+", " ").ToLower());
                this.Id = id;
                this.Age = age;
                // First letter uppercase and rest of them lowercase also taking into account if comment length less than 2
                if (comment.Length > 1)
                {
                    this.Comment = comment.Substring(0, 1).ToUpper() + comment.Substring(1).ToLower();
                }
                else
                {
                    this.Comment = comment.ToUpper();
                }
            }
            catch (Exception e)
            {
                log.Error("ERROR: Exception when creating CustomerObj, details: " + e.Message);
            }
        }



    }
}

