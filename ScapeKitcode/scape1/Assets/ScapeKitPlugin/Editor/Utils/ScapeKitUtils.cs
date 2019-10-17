using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace ScapeKitUnity 
{
    internal static class ThreadStartExtensions 
    {
        internal static void ExecuteDelegate(this ThreadStart threadDelegate, bool useAsync)
        {
            if (useAsync) 
            {
                var newThread = new Thread(threadDelegate);
                newThread.Start();
            }
            else 
            {
                threadDelegate.Invoke();
            }
        }
    }

    internal static class StringExtensions 
    {

        internal static bool IsNullOrEmpty(this string str)
        {
            return str == null || str.Length == 0;
        }

        internal static void WriteToFile(this string content, string fileName) 
        {
            System.IO.File.WriteAllText (fileName, content);
        }

        internal static void ExecuteBash(this string command, bool isAsync, Action<string> result, Action<string> error)
        {
            string tmpScript = @"#!/bin/bash" + System.Environment.NewLine + command;
            tmpScript.WriteToFile(@"tmp.sh");

            ThreadStart threadStart = new ThreadStart (delegate() 
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = @"tmp.sh",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                UnityEngine.Debug.Log(process.StartInfo.FileName + " " + process.StartInfo.Arguments);

                StringBuilder resultList = new StringBuilder(" "); 
                process.OutputDataReceived += (sender, args) => 
                { 
                    if(args.Data != null) 
                    {
                        resultList.Append(args.Data); 
                    }
                };

                StringBuilder errorList = new StringBuilder(""); 
                process.ErrorDataReceived += (sender, args) => 
                { 
                    if(args.Data != null) 
                    {
                        errorList.Append(args.Data); 
                    }
                };

                process.Start();
                {
                    // read from standard output
                    process.BeginOutputReadLine();
                    // read from error output
                    process.BeginErrorReadLine(); 
                }
                process.WaitForExit();
                {
                    result(resultList.ToString());

                    if(errorList.Length > 0) 
                    {
                        error(errorList.ToString());
                    }
                }
                process.WaitForExit();
                process.Close();
                process.Dispose();
            });

            threadStart.ExecuteDelegate(isAsync);
        }
    }
}