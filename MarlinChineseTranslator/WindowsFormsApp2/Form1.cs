using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        // CAUTION:
        // This Chinese Simplification Alphabed table is aligned with 
        // the Marlin font. If you don't know how to modify the Marlin
        // font, you should NOT touch this app and its code.
        
        // 位限率弃放化换终未中户刷到径直长率用应消取成完成击从抖起零归加
        //const string Alphabet = "不败失先知参维准备就绪卡已插入拔出主菜单自动开始关闭步进电机回原点设置偏移定预热所有床降温打源挤抽轴调平速度喷嘴风扇流控制最小大当前运耗材对比载保存恢复厂更新信息界面整暂停印继续止由储无休眠等待操作";
        //const string Alphabet = "完成取消应用加抖长直径配刷终反向上升换初化首限位错冗余高低校败失先中参心准备请绪卡已插入拔出主菜单自动开始关闭步进电机回原点设置偏移定预热所有床降温打源挤抽轴调平速度喷嘴风扇流控制最小大当前运耗材对比载保存恢复厂更新信息界面整暂停印继续止由储无休眠等待操作";
        const string Alphabet = "完成就空应用加抖长直径驶刷微反按探针换初化首限位错交替高低校败失先中越心准备请绪卡已插入拔出主菜单自动开始关闭步进电机回原点设置偏移定预热所有床降温打源挤抽轴调平速度喷嘴风扇流控制最小大当前运耗材对比载保存恢复厂更新信息界面整暂停印继续止悬储无休眠等待操作";
        const int OriginalStartCode = 151;
        const int StartCode = 128;
        
        /// <summary>
        /// Load language head file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Clear();
                richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
            }
        }

        /// <summary>
        /// Restore original characters from \x code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();

            Regex rgx = new Regex(@"\\x[0-9a-fA-F]+");

            foreach (string line in richTextBox1.Lines)
            {
                string str = line;
                Match match = rgx.Match(line);
                
                while(match.Success)
                {
                    string res = str.Substring(match.Index, match.Length);
                    string hex = res.Substring(2, 2);

                    int val = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);

                    string cnStr = Alphabet.Substring(val - (int)numericUpDown2.Value, 1);

                    str = str.Replace(res, cnStr);

                    match = rgx.Match(str);
                }

                richTextBox2.AppendText(str + "\n");        
            }
        }

        /// <summary>
        /// Generate localized head file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();
            richTextBox4.Clear();

            Dictionary<char, int> CharUsing= new Dictionary<char, int>();
            Dictionary<char, int> CharCode = new Dictionary<char, int>();
            Dictionary<char, int> dicUndefinedChars = new Dictionary<char, int>();

            int code = (int)numericUpDown2.Value;

            foreach(char ch in Alphabet)
            {
                CharUsing.Add(ch, 0);
                CharCode.Add(ch, code++);
            }

            Regex rgx = new Regex("\".*\"", RegexOptions.ECMAScript);

            foreach(string line in richTextBox2.Lines)
            {
                string str = line;
                Match match = rgx.Match(str);

                if (match.Success)
                {
                    string subStr = str.Substring(match.Index, match.Length);
                    StringBuilder sb = new StringBuilder();

                    foreach(char ch in subStr)
                    {
                        //int pos = Alphabet.IndexOf(ch);
                        
                        if (CharCode.ContainsKey(ch))
                        {
                            sb.AppendFormat("\\x{0:x02}", CharCode[ch]);
                            CharUsing[ch]++;
                        } else
                        {
                            if (ch > 128)
                            {
                                if (dicUndefinedChars.ContainsKey(ch))
                                {
                                    dicUndefinedChars[ch]++;
                                } else
                                {
                                    dicUndefinedChars[ch] = 1;
                                }
                            }

                            sb.Append(ch);
                        }
                    } // foreach

                    // Replace old string with hex code
                    str = str.Replace(subStr, sb.ToString()); 
                } // if

                richTextBox3.AppendText(str + "\n");
            } // for each

            //richTextBox4.AppendText(string.Format("\nlength:{0}", richTextBox4.Text.Length));
            richTextBox4.AppendText(string.Format("Undefined characters({0}):{1}", dicUndefinedChars.Count, new string(dicUndefinedChars.Keys.ToArray())));

            StringBuilder sbUnusedChars = new StringBuilder();
            {
                StringBuilder sbCharUsage = new StringBuilder();

                foreach (char key in CharUsing.Keys)
                {
                    sbCharUsage.AppendFormat("{0}({1})", key, CharUsing[key]);
                    if (CharUsing[key] == 0)
                    {
                        sbUnusedChars.Append(key);
                    }
                }

                richTextBox4.AppendText("\nUsed slots:" + sbCharUsage.ToString());
                richTextBox4.AppendText(string.Format("\nUnused slots({1}): {0}", sbUnusedChars.ToString(), sbUnusedChars.Length));
            }

            if (sbUnusedChars.Length < dicUndefinedChars.Count) {
                richTextBox4.AppendText(string.Format("\nUnfortunately, all slots have been used. need {0} more", sbUnusedChars.Length - dicUndefinedChars.Count));
            } else {
                richTextBox4.AppendText(string.Format("\nLucky, there are {0} slots left.", sbUnusedChars.Length - dicUndefinedChars.Count));
            }
        }
    }
}
