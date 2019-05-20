﻿using System;
using System.Text;
using System.IO;
using Mineral;

namespace MineralNode
{
    class Program
    {
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(ex.GetType().ToString());
            builder.AppendLine(ex.Message);
            builder.AppendLine(ex.StackTrace);
            builder.AppendLine();
            File.AppendAllText("./error-log", builder.ToString());
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Mineral.Core.Config.Arguments.Args.SetParam(null, "config.json");

            //MainService service = new MainService();
            //if (service.Initialize(args))
            //    service.Run();


        }
    }
}
