using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.IO;

using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] error = { "App1.Test.i is not accessible in this context because it is Protected", "App1.Test.j is not accessible in this context because it is Private", "Test.Protected Sub X() is not accessible in this context because it is Protected", "GlobalProxySelection is obsolete" };
            string[] projects = { "App1", "App1", "App1", "App1" };
            string[] errorFilePath = { "C:\\Users\\Sanjib Sen\\source\\repos\\App1", "C:\\Users\\Sanjib Sen\\source\\repos\\App1", "C:\\Users\\Sanjib Sen\\source\\repos\\App1", "C:\\Users\\Sanjib Sen\\source\\repos\\App1" };
             int []errorLine = {7,8,9};
             string[] errorFile = { "MainPage.xaml.vb", "MainPage.xaml.vb", "MainPage.xaml.vb", "MainPage.xaml.vb"};
             for (int j = 0; j < error.Length; j++)
             {
                 Console.WriteLine(j + 1 + "." + error[j]);
             }
             Console.WriteLine("please enter your choice");
             int choice = Convert.ToInt32(Console.ReadLine());

             if (error[choice - 1].Contains("is not accessible in this context because it is Protected") || error[choice - 1].Contains("is not accessible in this context because it is Private"))
             {
                 string[] splitError = error[choice - 1].Split(' ');
                 string[] classNameList = splitError[0].Split('.');
                 string className = "";
                 string variableName = "";
                 string funcName = "";
                 bool hasFunc = false;
                 if (classNameList.Count() > 2)
                 {
                     className = classNameList[1];
                     variableName = classNameList[2];
                     hasFunc = false;
                 }
                 else if (classNameList.Count() == 2)
                 {
                     className = classNameList[0];
                     funcName = splitError[2];
                     hasFunc = true;
                 }
                 string[] files = System.IO.Directory.GetFiles(errorFilePath[choice - 1], "*.vb");
                 foreach (var path in files)
                 {

                     string[] fileContent = File.ReadAllLines(path);
                     if (fileContent.Contains("Class " + className))
                     {

                         // Open the file to read from.
                         string[] readText = File.ReadAllLines(path);
                         int index = 0;
                         for (int i = 0; i < readText.Length; i++)
                         {
                             string[] words = readText[i].Split(' ');
                             if (hasFunc)
                             {
                                 if (words.Contains(funcName))
                                 {
                                     index = Array.IndexOf(words, funcName);
                                     if (words[index - 2] != null)
                                     {
                                         if (words[index - 2] == "Private" || words[index - 2] == "Protected")
                                         {
                                             words[index - 2] = "Public";
                                             string s = "";
                                             foreach (var c in words)
                                             {
                                                 s = s + c + " ";
                                             }
                                             readText[i] = s;
                                         }
                                     }

                                 }
                             }
                             else
                             {
                                 if (words.Contains(variableName))
                                 {
                                     index = Array.IndexOf(words, variableName);
                                     if (words[index - 1] != null)
                                     {
                                         if (words[index - 1] == "Private" || words[index - 1] == "Protected")
                                         {
                                             words[index - 1] = "Public";
                                             string s = "";
                                             foreach (var c in words)
                                             {
                                                 s = s + c + " ";
                                             }
                                             readText[i] = s;
                                         }
                                     }
                                 }
                             }
                         }
                         File.WriteAllLines(path, readText);

                     }
                 }
             }
             else if (error[choice - 1].Contains("GlobalProxySelection is obsolete"))
             {
                 string[] files = System.IO.Directory.GetFiles(errorFilePath[choice - 1], "*.vb");
                 foreach (var path in files)
                 {

                     string[] fileContent = File.ReadAllLines(path);
                     for (int k = 0; k < fileContent.Length; k++)
                     {
                         if (fileContent[k].Contains("GlobalProxySelection"))
                         {
                             int indx = k;
                             string[] tmp = fileContent[k].Split(' ');
                             string corrected = "";
                             string newProxy = "";
                             foreach (var t in tmp)
                             {
                                 if (t.Contains("GlobalProxySelection"))
                                 {
                                     string[] proxy = t.Split('.');
                                     newProxy = proxy[0] + ".WebRequest.DefaultWebProxy";
                                 }
                                 else
                                 {
                                     newProxy = t==string.Empty?" ":t+" ";
                                 }
                                 corrected = corrected + newProxy;
                             }
                             fileContent[k] = corrected;
                         }
                     }
                     File.WriteAllLines(path, fileContent);     
                 }
                     
             }
            
        }
    }
}
