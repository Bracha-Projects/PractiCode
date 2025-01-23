using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        private static Selector SelectorChild(string level)
        {
            Selector current = new Selector();
            int j = 0;
            if (Char.IsLetter(level[0]))
            {
                while (Char.IsLetter(level[j++]));
                string name = level.Substring(0, j-1);
                if(HtmlHelper.Instance.HtmlTags.Contains(name))
                    current.TagName = name;
            }
            var attributes = level.Substring(j).Split('.');
            for (int k = 0; k < attributes.Length; k++)
            {
                if (!attributes[k].Contains('#'))
                    current.Classes.Add(attributes[k]);
                else
                {
                    var id = attributes[k].Split('#');
                    if (id.Length > 1)
                    {
                        current.Classes.Add(id[0]);
                        current.Id = id[1];
                    }
                    else current.Id = id[0];
                }
            }
            return current;
        }
        public static Selector ExtractSelector(string query)
        {
            var levels = query.Split(' ').ToList();
            Selector root = SelectorChild(levels[0]);
            Selector current = root;
            for (int i = 1; i < levels.Count(); i++)
            {
                current.Child = SelectorChild(levels[i]);
                current.Child.Parent = current;
                current = current.Child;
            }
            return root;
        }
    }
}
