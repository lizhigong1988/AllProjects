using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Cryptography;

namespace WindowLib.Tools
{
    public class InputTypeControl
    {
        #region 非空textBox/password提示
        public static bool GetAssertNotEmpty(DependencyObject obj)
        {
            return (bool)obj.GetValue(AssertNotEmptyProperty);
        }

        public static void SetAssertNotEmpty(DependencyObject obj, bool value)
        {
            obj.SetValue(AssertNotEmptyProperty, value);
        }

        public static readonly DependencyProperty AssertNotEmptyProperty =
            DependencyProperty.RegisterAttached("AssertNotEmpty", typeof(bool), 
            typeof(InputTypeControl), new PropertyMetadata(false, OnAssertNotEmptyChanged));

        private static void OnAssertNotEmptyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (obj is TextBox)
                {
                    TextBox tb = obj as TextBox;
                    tb.Background = Brushes.SkyBlue;
                }
                else if (obj is PasswordBox)
                {
                    PasswordBox pb = obj as PasswordBox;
                    pb.Background = Brushes.SkyBlue;
                }
                else if (obj is ComboBox)
                {
                    ComboBox cb = obj as ComboBox;
                    cb.Background = Brushes.SkyBlue;
                }
            }
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
            DATE,           //日期
            TIME,           //时间
            TEXT,
            TEXT_EXT
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
                    case InputType.DATE:
                        tb.TextChanged += tb_TextDateTypeChanged;
                        break;
                    case InputType.TIME:
                        tb.TextChanged += tb_TextTimeTypeChanged;
                        break;
                    case InputType.TEXT:
                        tb.TextChanged += tb_TextTextTypeChanged;
                        break;
                    case InputType.TEXT_EXT:
                        tb.TextChanged += tb_SpecialTextTypeChanged;
                        break;
                }
            }
        }

        private static void tb_TextDateTypeChanged(object sender, TextChangedEventArgs e)
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
                tb.TextChanged -= tb_TextDateTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_TextDateTypeChanged;
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

        static string specialWord = "~#$%^&*_+-={}|[]\\'<>/\n\r\t !@();:\",.?～！＠＃￥％…＆×（）—＋｛｝｜：“《》？·－＝【】＼；‘，。、”’　`";


        private static void tb_TextTextTypeChanged(object sender, TextChangedEventArgs e)
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
                    if (-1 != specialWord.IndexOf(newtext.ElementAt(i)))
                    {
                        continue;
                    }
                    newvalue += newtext.ElementAt(i);
                }
                tb.TextChanged -= tb_TextTextTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_TextTextTypeChanged;
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

        static string totalSpecialWord = "\n\r\t";

        private static void tb_SpecialTextTypeChanged(object sender, TextChangedEventArgs e)
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
                    if (-1 != totalSpecialWord.IndexOf(newtext.ElementAt(i)))
                    {
                        continue;
                    }
                    newvalue += newtext.ElementAt(i);
                }
                tb.TextChanged -= tb_SpecialTextTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_SpecialTextTypeChanged;
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

        private static void tb_TextTimeTypeChanged(object sender, TextChangedEventArgs e)
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
                tb.TextChanged -= tb_TextTimeTypeChanged;
                tb.Text = newvalue;
                tb.TextChanged += tb_TextTimeTypeChanged;
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
