using System;


internal class ConsoleUI
{

    internal void Start()
    {
        BusinessLogic bll = new BusinessLogic();
        bll.Log += logreciever_loading;

        Console.WriteLine("Copying Latest HF Readme files to the Directory..");
        try
        {
            bll.CopyFilestoDirectory();
            Console.WriteLine("All the Latest HF Readme are Copied Succesfully.... ");
            bll.Log -= logreciever_loading;
            bll.Log += logreciever_Appending;
            Console.WriteLine("Appending Latest HF items to the LatestHFItems.txt...");
            bll.GetHFList();
            Console.WriteLine("Reformating the Items into Devolopement and QA Lists");
            bll.FormatItems();
            Console.WriteLine("Gathering the Required Information for the QA items");

        //    Console.WriteLine("Finishing the Process and Gathering the report");

            Console.WriteLine("Hot Fix Review Sheet is Generated Succesfully.");
            Console.ReadLine();
        }
        catch (SystemException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Check Your connection persists with the Ingernet Domain and try again");
        }
    }

    private void logreciever_Appending(string Message)
    {
        Console.WriteLine("\tAppend Complete for V" + Message);
    }

    void logreciever_loading(string Message)
    {
        Console.WriteLine("\tCopied V" + Message);
    }
}
