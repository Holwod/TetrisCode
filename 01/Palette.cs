﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace _01
{
    class Palette
    {
        private int _width = 15; //画板宽度
        private int _height = 25;//画板高度
        private Color[,] coorArr;//固定画板数组
        private Color disapperColor;//背景色
        private Graphics gpPalette; //砖块活动画板
        private Graphics gpReady; //下一个砖块样式画板
        private BlockGroup bGroup; //砖块生产机
        private Block runBlock;//正在活动的砖块
        private Block readyBlock;//下一个砖块
        private int rectPix; //单元格像素
        private int score; //计分板的分数
        public int Score { get { return score; } }

        private System.Timers.Timer timerBlock;  //定时器
        private int timeSpan = 800; //定时器的时间间隔

        public int TimeSpan{ set {timeSpan = value ; } }

        public Palette(int x, int y, int pix, Color dColor, Graphics gp, Graphics gr)
        {
            _width = x;
            _height = y;
            coorArr = new Color[_width, _height];
            disapperColor = dColor;
            gpPalette = gp;
            gpReady = gr;
            rectPix = pix;
        }

        public void Start() //游戏开始
        {
            score= 0;
            bGroup = new BlockGroup();
            runBlock = bGroup.GetABlock();
            runBlock.XPos = _width / 2;
            int y = 0;
            for (int i = 0; i < runBlock.Length; i++)
            {
                if (runBlock[i].Y > y)
                {
                    y = runBlock[i].Y;
                }
            }
            runBlock.YPos = y;
            gpPalette.Clear(disapperColor); //清空画板
            runBlock.Paint(gpPalette); //运行砖块
            Thread.Sleep(20);//间隔太短会产生一样的砖块
            readyBlock = bGroup.GetABlock();
            readyBlock.XPos = 2;
            readyBlock.YPos = 2;
            gpReady.Clear(disapperColor);
            readyBlock.Paint(gpReady);

            timerBlock = new System.Timers.Timer(timeSpan);
            timerBlock.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timerBlock.AutoReset = true;
            timerBlock.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            CheckAndOverBlock();
            Down();
        }

        public bool Down() //砖块向下移动一个单元格
        {
            int xPos = runBlock.XPos;
            int yPos = runBlock.YPos + 1;
            for (int i = 0; i < runBlock.Length; i++)
            {
                if (yPos - runBlock[i].Y > _height - 1) //如果超出下边界则失败
                {
                    return false;
                }
                if (!coorArr[xPos + runBlock[i].X, yPos - runBlock[i].Y].IsEmpty)//如果下面有对象挡则失败
                {
                    return false;
                }
            }
            runBlock.erase(gpPalette);
            runBlock.YPos++;
            runBlock.Paint(gpPalette);
            return true;
        }

        public void Drop() //丢下砖块
        {
            timerBlock.Stop();
            while(Down());
            timerBlock.Start();
        }

        public void MoveLeft() //向左移动一个单元格
        {
            int xPos = runBlock.XPos - 1;
            int yPos = runBlock.YPos;
            for (int i = 0; i < runBlock.Length; i++)
            {
                if (xPos + runBlock[i].X < 0)
                {
                    return;
                }
                if (!coorArr[xPos + runBlock[i].X, yPos - runBlock[i].Y].IsEmpty)
                {
                    return;
                }   
            }
            runBlock.erase(gpPalette);
            runBlock.XPos--;
            runBlock.Paint(gpPalette);
        }

        public void MoveRight()
        {
            int xPos = runBlock.XPos + 1;
            int yPos = runBlock.YPos;
            for (int i = 0; i < runBlock.Length; i++)
            {
                if (xPos + runBlock[i].X > _width - 1)
                {
                    return;
                }
                if (!coorArr[xPos + runBlock[i].X, yPos - runBlock[i].Y].IsEmpty)
                {
                    return;
                }

            }
            runBlock.erase(gpPalette);
            runBlock.XPos++;
            runBlock.Paint(gpPalette);
        }

        public void DeasilRotate() //顺时针旋转
        {
            for (int i = 0; i < runBlock.Length; i++)
            {
                int x = runBlock.XPos + runBlock[i].Y;
                int y = runBlock.YPos + readyBlock[i].X;
                if (x < 0 || x > _width - 1)
                {
                    return;
                }
                if (y < 0 || y > _height - 1)
                {
                    return;
                }
                if (!coorArr[x, y].IsEmpty)
                {
                    return;
                }
            }
            runBlock.erase(gpPalette);
            runBlock.DeasilRotate();
            runBlock.Paint(gpPalette);
        }
       
        public void ContraRotate() //逆时针旋转
        {
            for (int i = 0; i < runBlock.Length; i++)
            {
                int x = runBlock.XPos - runBlock[i].Y;
                int y = runBlock.YPos - readyBlock[i].X;
                if (x < 0 || x > _width - 1)
                {
                    return;
                }
                if (y < 0 || y > _height - 1)
                {
                    return;
                }
                if (!coorArr[x, y].IsEmpty)
                {
                    return;
                }
            }
            runBlock.erase(gpPalette);
            runBlock.ContraRotate();
            runBlock.Paint(gpPalette);
        }

        private void PaintBackground(Graphics gp)  //重置画板背景颜色
        {
            //画固定的砖块
            gp.Clear(Color.Black);
            for (int i = 0; i < _height; i++) //访问coorArr二维数组
            {
                for (int j = 0; j < _width; j++)
                {
                    if (!coorArr[j, i].IsEmpty)
                    {
                        SolidBrush sb = new SolidBrush(coorArr[j, i]);
                        gp.FillRectangle(sb, j * rectPix + 1,
                                         i * rectPix + 1,
                                         rectPix - 2,
                                         rectPix - 2);
                            
                    }
                }
            }
        }

        public void PaintPalette(Graphics gp) //画整块画板
        {
            PaintBackground(gp); //画背景
            if (runBlock!=null) //画活动的砖块
            {
                runBlock.Paint(gp);
            }
        }

        public void PaintReady(Graphics gp) //重画下一个砖块
        {
            if (readyBlock!= null)
            {
                readyBlock.Paint(gp);
            }
        }

        public void CheckAndOverBlock() //检查砖块是否到底，到底则产生新的砖块
        {
            bool over = false;
            for (int i = 0; i < runBlock.Length; i++)
            {
                int x = runBlock.XPos + runBlock[i].X;
                int y = runBlock.YPos - runBlock[i].Y;
                if (y == _height - 1)  //如果到达下边界，则当前砖块结束
                {
                    over = true;
                    break;
                }
                if (!coorArr[x, y + 1].IsEmpty) //如果下面有其他砖块，则当前砖块结束
                {
                    over = true;
                    break;
                }
            }
            if (over)//是否结束
            {
                for (int i = 0; i < runBlock.Length; i++)
                {
                    coorArr[runBlock.XPos + runBlock[i].X, runBlock.YPos - runBlock[i].Y] = runBlock.BlockColor;
                }
                CheckAndDelFullRow(); //在新砖块产生之前清除满行
                //产生新的砖块
                runBlock = readyBlock;
                runBlock.XPos = _width / 2;
                int y = 0;
                for (int i = 0; i < runBlock.Length; i++)
                {
                    if (runBlock[i].Y > y) 
                    {
                        y = runBlock[i].Y;
                    }
                }
                runBlock.YPos= y;
                //检查新产生的砖块所占用的地方是否已经有砖块，有则游戏结束
                for (int i = 0; i < runBlock.Length; i++)
                {
                    if (!coorArr[runBlock.XPos + runBlock[i].X, runBlock.YPos - runBlock[i].Y].IsEmpty) 
                    {
                        //游戏结束
                        StringFormat drawFormat = new StringFormat();
                        drawFormat.Alignment = StringAlignment.Center;
                        gpPalette.DrawString("GAME OVER",
                                             new Font("Arial Black", 25f),
                                             new SolidBrush(Color.White),
                                             new RectangleF(0, _height * rectPix / 2 - 100, _width * rectPix, 100),
                                             drawFormat); 
                        timerBlock.Stop();
                        return;
                    }
                }

                runBlock.Paint(gpPalette);
                //获取新的准备砖块
                readyBlock = bGroup.GetABlock();
                readyBlock.XPos = 2;
                readyBlock.YPos = 2;
                gpReady.Clear(Color.Black);
                readyBlock.Paint(gpReady);
            }
        }

        private void CheckAndDelFullRow() //检查并删除满行
        {
            //找出当前砖块所在行的1范围
            int lowRow = runBlock.YPos - runBlock[0].Y;  //当前砖块x最小值
            int highRow = lowRow; //当前砖块y的最大值
            for (int i = 1; i < runBlock.Length; i++)
            {
                int y = runBlock.YPos - runBlock[i].Y;
                if (y < lowRow) 
                {
                    lowRow = y;
                }
                if (y > highRow)
                {
                    highRow= y;
                }
            }
            bool repaint = false; //判断是否重画标志
            for (int i = lowRow; i <= highRow; i++) //检查清行，有则删除一行
            {
                bool rowFull = true; //是否满行标值
                for (int j = 0; j < _width; j++)
                {
                    if (coorArr[j,i].IsEmpty)
                    {
                        rowFull = false;
                        break;
                    }
                }
                if (rowFull) //如果清行，则删除这一行
                {
                    repaint = true;
                    for (int k = i; k > 0; k--)
                    {
                        for (int j = 0; j < _width; j++)
                        {
                            coorArr[j, k] = coorArr[j, k - 1];
                        }
                    }
                    for (int j = 0; j < _width; j++) //清空第0行
                    {
                        coorArr[j, 0] = Color.Empty;
                    }
                    //计分
                    score +=100;
                }
            }
            if (repaint) //重画
            {
                PaintBackground(gpPalette);
            }
        }

        public void Pause() //暂停
        {
            if (timerBlock.Enabled == true)
            {
                timerBlock.Enabled = false;
            }
        }

        public void EndPouse() //结束暂停
        {
            if (timerBlock.Enabled == false)
            {
                timerBlock.Enabled = true;
            }
        }

        public void Close() //关闭
        {
            timerBlock.Close();
            gpPalette.Dispose();
            gpReady.Dispose();
        }
    }
}
