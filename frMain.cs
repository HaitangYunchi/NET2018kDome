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
            toolTip.SetToolTip(ReplaceBind, "卡密ID填入对应卡密，机器码留空则为解绑，填入新的机器码则为换绑");
        }
        // 配置数据类
        public class ConfigData
        {
            public string SoftWareID { get; set; }
            public string OpenID { get; set; }
        }
        private void LoadConfig()
        {
            try
            {
                // 如果文件不存在，则创建默认配置
                if (!File.Exists(configPath))
                {
                    CreateDefaultConfig();
                }

                // 读取并解析JSON文件
                string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                JsonElement root = doc.RootElement;

                // 赋值到文本框
                OpenID.Text = root.GetProperty("OpenID").GetString();
                exampleID.Text = root.GetProperty("SoftWareID").GetString();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"JSON解析错误: {ex.Message}\n将创建新的配置文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateDefaultConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateDefaultConfig()
        {
            try
            {
                // 默认配置值
                var defaultConfig = new
                {
                    SoftWareID = "你的软件实例ID",
                    OpenID = "你的OpenID"
                };

                // 序列化为JSON并写入文件
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,  // 美化格式，便于阅读
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string jsonString = JsonSerializer.Serialize(defaultConfig, options);
                File.WriteAllText(configPath, jsonString, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建配置文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 保存配置到文件
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
                MessageBox.Show($"保存配置失败: {ex.Message}", "错误",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        private void SaveJson_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证输入
                if (string.IsNullOrWhiteSpace(OpenID.Text) || string.IsNullOrWhiteSpace(exampleID.Text))
                {
                    MessageBox.Show("OpenID 和 SoftWareID 不能为空", "警告",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 创建配置对象
                var config = new ConfigData
                {
                    OpenID = OpenID.Text,
                    SoftWareID = exampleID.Text
                };

                // 保存配置
                SaveConfig(config);

                MessageBox.Show("配置保存成功", "成功",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置时出错: {ex.Message}", "错误",
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
            string response = @"    var timestamp = await up.GetRemainingUsageTime(id,key,Code); // 获取返回值 永久 = -1，已过期 = 0，未激活 = 1，其余返回时间戳
    if (timestamp == -1)
    {
        txtResult.Text = ""永久"";
    }
    else if (timestamp == 0)
    {
        txtResult.Text = ""已过期"";
    }
    else if (timestamp == 1)
    {
        txtResult.Text = ""未激活"";
    }
    else
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timestamp); // 卡密剩余时间转换成{days}天{hours}小时{minutes}分钟{seconds}秒
        int days = timeSpan.Days;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        txtResult.Text = $""{days}天{hours}小时{minutes}分钟{seconds}秒"";
    }";
            string id = exampleID.Text;
            string key = OpenID.Text;
            var timestamp = await up.GetRemainingUsageTime(id, key, Code.Text);
            if (timestamp == 0)
            {
                txtResult.Text = $"{response}\r\n\r\n最终输出为：\r\n已过期";
            }
            else if (timestamp == 1)
            {
                txtResult.Text = $"{response}\r\n\r\n最终输出为：\r\n未激活";
            }
            else if (timestamp == -1)
            {
                txtResult.Text = $"{response}\r\n\r\n最终输出为：\r\n永久";
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(timestamp);

                int days = timeSpan.Days;
                int hours = timeSpan.Hours;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;

                txtResult.Text = $"{response}\r\n\r\n最终输出为：\r\n{days}天{hours}小时{minutes}分钟{seconds}秒";
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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {

                    if (!int.TryParse(txtDays.Text, out int days))
                    {
                        MessageBox.Show("请输入有效的天数", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string remark = txtRemark.Text;

                    string response = await up.CreateNetworkAuthentication(days, remark, id, key);

                    // 显示结果
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private async void Activa_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {

                    if (authID.Text == "")
                    {
                        MessageBox.Show("请输入卡密ID", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string remark = txtRemark.Text;

                    string response = await up.ActivationKey(authID.Text, id, Code.Text);

                    // 显示结果
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void send_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {


                try
                {

                    if (sendMess.Text == "")
                    {
                        MessageBox.Show("发送给消息不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string remark = txtRemark.Text;

                    string response = await up.MessageSend(id, sendMess.Text);//发送消息

                    // 显示结果
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (exampleID.Text == "")
            {
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var response = await up.GetUpdate(id, key, Code.Text);
                string _info = @"获取数据为Json格式，后续可以在程序中直接解析Json进行调用组对象数据
也可以使用单功能按钮给出的实例代码
使用时引用System.Text.Json.dll  Newtonsoft.Json 和 HaiTangUpdate.dll
并实例化HaiTangUpdate.dll

        HaiTangUpdate.Update up = new HaiTangUpdate.Update();   // 实例化
        var response = await up.GetUpdate( 软件ID, 你的OpenID, 机器码 );   // 机器码可空
        txtResult.Text = response

输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetDownloadLink( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetSoftwareID( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetSoftwareName( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
             var response = await up.GetVersionNumber( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
             txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
             var response = await up.GetVersionInformation( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
             txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetNotice( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetNumberOfVisits( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            txtResult.Text = response;
        }


最终输出结果如下：

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
                response = "无";
            }

            if (exampleID.Text == "")
            {
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetMiniVersion( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            if (string.IsNullOrEmpty(response))
            {
                response = ""无"";
            }
            txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

例：
        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetIsItEffective( 软件ID, 你的OpenID, 机器码 );
            txtResult.Text = response;
        }


最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var response = await up.GetExpirationDate(id, key, Code.Text);

                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程
永久返回 7258089599000 转换为时间（2199-12-32 23:59:59）

        var response = await up.GetExpirationDate(id, key, Code.Text);
        txtResult.Text = response;

最终输出结果如下（返回时间戳）

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetRemarks( 软件ID, 你的OpenID, 机器码 ); 
            txtResult.Text = response;
        }

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程
返回天数 99999 = 永久

        var response = await up.GetNumberOfDays(id, key, Code.Text);
        txtResult.Text = response;

最终输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程
例：

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetNetworkVerificationId( 软件ID, 你的OpenID, 机器码 ); 
            txtResult.Text = response;
        }

获取软件使用的卡密ID,输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程
例：

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetTimeStamp( 软件ID, 你的OpenID, 机器码 ); // 机器码可空 
            txtResult.Text = response;
        }

输出结果如下(返回时间戳)：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程
例：

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetMandatoryUpdate( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            txtResult.Text = response;
        }

输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程
例：

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetSoftwareMd5( 软件ID, 你的OpenID, 机器码 ); // 机器码可空
            txtResult.Text = response;
        }

输出结果如下：

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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (CloudVar.Text == "")
            {
                MessageBox.Show("变量名不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {

                string _info = @"因为库是采用异步加载的方式，所以我们定义事件为[async]
我们调用库方法的时候要用[await]
采用异步他不阻塞UI线程
例：

        private async void button_Click(object sender, EventArgs e)
        {
            var response = await up.GetCloudVariables( 软件ID, 你的OpenID, 变量名 ); 
            txtResult.Text = response;
        }

输出变量 [" + CloudVar.Text + @"] 的值如下：

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
            string _info = @"获取验证码：
            var response = await up.GetNetworkCode(实例ID, OpenID);
            return response;

最终输出如下：

";
            txtResult.Text = _info + response;
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.CustomerRegister(id, Email.Text, password.Text, Nickname.Text);
            string _info = @"用户注册：
            var response = await up.CustomerRegister(实例ID, 邮箱, 密码，昵称,头像地址,验证码); // 昵称,头像地址,验证码 可空
            return response;

成功返回 Ture 失败或已注册返回 False：

";
            txtResult.Text = _info + response;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.CustomerLogon(id, key, Email.Text, password.Text);
            string _info = @"获取登录信息：
            var response = await up.CustomerLogon(实例ID,OpenID, 邮箱, 密码);
            return response;

成功返回 Ture 失败返回 False：

";
            txtResult.Text = _info + response;
        }

        private async void button22_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserInfo(id, key, Email.Text, password.Text);
            string _info = @"获取用户信息：
            var response = await up.GetUserInfo(实例ID,OpenID, 邮箱, 密码);
            return response;

返回用户所有信息：

";
            txtResult.Text = _info + response;
        }

        private async void button23_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserId(id, key, Email.Text, password.Text);
            string _info = @"获取用户ID：
            var response = await up.GetUserId(实例ID,OpenID, 邮箱, 密码);
            return response;

返回用户ID：

";
            txtResult.Text = _info + response;
        }

        private async void button24_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserAvatar(id, key, Email.Text, password.Text);
            string _info = @"获取用户头像：
            var response = await up.GetUserAvatar(实例ID,OpenID, 邮箱, 密码);
            if (response != null)
            {
                txtResult.Text = response;
            }
            else
            {
                txtResult.Text = ""http://admin.2018k.cn/images/rocket.png"" //这是调用演示图片;
            }

返回用户头像地址：

";
            if (response != null)
            {
                txtResult.Text = _info + response;
            }
            else
            {
                txtResult.Text = _info + $"http://admin.2018k.cn/images/rocket.png  //这是调用演示图片";
            }
        }

        private async void button25_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserNickname(id, key, Email.Text, password.Text);
            string _info = @"获取用户昵称：
            var response = await up.GetUserNickname(实例ID,OpenID, 邮箱, 密码);
            return response;

返回用户昵称：

";
            txtResult.Text = _info + response;
        }

        private async void button26_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserEmail(id, key, Email.Text, password.Text);
            string _info = @"获取邮箱：
            var response = await up.GetUserEmail(实例ID,OpenID, 邮箱, 密码);
            return response;

返回用户邮箱地址：

";
            txtResult.Text = _info + response;
        }

        private async void button27_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserBalance(id, key, Email.Text, password.Text);
            string _info = @"获取账户剩余时长：
            var response = await up.GetUserBalance(实例ID,OpenID, 邮箱, 密码);
            return response;

返回账户剩余时长（分钟数）：

";
            txtResult.Text = _info + response;
        }

        private async void button28_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserLicense(id, key, Email.Text, password.Text);
            string _info = @"获取账户授权：
            var response = await up.GetUserLicense(实例ID,OpenID, 邮箱, 密码);
            return response;

返回账户授权状态：

";
            txtResult.Text = _info + response;
        }

        private async void button29_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            var response = await up.GetUserTimeCrypt(id, key, Email.Text, password.Text);
            string _info = @"获取登录时间戳：
            var response = await up.GetUserTimeCrypt(实例ID,OpenID, 邮箱, 密码);
            return response;

返回登录时的时间戳，用于对比本地时间戳和服务器返回的时间戳，以此来验证是否正确登录，实际应用中可以忽略这个：

";
            txtResult.Text = _info + response;

        }

        private async void button32_Click(object sender, EventArgs e)
        {
            string id = exampleID.Text;
            string key = OpenID.Text;
            if (authID.Text == "")
            {
                MessageBox.Show("请输入卡密ID", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (Email.Text == "")
            {
                MessageBox.Show("用户邮箱不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (password.Text == "")
            {
                MessageBox.Show("请输入密码", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var response = await up.Recharge(id, key, Email.Text, password.Text, authID.Text);
                string _info = @"用户充值：
            var response = await up.Recharge(实例ID,OpenID, 邮箱, 密码,卡密ID);
            return response;

返回用户充值信息：

";
                txtResult.Text = _info + response;
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("都说了这是多余的，你还点击，是不是傻啊？", "灵魂拷问", MessageBoxButtons.OK, MessageBoxIcon.Question);
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
                MessageBox.Show("软件ID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (OpenID.Text == "")
            {
                MessageBox.Show("OpenID不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {

                    if (authID.Text == "")
                    {
                        MessageBox.Show("请输入卡密ID", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string response = await up.ReplaceBind(id, key, authID.Text, Code.Text);

                    // 显示结果
                    txtResult.Text = response;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


