using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Cryptography;

namespace ClientControls
{
    public class InputTypeControl
    {

        #region 验证身份证格式是否合法
        /// <summary>
        /// 18位身份证验证
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns></returns>
        public static bool CheckIDCard18(string Id)
        {
            if (Id.Length != 18)
            {
                return false;
            }
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        #endregion

        #region 座机/手机/数字验证
        //座机号码验证
        public static bool IsTelephone(string str_telephone)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(str_telephone, @"^(\d{3,4}-)?\d{6,8}$") ||
                System.Text.RegularExpressions.Regex.IsMatch(str_telephone, @"^[1]+[3-8]+\d{9}") ||
                System.Text.RegularExpressions.Regex.IsMatch(str_telephone, @"^\d{6,8}$"));
        }

        //电子邮箱验证
        public static bool IsEmail(string str_email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
        #endregion
        #region 币输入控制
        public enum InputType
        {
            TYPE_NULL,      //初始化
            AMT,            //金额
            DIGITAL,        //有符号整数，任意长度
            UNSINDIGITAL,   //无符号整数，任意长度
            UNSINDIGITAL_EXT,   //无符号整数，自动归零
            UNSINFLOAT,     //无符号浮点数
            UNSCOUNT,       //有符号浮点数，自动归零
        };


        public static InputType GetInputType(DependencyObject obj)
        {
            return (InputType)obj.GetValue(InputTypeProperty);
        }

        public static void SetInputType(DependencyObject obj, InputType value)
        {
            obj.SetValue(InputTypeProperty, value);
        }

        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.RegisterAttached("InputType", typeof(InputType), typeof(InputTypeControl),
        new PropertyMetadata(InputType.TYPE_NULL, OnInputTypeChanged));

        private static void OnInputTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = obj as TextBox;
            #region textbox
            if (null != tb)
            {
                switch ((InputType)e.NewValue)
                {
                    case InputType.AMT:
                        tb.MaxLength = 16;
                        tb.Text = "¥0.00";
                        tb.CaretIndex = 2;
                        tb.TextChanged += tb_TextAmtTypeChanged;
                        break;
                    case InputType.DIGITAL:
                        tb.TextChanged += tb_TextDigitalTypeChanged;
                        break;
                    case InputType.UNSINDIGITAL:
                        tb.TextChanged += tb_TextUnsDigitalTypeChanged;
                        break;
                    case InputType.UNSINDIGITAL_EXT:
                        tb.TextChanged += tb_TextUnsDigitalExtTypeChanged;
                        break;
                    case InputType.UNSINFLOAT:
                        tb.TextChanged += tb_TextUnsFloatTypeChanged;
                        break;
                    case InputType.UNSCOUNT:
                        tb.TextChanged += tb_TextUnsCountTypeChanged;
                        tb.GotFocus += tb_TextUnsCountGotFocus;
                        break;
                }
            }
            #endregion
        }

