using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;// Seri port iletişimi için
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace KartOkuyucu
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort; // Seri port nesnesi tanımlandı
        private string sector12Data;
        private System.Windows.Forms.Button button8;

        public Form1()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form1_Load);
            serialPort = new SerialPort(); // Seri port nesnesi oluşturuluyor
            serialPort.DataReceived += SerialPort_DataReceived; // Gelen veri olayına abone olundu
            button7.Click += new EventHandler(button7_Click); // Temizle butonu tıklama olayına abone olundu
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde mevcut portları listele
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                textBox1.Text = sector12Data;
            });
            try
            {
                if (serialPort.IsOpen)
                {
                    string data = serialPort.ReadLine();
                    Console.WriteLine("Received data: " + data); // Gelen veriyi konsola yazdır

                    if (data.StartsWith("UID:"))
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            textBox4.Text = data.Substring(4).Trim();
                        }));
                    }
                    else if (data.StartsWith("Sector12Data:"))
                    {
                        string sector12Data = data.Substring(13).Trim();
                        Console.WriteLine("Sector12Data: " + sector12Data); // Sector12Data verisini konsola yazdır
                        this.Invoke(new MethodInvoker(delegate
                        {
                            textBox1.Text = sector12Data;
                        }));
                    }
                }
            }
            catch (IOException ex)
            {
                // Handle the exception (e.g., log it, notify the user, etc.)
                Console.WriteLine("IOException: " + ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem != null)
                {
                    serialPort.PortName = comboBox1.SelectedItem.ToString();
                    serialPort.BaudRate = 9600;
                    serialPort.Open();
                    serialPort.DiscardInBuffer(); // Giriş tamponunu temizle
                    serialPort.DiscardOutBuffer(); // Çıkış tamponunu temizle
                    label1.Text = "Port Açık";
                    label3.Text = "Port başarıyla açıldı.";
                }
                else
                {
                    MessageBox.Show("Lütfen bir port seçin.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                label3.Text = "Port açma hatası: " + ex.Message;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("R\n"); // Arduino'ya 'R' komutu göndererek UID okumayı tetikle
            }
            else
            {
                MessageBox.Show("Önce portu açmalısınız.");
                label3.Text = "Önce portu açmalısınız.";
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer(); // Giriş tamponunu temizle
                    serialPort.DiscardOutBuffer(); // Çıkış tamponunu temizle
                    serialPort.Close();
                    label1.Text = "Port Kapalı";
                    label3.Text = "Port başarıyla kapatıldı.";
                }
                else
                {
                    MessageBox.Show("Port zaten kapalı.");
                    label3.Text = "Port zaten kapalı.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                label3.Text = "Port kapatma hatası: " + ex.Message;
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine("D");
              //  serialPort.Write("D"); // Arduino'ya 'D' komutu göndererek Sektör 12'yi okumayı tetikle
            }
            else
            {
                MessageBox.Show("Önce portu açmalısınız.");
                label3.Text = "Önce portu açmalısınız.";
            }
        }

      

        private void button5_Click_1(object sender, EventArgs e)
        {
if (serialPort.IsOpen)
            {
                string dataToWrite = textBox2.Text;
                serialPort.WriteLine("W" + dataToWrite); // "W" ile birlikte veriyi tek bir satırda gönder
                                                         //  serialPort.Write("W"); // Arduino'ya 'W' komutu göndererek yazma işlemini tetikle
                                                         //  serialPort.Write(dataToWrite); // TextBox2'deki veriyi gönder
                MessageBox.Show("Veri yazma işlemi tamamlandı.");
            }
            else
            {
                MessageBox.Show("Önce portu açmalısınız.");
                label3.Text = "Önce portu açmalısınız.";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Form üzerindeki tüm TextBox kontrollerini temizle
            ClearAllTextBoxes(this);
        }
        private void ClearAllTextBoxes(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is System.Windows.Forms.TextBox)
                {
                    ((System.Windows.Forms.TextBox)c).Clear();
                }
                else if (c.HasChildren)
                {
                    ClearAllTextBoxes(c);
                }
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    // Arduino'ya reset komutu gönder
                    serialPort.DtrEnable = true;
                    System.Threading.Thread.Sleep(100);
                    serialPort.DtrEnable = false;
                    MessageBox.Show("Arduino resetlendi.");
                }
                else
                {
                    MessageBox.Show("Önce portu açmalısınız.");
                    label3.Text = "Önce portu açmalısınız.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                label3.Text = "Reset hatası: " + ex.Message;
            }


        }

    }

}







