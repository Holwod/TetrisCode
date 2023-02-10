using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _01
{
    public partial class FrmTetris : Form
    {
        public FrmTetris()
        {
            InitializeComponent();
        }

        private Palette p;
        private Keys downKey;
        private Keys dropKey;
        private Keys moveLeftKey;
        private Keys moveRightKey;
        private Keys deasilRotateKey;
        private Keys contraRotateKey;
        private int paletteWidth; //画板宽度
        private int paletteHeight; //画板高度
        private Color paletteColor; //背景色
        private int rectPix; //单元格像素

       
        private void btnStart_Click(object sender, EventArgs e)
        {
            txtScore.BackColor= Color.Black;
            txtScore.ForeColor= Color.ForestGreen;
            txtScore.Text = "0";
            if (p != null)
            {
                p.Close();
            }
            p = new Palette(paletteWidth,paletteHeight,rectPix,paletteColor,
                    Graphics.FromHwnd(pbRun.Handle),
                    Graphics.FromHwnd(lblReady.Handle));
            if (txtSpeed.Text.Trim() == "")
            {
                MessageBox.Show("请输入游戏难度默认为800");
            }
            else 
            {
                if (Regex.IsMatch(txtSpeed.Text.Trim(), @"^[0-9]{1,4}$") )
                {
                    p.TimeSpan = Convert.ToInt32(txtSpeed.Text.Trim());
                    p.Start();
                }
                else 
                {
                    MessageBox.Show("请输入数字！！！范围在0-1200","错误提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
            
           

        }

        private void pbRun_Paint(object sender, PaintEventArgs e)
        {
            if (p!=null)
            {
                p.PaintPalette(e.Graphics);
            }
        }

        private void lblReady_Paint(object sender, PaintEventArgs e)
        {
           
            if (p!=null)
            {
                p.PaintReady(e.Graphics);
            }
        }

        private void FrmTetris_Load(object sender, EventArgs e)
        {
            Config config= new Config();
            config.LoadFromXmlFile();
            downKey = config.DownKey;
            dropKey = config.DropKey;
            moveLeftKey = config.MoveLeftKey;
            moveRightKey = config.MoveRightKey;
            deasilRotateKey= config.DeasilRotateKey;
            contraRotateKey = config.ContraRotateKey;
            paletteWidth = config.CoorWidth;
            paletteHeight =config.CoorHeight;
            paletteColor = config.BackColor;
            rectPix = config.RectPix;
            this.Width = paletteWidth * rectPix + 200;
            this.Height = paletteHeight * rectPix + 58;
            pbRun.Width = paletteWidth * rectPix;
            pbRun.Height = paletteHeight * rectPix;
            txtSpeed.Text = "800";
        }

        private void FrmTetris_KeyDown(object sender, KeyEventArgs e)
        {
            txtScore.Text = p.Score.ToString();

            if (p != null)
            {
                if (e.KeyValue == 32)  //屏蔽回车键
                {
                    e.Handled = true;
                }
                if (e.KeyCode == downKey)
                {
                    p.Down();
                }
                if (e.KeyCode == dropKey)
                {
                    p.Drop();
                }
                if (e.KeyCode == moveLeftKey)
                {
                    p.MoveLeft();
                }
                else if (e.KeyCode == moveRightKey)
                {
                    p.MoveRight();
                }
                else if (e.KeyCode == Keys.N)
                {
                    p.DeasilRotate();
                }
                else if (e.KeyCode == Keys.M)
                {
                    p.ContraRotate();
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (p == null)
            {
                panel1.BackColor = Color.White;
                return;
                
            }
            if (button1.Text == "暂停")
            {
                p.Pause();
                button1.Text = "继续";
                panel1.BackColor = Color.Red;
            }
            else 
            {
                p.EndPouse();
                button1.Text = "暂停";
                panel1.BackColor = Color.Green;
            }

        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            if (button1.Text =="暂停")
            {
                button1.PerformClick();
            }
            //using 自动生成try finally 
            using (FrmConfig frmConfig = new FrmConfig())
            {
                frmConfig.ShowDialog();
            }
        }

        private void FrmTetris_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (p!= null)
            {
                p.Close();
            }
        }

        private void txtSpeed_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSpeed_MouseDown(object sender, MouseEventArgs e)
        {
           
        }
    }
}
