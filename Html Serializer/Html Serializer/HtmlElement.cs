using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            HtmlElement current = this;
            q.Enqueue(current);
            while (q.Count > 0)
            {
                current = q.Dequeue();
                for (int i = 0; i < current.Children.Count(); i++)
                {
                    q.Enqueue(current.Children[i]);
                }
                yield return current;
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement parent = this.Parent;
            yield return this;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }
        private bool IsEqual(Selector other)
        {
            if(other == null) return false;
            if(other.Id != this.Id) return false;
            if(other.Classes.Equals(this.Classes)) return false;
            if(other.TagName != this.Name) return false;
            return true;    
        }
        private void FilteredList(Selector selector,HtmlElement element,HashSet<HtmlElement> results)
        {
            var children = element.Descendants();
            foreach (var child in children)
            {
                if(child.IsEqual(selector))
                {
                    if(selector.Child == null)
                    {
                        results.Add(child);
                    }
                    else FilteredList(selector.Child, child, results);
                }
            }
        }
        public HashSet<HtmlElement> FilteredList(string selector)
        {
            Selector s = Selector.ExtractSelector(selector);
            HashSet<HtmlElement> results = new HashSet<HtmlElement>();
            FilteredList(s,this, results);
            return results;
        }
       
    }
}
