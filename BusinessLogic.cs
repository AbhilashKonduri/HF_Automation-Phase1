using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

delegate void ProgressInfo(string Message);

class BusinessLogic
{
    public event ProgressInfo Log; //default value is null
    string[] Base_versions = new string[5] { "v2018\\12.00.25.0003", "v2016\\11.00.84.0099", "v2014R1\\10.01.15.0060", "v2014\\10.00.73.0047", "v2011R1\\09.01.30.0055" };
    DataAccessLayer dal = new DataAccessLayer();
    List<string> TextFiles = new List<string>();


    internal void CopyFilestoDirectory()
    {
        for (int i = 0; i < Base_versions.Length; i++)
        {
            TextFiles.Add(dal.GetFiles(Base_versions[i]));
            Get_logged(i);
        }

    }

    internal void FormatItems()
    {
        string QA_List = "";
        string line = null;
        string TextPath = ConfigurationManager.AppSettings["DestinationPath"] + "LatestHFItems.txt";
        StreamWriter Write_dev = new StreamWriter(ConfigurationManager.AppSettings["DestinationPath"] + "HF_devItems.txt");
        StreamWriter write_qa = new StreamWriter(ConfigurationManager.AppSettings["DestinationPath"] + "HF_QAItems.txt");

        StreamReader LatestHFItems = new StreamReader(TextPath);
        while ((line = LatestHFItems.ReadLine()) != null)
        {
            if (line.Trim().ToLower().Substring(0, 2) == "tr" || line.Trim().ToLower().Substring(0, 2) == "hf" || line.Trim().ToLower().Substring(0, 2) == "dm" || line.Trim().ToLower().Substring(0, 2) == "cr")
            {

                string ItemNumber = line.Trim().ToLower().Substring(3, 6);
                string Description = line.Trim().ToLower().Substring(9);
                if (line.Trim().ToLower().Contains("atp") || line.Trim().ToLower().Contains("qtp") || line.Trim().ToLower().Contains("api") || line.Trim().ToLower().Contains("wrapper"))
                    Write_dev.WriteLine(line);
                else
                {
                    if (QA_List != "")
                    {
                        QA_List += ",";
                    }
                    write_qa.WriteLine(line);
                    QA_List += ItemNumber;
                }

            }
        }
        write_qa.Close();
        Write_dev.Close();
        LatestHFItems.Close();
        string url = "https://jts.ingrnet.com/report.asp?ProductGroup=01&StatusClosed=on&JTSNo=" +
            QA_List.Trim() +
            "&CommitmentsInclude=both&view=list_table&viewFile=view&SortBy=10&SortDir=0&SortBy2=1&SortDir2=0&SortBy3=100&SortDir3=0&voChildren=on&voEdit=on&voFunction=count";
        System.Diagnostics.Process.Start(url);
    }
    private void Get_logged(int i)
    {
        if (Base_versions[i].Contains("2014R1") || Base_versions[i].Contains("2011R1"))
            Log?.Invoke(Base_versions[i].Substring(1, 6));
        else
            Log?.Invoke(Base_versions[i].Substring(1, 4));
    }

    internal void GetHFList()
    {
        for (int i = 0; i < TextFiles.Count; i++)
        {
            int Old_HFVersion = CheckHFVersion(TextFiles[i]);
            dal.GetNewHFList(Base_versions[i], Old_HFVersion);
            Get_logged(i);
        }

    }

    private Int32 CheckHFVersion(string v)
    {
        string[] version = new string[5] { "2018", "2016", "2014R1", "2014", "2011" };
        string Old_HFVersion = null;
        for (int i = 0; i < version.Length; i++)
        {
            if (v.Contains(version[i]))
            {
                Old_HFVersion = ConfigurationManager.AppSettings[version[i]];
                break;
            }

        }
        return Convert.ToInt32(Old_HFVersion);
    }
}

