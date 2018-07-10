using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class FileINI
{
    Dictionary<string, string> Items = new Dictionary<string, string>();
	public FileINI(string INIPath)
	{
        if (File.Exists(INIPath))
        {
            string groupKey = "";
            string sRaw = File.ReadAllText(INIPath);
            int line = 1;
            foreach (String data in Regex.Split(sRaw, "\r\n", RegexOptions.IgnoreCase))
            {
                if (data.Trim().IndexOf("#") == 0 || data.Trim().IndexOf(";") == 0) continue;
                Match group = Regex.Match(data, @"\[(.*?)\]");
                if (group.Success)
                {
                    groupKey = group.Groups[1].Value;
                }
                else
                {
                    Match val = Regex.Match(data, @"(.*?)=(.*)");
                    if (!val.Success || Regex.Matches(data, @"=").Count > 1) continue;
                    var name = groupKey + "." + val.Groups[1].Value.Trim();
                    var value = val.Groups[2].Value.Trim();
                    if (Items.ContainsKey(name)) Items[name] = value; else Items.Add(groupKey + "." + val.Groups[1].Value, val.Groups[2].Value);
                }
                line++;
            }
            sRaw = null;
        }
	}

    public string getValue(string key)
    {
        if (!Items.ContainsKey(key.Trim())) return ""; else return Items[key.Trim()];
    }
}

