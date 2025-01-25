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

            // Check if the level is not empty
            if (!string.IsNullOrEmpty(level) && Char.IsLetter(level[0]))
            {
                while (j < level.Length && Char.IsLetter(level[j]))
                {
                    j++;
                }
                string name = level.Substring(0, j);
                if (HtmlHelper.Instance.HtmlTags.Contains(name))
                {
                    current.TagName = name;
                }
            }

            // Splitting attributes
            var attributes = level.Substring(j).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            for (int k = 0; k < attributes.Length; k++)
            {
                if (attributes[k].Contains('#'))
                {
                    var idParts = attributes[k].Split('#');
                    if (idParts.Length > 1)
                    {
                        if (!String.IsNullOrEmpty(idParts[0]))
                        {
                            current.Classes.Add(idParts[0]);
                        }
                        current.Id = idParts[1];
                    }
                    else
                    {
                        current.Id = idParts[0];
                    }
                }
                else
                {
                    current.Classes.Add(attributes[k]);
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
