using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NET_2018K
{
    public partial class frMain : Form
    {
        HaiTangUpdate.Update up = new HaiTangUpdate.Update();
        private readonly string configPath = "config.json";
        public frMain()
        {
            InitializeComponent();
            LoadConfig();
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(ReplaceBind, "����ID�����Ӧ���ܣ�������������Ϊ��������µĻ�������Ϊ����");
        }
        // ����������
        public class ConfigData
        {
            public string SoftWareID { get; set; }
            public string OpenID { get; set; }
        }
        private void LoadConfig()
        {
            try
            {
                // ����ļ������ڣ��򴴽�Ĭ������
                if (!File.Exists(configPath))
                {
                    CreateDefaultConfig();
                }

                // ��ȡ������JSON�ļ�
                string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                JsonElement root = doc.RootElement;

                // ��ֵ���ı���
                OpenID.Text = root.GetProperty("OpenID").GetString();
                exampleID.Text = root.GetProperty("SoftWareID").GetString();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"JSON��������: {ex.Message}\n�������µ������ļ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateDefaultConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��������: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateDefaultConfig()
        {
            try
            {
                // Ĭ������ֵ
                var defaultConfig = new
                {
                    SoftWareID = "������ʵ��ID",
                    OpenID = "���OpenID"
                };

                // ���л�ΪJSON��д���ļ�
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,  // ������ʽ�������Ķ�
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string jsonString = JsonSerializer.Serialize(defaultConfig, options);
                File.WriteAllText(configPath, jsonString, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"���������ļ�ʧ��: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // �������õ��ļ�
        private void SaveConfig(ConfigData config)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string jsonString = JsonSerializer.Serialize(config, options);
                File.WriteAllText(configPath, jsonString, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��������ʧ��: {ex.Message}", "����",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        private void SaveJson_Click(object sender, EventArgs e)
        {
            try
            {
                // ��֤����
                if (string.IsNullOrWhiteSpace(OpenID.Text) || string.IsNullOrWhiteSpace(exampleID.Text))
                {
                    MessageBox.Show("OpenID �� SoftWareID ����Ϊ��", "����",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // �������ö���
                var config = new ConfigData
                {
                    OpenID = OpenID.Text,
                    SoftWareID = exampleID.Text
                };

                // ��������
                SaveConfig(config);

                MessageBox.Show("���ñ���ɹ�", "�ɹ�",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��������ʱ����: {ex.Message}", "����",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/HaitangYunchi/NET2018kDome";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        private async void Check_id_Click(object sender, EventArgs e)
        {
            string response = @"    var timestamp = await up.GetRemainingUsageTime(id,key,Code); // ��ȡ����ֵ ���� = -1���ѹ��� = 0��δ���� = 1�����෵��ʱ���
    if (timestamp == -1)
    {
        txtResult.Text = ""����"";
    }
    else if (timestamp == 0)
    {
        txtResult.Text = ""�ѹ���"";
    }
    else if (timestamp == 1)
    {
        txtResult.Text = ""δ����"";
    }
    else
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timestamp); // ����ʣ��ʱ��ת����{days}��{hours}Сʱ{minutes}����{seconds}��
        int days = timeSpan.Days;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        txtResult.Text = $""{days}��{hours}Сʱ{minutes}����{seconds}��"";
    }";
            string id = exampleID.Text;
            string key = OpenID.Text;
            var timestamp = await up.GetRemainingUsageTime(id, key, Code.Text);
            if (timestamp == 0)
            {
                txtResult.Text = $"{response}\r\n\r\n�������Ϊ��\r\n�ѹ���";
            }
            else if (timestamp == 1)
            {
                txtResult.Text = $"{response}\r\n\r\n�������Ϊ��\r\nδ����";
            }
            else if (timestamp == -1)
            {
                txtResult.Text = $"{response}\r\n\r\n�������Ϊ��\r\n����";
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(timestamp);

                int days = timeSpan.Days;
                int hours = timeSpan.Hours;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;

                txtResult.Text = $"{response}\r\n\r\n�������Ϊ��\r\n{days}��{hours}Сʱ{minutes}����{seconds}��";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Code.Text = up.GetMachineCode();
        }

        private async void authbutton_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {

                    if (!int.TryParse(txtDays.Text, out int days))
                    {
                        MessageBox.Show("��������Ч������", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string remark = txtRemark.Text;

                    string response = await up.CreateNetworkAuthentication(days, remark, id, key);

                    // ��ʾ���
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"��������: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private async void Activa_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {

                    if (authID.Text == "")
                    {
                        MessageBox.Show("�����뿨��ID", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string remark = txtRemark.Text;

                    string response = await up.ActivationKey(authID.Text, id, Code.Text);

                    // ��ʾ���
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"��������: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void send_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {


                try
                {

                    if (sendMess.Text == "")
                    {
                        MessageBox.Show("���͸���Ϣ����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string remark = txtRemark.Text;

                    string response = await up.MessageSend(id, sendMess.Text);//������Ϣ

                    // ��ʾ���
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"��������: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var response = await up.GetUpdate(id, key, Code.Text);
                string _info = @"��ȡ����ΪJson��ʽ�����������ڳ�����ֱ�ӽ���Json���е������������
Ҳ����ʹ�õ����ܰ�ť������ʵ������
ʹ��ʱ����System.Text.Json.dll  Newtonsoft.Json �� HaiTangUpdate.dll
��ʵ����HaiTangUpdate.dll

        HaiTangUpdate.Update up = new HaiTangUpdate.Update();   // ʵ����
        var response = await up.GetUpdate( ���ID, ���OpenID, ������ );   // ������ɿ�
        txtResult.Text = response

���������£�

";
                //string json = up.AesDecrypt(jsonAes, key);
                txtResult.Text = _info + response;
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetDownloadLink(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetDownloadLink( ���ID, ���OpenID, ������ ); // ������ɿ�
            txtResult.Text = response;
        }

�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetSoftwareID(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetSoftwareID( ���ID, ���OpenID, ������ ); // ������ɿ�
            txtResult.Text = response;
        }

�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetSoftwareName(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetSoftwareName( ���ID, ���OpenID, ������ ); // ������ɿ�
            txtResult.Text = response;
        }

�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetVersionNumber(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
             var response = await up.GetVersionNumber( ���ID, ���OpenID, ������ ); // ������ɿ�
             txtResult.Text = response;
        }

�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetVersionInformation(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
             var response = await up.GetVersionInformation( ���ID, ���OpenID, ������ ); // ������ɿ�
             txtResult.Text = response;
        }

�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetNotice(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetNotice( ���ID, ���OpenID, ������ ); // ������ɿ�
            txtResult.Text = response;
        }

�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetNumberOfVisits(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetNumberOfVisits( ���ID, ���OpenID, ������ ); // ������ɿ�
            txtResult.Text = response;
        }


�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button11_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetMiniVersion(id, key, Code.Text);
            if (string.IsNullOrEmpty(response))
            {
                response = "��";
            }

            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetMiniVersion( ���ID, ���OpenID, ������ ); // ������ɿ�
            if (string.IsNullOrEmpty(response))
            {
                response = ""��"";
            }
            txtResult.Text = response;
        }

�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button12_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetIsItEffective(id, key, Code.Text);
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

����
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetIsItEffective( ���ID, ���OpenID, ������ );
            txtResult.Text = response;
        }


�������������£�

";
                txtResult.Text = _info + response;
            }
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var response = await up.GetExpirationDate(id, key, Code.Text);

                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�
���÷��� 7258089599000 ת��Ϊʱ�䣨2199-12-32 23:59:59��

        var response = await up.GetExpirationDate(id, key, Code.Text);
        txtResult.Text = response;

�������������£�����ʱ�����

";
                txtResult.Text = _info + response;
            }
        }

        private async void button14_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetRemarks( ���ID, ���OpenID, ������ ); 
            txtResult.Text = response;
        }

�������������£�

";
                var response = await up.GetRemarks(id, key, Code.Text);

                txtResult.Text = _info + response;
            }
        }

        private async void button15_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�
�������� 99999 = ����

        var response = await up.GetNumberOfDays(id, key, Code.Text);
        txtResult.Text = response;

�������������£�

";
                var response = await up.GetNumberOfDays(id, key, Code.Text);
                txtResult.Text = _info + response;
            }
        }

        private async void button16_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�
����

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetNetworkVerificationId( ���ID, ���OpenID, ������ ); 
            txtResult.Text = response;
        }

��ȡ���ʹ�õĿ���ID,���������£�

";
                var response = await up.GetNetworkVerificationId(id, key, Code.Text);

                txtResult.Text = _info + response;
            }
        }

        private async void button17_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�
����

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetTimeStamp( ���ID, ���OpenID, ������ ); // ������ɿ� 
            txtResult.Text = response;
        }

����������(����ʱ���)��

";
                var response = await up.GetTimeStamp(id, key);

                txtResult.Text = _info + response;
            }
        }

        private async void button18_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�
����

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetMandatoryUpdate( ���ID, ���OpenID, ������ ); // ������ɿ�
            txtResult.Text = response;
        }

���������£�

";
                var response = await up.GetMandatoryUpdate(id, key, Code.Text);

                txtResult.Text = _info + response;
            }
        }

        private async void button19_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�
����

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetSoftwareMd5( ���ID, ���OpenID, ������ ); // ������ɿ�
            txtResult.Text = response;
        }

