using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectorGui
{

    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        Dictionary<string, string> keyValueMap;
        string ResultmessageERP;
        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                label3.Text = "Invalid Ec Url";
                label3.ForeColor = Color.Red;
                return;
            }
            dynamic converted;
            HttpClient client = new HttpClient();
            var url = textBox1.Text;
            HttpRequestMessage request = new HttpRequestMessage();
            if (comboBox2.Text == "Get") request = new HttpRequestMessage(HttpMethod.Get, url);
            else if (comboBox2.Text == "Post") request = new HttpRequestMessage(HttpMethod.Post, url);
            else
            {
                label3.Text = "Please Select Ecommerce method ";
                return;
            }
            if (Ec_Token != "") request.Headers.Add("Authorization", Ec_Token);
            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                ResultmessageEc = await response.Content.ReadAsStringAsync();
                converted = ParseJson(ResultmessageEc);

            }
            catch (HttpRequestException)
            {
                label3.Text = "Invalid Ecommerce request";
                label3.ForeColor = Color.Red;
                return;
            }
            if (converted == null)
            {
                label3.Text = "Empty data Ec";
                label3.ForeColor = Color.Red;
                return;
            }
            else
            {
                keyValueMap = new Dictionary<string, string>();
                DataGridViewComboBoxColumn dgvCmb = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Ec",
                    Name = "Ec",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                };
                dgvCmb.Items.Clear();
                DataGridViewTextBoxColumn EcType = new DataGridViewTextBoxColumn
                {
                    ReadOnly = true,
                    HeaderText = "EcType",
                    Name = "EcType",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                };
                DataGridViewCheckBoxColumn dgvCheckBox = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "Add_Value",
                    Name = "Add_Value",
                };
                DataGridViewComboBoxColumn Value = new DataGridViewComboBoxColumn
                {
                    HeaderText = "Value",
                    Name = "Value",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                    ReadOnly = true,
                };
                DataGridViewTextBoxColumn dgvTextBox = new DataGridViewTextBoxColumn
                {
                    HeaderText = "valueadded",
                    Name = "valueadded",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                    ReadOnly = true,
                };
                Value.Items.Add("Custom Value");
                JObject converted2 = JsonConvert.DeserializeObject<JArray>(ResultmessageEc).ToObject<List<JObject>>().FirstOrDefault();
                foreach (KeyValuePair<string, JToken> keyValuePair in converted2)
                {
                    keyValueMap.Add(keyValuePair.Key, keyValuePair.Value.ToString());
                }
                dataGridView1.Columns.Add(dgvCmb);
                dataGridView1.Columns.Add(EcType);
                dataGridView1.Columns.Add(dgvCheckBox);
                dataGridView1.Columns.Add(Value);
                dataGridView1.Columns.Add(dgvTextBox);
                for(int i = 0; i < dataGridView1.RowCount - 1; i++) {
                    DataGridViewRow ligne = dataGridView1.Rows[i];
                    string k = ligne.Cells["ERPType"].Value.ToString();
                    if (!string.IsNullOrEmpty(k))
                    {
                        DataGridViewComboBoxCell cmbCell = (DataGridViewComboBoxCell)ligne.Cells["Ec"];
                        DataGridViewComboBoxCell valueCell = (DataGridViewComboBoxCell)ligne.Cells["Value"];
                        foreach (KeyValuePair<string, JToken> x in converted2)
                        {
                            if (x.Value.Type.ToString() == k)
                            {
                                cmbCell.Items.Add(x.Key.ToString()); ;
                                valueCell.Items.Add(x.Key.ToString());
                            }
                        }
                    }
                }
                
                
                
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                button4.Enabled = true;
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Add_Value"].Index && e.RowIndex >= 0)
            {
                DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (checkBoxCell.Value == checkBoxCell.TrueValue)
                {
                    dataGridView1.Rows[e.RowIndex].Cells["Value"].ReadOnly = false;
                }
            }

        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Ec"].Index && e.RowIndex >= 0)
            {
                DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string selectedValue = comboBoxCell.Value.ToString();
                DataGridViewTextBoxCell textBoxCell = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells["EcType"];
                if (string.IsNullOrEmpty(selectedValue))
                {
                    textBoxCell.Value = "";
                }
                else
                {
                    JObject converted = JsonConvert.DeserializeObject<JArray>(ResultmessageEc).ToObject<List<JObject>>().FirstOrDefault(); ;
                    textBoxCell.Value = converted[comboBoxCell.Value].Type;
                }
            }
            if (e.ColumnIndex == dataGridView1.Columns["Value"].Index && e.RowIndex >= 0)
            {
                DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string selectedValue = comboBoxCell.Value.ToString();
                DataGridViewTextBoxCell textBoxCell = (DataGridViewTextBoxCell)dataGridView1.Rows[e.RowIndex].Cells["valueadded"];
                if (selectedValue == "Custom Value")
                {
                    textBoxCell.ReadOnly = false;
                }
                else
                {
                    textBoxCell.ReadOnly = true;
                }
            }
        }


        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private async void button2_Click(object sender, EventArgs e)
        {
            JArray JErp = JsonConvert.DeserializeObject<JArray>(ResultmessageERP);
            JArray JEcs = JsonConvert.DeserializeObject<JArray>(ResultmessageEc);
            foreach (DataGridViewRow ligne in dataGridView1.Rows)
            {
                string EcV = ligne.Cells["Ecommerce"].Value.ToString();
                string ErpV = ligne.Cells["ERP"].Value.ToString();
                DataGridViewCheckBoxCell checkBoxCell1 = (DataGridViewCheckBoxCell)ligne.Cells[4];
                Console.WriteLine(checkBoxCell1.Value);
                if (checkBoxCell1.Value == null)
                {
                    JErp[ErpV] = JEcs[EcV];
                }
                else
                {
                    string textboxvalue = ligne.Cells["valueadded"].Value.ToString();
                    JObject newvalue = new JObject();
                    int result;
                    if (int.TryParse(textboxvalue, out result))
                    {
                        newvalue["salem"] = int.Parse(textboxvalue);
                    }
                    else
                    {
                        newvalue["salem"] = textboxvalue;
                    }
                    JErp[ErpV] = newvalue["salem"];
                }
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, textBox3.Text);
            Console.WriteLine(JErp.ToString());
            var content = new StringContent(JErp.ToString(), null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                label3.Text = "Success";
                label3.ForeColor = Color.Green;
            }
            else
            {
                label3.Text = "Failed";
                label3.ForeColor = Color.Red;
            }

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            ComboBox comboBox = new ComboBox();
        }


        string ResultmessageEc;
        JObject converted;
        private async void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" && custom_structure == "")
            {
                label3.Text = "Fill the ERP URL or Enter a custom structure";
                label3.ForeColor = Color.Red;
                return;
            }
            if (textBox2.Text == "")
            {
                try { JObject converted = JsonConvert.DeserializeObject<JObject>(custom_structure); }
                catch (InvalidCastException)
                {
                    label3.Text = "Invalid Custom Structure";
                    label3.ForeColor = Color.Red;
                    return;
                }
                ResultmessageERP = converted.ToString();
            }
            else
            {
                HttpClient client = new HttpClient();
                var url = textBox2.Text;
                HttpRequestMessage request = new HttpRequestMessage();
                if (comboBox1.Text == "Get") request = new HttpRequestMessage(HttpMethod.Get, url);
                else if (comboBox1.Text == "Post") request = new HttpRequestMessage(HttpMethod.Post, url);
                else
                {
                    label3.Text = "Please Select ERP method";
                    label3.ForeColor = Color.Red;
                    return;
                }
                if (Erp_Token != "") request.Headers.Add("Authorization", Erp_Token);
                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var messageTask = response.Content.ReadAsStringAsync();
                    messageTask.Wait();
                    ResultmessageERP = messageTask.Result;
                    if (ResultmessageERP[0] == '[' && ResultmessageERP[ResultmessageERP.Length - 1] == ']')
                    {
                        converted = JsonConvert.DeserializeObject<JArray>(ResultmessageERP).ToObject<List<JObject>>().FirstOrDefault();
                    }
                    else if (ResultmessageERP[0] == '{' && ResultmessageERP[ResultmessageERP.Length - 1] == '}')
                    {
                        converted = JsonConvert.DeserializeObject<JObject>(ResultmessageERP);
                    }

                }
                catch (HttpRequestException)
                {
                    label3.Text = "Invalid ERP request";
                    label3.ForeColor = Color.Red;
                    return;
                }

            }
            if (converted == null)
            {
                label3.Text = "ERP data is empty can't get structure";
                label3.ForeColor = Color.Red;
                return;
            }
            else
            {

                keyValueMap = new Dictionary<string, string>();
                DataGridViewTextBoxColumn Ecolumn = new DataGridViewTextBoxColumn();
                Ecolumn.HeaderText = "ERP";
                Ecolumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                Ecolumn.Name = "ERP";
                Ecolumn.ReadOnly = true;
                dataGridView1.Columns.Add(Ecolumn);
                DataGridViewTextBoxColumn TypeEcolumn = new DataGridViewTextBoxColumn();
                TypeEcolumn.HeaderText = "ERPType";
                TypeEcolumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                TypeEcolumn.Name = "ERPType";
                TypeEcolumn.ReadOnly = true;
                dataGridView1.Columns.Add(TypeEcolumn);
                 foreach (KeyValuePair<string, JToken> keyValuePair in converted)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                    row.Cells[0].Value = keyValuePair.Key.ToString();
                    row.Cells[1].Value = keyValuePair.Value.Type.ToString();
                    dataGridView1.Rows.Add(row);
                    keyValueMap.Add(keyValuePair.Key, keyValuePair.Value.ToString());
                }
                dataGridView1.Refresh();
                button1.Enabled = true;
                converted = null;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        private dynamic ParseJson(string result)
        {
            JObject Jsobject;
            JArray Jsarray;
            if (result[0] == '[' && result[result.Length - 1] == ']')
            {
                Jsarray = JsonConvert.DeserializeObject<JArray>(result);
                return Jsarray;
            }
            else
            {
                Jsobject = JObject.Parse(result);
                return Jsobject;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Form custom_form = new Form();
            custom_form.AutoSize = true;
            RichTextBox custom_textBox = new RichTextBox();
            custom_textBox.Text = custom_structure;
            custom_textBox.Size = new System.Drawing.Size(500, 300);
            Button custom_button = new Button();
            custom_button.Text = "Submit structure";
            custom_button.Location = new System.Drawing.Point(600, 150);
            custom_button.Name = "button5";
            custom_button.Size = new System.Drawing.Size(233, 80);
            custom_button.TabIndex = 18;
            custom_button.UseVisualStyleBackColor = true;
            Label custom_label = new Label();
            custom_label.AutoSize = true;
            custom_label.Location = new Point(600, 10);
            custom_button.Click += delegate (object sender2, EventArgs e2) { custom_button_Click(sender, e, custom_form, custom_textBox.Text, custom_label); };
            custom_form.Controls.Add(custom_textBox);
            custom_form.Controls.Add(custom_button);
            custom_form.Controls.Add(custom_label);
            custom_form.ShowDialog();
        }
        string custom_structure = "";
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {


        }

        private void custom_button_Click(object sender, EventArgs e, Form custom_form, string text, Label custom_label)
        {
            if (text == "" || (text[0] == '{' && text[text.Length - 1] == '}'))
            {
                custom_structure = text;
                custom_form.Close();
            }
            else
            {
                custom_label.Text = "Error : Not a Json Structure";
            }

        }

        private void tokenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void eRPTokenToolStripMenuItem_Click(object sender, EventArgs e4)
        {
            Form ERP_Token_form = new Form();
            ERP_Token_form.Size = new System.Drawing.Size(420,200);
            RichTextBox ERP_Token_Textbox = new RichTextBox();
            ERP_Token_Textbox.Text = custom_structure;
            ERP_Token_Textbox.Size = new System.Drawing.Size(400, 100);
            Button ERP_Token_Button = new Button();
            ERP_Token_Button.Text = "Submit Token";
            ERP_Token_Button.Location = new System.Drawing.Point(180, 110);
            ERP_Token_Button.Name = "button5";
            ERP_Token_Button.Size = new System.Drawing.Size(60, 30);
            ERP_Token_Button.TabIndex = 18;
            ERP_Token_Button.UseVisualStyleBackColor = true;
            ERP_Token_Button.Click += delegate (object sender3, EventArgs e3) { ERP_Token_Buton_Click(sender, e3, ERP_Token_form, ERP_Token_Textbox.Text); };
            ERP_Token_form.Controls.Add(ERP_Token_Textbox);
            ERP_Token_form.Controls.Add(ERP_Token_Button);
            ERP_Token_form.ShowDialog();

        }
        string Erp_Token = "";
        private void ERP_Token_Buton_Click(object sender, EventArgs e, Form window, string Token)
        {
            Erp_Token = "Bearer " + Token;
            window.Close();
        }

        private void ecommerceTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Ec_Token_form = new Form();
            Ec_Token_form.Size = new System.Drawing.Size(420, 200);
            RichTextBox Ec_Token_Textbox = new RichTextBox();
            Ec_Token_Textbox.Text = custom_structure;
            Ec_Token_Textbox.Size = new System.Drawing.Size(400, 100);
            Button Ec_Token_Button = new Button();
            Ec_Token_Button.Text = "Submit Token";
            Ec_Token_Button.Location = new System.Drawing.Point(180, 110);
            Ec_Token_Button.Name = "button5";
            Ec_Token_Button.Size = new System.Drawing.Size(60, 30);
            Ec_Token_Button.TabIndex = 18;
            Ec_Token_Button.UseVisualStyleBackColor = true;
            Ec_Token_Button.Click += delegate (object sender2, EventArgs e2) { Ec_Token_Buton_Click(sender, e, Ec_Token_form, Ec_Token_Textbox.Text); };
            Ec_Token_form.Controls.Add(Ec_Token_Textbox);
            Ec_Token_form.Controls.Add(Ec_Token_Button);
            Ec_Token_form.ShowDialog();

        }
        string Ec_Token = "";
        private void Ec_Token_Buton_Click(object sender, EventArgs e, Form window, string Token)
        {
            Ec_Token = "Bearer " + Token;
            window.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                label5.Visible = true;
                comboBox1.Visible = true;
            }
            else
            {
                label5.Visible = false;
                comboBox1.Visible = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                label6.Visible = true;
                comboBox2.Visible = true;
            }
            else
            {
                label6.Visible = false;
                comboBox2.Visible = false;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
        }
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        private async void button4_ClickAsync(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                label3.Text = "ERP post url is empty or not Valid";
                label3.ForeColor = Color.Red;
                return;

            }
            StreamWriter ef = new StreamWriter(Path.Combine(projectDirectory, "griddata.txt"));
            foreach (DataGridViewRow ligne in dataGridView1.Rows)
            {
                if (ligne.Cells["Ec"].Value == null)
                {
                    label3.Text = "Ec column value is missing";
                    label3.ForeColor = Color.Red;
                    return;

                }
                else if (string.IsNullOrEmpty(ligne.Cells["Ec"].Value.ToString()))
                {
                    label3.Text = "Ec column value is missing";
                    label3.ForeColor = Color.Red;
                    return;
                }
                ef.WriteLine(ligne.Cells["Ec"].Value.ToString());
                DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)ligne.Cells["Add_Value"];
                if (Convert.ToBoolean(checkBoxCell.Value))
                {
                    ef.WriteLine("True");
                    if (ligne.Cells["Value"].Value == null)
                    {
                        label3.Text = "Choose which value you want to add";
                        label3.ForeColor = Color.Red;
                        return;

                    }
                    if (ligne.Cells["Value"].Value.ToString() == "Custom Value")
                    {
                        ef.WriteLine("Custom_Value");
                        if (ligne.Cells["valueadded"].Value == null)
                        {
                            label3.Text = "Enter your custom value to add";
                            label3.ForeColor = Color.Red;
                            return;

                        }
                        else ef.WriteLine(ligne.Cells["valueadded"].Value.ToString());
                    }
                    else
                    {
                        ef.WriteLine(ligne.Cells["Value"].Value);
                    }
                }
                else
                {
                    ef.WriteLine("False");
                }
            }
            ef.Close();
            StreamWriter df = new StreamWriter(Path.Combine(projectDirectory, "datafile.txt"));
            df.WriteLine(textBox2.Text);
            df.WriteLine(textBox1.Text);
            df.WriteLine(textBox3.Text);
            df.WriteLine(comboBox2.Text);
            df.WriteLine(comboBox1.Text);
            df.WriteLine(Ec_Token);
            df.WriteLine(Erp_Token);
            df.WriteLine(custom_structure);
            df.Close();
            List<string> ERPColumnMapping = new List<string>();
            List<string> EcColumnMapping = new List<string>();
            foreach (DataGridViewRow ligne in dataGridView1.Rows)
            {
                ERPColumnMapping.Add(ligne.Cells["ERP"].Value.ToString());
                EcColumnMapping.Add(ligne.Cells["Ec"].Value.ToString());

            }
            // Background service logic
            string serviceCode = $@"using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;


