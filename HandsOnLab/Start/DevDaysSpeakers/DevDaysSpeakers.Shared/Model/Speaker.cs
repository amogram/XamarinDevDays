﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevDaysSpeakers.Model
{
    public class Speaker
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public string Title { get; set; }

        public string Avatar => "http://www.pngall.com/wp-content/uploads/2016/04/Happy-Person-Free-Download-PNG.png";

        //Azure information for version
        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }
    }
}
