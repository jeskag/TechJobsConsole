using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace TechJobsConsole
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;

        public static List<Dictionary<string, string>> FindAll()
        {
            LoadData();
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<Dictionary<string, string>> FindByValue(string value)
        {
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            //Need 2 foreach loops.  First one mirrors FindbyColumn&Value...2nd is KeyValuePair
            foreach (Dictionary<string, string> job in AllJobs)
            {

                foreach (KeyValuePair<string, string> line in job)
                {
                    //newValue = user entered search
                    //value = value in dictionary
                    //does jobs already contain line - see if jobs contain line -- is line already in job
                    string newValue = line.Value.ToString().ToLower();
                    //if (line.value.ToString().Equals(value))
                    if (newValue.Contains(value.ToString().ToLower()))
                    {
                        if (!jobs.Contains(job))
                        {
                            jobs.Add(job);
                        }
                    }

                }
            }

                return jobs;
        }

        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[column];

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }
            return values;
        }

        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();

            //Dictionary is each row (that WAS from the csv file)
            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            //job = search results
            foreach (Dictionary<string, string> row in AllJobs)
            {
                //value = search term
                //aValue = a cell in excel
                string aValue = row[column].ToString().ToLower();

                //if the cell contains the search term, add the row to the search results
                if (aValue.Contains(value.ToString().ToLower()))
                {
                    jobs.Add(row);
                }
            }

            //returns search results
            return jobs;
        }

        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }
    }
}
