using System.Configuration;
using System;
using System.IO;
using System.Collections.Generic;

class DataAccessLayer
{

    internal string GetFiles(string version)
    {
        FileInfo LatestFile = null;
        string Path = @"\\\\ingrnet.com\\in\\PPM\\3DBuildsHF\\Active\\Customer\\End-User\\" + version + "\\Hotfixes_ServicePacks";
        string[] folders = Directory.GetDirectories(Path);
        DateTime lastUpdated = DateTime.MinValue;
        foreach (string folder in folders)
        {
            System.IO.FileInfo[] files;
            string filepath = folder + "\\DVD";
            try
            {
                files = new DirectoryInfo(filepath).GetFiles("*.txt");
            }
            catch (Exception ex)
            {
                continue;
            }
            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime > lastUpdated)
                {
                    lastUpdated = file.LastWriteTime;

                    LatestFile = file;
                }
            }
        }

        string destination_path = DestinationPath(version);
        LatestFile.CopyTo(destination_path, true);
        return destination_path;
    }

    internal void GetNewHFList(string base_version, int OldHF)
    {

        StreamWriter sw = new StreamWriter(DestinationPath("LatestHFItems"),true);
        StreamReader Version_file = new StreamReader(DestinationPath(base_version));
        string line;
        int Line_key = 0;
        while ((line = Version_file.ReadLine()) != null)
        {
            if (line.Trim().Length > 1)
            {
                if (line.Trim().ToLower().Substring(0, 6) == "hotfix")
                {
                    if (Convert.ToInt32(line.Trim().ToLower().Substring(6)) > OldHF)
                        Line_key = 1;
                    else
                        Line_key = 0;
                }

                if (Line_key == 1)
                {
                    sw.WriteLine(line);
                }
            }

        }
        Version_file.Close();
        sw.Close();

    }


    private string DestinationPath(string version)
    {
        string Path_1 = ConfigurationManager.AppSettings["DestinationPath"];
        string Path_2 = null;

        switch (version)
        {
            case "v2018\\12.00.25.0003":
                Path_2 = "2018.txt";
                break;
            case "v2016\\11.00.84.0099":
                Path_2 = "2016.txt";
                break;
            case "v2014R1\\10.01.15.0060":
                Path_2 = "2014R1.txt";
                break;
            case "v2014\\10.00.73.0047":
                Path_2 = "2014.txt";
                break;
            case "v2011R1\\09.01.30.0055":
                Path_2 = "2011R1.txt";
                break;
            case "LatestHFItems":
                Path_2 = "LatestHFItems.txt";
                break;
        }

        string Path = Path_1 + Path_2;
        return @Path;
    }






}

