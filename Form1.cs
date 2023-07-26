using System.Data;
using System.Runtime.Intrinsics.Arm;
using System.Windows.Forms;

namespace BelsoEllenorUtils
{
    public partial class APMS : Form
    {
        public APMS()
        {
            InitializeComponent();
        }

        class Global
        {
            public static string[] sor = new string[100];
            public static int sorcount = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "MTA naplófájlok (*.log)|*.LOG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in ofd.FileNames)
                {
                    if (File.Exists(file))
                    {
                        string[] lines = File.ReadAllLines(file);
                        int i = 0;
                        foreach (string line in lines)
                        {
                            i++;
                            if (line.Contains("adott neked"))
                            {
                                Global.sorcount++;
                                string me_line = File.ReadLines(file).Skip(i).Take(1).First();
                                string money_line = File.ReadLines(file).Skip(i - 1).Take(1).First();
                                string sender2 = getBetween(money_line, "[SeeMTA]: ", " adott");
                                string money = money_line.Substring(money_line.LastIndexOf('e') + 3);
                                string money_formatted = money.Substring(0, money.IndexOf("d"));
                                string money_formatted_output = money_formatted + "$";
                                string receiver;
                                if (me_line.Contains("neki:"))
                                {
                                    receiver = me_line.Substring(me_line.LastIndexOf(':') + 1);
                                }
                                else
                                {
                                    string me_linenew = File.ReadLines(file).Skip(i - 2).Take(1).First();
                                    receiver = me_linenew.Substring(me_linenew.LastIndexOf(':') + 1);
                                }
                                string sorcountstring = Global.sorcount.ToString();
                                dataGridView1.Rows.Add(sorcountstring, file, sender2, receiver, money_formatted_output);
                                Global.sor[Global.sorcount] = money_line;
                            }
                        }
                    }
                }
            }
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string fajl = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            int nev = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            string ram = Global.sor[nev];
            if (File.Exists(fajl))
            {
                string[] lines = File.ReadAllLines(fajl);
                int i = 0;
                foreach (string line in lines)
                {
                    i++;
                    if (line.Contains(ram))
                    {
                        string msg = "";
                        for (int a = i - 20; a < i + 20 && a < lines.Length; a++) // 0-tól indexeljük a tömböt, ezért 19-tõl kezdünk
                        {
                            msg = msg + lines[a] + Environment.NewLine;
                        }
                        var formPopup = new Form();
                        formPopup.Text = "Log nézegetõ";
                        formPopup.Size = new Size(600, 670);
                        Label mylab = new Label();
                        mylab.Text = msg;
                        mylab.Size = new Size(600, 670);
                        formPopup.Controls.Add(mylab);
                        formPopup.Show(this);
                    }
                }
            }
        }
    }
}