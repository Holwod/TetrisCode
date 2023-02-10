using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _01
{
    class InfoArr 
    {
        //砖块信息数组
        
        private ArrayList info = new ArrayList(); //砖块信息集合   
        private int _length = 0;  //砖块信息数

        public int Length 
        {
            get { return _length; }
        }

        public BlockInfo this[int index]  //根据下标，返回一个BlockInfo
        {
            get { return (BlockInfo)info[index]; }
        }

        public string this[string id] //索引器，根据一个字符串的id的值的下标，给相应id的颜色赋值
        {
            set 
            {
                if (value == "")
                {
                    return;
                }
                for (int i = 0; i < info.Count; i++)
                {
                    if (((BlockInfo)info[i]).GetIdStr() == id)
                    {
                        try
                        {
                            ((BlockInfo)info[i]).Bcolor = Color.FromArgb(Convert.ToInt32(value));
                        }
                        catch (System.FormatException)
                        {
                            MessageBox.Show("颜色信息有误！请删除BlockSet.xml文件，并重新启动程序","错误信息",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private BitArray StrToBit(string id)  //把字符串转换为BitArray
        {
            if (id.Length != 25) 
            {
                throw new System.FormatException("砖块样式信息不合法！请删除BlockSet.xml文件，并重新启动");
            }
            BitArray ba = new BitArray(25);
            for (int i = 0; i < 25; i++)
            {
                ba[i] = (id[i] == '0') ? false : true;
            }
            return ba;
        }

        public void Add(BitArray id, Color bColor) //添加一个砖块信息
        {
            if (id.Length !=25)
            {
                throw new System.FormatException("砖块样式信息不合法！请删除BlockSet.xml文件，并重新启动");
            }
            info.Add(new BlockInfo(id,bColor)); //给动态数组info添加一个砖块信息
            _length++; //长度加1
        }

        public void Add(string id, string bColor)
        {
            Color temp;
            if (!(bColor ==""))
            {
                temp = Color.FromArgb(Convert.ToInt32(bColor)); //把字符串转换为颜色类
            }
            else 
            {
                temp = Color.Empty;
            }
            info.Add(new BlockInfo(StrToBit(id),temp)); //把字符串转换为BitArray类
            _length++;
        }

    }
}
