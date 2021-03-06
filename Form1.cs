﻿using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefSharpCallFromJavaScript
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private ChromiumWebBrowser _browser;

        public void InitializeChromium()
        {
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSettings settings = new CefSettings();
            // これを入れないと黒い余白が発生していまう。
            Cef.EnableHighDPISupport();
            Cef.Initialize(settings);
            // こちらの接続先はWeb側でデバッグ実行した時に表示されるローカルの接続先をコピペしてください。
            _browser = new ChromiumWebBrowser("https://localhost:44315/");
            Controls.Add(_browser);
            _browser.Dock = DockStyle.Fill;

            var eventObject = new ScriptedMethodsBoundObject();
            eventObject.EventArrived += OnJavascriptEventArrived;
            _browser.RegisterJsObject("boundEvent", eventObject, options: BindingOptions.DefaultBinder);
        }

        public class ScriptedMethodsBoundObject
        {
            public event Action<string, object> EventArrived;

            public void RaiseEvent(string eventName, object eventData = null)
            {
                if (EventArrived != null)
                {
                    EventArrived(eventName, eventData);
                }
            }
        }

        public static void OnJavascriptEventArrived(string eventName, object eventData)
        {
            var jsonString = eventData.ToString();
            string path = string.Empty;
            if (jsonString.Contains("memo"))
            {
                path = @"notepad.exe";
                Process.Start(path);
            }

            Console.WriteLine("Event arrived: {0}", eventName); // output 'click'

        }
    }
}