���������£�

";
                var response = await up.GetSoftwareMd5(id, key, Code.Text);

                txtResult.Text = _info + response;
            }
        }

        private async void button20_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (CloudVar.Text == "")
            {
                MessageBox.Show("����������Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {

                string _info = @"��Ϊ���ǲ����첽���صķ�ʽ���������Ƕ����¼�Ϊ[async]
���ǵ��ÿⷽ����ʱ��Ҫ��[await]
�����첽��������UI�߳�
����

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetCloudVariables( ���ID, ���OpenID, ������ ); 
            txtResult.Text = response;
        }

������� [" + CloudVar.Text + @"] ��ֵ���£�

";

                var response = await up.GetCloudVariables(id, key, CloudVar.Text);

                txtResult.Text = _info + response;
            }
        }

        private async void button21_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetNetworkCode(id, key);
            string _info = @"��ȡ��֤�룺
            var response = await up.GetNetworkCode(ʵ��ID, OpenID);
            return response;

����������£�

";
            txtResult.Text = _info + response;
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.CustomerRegister(id, Email.Text, password.Text, Nickname.Text);
            string _info = @"�û�ע�᣺
            var response = await up.CustomerRegister(ʵ��ID, ����, ���룬�ǳ�,ͷ���ַ,��֤��); // �ǳ�,ͷ���ַ,��֤�� �ɿ�
            return response;

