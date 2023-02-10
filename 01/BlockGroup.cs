using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01
{
    class BlockGroup
    {
        private InfoArr info; //存放所有砖块样式信息
        private Color disapperColor; //背景色
        private int rectPix; //单元格像素

        public BlockGroup()
        { 
            Config config = new Config();
            config.LoadFromXmlFile();
            info = new InfoArr();
            info = config.Info;
            disapperColor = config.BackColor;
            rectPix = config.RectPix;
        }

        public Block GetABlock() //从砖块中随机抽取一个砖块样式并返回
        { 
            Random rd= new Random();
            int keyOrder = rd.Next(0,info.Length);
            BitArray ba = info[keyOrder].ID;
            int struNum = 0;
            foreach (bool b in ba)
            {
                if (b)
                {
                    struNum++;
                }
            }
            Point[] structArr = new Point[struNum];//新建一个Point数组，并确定其长度，以创建新的Block
            int k = 0;
            for (int j = 0; j < ba.Length; j++)  //用循环给Point数组structArr赋值
            {
                if (ba[j])
                {
                    structArr[k].X = j / 5 - 2;
                    structArr[k].Y = 2 - j % 5;
                    k++;
                }
            }
            return new Block(structArr, info[keyOrder].Bcolor, disapperColor, rectPix);//创建一个新砖块并返回
        }
    }
}