        private static void tb_TextDigitalTypeChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (null != tb)
            {
                string newtext = tb.Text;
                int oldLength = newtext.Length;
                string newvalue = "";
                int index = tb.CaretIndex;
                for (int i = 0; i < newtext.Length; i++)
                {
                    if (-1 == "-0123456789".IndexOf(newtext.ElementAt(i)))
                    {
                        continue;
                    }
                    newvalue += newtext.ElementAt(i);
                }
                tb.TextChanged -= tb_TextDigitalTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_TextDigitalTypeChanged;
                if ((newvalue.Length == oldLength - 1))
                {

                    if (index > 0)
                    {
                        tb.CaretIndex = index - 1;
                    }
                    else
                    {
                        tb.CaretIndex = 0;
                    }
                }
                else
                {
                    tb.CaretIndex = index;
                }
            }
        }

        private static void tb_TextUnsDigitalTypeChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (null != tb)
            {
                string newtext = tb.Text;
                int oldLength = newtext.Length;
                string newvalue = "";
                int index = tb.CaretIndex;
                for (int i = 0; i < newtext.Length; i++)
                {
                    if (-1 == "0123456789".IndexOf(newtext.ElementAt(i)))
                    {
                        continue;
                    }
                    newvalue += newtext.ElementAt(i);
                }
                tb.TextChanged -= tb_TextUnsDigitalTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_TextUnsDigitalTypeChanged;
                if ((newvalue.Length == oldLength - 1))
                {

                    if (index > 0)
                    {
                        tb.CaretIndex = index - 1;
                    }
                    else
                    {
                        tb.CaretIndex = 0;
                    }
                }
                else
                {
                    tb.CaretIndex = index;
                }
            }
        }

        private static void tb_TextUnsDigitalExtTypeChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (null != tb)
            {
                string newtext = tb.Text;
                int oldLength = newtext.Length;
                string newvalue = "";
                int index = tb.CaretIndex;
                for (int i = 0; i < newtext.Length; i++)
                {
                    if (-1 == "0123456789".IndexOf(newtext.ElementAt(i)))
                    {
                        continue;
                    }
                    newvalue += newtext.ElementAt(i);
                }
                if (newvalue == "")
                {
                    newvalue = "0";
                }
                tb.TextChanged -= tb_TextUnsDigitalExtTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_TextUnsDigitalExtTypeChanged;
                if ((newvalue.Length == oldLength - 1))
                {

                    if (index > 0)
                    {
                        tb.CaretIndex = index - 1;
                    }
                    else
                    {
                        tb.CaretIndex = 0;
                    }
                }
                else
                {
                    tb.CaretIndex = index;
                }
            }
        }

        private static void tb_TextUnsFloatTypeChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (null != tb)
            {
                string newtext = tb.Text;
                int oldLength = newtext.Length;
                string newvalue = "";
                int index = tb.CaretIndex;
                bool hasDot = false;
                for (int i = 0; i < newtext.Length; i++)
                {
                    if (-1 == "0123456789.".IndexOf(newtext.ElementAt(i)))
                    {
                        continue;
                    }
                    if ('.' == newtext.ElementAt(i))
                    {
                        if (hasDot)
                        {
                            continue;
                        }
                        hasDot = true;
                    }
                    newvalue += newtext.ElementAt(i);
                }
                tb.TextChanged -= tb_TextUnsFloatTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_TextUnsFloatTypeChanged;
                if ((newvalue.Length == oldLength - 1))
                {
                    if (index > 0)
                    {
                        tb.CaretIndex = index - 1;
                    }
                    else
                    {
                        tb.CaretIndex = 0;
                    }
                }
                else
                {
                    tb.CaretIndex = index;
                }
            }
        }

        private static void tb_TextUnsCountGotFocus(object sender, RoutedEventArgs e)
        { 
            TextBox tb = sender as TextBox;
            if (null != tb)
            {
                tb.SelectAll();
            }
        }
        private static void tb_TextUnsCountTypeChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (null != tb)
            {
                string newtext = tb.Text;
                int oldLength = newtext.Length;
                string newvalue = "";
                int index = tb.CaretIndex;
                bool hasDot = false;
                for (int i = 0; i < newtext.Length; i++)
                {
                    if (-1 == "0123456789.".IndexOf(newtext.ElementAt(i)))
                    {
                        continue;
                    }
                    if (newtext.ElementAt(i) == '.')
                    {
                        if (hasDot)
                        {
                            continue;
                        }
                        hasDot = true;
                    }
                    newvalue += newtext.ElementAt(i);
                }
                if ("" == newvalue)
                {
                    newvalue = "0";
                }
                tb.TextChanged -= tb_TextUnsCountTypeChanged;
                tb.Text = decimal.Parse(newvalue).ToString();
                tb.TextChanged += tb_TextUnsCountTypeChanged;
                if ((newvalue.Length == oldLength - 1))
                {
                    if (index > 0)
                    {
                        tb.CaretIndex = index - 1;
                    }
                    else
                    {
                        tb.CaretIndex = 0;
                    }
                }
                else
                {
                    tb.CaretIndex = index;
                }
            }
        }

        private static void tb_TextAmtTypeChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string newtext = tb.Text;
            if ("" == newtext)
            {
                tb.TextChanged -= tb_TextAmtTypeChanged;
                tb.Text = "¥0.00";
                tb.CaretIndex = 2;
                tb.TextChanged += tb_TextAmtTypeChanged;
                return;
            }
            int oldLength = newtext.Length;
            string newIntager = "";
            string newFlt = "";
            int index  =  tb.CaretIndex;
            if ("¥" != newtext.Substring(0, 1)) {
                index++;
            }
            int dot = 0;
            for (int i = 0; i < newtext.Length; i++) {
                if ('.' == newtext.ElementAt(i)) {
                    dot++;
                }
                if (-1 != "-0123456789".IndexOf(newtext.ElementAt(i))) {
                    if (dot > 0)
                    {
                        if ('-' != newtext.ElementAt(i))
                        {
                            newFlt += newtext.ElementAt(i);
                        }
                    }
                    else {
                        if ('-' == newtext.ElementAt(i))
                        {
                            if (newIntager.Length == 0)
                            {
                                newIntager += newtext.ElementAt(i);
                            }
                            else if (newIntager == "0") {
                                newIntager = "-0";
                            }
                            else if (newIntager == "-")
                            {
                                newIntager = "";
                            }
                        }
                        else
                        {
                            newIntager += newtext.ElementAt(i);
                        }
                    }
                }
            }
            if ("" == newIntager) {
                newIntager = "0";
            }
            if (newFlt.Length < 2) {
                newFlt += "00";
            }
            string newValue = "";
            if ("-" == newIntager || "-0" == newIntager)
            {
                newValue = "¥-0." + newFlt.Substring(0, 2);
            }
            else
            {
                newValue = "¥" + long.Parse(newIntager).ToString() + "." + newFlt.Substring(0, 2);
            }
            int newLength = newValue.Length;
            tb.TextChanged -= tb_TextAmtTypeChanged;
            tb.Text = newValue;
            tb.TextChanged += tb_TextAmtTypeChanged;
            if ((newLength == oldLength - 1) && (index < oldLength - 2))
            {
                if (index > 0)
                {
                    tb.CaretIndex = index - 1;
                }
                else
                {
                    tb.CaretIndex = 0;
                }
            }
            else
            {
                tb.CaretIndex = index;
            }
            if ("¥0.00" == tb.Text)
            {
                tb.CaretIndex = 2;
            }
            if ("¥-0.00" == tb.Text)
            {
                tb.CaretIndex = 3;
            }
            if (dot > 1)
            {
                tb.CaretIndex = newValue.Length - 2;
            }
        }
        #endregion
    }
}
