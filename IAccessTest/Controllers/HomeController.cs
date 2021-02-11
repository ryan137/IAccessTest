using IAccessTest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;

namespace IAccessTest.Controllers
{
    public class HomeController : Controller
    {
        private static string filePath = System.AppDomain.CurrentDomain.BaseDirectory + @".\CsvFolder\Testing.csv";

        public ActionResult Index()
        {
            var homeModel = new HomeModel();
            homeModel.DataModelList = SearchFromCsvFile();

            return View(homeModel);
        }

        [HttpPost]
        public ActionResult Insert()
        {
            InsertData();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Search(HomeModel param)
        {
            if (param.Keywords == "")
            {
                return RedirectToAction("Index");
            }
            else
            {
                param.DataModelList = SearchFromCsvFile(param.Keywords);
                return View("Index", param);
            }
        }

        public void InsertData()
        {
            // Initialize Stringbuilder
            StringBuilder sb = new StringBuilder();

            // Add Column
            sb.Append("Number,");
            sb.Append("Content");
            sb.AppendLine();

            int maxRecord = 100000;
            List<DataModel> listModel = new List<DataModel>();

            // Generate Guid and Random Strings
            for (int i = 0; i < maxRecord; i++)
            {
                Guid newGuid = Guid.NewGuid();
                string randomStr = HomeController.RandomString(HomeController.RandomInt());

                sb.Append(newGuid + ",");
                sb.Append(randomStr);
                sb.AppendLine();
            }

            // Create csv file
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        public List<DataModel> SearchFromCsvFile(string keywords = "")
        {
            List<DataModel> dataModelList = new List<DataModel>();

            if (System.IO.File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    if (!string.IsNullOrEmpty(keywords))
                    {
                        while (!reader.EndOfStream)
                        {
                            var dataModel = GetLineFromCsv(reader);
                            var matchTimes = SearchPattern(dataModel.Content, keywords);
                            if (matchTimes != 0)
                            {
                                dataModel.MatchTimes = matchTimes;
                                dataModelList.Add(dataModel);
                            }
                        }
                    }
                    else
                    {
                        while (!reader.EndOfStream && dataModelList.Count <= 51)
                        {
                            var dataModel = GetLineFromCsv(reader);
                            dataModelList.Add(dataModel);
                        }

                        dataModelList.RemoveAt(0);
                    }
                }
            }

            return dataModelList;
        }

        public DataModel GetLineFromCsv(StreamReader reader)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');

            DataModel dataModel = new DataModel()
            {
                ID = values[0],
                Content = values[1],
                MatchTimes = 0
            };

            return dataModel;
        }

        public static int SearchPattern(string text, string pattern)
        {
            int lengthOfText = text.Length;
            int lengthOfPattern = pattern.Length;
            int matchTimes = 0;
            int searchLength = lengthOfText - (lengthOfPattern - 1);

            for (int i = 0; i < searchLength; i++)
            {
                int j;

                for (j = 0; j < lengthOfPattern; j++)
                {
                    var currSearch = text[i + j];
                    var currPattern = pattern[j];
                    if (currSearch != currPattern)
                    {
                        break;
                    }
                }

                if (j == lengthOfPattern)
                {
                    matchTimes += 1;
                }
            }

            return matchTimes;
        }

        public static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.[]`; ";

            string randomString = "";

            //for (int i = 0; i < length; i++)
            //{
            //    randomString += chars[random.Next(chars.Length)].ToString();
            //}

            randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var strSizeEnc = System.Text.Encoding.ASCII.GetByteCount(randomString);
            var strSize = randomString.Length * sizeof(char);

            return randomString;
        }

        public static int RandomInt()
        {
            int randomInt = random.Next(1000, 2000);

            return randomInt;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}