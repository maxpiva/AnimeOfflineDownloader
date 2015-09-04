﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADBaseLibrary;

namespace AnimeDownloader
{
    public class Log
    {
        public string DateTime { get; set; }
        public LogType Type { get; set; }

        public Color Color
        {
            get
            {
                switch (Type)
                {
                    case LogType.Warn:
                        return Color.FromArgb(255,200,200,0);
                    case LogType.Error:
                        return Color.Red;
                }
                return Color.Black;
            }
        }

        public string Text { get; set; }
    }
}
