using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KODlab1_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string haffmanCoding(string text)
        {
            text.Replace("\n", " ");
            Dictionary<string, int> countChars = new Dictionary<string, int>();
            Dictionary<char, string> cods = new Dictionary<char, string>();
            foreach (var cha in text)
            {
                if (countChars.ContainsKey(cha.ToString()))
                {
                    countChars[cha.ToString()] += 1;
                }
                else
                {
                    countChars.Add(cha.ToString(), 0);
                    cods.Add(cha, "");
                }
            }
            int l = text.Length;

            while (countChars.Count > 1)
            {
                countChars = countChars.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                var first = countChars.ElementAt(0);
                var second = countChars.ElementAt(1);
                //countChars["aaaaaaaaa"] = 0;
                countChars.Remove(first.Key);
                countChars.Remove(second.Key);
                countChars.Add(second.Key + first.Key, first.Value + second.Value);
                foreach (var cha in first.Key)
                {
                    cods[cha] = "1" + cods[cha];
                }
                foreach (var cha in second.Key)
                {
                    cods[cha] = "0" + cods[cha];
                }
            }
            string ans = JsonConvert.SerializeObject(cods, Formatting.None);
            ans += "*";
            foreach (var cha in text)
            {
                ans += cods[cha];
            }
            return ans;
        }
        public string haffmanDecoding(string text)
        {
            string[] tex = text.Split('*');
            Dictionary<char, string> codsh = JsonConvert.DeserializeObject<Dictionary<char, string>>(tex[0]);
            Dictionary<string, char> cods = new Dictionary<string, char>();
            foreach (var item in codsh)
            {
                cods.Add(item.Value, item.Key);
            }
            //string ans = JsonConvert.SerializeObject(cods, Formatting.None);
            string ans = "";
            char[] chars = tex[1].ToCharArray();
            for (int i = 0; i < chars.Length;)
            {
                string key = chars[i].ToString();
                int j = i + 1;
                while (!cods.ContainsKey(key))
                {
                    key += chars[j].ToString();
                    j++;
                }
                ans += cods[key].ToString();
                i += key.Length;
            }
            return ans;
        }
        
        public static string lzwCoding(string uncompressed)
        {
            
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 1280; i++)
                dictionary.Add(((char)i).ToString(), i);

            string w = string.Empty;
            List<int> compressed = new List<int>();

            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                     compressed.Add(dictionary[w]);
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);
            string ans = "";
            foreach(var item in compressed)
            {
                ans += Convert.ToString(item)+",";
            }
            return ans.Substring(0,ans.Length-1); 
        }

        public static string lzwDecoding(string text)
        {
            List<int> compressed = new List<int>();
            foreach (var str in text.Split(','))
                compressed.Add(Convert.ToInt32(str));
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 1280; i++)
                dictionary.Add(i, ((char)i).ToString());
            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);
            foreach (int k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];
                decompressed.Append(entry);
                dictionary.Add(dictionary.Count, w + entry[0]);
                w = entry;
            }
            return decompressed.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked & checkBox2.Checked)
            {
                richTextBox2.Text = haffmanCoding(lzwCoding(richTextBox1.Text));
            }
            else if (checkBox1.Checked & !checkBox2.Checked)
            {
                richTextBox2.Text = lzwCoding(richTextBox1.Text);
            }
            else if (!checkBox1.Checked & checkBox2.Checked)
            {
                richTextBox2.Text = haffmanCoding(richTextBox1.Text);
            }
            else
            {
                richTextBox2.Text = "метод не выбран";
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked & checkBox2.Checked)
            {
                richTextBox1.Text = lzwDecoding(haffmanDecoding(richTextBox2.Text));
            }
            else if (checkBox1.Checked & !checkBox2.Checked)
            {
                richTextBox1.Text = lzwDecoding(richTextBox2.Text);
            }
            else if (!checkBox1.Checked & checkBox2.Checked)
            {
                richTextBox1.Text = haffmanDecoding(richTextBox2.Text);
            }
            else
            {
                richTextBox1.Text = "метод не выбран";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);
            textBox1.Text = filename;
            richTextBox1.Text = fileText;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = saveFileDialog1.FileName;
            // сохраняем текст в файл
            System.IO.File.WriteAllText(filename, richTextBox2.Text);
            
        }
    }
}
