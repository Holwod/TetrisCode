using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace _01
{
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        //声明二维数组存放每个方块的信息
        //0表示颜色没有改变，1表示颜色发生改变
        private bool[,] struArr = new bool[5,5];
        //声明一个颜色类型的变量
        private Color blockColor = Color.Red;
        //声明Config类进行信息的读取或保存工作
        private Config config = new Config();


        private void Form1_Load(object sender, EventArgs e)
        {
            config.LoadFromXmlFile(); //读取Xml文件
            InfoArr info = config.Info;
            //读取砖块样式
            ListViewItem myItem = new ListViewItem();
            for (int i = 0; i < info.Length; i++)
            {
                myItem = lsvBlockSet.Items.Add(info[i].GetIdStr());
                myItem.SubItems.Add(info[i].GetColorStr());
            }

            //读取快捷键
            txtDown.Text = ((Keys)config.DownKey).ToString();
            txtDown.Tag = config.DownKey;
            txtDrop.Text = ((Keys)config.DropKey).ToString();
            txtDrop.Tag = config.DropKey;
            txtLeft.Text = ((Keys)config.MoveLeftKey).ToString();
            txtLeft.Tag = config.MoveLeftKey;
            txtRight.Text = ((Keys)config.MoveRightKey).ToString();
            txtRight.Tag = config.MoveRightKey;
            txtDeasil.Text = ((Keys)config.DeasilRotateKey).ToString();
            txtDeasil.Tag= config.DeasilRotateKey;
            txtContra.Text = ((Keys)config.ContraRotateKey).ToString();
            txtContra.Tag = config.ContraRotateKey;

            //读取环境设置参数
            txtCoorWidth.Text = config.CoorWidth.ToString();
            txtCoorHeight.Text = config.CoorHeight.ToString();  
            txtRectPix.Text = config.RectPix.ToString();
            lblBackColor.BackColor= config.BackColor;
        }

        private void lblMode_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            gp.Clear(Color.Black);
            Pen p = new Pen(Color.White);
            for (int i =31; i < 155; i=i+31) //画横白线
            {
                //(什么样的画笔，从哪一个点开始画的坐标x,y,画到哪一个点结束x,y)
                gp.DrawLine(p,1,i,155,i);
            }
            for (int i = 31; i < 155; i =i+31) //画竖白线
            {
                gp.DrawLine(p,i,1,i,155);
            }

            //填充方块的代码
            SolidBrush s = new SolidBrush(blockColor);
            for (int x = 0;  x < 5; x++)
            {
                for (int y = 0; y <5; y++)
                {
                    if (struArr[x,y])
                    {
                        gp.FillRectangle(s, 31 * x + 1, 31 * y + 1, 30, 30);
                    }
                }
            }
        }

        private void lblMode_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) //判断是否点击鼠标左键
            {
                return;
            }
            int xPos, yPos; //xPos表示数组的第一个下标，yPos表示数组的第二个下标
            xPos = e.X / 31; //e.X鼠标点击的坐标X
            yPos = e.Y / 31;
            struArr[xPos,yPos] = !struArr[xPos,yPos];
            bool b = struArr[xPos,yPos];
            Graphics gp = lblMode.CreateGraphics();
            //刷子
            SolidBrush s = new SolidBrush(b ? blockColor : Color.Black); //1为红色，0为黑色
            //(bursh,左上角的坐标值x,y,width,hight)
            gp.FillRectangle(s,31*xPos+1,31*yPos+1,30,30);
            gp.Dispose();
        }

        private void lblColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();  //打开颜色对话框
            blockColor = colorDialog1.Color; //改变颜色
            lblColor.BackColor = colorDialog1.Color; 
            lblMode.Invalidate(); //使lblMode重置，既执行他的paint事件
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool isEmpty = false;//首先找图案是否为空
            foreach (bool i in struArr)
            {
                if (i)
                {
                    isEmpty = true;
                    break;
                }
            }
            if (!isEmpty)
            {
                MessageBox.Show("图案为空，请先用鼠标点击左边窗口绘制图案！","提示窗口",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            StringBuilder sb = new StringBuilder(25); //把砖块信息转为字符串
            foreach (bool i in struArr)
            {
                sb.Append(i ? "1" : "0");
            }
            string blockString = sb.ToString();
            //再检查是否有重复的图案
      
            foreach (ListViewItem item in lsvBlockSet.Items)
            {
                if (item.SubItems[0].Text == blockString)
                {
                    MessageBox.Show("该图案以存在！", "提示窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
               
            }
            //把新砖块图案信息添加进ListView
            ListViewItem myItem = new ListViewItem();
            myItem = lsvBlockSet.Items.Add(blockString);
            myItem.SubItems.Add(Convert.ToString(blockColor.ToArgb())) ;
        }

        private void lsvBlockSet_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected) //避免重复执行事件
            {
                blockColor = Color.FromArgb(int.Parse(e.Item.SubItems[1].Text)); //把字符串信息转换为颜色类
                lblColor.BackColor= blockColor;
                string s = e.Item.SubItems[0].Text;//取砖块样式信息
                for (int i = 0; i < s.Length; i++)//把砖块样式信息从字符串转换为二维数组
                {
                    struArr[i / 5, i % 5] = (s[i] == '1') ? true : false;
                }
                lblMode.Invalidate();   
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (lsvBlockSet.SelectedItems.Count == 0)
            {
                MessageBox.Show("请在右边选择一个条目进行删除！","提示窗口",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return; 
            }
            lsvBlockSet.Items.Remove(lsvBlockSet.SelectedItems[0]);
            btnClear.PerformClick();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 5; x++)   //清空画板
            {
                for (int y = 0; y < 5; y++)
                {
                    struArr[x, y] = false;
                }
            }
            lblMode.Invalidate();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lsvBlockSet.SelectedItems.Count == 0)
            {
                MessageBox.Show("请在右边窗口选择一个进行修改！","提示窗口",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            bool isEmpty = false;
            foreach (bool  i in struArr)
            {
                if (i)
                {
                    isEmpty = true;
                    break;
                }
            }
            if (!isEmpty)
            {
                MessageBox.Show("图案为空，请先用鼠标点击左边窗口绘制图案再进行修改！", "提示窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            StringBuilder sb = new StringBuilder(25);
            foreach (bool i in struArr)
            {
                sb.Append(i ? "1" : "0");
            }
            lsvBlockSet.SelectedItems[0].SubItems[0].Text = sb.ToString();
            lsvBlockSet.SelectedItems[0].SubItems[1].Text = Convert.ToString(blockColor.ToArgb());
        }

        private void gbKeySet_Enter(object sender, EventArgs e)
        {

        }

        private void txtContra_KeyDown(object sender, KeyEventArgs e)
        {
            //排除不适合的键
            if ((e.KeyValue >= 33 && e.KeyValue <=36)||(e.KeyValue >= 45 && e.KeyValue<=46)||
                (e.KeyValue >= 48 && e.KeyValue <= 57) || (e.KeyValue >= 65 && e.KeyValue <= 90)||
                (e.KeyValue >= 96 && e.KeyValue <= 107) || (e.KeyValue >= 109 && e.KeyValue <= 111)||
                (e.KeyValue >= 186 && e.KeyValue <= 192) || 
                (e.KeyValue >= 219 && e.KeyValue <= 222)) 
            {
                //检查是否存在冲突的快捷键
                foreach (Control c  in gbKeySet.Controls)
                {
                   Control TempC = c as TextBox;
                    if (TempC != null && ((TextBox)TempC).Text != "")
                    {
                        if (((int)((TextBox)TempC).Tag) ==e.KeyValue)
                        {
                            ((TextBox)TempC).Text = "";
                            ((TextBox)TempC).Tag = Keys.None;
                        }
                    }
                }
                ((TextBox)sender).Text = e.KeyCode.ToString();
                ((TextBox)sender).Tag = (Keys)e.KeyValue;
            }
        }

        private void lblBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(); //打开颜色对话框
            lblBackColor.BackColor= colorDialog1.Color;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            InfoArr info = new InfoArr();
            foreach (ListViewItem item in lsvBlockSet.Items)
            {
                info.Add(item.SubItems[0].Text, item.SubItems[1].Text);
            }
            config.Info = info; //把info赋值给coinfig对象的Info属性
            config.DownKey = (Keys)txtDown.Tag;
            config.DropKey= (Keys)txtDrop.Tag;  
            config.MoveLeftKey =(Keys)txtLeft.Tag;
            config.MoveRightKey=(Keys)txtRight.Tag;
            config.DeasilRotateKey = (Keys)txtDeasil.Tag;
            config.ContraRotateKey=(Keys)txtContra.Tag;
            config.CoorWidth = int.Parse(txtCoorWidth.Text);
            config.CoorHeight = int.Parse(txtCoorHeight.Text);
            config.RectPix =int.Parse(txtRectPix.Text);
            config.BackColor = lblBackColor.BackColor;
            config.SaveToXmlFile();  //保存为xml文件
        }

        private void btnColse_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
