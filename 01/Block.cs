using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01
{
    class Block
    {
        protected Point[] structArr; //C#中专门存放坐标值的类型Point （会装箱拆箱）
        protected int _xPos; //砖块中心所在的x坐标
        protected int _yPos;//砖块中心点所在的y坐标
        protected Color _blockColor; //砖块颜色
        protected Color disapperColor; //擦除颜色
        protected int rectPix;//每单元格像素

        public Block() { }

        public Block(Point[] sa, Color bColor, Color dColor, int pix)
        {
            //重载构造函数，给成员变量赋值
            _blockColor = bColor;
            disapperColor = dColor;
            rectPix= pix;
            structArr= sa;
        }



        public Point this[int index]  //根据索引反向砖块里小作标
        {
            get { return structArr[index]; }
        }

        public int Length //属性，表示structArr的长度
        {
            get { return structArr.Length; }
        }

        #region 成员变量的属性
        public int XPos
        {
            get { return _xPos; }
            set { _xPos = value; }
        }

        public int YPos
        {
            get { return _yPos; }
            set { _yPos = value; }
        }

        public Color BlockColor
        {
            get { return _blockColor; }
        }
        #endregion

        public void DeasilRotate() //顺时针旋转
        {
            int temp;
            for (int i = 0; i < structArr.Length; i++)
            {
                temp = structArr[i].X;
                structArr[i].X = structArr[i].Y;
                structArr[i].Y = -temp;
            }
        }

        public void ContraRotate()  //逆时针旋转
        {
            int temp;
            for (int i = 0; i < structArr.Length; i++)
            {
                temp = structArr[i].X;
                structArr[i].X = -structArr[i].Y;
                structArr[i].Y = temp;  
            }
        }

        private Rectangle PointToRect(Point p) //把一个坐标点转化为画布坐标 {矩阵正方形}
        {
            return new Rectangle((_xPos + p.X) * rectPix + 1,
                                    (_yPos - p.Y) * rectPix + 1,
                                     rectPix - 2,
                                     rectPix - 2);
        }

        public virtual void Paint(Graphics gp)  //在指定画板下绘制砖块
        {
            SolidBrush sb = new SolidBrush(_blockColor);
            foreach (Point p in structArr)
            {
                lock(gp) //线程同步锁
                {
                    gp.FillRectangle(sb,PointToRect(p));

                }
            }
        }

        public void erase(Graphics gp)  //擦除矩形
        {
            SolidBrush sb = new SolidBrush(disapperColor);
            foreach (Point p in structArr)
            {
                lock (gp) 
                {
                    gp.FillRectangle(sb, PointToRect(p));
                }
            }
        } 
    }
}
