using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 图片识别
{
    class CommonDef
    {
        /// <summary>
        /// 从图片中抓取像素值层数4层 3x3=9个
        /// </summary>
        public static int IMAGE_TAG_LENGTH = 5;

        /// <summary>
        /// 图片大小
        /// </summary>
        public static int IMAGE_DEFAULT_SIZE = 35;
        /// <summary>
        /// 容差
        /// </summary>
        public static int COLOR_TOLERANCE = 80;
    }
}
