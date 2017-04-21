using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 兄弟战队专用CRC计算器
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string testCode = "1";
            tbTest.Text = crc16_modbus(Encoding.Default.GetBytes(testCode), (uint)testCode.Length).ToString("X4");
        }
        
        public uint crc16_modbus(byte[] modbusdata, uint Length)//Length为modbusdata的长度
        {
            uint i, j;
            uint crc16 = 0xFFFF;
            for (i = 0; i < Length; i++)
            {
                crc16 ^= modbusdata[i]; // CRC = BYTE xor CRC
                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1) //如果CRC最后一位为1􀁋右移一位后carry=1􀁋则将CRC右移一位后􀁋再与POLY16=0xA001进行xor运算
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    else //如果CRC最后一位为0􀁋则只将CRC右移一位
                        crc16 = crc16 >> 1;
                }
            }
            return crc16;
        }
    }
}