namespace WorkerService1
{{
    public class Program
    {{
        public static void Main()
        {{
            Worker worker = new Worker();
            worker.WriteToFile("" [ ""+DateTime.Now+"" ] ""+""Welcome"");
            var executeTask = worker.ExecuteAsync();
            worker.WriteToFile("" [ ""+DateTime.Now+"" ] ""+ ""Good day"");
            var x = Console.ReadLine();

        }}
    }}
}}

public class Worker
{{
    public async Task ExecuteAsync()
    {{
        try
        {{
            WriteToFile("" [ ""+DateTime.Now+"" ] ""+""Service started"");
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;
            StreamReader df = new StreamReader(Path.Combine(projectDirectory,""datafile.txt""));
            string ErpGetUrl = df.ReadLine();
            string EcGetUrl = df.ReadLine();
            string ErpPostUrl = df.ReadLine();
            string EcMethod = df.ReadLine();
            string ErpMethod = df.ReadLine();
            string Ec_Token = df.ReadLine();
            string Erp_Token = df.ReadLine();
            string custom_structure = df.ReadLine();
            string ResultmessageERP;
            JObject Jcontent = new JObject();

            df.Close();
            string ResultmessageEc;
            while (true)
            {{
                WriteToFile("" [ ""+DateTime.Now+"" ] ""+""Begin Ec Get request at "");
                JArray converted = new JArray();
                HttpClient client = new HttpClient();
                var url = EcGetUrl;
                HttpRequestMessage request = new HttpRequestMessage();
                if (EcMethod == ""Get"") request = new HttpRequestMessage(HttpMethod.Get, url);
                else if (EcMethod == ""Post"") request = new HttpRequestMessage(HttpMethod.Post, url);
                if (Ec_Token != """") request.Headers.Add(""Authorization"", Ec_Token);
                try
                {{
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var messageTask = response.Content.ReadAsStringAsync();
                    messageTask.Wait();
                    ResultmessageEc = messageTask.Result;
                    WriteToFile("" [ ""+DateTime.Now+"" ] ""+ ""Service: GET Ec DATA << SUCCESS >> "");
                }}
                catch (HttpRequestException)
                {{
                    WriteToFile("" [ ""+DateTime.Now+"" ] ""+ ""Service: GET Ec DATA << FAILED >> "");
                    return;
                }}
                WriteToFile("" [ ""+DateTime.Now+"" ] ""+""Service Finished Ec get request at "");
                WriteToFile("" [ ""+DateTime.Now+"" ] ""+""Service Begin ERP data get Request at "");
                HttpClient client2 = new HttpClient();
                var url2 = ErpGetUrl;
                HttpRequestMessage request2 = new HttpRequestMessage();
                if (ErpMethod == ""Get"") request2 = new HttpRequestMessage(HttpMethod.Get, url2);
                else if (ErpMethod == ""Post"") request2 = new HttpRequestMessage(HttpMethod.Post, url2);
                if (Erp_Token != """") request2.Headers.Add(""Authorization"", Erp_Token);
                try
                {{
                    var response2 = await client2.SendAsync(request2);
                    response2.EnsureSuccessStatusCode();
                    var messageTask2 = response2.Content.ReadAsStringAsync();
                    messageTask2.Wait();
                    ResultmessageERP = messageTask2.Result;
                    WriteToFile("" [ ""+DateTime.Now+"" ] "" + ""Service: GET ERP DATA << SUCCESS >> "");
                }}
                catch (HttpRequestException)
                {{
                    WriteToFile("" [ ""+DateTime.Now+"" ] "" + ""Service: GET ERP DATA << FAILED >> "");
                    return;
                }}
                WriteToFile("" [ ""+DateTime.Now+"" ] "" + ""Service Finished ERP data get method at "");
                WriteToFile("" [ ""+DateTime.Now+"" ] "" + "" Checking Data... "");
                JArray JEc = JsonConvert.DeserializeObject<JArray>(ResultmessageEc);
                JArray JErp = JsonConvert.DeserializeObject<JArray>(ResultmessageERP);
                ;
                int added = JEc.Count - JErp.Count;
                if (added > 0)
                {{
                    WriteToFile("" [ ""+DateTime.Now+"" ] ""+ ""Service Found Missing data "");
                    WriteToFile("" [ ""+DateTime.Now+"" ] "" + ""Service started Synchronizing "");
                    for (int i = JEc.Count - added; i < JEc.Count; i++)
                    {{
                        Jcontent = JsonConvert.DeserializeObject<JObject>(JErp[0].ToString());
                        JObject ogdata = JsonConvert.DeserializeObject<JObject>(JEc[i].ToString());
                        StreamReader ef = new StreamReader(Path.Combine(projectDirectory,""griddata.txt""));
                        string line;
                        foreach (JProperty property in Jcontent.Properties())
                        {{
                            line = ef.ReadLine();
                            string EcCell = line;
                            Jcontent[property.Name] = ogdata[EcCell];
                            line = ef.ReadLine();
                            if (line == ""True"")
                            {{
                                line = ef.ReadLine();
                                if (line == ""Custom_Value"")
                                {{
                                    line = ef.ReadLine();
                                    string value1 = Jcontent[property.Name].ToString();
                                    Jcontent[property.Name] = value1 + line;
                                }}
                                else
                                {{
                                    Jcontent[property.Name] = Jcontent[property.Name].ToString() + ogdata[line].ToString();
                                }}
                            }}
                        }}
                        ef.Close();

                        var client3 = new HttpClient();
                        var request3 = new HttpRequestMessage(HttpMethod.Post, ErpPostUrl);
                        var content3 = new StringContent(Jcontent.ToString(), null, ""application/json"");
                        request3.Content = content3;
                        try
                        {{
                            var response3 = await client3.SendAsync(request3);
                            response3.EnsureSuccessStatusCode();
                            WriteToFile("" [ ""+DateTime.Now+"" ] ""+ ""Service: POST DATA << SUCCESS >> "");
                        }}
                        catch (HttpRequestException)
                        {{
                            WriteToFile("" [ ""+DateTime.Now+"" ] ""+ ""Service: POST DATA << FAILED >> "");
                            return;
                        }}
                    }}
                }}
                else
                {{
                    WriteToFile("" [ ""+DateTime.Now+"" ] ""+""Service: DATA SYNCED "");
                }}
                WriteToFile(""------------------------------------------------------------------------------------------------"");
                await Task.Delay(60000);
            }}
        }}
        catch (Exception ex)
        {{
            WriteToFile("" [ ""+DateTime.Now+"" ] ""+""unexpected error "" + ex.ToString());
        }}
    }}
    public void WriteToFile(string message)
    {{
        string x = Environment.CurrentDirectory;
        string path = Path.Combine(x,""logs"");
        if (!Directory.Exists(path))
        {{
            Directory.CreateDirectory(path);
        }}
        string filepath = Path.Combine(path, ""ServiceLog.txt"");
        if (!System.IO.File.Exists(filepath))
        {{
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {{
                sw.WriteLine(message);

            }}
        }}
        else
        {{
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {{
                sw.WriteLine(message);
            }}
        }}
    }}
}}";
            Console.WriteLine(serviceCode);
            // Compile and run the background service
            await CompileAndRunService(serviceCode);
        }
        private async Task CompileAndRunService(string serviceCode)
        {
            string x = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString();
            string direc = "ServiceDirectory";
            string fullpath = Path.Combine(x, direc);
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }
            string outputfile = Path.Combine(fullpath, "MyProgram.exe");
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true; // Set to true to generate an .exe file
            parameters.OutputAssembly =  outputfile;
            parameters.CompilerOptions = "/target:winexe /platform:anycpu";
            parameters.IncludeDebugInformation = true;
            parameters.GenerateInMemory = false;
            parameters.CompilerOptions = $"/out:{outputfile}";

            //Dependicies
            parameters.ReferencedAssemblies.Add("System.Net.Http.dll");
            parameters.ReferencedAssemblies.Add("Newtonsoft.Json.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.dll");
            parameters.ReferencedAssemblies.Add("System.IO.dll");


            var coreAssembly = typeof(System.Linq.Enumerable).Assembly.Location;
            parameters.ReferencedAssemblies.Add(coreAssembly);
            var systemAssembly = typeof(System.ComponentModel.ITypedList).Assembly.Location;
            parameters.ReferencedAssemblies.Add(systemAssembly);

            //Compiling...
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, serviceCode);

            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    MessageBox.Show($"Error: {error.ErrorText}", "Service Compilation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"Service compiled and saved as {outputfile}", "Service Compilation Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string[] referencedAssemblies = new string[]
                {
                "System.Net.Http.dll",
                "Newtonsoft.Json.dll",
                "System.Runtime.dll",
                "System.IO.dll",
                // Add more DLLs as needed for your dependencies
                };
                foreach (string referencedAssembly in referencedAssemblies)
                {
                    string sourcePath = AppDomain.CurrentDomain.BaseDirectory + referencedAssembly;
                    string destinationPath = Path.Combine(Directory.GetParent(outputfile).ToString(), referencedAssembly);

                    if (!File.Exists(destinationPath) && File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, destinationPath);
                        Console.WriteLine($"Copied {referencedAssembly} to the debug folder.");
                    }
                }
            }
        }
    }
}