�ɹ����� Ture ʧ�ܻ���ע�᷵�� False��

";
            txtResult.Text = _info + response;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.CustomerLogon(id, key, Email.Text, password.Text);
            string _info = @"��ȡ��¼��Ϣ��
            var response = await up.CustomerLogon(ʵ��ID,OpenID, ����, ����);
            return response;

�ɹ����� Ture ʧ�ܷ��� False��

";
            txtResult.Text = _info + response;
        }

        private async void button22_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserInfo(id, key, Email.Text, password.Text);
            string _info = @"��ȡ�û���Ϣ��
            var response = await up.GetUserInfo(ʵ��ID,OpenID, ����, ����);
            return response;

�����û�������Ϣ��

";
            txtResult.Text = _info + response;
        }

        private async void button23_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserId(id, key, Email.Text, password.Text);
            string _info = @"��ȡ�û�ID��
            var response = await up.GetUserId(ʵ��ID,OpenID, ����, ����);
            return response;

�����û�ID��

";
            txtResult.Text = _info + response;
        }

        private async void button24_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserAvatar(id, key, Email.Text, password.Text);
            string _info = @"��ȡ�û�ͷ��
            var response = await up.GetUserAvatar(ʵ��ID,OpenID, ����, ����);
            if (response != null)
            {
                txtResult.Text = response;
            }
            else
            {
                txtResult.Text = ""http://admin.2018k.cn/images/rocket.png"" //���ǵ�����ʾͼƬ;
            }

