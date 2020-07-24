using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mono.Options;

namespace se
{
    [XmlRoot("commit")]
    public class SvnCommit
    {
        [XmlAttribute("revision")]
        public string revision;
        [XmlElement("author")]
        public string name;
        [XmlElement("name")]
        public string date;
    }

    [XmlRoot("entry")]
    public class SvnEntry
    {
        [XmlAttribute("kind")]
        public string kind;
        [XmlElement("name")]
        public string name;
        [XmlElement("commit")]
        public SvnCommit commit;
    }

    [XmlRoot("list")]
    public class SvnList
    {
        [XmlAttribute("path")]
        public string path;
        [XmlElement("entry")]
        public List<SvnEntry> entries;
    }

    [XmlRoot("lists")]
    public class SvnLists
    {
        [XmlElement("list")]
        public List<SvnList> lists;
    }

    class SvnExporter
    {
        static void Main(string[] args)
        {
            string svnurl = "";
            string outdir = "";
            List<string> includes = new List<string>();
            List<string> excludes = new List<string>();
            bool help = false;
            var options = new OptionSet {
                { "u|svnurl=", "svn url", u => svnurl = u },
                { "o|outdir=", "out dir", o => outdir = o},
                { "i|include=", "include suffix", i => includes.Add(i)},
                { "e|exclude=", "exclude suffix", e => excludes.Add(e)},
                { "h|help", "show this message and exit", h => help = h != null },
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("Svn Exporter: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `svn_exporter --help' for more information.");
                return;
            }

            if (help)
            {
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string svn_list_xml = Path.Combine(outdir, "svn_list.xml");
            string svn_err_txt = Path.Combine(outdir, "svn_err.txt");

            XmlSerializer xml_lists = new XmlSerializer(typeof(SvnLists));
            SvnLists xml_root;

            Dictionary<string, int> dir_old = new Dictionary<string, int>();

            if (File.Exists(svn_list_xml))
            {
                xml_root = (SvnLists)xml_lists.Deserialize(new StringReader(File.ReadAllText(svn_list_xml)));
                foreach (SvnEntry entry in xml_root.lists[0].entries)
                {
                    dir_old.Add(entry.name, Int32.Parse(entry.commit.revision));
                }
            }

            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                Process p = new Process();
                p.StartInfo.FileName = "svn";
                p.StartInfo.Arguments = String.Format("list --xml --depth infinity {0}", svnurl);
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.StandardErrorEncoding = Encoding.UTF8;

                try
                {
                    p.Start();
                }
                catch (SystemException e)
                {
                    Console.WriteLine("Exception caught: {0}", e);
                    Environment.Exit(1);
                }

                string str_out = p.StandardOutput.ReadToEnd();

                xml_root = (SvnLists)xml_lists.Deserialize(new StringReader(str_out));

                File.WriteAllText(svn_list_xml, str_out, Encoding.UTF8);

                string str_err = p.StandardError.ReadToEnd();
                File.WriteAllText(svn_err_txt, str_err, Encoding.UTF8);

                p.WaitForExit();
            }

            {
                foreach (SvnEntry entry in xml_root.lists[0].entries)
                {
                    if ("file" != entry.kind)
                    {
                        continue;
                    }
                    string ext = Path.GetExtension(entry.name);
                    if (excludes.Contains(ext) || (0 < includes.Count && !includes.Contains(ext)))
                    {
                        continue;
                    }

                    int old_value = 0;
                    if (dir_old.TryGetValue(entry.name, out old_value) && old_value >= Int32.Parse(entry.commit.revision))
                    {
                        continue;
                    }
                    FileInfo fi = new FileInfo(Path.Combine(outdir, entry.name));
                    Directory.CreateDirectory(fi.DirectoryName);
                    string args_str = String.Format("export --force --depth files \"{0}/{1}@HEAD\" \"{2}\"", xml_root.lists[0].path, entry.name, fi.DirectoryName);

                    ProcessStartInfo psi = new ProcessStartInfo("svn", args_str);
                    psi.RedirectStandardError = true;
                    psi.StandardErrorEncoding = Encoding.UTF8;

                    try
                    {
                        string str_err = Process.Start(psi).StandardError.ReadToEnd();
                        if (null != str_err && 0 < str_err.Length)
                        {
                            File.AppendAllText(svn_err_txt, "---\n", Encoding.UTF8);
                            File.AppendAllText(svn_err_txt, entry.name + "\n", Encoding.UTF8);
                            File.AppendAllText(svn_err_txt, str_err, Encoding.UTF8);
                        }
                    }
                    catch (SystemException e)
                    {
                        Console.WriteLine("Exception caught: {0}", e);
                        Environment.Exit(1);
                    }
                }
            }
        }
    }
}
