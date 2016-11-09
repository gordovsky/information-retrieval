using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Index
{
    public class Document
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public Document(int docId, string title, string body)
        {
            Id = docId;
            Body = body;
            Title = title;
        }
    }
}