�����û�ͷ���ַ��

";
            if (response != null)
            {
                txtResult.Text = _info + response;
            }
            else
            {
                txtResult.Text = _info + $"http://admin.2018k.cn/images/rocket.png  //���ǵ�����ʾͼƬ";
            }
        }

        private async void button25_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserNickname(id, key, Email.Text, password.Text);
            string _info = @"��ȡ�û��ǳƣ�
            var response = await up.GetUserNickname(ʵ��ID,OpenID, ����, ����);
            return response;

�����û��ǳƣ�

";
            txtResult.Text = _info + response;
        }

        private async void button26_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserEmail(id, key, Email.Text, password.Text);
            string _info = @"��ȡ���䣺
            var response = await up.GetUserEmail(ʵ��ID,OpenID, ����, ����);
            return response;

�����û������ַ��

";
            txtResult.Text = _info + response;
        }

        private async void button27_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserBalance(id, key, Email.Text, password.Text);
            string _info = @"��ȡ�˻�ʣ��ʱ����
            var response = await up.GetUserBalance(ʵ��ID,OpenID, ����, ����);
            return response;

�����˻�ʣ��ʱ��������������

";
            txtResult.Text = _info + response;
        }

        private async void button28_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserLicense(id, key, Email.Text, password.Text);
            string _info = @"��ȡ�˻���Ȩ��
            var response = await up.GetUserLicense(ʵ��ID,OpenID, ����, ����);
            return response;

�����˻���Ȩ״̬��

";
            txtResult.Text = _info + response;
        }

        private async void button29_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserTimeCrypt(id, key, Email.Text, password.Text);
            string _info = @"��ȡ��¼ʱ�����
            var response = await up.GetUserTimeCrypt(ʵ��ID,OpenID, ����, ����);
            return response;

���ص�¼ʱ��ʱ��������ڶԱȱ���ʱ����ͷ��������ص�ʱ������Դ�����֤�Ƿ���ȷ��¼��ʵ��Ӧ���п��Ժ��������

";
            txtResult.Text = _info + response;

        }

        private async void button32_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (authID.Text == "")
            {
                MessageBox.Show("�����뿨��ID", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (Email.Text == "")
            {
                MessageBox.Show("�û����䲻��Ϊ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (password.Text == "")
            {
                MessageBox.Show("����������", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var response = await up.Recharge(id, key, Email.Text, password.Text, authID.Text);
                string _info = @"�û���ֵ��
            var response = await up.Recharge(ʵ��ID,OpenID, ����, ����,����ID);
            return response;

�����û���ֵ��Ϣ��

";
                txtResult.Text = _info + response;
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("��˵�����Ƕ���ģ��㻹������ǲ���ɵ����", "��꿽��", MessageBoxButtons.OK, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                string url = "https://space.bilibili.com/3493128132626725";
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
        }

        private async void ReplaceBind_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("���ID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID����Ϊ�գ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {

                    if (authID.Text == "")
                    {
                        MessageBox.Show("�����뿨��ID", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string response = await up.ReplaceBind(id, key, authID.Text, Code.Text);

                    // ��ʾ���
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"��������: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://space.bilibili.com/3493128132626725";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }

}


