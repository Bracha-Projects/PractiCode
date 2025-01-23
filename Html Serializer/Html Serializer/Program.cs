using Html_Serializer;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml.Linq;

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
HtmlElement ExtractAttributes(string line)
{
    HtmlElement element = new HtmlElement();
    element.Name = line.Split(' ')[0];
    foreach (Match attr in new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line))
    {
          if (attr.Groups.Count == 3)
          {
            string name = attr.Groups[1].Value;
            string value = attr.Groups[2].Value;
            element.Attributes[name] = value;
            if (name == "id")
                element.Id = value;
            if (name == "class")
                element.Classes = value.Split(' ').ToList();
          }
    }
    return element;
}

var html = await Load("https://hebrewbooks.org/beis");
var cleanHtml = new Regex("\\s").Replace(html," ");
var tagMatches = Regex.Matches(cleanHtml, @"<\/?([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>|([^<]+)").Where(l=>!String.IsNullOrWhiteSpace(l.Value));

var htmlLines = new List<string>();
foreach (Match item in tagMatches)
{
    string tag = item.Value.Trim();
    if(tag.StartsWith('<'))
    {
        htmlLines.Add(tag.Trim('<','>'));
    }
    else
    {
        htmlLines.Add(tag);
    }
}

HtmlElement root = ExtractAttributes(htmlLines[1]);
HtmlElement current = root;
for (int i = 2; i < htmlLines.Count(); i++)
{
    string firstWord = htmlLines[i].Trim().Split(' ')[0];
    if (HtmlHelper.Instance.HtmlTags.Contains(firstWord) || HtmlHelper.Instance.HtmlVoidTags.Contains(firstWord))
    {
        HtmlElement element = ExtractAttributes(htmlLines[i]);
        current.Children.Add(element);
        element.Parent = current;
        if(!HtmlHelper.Instance.HtmlVoidTags.Contains(firstWord))
            current = element;
    }
    else if (firstWord[0] == '/' && current != null)
    {
        current = current.Parent;
    }
    else if(current != null)
    {
        current.InnerHtml += htmlLines[i];
    }
}

foreach (var item in root.FilteredList("div.popup")) 
{
    Console.WriteLine(item.Name +" " + item.Classes);
}


