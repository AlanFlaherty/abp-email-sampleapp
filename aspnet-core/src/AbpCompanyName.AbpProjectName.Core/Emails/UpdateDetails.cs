
using System;
using System.Collections.Generic;

namespace AbpCompanyName.AbpProjectName.Core.Emails
{
    //TODO: rename to packet something.... 
    public class UpdateDetails
    {
        // TODO: implement as a class with deserialisation properly but this should work for now
        public String FolderName { get; set; }
        public int MesssageCount { get; set; }
        public int UnreadMessageCount { get; set; }
        public int RecentMessageCount { get; set; }

        public int PageNumber { get; set; }
        public IList<EmailSummary> Messages {get; set;}
    }
}