using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Adb_Package_Uninstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uninstall.Enabled = false;
            checkBox1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            adb_cmd("adb push aapt-arm-pie /data/local/tmp");
            adb_cmd("adb shell chmod 0755 /data/local/tmp/aapt-arm-pie");
            string[] rs = split(adb_cmd("adb shell pm list packages -f"), "package:");

            for (int a = 1; a < rs.Length; a++)
            {
                string[] rs2 = split(rs[a], "=");
                this.listBox1.Items.Add(rs2[0]);
            }
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            String temp = (String)listBox1.Items[index];
            string re3 = adb_cmd("adb shell /data/local/tmp/aapt-arm-pie d badging " + temp);
            //MessageBox.Show(temp);
            if (re3.Contains("application-label:"))
            {
                string name = split(re3, "application-label:")[1];
                string name2 = split(name, "\n")[0];
                //MessageBox.Show(name2);
                name2 = name2.Replace("'","");
                appl.Text = name2;
            }
            else
            {
                appl.Text = "-ERROR-";
            }

            if (re3.Contains("package: name='"))
            {
                string name = split(re3, "package: name='")[1];
                string name2 = split(name, "'")[0];
                //MessageBox.Show(name2);
                appl2.Text = name2;
            }
            else
            {
                appl2.Text = "-ERROR-";
            }

            if (appl2.Text.Contains("-ERROR-"))
            {
                uninstall.Enabled = false;
            }
            else
            {
                uninstall.Enabled = true;
            }
        }




        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = adb_cmd(textBox1.Text);
        }

        public string[] split(string a, string b)
        {
            return a.Split(new string[] { b }, StringSplitOptions.None);
        }

        public string adb_cmd(string str)
        {
            ProcessStartInfo cmd = new ProcessStartInfo();
            Process process = new Process();
            cmd.FileName = @"cmd";
            cmd.WindowStyle = ProcessWindowStyle.Hidden;             // cmd창이 숨겨지도록 하기
            cmd.CreateNoWindow = true;                               // cmd창을 띄우지 안도록 하기

            cmd.UseShellExecute = false;
            cmd.RedirectStandardOutput = true;        // cmd창에서 데이터를 가져오기
            cmd.RedirectStandardInput = true;          // cmd창으로 데이터 보내기
            cmd.RedirectStandardError = true;          // cmd창에서 오류 내용 가져오기

            process.EnableRaisingEvents = false;
            process.StartInfo = cmd;
            process.Start();
            //process.StandardInput.Write(@"chcp 65001" + Environment.NewLine);
            process.StandardInput.Write(@"cd adb" + Environment.NewLine);
            process.StandardInput.Write(str + Environment.NewLine);
            // 명령어를 보낼때는 꼭 마무리를 해줘야 한다. 그래서 마지막에 NewLine가 필요하다
            process.StandardInput.Close();

            string result = process.StandardOutput.ReadToEnd();
            //StringBuilder sb = new StringBuilder();
            //sb.Append("[Result Info]" + DateTime.Now + "\r\n");
            //sb.Append(result);
            //sb.Append("\r\n");

            process.WaitForExit();
            process.Close();
            return result;
        }

        private void uninstall_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            String temp = (String)listBox1.Items[index];


            //richTextBox1.Text =  adb_cmd("adb pull " + temp + " ../" + appll + ".apk");
            string result = adb_cmd("adb pull " + temp + " ../" + appl.Text.Replace(" ", "_") + ".apk");
            if (result.Contains("No such file or directory"))
            {
                result = adb_cmd("adb pull " + temp + " ../" + temp.Replace("/", "_") + ".apk");
            }
            string r_r = adb_cmd("adb shell pm uninstall -k --user 0 " + appl2.Text);
            if (r_r.Contains("Success"))
            {
                MessageBox.Show("Success Uninstall");
            }
            else
            {
                MessageBox.Show("Fail Uninstall");
            }

        }
    }
}
