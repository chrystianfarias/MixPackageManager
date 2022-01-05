using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;

namespace MixMods.MixPackageManager.Commands.Script
{
    public class INI
    {
        public IniData data;
        public string path;

        public INI(IniData data, string path)
        {
            this.data = data;
            this.path = path;
        }

        public static INI OpenIni(string path)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);
            return new INI(data, path);
        }

        public string[] getSections()
        {
            var sections = new string[data.Sections.Count];
            var idx = 0;
            foreach(var section in data.Sections)
            {
                sections[idx] = section.SectionName;
                idx++;
            }
            return sections;
        }
        public Section getSection(string section)
        {
            return new Section(this, section);
        }
        public Section addSection(string section)
        {
            data.Sections.AddSection(section);
            return new Section(this, section);
        }
        public void removeSection(string section)
        {
            data.Sections.RemoveSection(section);
        }
        public void save()
        {
            var parser = new FileIniDataParser();
            parser.WriteFile(path, data);
        }

        public class Section
        {
            INI ini;
            string section;

            public Section(INI ini, string section)
            {
                this.ini = ini;
                this.section = section;
            }
            public string[] getKeys()
            {
                var keys = new string[ini.data[section].Count];
                var idx = 0;
                foreach (var key in ini.data[section])
                {
                    keys[idx] = key.KeyName;
                    idx++;
                }
                return keys;
            }
            public string getValue(string key)
            {
                return ini.data[section][key];
            }
            public void setValue(string key, string value)
            {
                ini.data[section][key] = value;
            }
            public void delete(string key)
            {
                ini.data[section].RemoveKey(key);
            }
            public void add(string key, string value)
            {
                ini.data[section].AddKey(key, value);
            }
        }
    }
}
