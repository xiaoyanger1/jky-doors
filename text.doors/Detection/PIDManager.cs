using text.doors.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace text.doors.Detection
{
    public partial class PIDManager : Form
    {
        private TCPClient tcpClient;
        public PIDManager(TCPClient tcpConnection)
        {
            InitializeComponent();
            this.tcpClient = tcpConnection;
            Init();
        }
        private void Init()
        {
            if (!tcpClient.IsTCPLink)
                MessageBox.Show("连接未打开暂时不能设置PID！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

            bool IsSuccess = false;
            var P = tcpClient.GetPID("P", ref IsSuccess);
            var I = tcpClient.GetPID("I", ref IsSuccess);
            var D = tcpClient.GetPID("D", ref IsSuccess);

            txthp.Text = P.ToString();
            txthi.Text = I.ToString();
            txthd.Text = D.ToString();

        }

        private void btnhp_Click(object sender, EventArgs e)
        {
            double P = int.Parse(txthp.Text);
            var res = tcpClient.SendPid("P", P);
            if (!res)
            {
                MessageBox.Show("连接未打开暂时不能设置PID！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }

        }

        private void btnhi_Click(object sender, EventArgs e)
        {
            double I = int.Parse(txthi.Text);
            var res = tcpClient.SendPid("I", I);
            if (!res)
            {
                MessageBox.Show("连接未打开暂时不能设置PID！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }

        private void btnhd_Click(object sender, EventArgs e)
        {
            double D = int.Parse(btnhd.Text);
            var res = tcpClient.SendPid("D", D);
            if (!res)
            {
                MessageBox.Show("连接未打开暂时不能设置PID！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }
    }
}
