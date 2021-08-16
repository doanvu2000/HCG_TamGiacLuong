using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TamGiacLuong
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtAngleA.Clear();
            txtAngleB.Clear();
            txtAngleC.Clear();
            txtEdgea.Clear();
            txtEdgeb.Clear();
            txtEdgec.Clear();
            txtha.Clear();
            txthb.Clear();
            txthc.Clear();
            txtP.Clear();
            txtS.Clear();
            rtResult.Clear();
            cblRs.SelectedIndex = 0;
            baiToan = new BaiToan();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnShowRules_Click(object sender, EventArgs e)
        {
            StreamReader rs = new StreamReader("Rules.txt");
            rtResult.Text = rs.ReadToEnd();
        }
        public bool validate(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            try
            {
                double x = double.Parse(str);
                if (x <= 0)
                {
                    return false;
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        int vtKL;
        BaiToan baiToan = new BaiToan();
        List<int> list_GT;
        private void btnCal_Click(object sender, EventArgs e)
        {
            rtResult.Clear();
            TapLuatSuDung.Clear();
            if (validate(txtAngleA.Text)) baiToan.Bt[0] = 0;
            if (validate(txtAngleB.Text)) baiToan.Bt[1] = 0;
            if (validate(txtAngleC.Text)) baiToan.Bt[2] = 0;
            if (validate(txtEdgea.Text)) baiToan.Bt[3] = 0;
            if (validate(txtEdgeb.Text)) baiToan.Bt[4] = 0;
            if (validate(txtEdgec.Text)) baiToan.Bt[5] = 0;
            if (validate(txtha.Text)) baiToan.Bt[6] = 0;
            if (validate(txthb.Text)) baiToan.Bt[7] = 0;
            if (validate(txthc.Text)) baiToan.Bt[8] = 0;
            if (validate(txtP.Text)) baiToan.Bt[9] = 0;
            if (validate(txtS.Text)) baiToan.Bt[10] = 0;

            if (cblRs.Text == "")
            {
                MessageBox.Show("Vui lòng chọn thông số cần tính!");
                return;
            }
            else

                if (baiToan.Bt[cblRs.SelectedIndex] == 0)
            {
                MessageBox.Show("Thông số cần tính đã có trong giả thiết của bài toán.\n" +
                "Bạn hãy chọn lại thông số khác!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
                vtKL = cblRs.SelectedIndex;

            string _kl = cblRs.SelectedItem.ToString();
            if (string.IsNullOrEmpty(_kl))
            {
                MessageBox.Show("Vui lòng chọn thông số cần tính!");
                return;
            }
            baiToan.Bt[cblRs.SelectedIndex] = 1;
            list_GT = new List<int>(baiToan.Bt);
            for (int i = 0; i < list_GT.Count; i++)
            {
                if (list_GT[i] == 1)
                {
                    list_GT[i] = -1;
                }
            }
            bool giai = TimTapLuatToiUu();
            if (giai)
            {
                TinhToan();
                BaiToanCanGiai();
                Giai();
                list_GT.Clear();
                baiToan = new BaiToan();
            }
        }

        List<List<int>> TapLuat = new List<List<int>>();
        List<int> TapLuatSuDung = new List<int>();

        private void Form1_Load(object sender, EventArgs e)
        {
            rtResult.SelectionIndent += 20;
            DocTapLuat();
        }

        private void DocTapLuat()
        {
            StreamReader sr = new StreamReader("Rules.txt");
            string luat;
            //đọc tập luật có sẵn theo từng dòng
            while ((luat = sr.ReadLine()) != null)
            {
                themLuat(luat);
            }
        }

        private void themLuat(string luat)
        {
            List<int> r = new List<int>(11);

            //Khởi tạo list với giá trị mặc định = -1
            for (int i = 0; i < 11; i++)
                r.Add(-1);

            int vt = 0; //Vị trí của kí tự hiện tại
            int dodai = 0; //Độ dài của đối số
            int danhdau = 0;//Đánh dấu: 0:GT, 1:KL 

            while (luat[vt == 0 ? vt : vt - 1] != '.')
            {
                if (luat[vt] >= 'A' && luat[vt] <= 'z')
                    dodai++;
                else
                {
                    if (vt > 1)
                        if (luat[vt - 1] == '-' && luat[vt] == '>')
                        {
                            danhdau = 1;
                        }

                    if (dodai != 0)
                    {
                        string kt = luat.Substring(vt - dodai, dodai);//kí tự xét
                        switch (kt)
                        {
                            case "A":
                                r[0] = danhdau;
                                break;
                            case "B":
                                r[1] = danhdau;
                                break;
                            case "C":
                                r[2] = danhdau;
                                break;
                            case "a":
                                r[3] = danhdau;
                                break;
                            case "b":
                                r[4] = danhdau;
                                break;
                            case "c":
                                r[5] = danhdau;
                                break;
                            case "ha":
                                r[6] = danhdau;
                                break;
                            case "hb":
                                r[7] = danhdau;
                                break;
                            case "hc":
                                r[8] = danhdau;
                                break;
                            case "p":
                                r[9] = danhdau;
                                break;
                            case "S":
                                r[10] = danhdau;
                                break;
                        }

                        dodai = 0;
                    }
                }

                vt++;
            }

            //Add to list rules
            TapLuat.Add(r);
        }
        private bool TimTapLuatToiUu()
        {
            //Kiểm tra trạng thái của tất cả các luật
            //gán trạng thái mặc định = -1: không được sử dụng, 0: được sử dụng
            List<int> trangThaiLuat = new List<int>();

            for (int i = 0; i < TapLuat.Count; i++)
            {
                trangThaiLuat.Add(-1);
            }


            bool LuatTrienKhai = false;

            while (list_GT[vtKL] == -1)
            {
                LuatTrienKhai = false;
                for (int i = 0; i < TapLuat.Count; i++)
                {
                    if (trangThaiLuat[i] == -1)
                    {
                        bool khaDung = true;
                        for (int j = 0; j < 11; j++)
                        {
                            //tham số có ở VTrai của luật nhưng giả thiết không đủ
                            if ((TapLuat[i][j] == 0 && list_GT[j] != 0)
                                // Tham số có trong KL
                                || (TapLuat[i][j] == 1 && list_GT[j] == 0))
                            {
                                khaDung = false;
                                break;
                            }
                        }

                        if (khaDung)
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                //Luật thứ i có KL là đáp án
                                if (TapLuat[i][j] == 1)
                                {
                                    list_GT[j] = 0;
                                    break;
                                }
                            }
                            //Thêm vị trí các luật được dùng vào list
                            TapLuatSuDung.Add(i);
                            LuatTrienKhai = true;

                            //kiem tra ket qua da dc tim thay hay chưa
                            if (list_GT[vtKL] == 0)
                                break;

                            trangThaiLuat[i] = 0;
                        }
                    }
                }

                if (!LuatTrienKhai)
                {
                    MessageBox.Show("Thiếu giả thiết. \n Bài toán này không thể giải!",
                        "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (LuatTrienKhai)
            {
                TapLuatSuDung = ToiUuTapLuat(TapLuatSuDung);
            }

            return true;
        }

        private List<int> ToiUuTapLuat(List<int> tapLuatSuDung)// sử dụng tập luật suy diễn tiến 
        {
            List<int> LuatTrungGian = new List<int>();

            //Vị trí của luật cuối cùng trong tapLuatSuDung
            int vtLuatCuoi = tapLuatSuDung.Count - 1;

            LuatTrungGian.Add(tapLuatSuDung[vtLuatCuoi]);

            for (int i = tapLuatSuDung.Count - 2; i >= 0; i--)
            {
                //Kiểm tra xem kết luận của  tập luật ban đầu có nằm ở VP của luật cuối cùng?
                for (int j = 0; j < 11; j++)
                    if (TapLuat[tapLuatSuDung[i]][j] == 1)
                    {
                        //kiểm tra trước khi thêm vào luật trung gian
                        bool tonTai = false; //Thuật toán suy diễn tiến
                        for (int k = i + 1; k < tapLuatSuDung.Count; k++)
                            if (TapLuat[tapLuatSuDung[k]][j] == 0)
                            {
                                tonTai = true;
                                break;
                            }

                        if (tonTai)
                        {
                            vtLuatCuoi = i;
                            LuatTrungGian.Add(tapLuatSuDung[vtLuatCuoi]);
                            break;
                        }
                    }
            }
            //Đảo tập luật trung gian sẽ thu được tập luật tối ưu!
            LuatTrungGian.Reverse();
            return LuatTrungGian;
        }
        //Khởi tạo tính toán từ tập luật
        public void TinhToan()
        {
            //get value from user
            layGiaTri();

            Stack<double> nganXepGiaTri = new Stack<double>();
            for (int i = 0; i < TapLuatSuDung.Count; i++)//duyệt danh sách tập luật cần sử dụng
            {
                string btHauTo = convertExpression(TapLuatSuDung[i]); // chuyển công thức từ tập luật sang dạng dạng hậu tố

                int vt = 0;
                for (int j = 0; j < btHauTo.Length; j++)
                {
                    string kiTuTrungGian = "";
                    double temp1, temp2;
                    if (btHauTo[j] == ' ')
                    {
                        kiTuTrungGian = btHauTo.Substring(vt, j - vt);
                        vt = j + 1;

                        switch (kiTuTrungGian)
                        {
                            case "A":
                                nganXepGiaTri.Push(dsGiaTri[0]);
                                break;
                            case "B":
                                nganXepGiaTri.Push(dsGiaTri[1]);
                                break;
                            case "C":
                                nganXepGiaTri.Push(dsGiaTri[2]);
                                break;
                            case "a":
                                nganXepGiaTri.Push(dsGiaTri[3]);
                                break;
                            case "b":
                                nganXepGiaTri.Push(dsGiaTri[4]);
                                break;
                            case "c":
                                nganXepGiaTri.Push(dsGiaTri[5]);
                                break;
                            case "ha":
                                nganXepGiaTri.Push(dsGiaTri[6]);
                                break;
                            case "hb":
                                nganXepGiaTri.Push(dsGiaTri[7]);
                                break;
                            case "hc":
                                nganXepGiaTri.Push(dsGiaTri[8]);
                                break;
                            case "p":
                                nganXepGiaTri.Push(dsGiaTri[9]);
                                break;
                            case "S":
                                nganXepGiaTri.Push(dsGiaTri[10]);
                                break;
                            case "180":
                                nganXepGiaTri.Push(180);
                                break;
                            case "2":
                                nganXepGiaTri.Push(2);
                                break;
                            case "+":
                                temp2 = nganXepGiaTri.Pop();
                                temp1 = nganXepGiaTri.Pop();
                                nganXepGiaTri.Push(temp1 + temp2);
                                break;
                            case "-":
                                temp2 = nganXepGiaTri.Pop();
                                temp1 = nganXepGiaTri.Pop();
                                nganXepGiaTri.Push(temp1 - temp2);
                                break;
                            case "*":
                                temp2 = nganXepGiaTri.Pop();
                                temp1 = nganXepGiaTri.Pop();
                                nganXepGiaTri.Push(temp1 * temp2);
                                break;
                            case "/":
                                temp2 = nganXepGiaTri.Pop();
                                temp1 = nganXepGiaTri.Pop();
                                nganXepGiaTri.Push(temp1 / temp2);
                                break;
                            case "sqrt":
                                temp2 = nganXepGiaTri.Pop();
                                nganXepGiaTri.Push(Math.Sqrt(temp2));
                                break;
                            case "sin":
                                temp2 = nganXepGiaTri.Pop();
                                //chuyển sang độ
                                temp2 = Math.PI * (temp2 / 180);
                                nganXepGiaTri.Push(Math.Sin(temp2));
                                break;
                            case "cos":
                                temp2 = nganXepGiaTri.Pop();
                                //chuyển sang độ
                                temp2 = Math.PI * (temp2 / 180);
                                nganXepGiaTri.Push(Math.Cos(temp2));
                                break;
                            case "arcsin":
                                temp2 = nganXepGiaTri.Pop();
                                nganXepGiaTri.Push(Math.Round((Math.Asin(temp2) / Math.PI * 180), 2));
                                break;
                            case "arccos":
                                temp2 = nganXepGiaTri.Pop();
                                nganXepGiaTri.Push(Math.Round((Math.Acos(temp2) / Math.PI * 180), 2));
                                break;
                        }
                    }
                }

                for (int j = 0; j < 11; j++)//in ra danh sách kết quả cần tìm 
                {
                    if (TapLuat[TapLuatSuDung[i]][j] == 1)//duyệt theo gỉả thiết và tập luật
                    {
                        dsGiaTri[j] = Math.Round(nganXepGiaTri.Pop(), 2);
                        Console.WriteLine(dsGiaTri[j]);
                        break;
                    }
                }
            }
        }
        List<double> dsGiaTri;//danh sách giá trị được nhập từ form
        private void layGiaTri()
        {
            dsGiaTri = new List<double>();
            for (int i = 0; i < 11; i++)
            {
                dsGiaTri.Add(-1);
            }


            if (txtAngleA.Text != "")
            {
                dsGiaTri[0] = double.Parse(txtAngleA.Text);
            }

            if (txtAngleB.Text != "")
            {
                dsGiaTri[1] = double.Parse(txtAngleB.Text);
            }

            if (txtAngleC.Text != "")
            {
                dsGiaTri[2] = double.Parse(txtAngleC.Text);
            }

            if (txtEdgea.Text != "")
            {
                dsGiaTri[3] = double.Parse(txtEdgea.Text);
            }

            if (txtEdgeb.Text != "")
            {
                dsGiaTri[4] = double.Parse(txtEdgeb.Text);
            }

            if (txtEdgec.Text != "")
            {
                dsGiaTri[5] = double.Parse(txtEdgec.Text);
            }

            if (txtha.Text != "")
            {
                dsGiaTri[6] = double.Parse(txtha.Text);
            }

            if (txthb.Text != "")
            {
                dsGiaTri[7] = double.Parse(txthb.Text);
            }

            if (txthc.Text != "")
            {
                dsGiaTri[8] = double.Parse(txthc.Text);
            }

            if (txtP.Text != "")
            {
                dsGiaTri[9] = double.Parse(txtP.Text);
            }

            if (txtS.Text != "")
            {
                dsGiaTri[10] = double.Parse(txtS.Text);
            }
        }
        //Chuyển biểu thức sang hậu tố
        private string convertExpression(int vtri)
        {
            Stack<string> nganXepChuoi = new Stack<string>();
            string btHauTo = "";

            string kiTuTrungGian = "";
            StreamReader sr = new StreamReader("Rules.txt");
            for (int i = 0; i < vtri + 1; i++)
                kiTuTrungGian = sr.ReadLine();
            sr.Close();
            sr.Dispose();

            kiTuTrungGian = kiTuTrungGian.Substring(kiTuTrungGian.IndexOf('.') + 1);//lấy ra công thức sau dấu chấm

            int vt = 0;

            for (int i = 0; i < kiTuTrungGian.Length; i++)
            {
                if (kiTuTrungGian[i] == ' ' || i == kiTuTrungGian.Length - 1)
                {
                    //get operator and argument
                    string toanTu = "";
                    toanTu = (i != (kiTuTrungGian.Length - 1) ? kiTuTrungGian.Substring(vt, i - vt) :
                            kiTuTrungGian.Substring(vt, i - vt + 1));
                    vt = i + 1;

                    switch (toanTu)
                    {
                        case "A":
                        case "B":
                        case "C":
                        case "a":
                        case "b":
                        case "c":
                        case "ha":
                        case "hb":
                        case "hc":
                        case "p":
                        case "S":
                        case "180":
                        case "2":
                            btHauTo += toanTu + " ";
                            break;
                        case "+":
                        case "-":
                            while (nganXepChuoi.Count != 0 && nganXepChuoi.Peek() != "(")
                                btHauTo += nganXepChuoi.Pop() + " ";
                            nganXepChuoi.Push(toanTu);
                            break;
                        case "*":
                        case "/":
                            while (nganXepChuoi.Count != 0)
                                if (nganXepChuoi.Peek() == "*" || nganXepChuoi.Peek() == "/"
                                || nganXepChuoi.Peek() == "sin" || nganXepChuoi.Peek() == "cos" || nganXepChuoi.Peek() == "arcsin"
                                || nganXepChuoi.Peek() == "arccos" || nganXepChuoi.Peek() == "sqrt")
                                    btHauTo += nganXepChuoi.Pop() + " ";
                                else
                                    break;
                            nganXepChuoi.Push(toanTu);
                            break;
                        case "sin":
                        case "cos":
                        case "arcsin":
                        case "arccos":
                        case "sqrt":
                            if (nganXepChuoi.Count != 0)
                                while (nganXepChuoi.Peek() == "sin" || nganXepChuoi.Peek() == "cos" || nganXepChuoi.Peek() == "arcsin"
                                || nganXepChuoi.Peek() == "arccos" || nganXepChuoi.Peek() == "sqrt")
                                    btHauTo += nganXepChuoi.Pop() + " ";
                            nganXepChuoi.Push(toanTu);
                            break;
                        case "(":
                            nganXepChuoi.Push(toanTu);
                            break;
                        case ")":
                            if (nganXepChuoi.Count != 0)
                            {
                                while (nganXepChuoi.Peek() != "(")
                                    btHauTo += nganXepChuoi.Pop() + " ";
                                nganXepChuoi.Pop();
                            }
                            break;
                    }
                }
            }
            while (nganXepChuoi.Count != 0)
                btHauTo += nganXepChuoi.Pop() + " ";

            return btHauTo;
        }
        private void BaiToanCanGiai()
        {
            string dienGiai = "";
            dienGiai += "\n\t\t\t\t\t-------------------- BÀI TOÁN TAM GIÁC LƯỢNG SỬ DỤNG SUY DIỄN TIẾN--------------------\n\n"
                + "Giả thiết: \n";
            for (int i = 0; i < 11; i++)
            {
                if (baiToan.Bt[i] == 0)
                {
                    switch (i)
                    {
                        case 0:
                            dienGiai += "- Góc A: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 1:
                            dienGiai += "- Góc B: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 2:
                            dienGiai += "- Góc C: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 3:
                            dienGiai += "- Cạnh a: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 4:
                            dienGiai += "- Cạnh b: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 5:
                            dienGiai += "- Cạnh c: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 6:
                            dienGiai += "- Chiều cao ha: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 7:
                            dienGiai += "- Chiều cao hb: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 8:
                            dienGiai += "- Chiều cao hc: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 9:
                            dienGiai += "- Nửa chu vi p: "
                                + dsGiaTri[i] + ".\n";
                            break;
                        case 10:
                            dienGiai += "- Diện tích S: "
                                + dsGiaTri[i] + ".\n";
                            break;
                    }
                }
            }

            dienGiai += "\nKết luận:\n";

            switch (vtKL)
            {
                case 0:
                    dienGiai += "- Tính góc A ?\n";
                    break;
                case 1:
                    dienGiai += "- Tính góc B ?\n";
                    break;
                case 2:
                    dienGiai += "- Tính góc C ?\n";
                    break;
                case 3:
                    dienGiai += "- Tính độ dài cạnh a ?\n";
                    break;
                case 4:
                    dienGiai += "- Tính độ dài cạnh b ?\n";
                    break;
                case 5:
                    dienGiai += "- Tính độ dài cạnh c ?\n";
                    break;
                case 6:
                    dienGiai += "- Tính độ dài đường cao ha ?\n";
                    break;
                case 7:
                    dienGiai += "- Tính độ dài đường cao hb ?\n";
                    break;
                case 8:
                    dienGiai += "- Tính độ dài đường cao hc ?\n";
                    break;
                case 9:
                    dienGiai += "- Tính nửa chu vi tam giác p ?\n";
                    break;
                case 10:
                    dienGiai += "- Tính diện tích tam giác S ?\n";
                    break;
            }

            dienGiai += "\n\t Bài làm:\n\n";

            rtResult.Text += dienGiai;
        }
        private void Giai()
        {
            string giai = "Các luật sử dụng theo thứ tự: " + "\n";
            for (int i = 0; i < TapLuatSuDung.Count; i++)
            {
                for (int j = 0; j < 11; j++)
                    if (TapLuat[TapLuatSuDung[i]][j] == 1)
                        switch (j)
                        {
                            case 0:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": A = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 1:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": B = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 2:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": C = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 3:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": a = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 4:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": b = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 5:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": c = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 6:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": ha = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 7:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": hb = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 8:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": hc = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 9:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": p = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                            case 10:
                                giai += "\t r" + (TapLuatSuDung[i] + 1) + ": S = " + layCongThuc(TapLuatSuDung[i]) + "\n";
                                break;
                        }

            }
            for (int i = 0; i < TapLuatSuDung.Count; i++)
            {
                if (TapLuatSuDung.Count > 1)
                    giai += "* Bước " + (i + 1) + ": ";
                else
                    giai += "* ";
                for (int j = 0; j < 11; j++)
                {
                    if (TapLuat[TapLuatSuDung[i]][j] == 1)
                    {
                        switch (j)
                        {
                            case 0:
                                giai += "Tính góc A\n"
                                    + "- Áp dụng công thức: A = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: A = " + dsGiaTri[0] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy giá trị của góc A cần tìm là "
                                                        + dsGiaTri[0] + " độ.\n";
                                break;
                            case 1:
                                giai += "Tính góc B\n"
                                    + "- Áp dụng công thức: B = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: B = " + dsGiaTri[1] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy giá trị của góc B cần tính là "
                                                        + dsGiaTri[1] + " độ.\n";
                                break;
                            case 2:
                                giai += "Tính góc C\n"
                                    + "- Áp dụng công thức: C = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: C = " + dsGiaTri[2] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy giá trị của góc C cần tính là "
                                                        + dsGiaTri[2] + " độ.\n";
                                break;
                            case 3:
                                giai += "Tính độ dài cạnh a\n"
                                    + "- Áp dụng công thức: a = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: a = " + dsGiaTri[3] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy độ dài của cạnh a cần tính là "
                                                        + dsGiaTri[3] + ".\n";
                                break;
                            case 4:
                                giai += "Tính độ dài cạnh b\n"
                                    + "- Áp dụng công thức: b = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: b = " + dsGiaTri[4] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy độ dài của cạnh b cần tính là "
                                                        + dsGiaTri[4] + ".\n";
                                break;
                            case 5:
                                giai += "Tính độ dài cạnh c\n"
                                    + "- Áp dụng công thức: c = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: c = " + dsGiaTri[5] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy độ dài của cạnh c cần tính là "
                                                        + dsGiaTri[5] + ".\n";
                                break;
                            case 6:
                                giai += "Tính độ dài đường cao ha\n"
                                    + "- Áp dụng công thức: ha = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: ha = " + dsGiaTri[6] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy độ dài của đường cao ha cần tính là "
                                                        + dsGiaTri[6] + ".\n";
                                break;
                            case 7:
                                giai += "Tính độ dài đường cao hb\n"
                                    + "- Áp dụng công thức: hb = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: hb = " + dsGiaTri[7] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy độ dài của đường cao hb cần tính là "
                                                        + dsGiaTri[7] + ".\n";
                                break;
                            case 8:
                                giai += "Tính độ dài đường cao hc\n"
                                    + "- Áp dụng công thức: hc = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: hc = " + dsGiaTri[8] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy độ dài của đường cao hc cần tính là "
                                                        + dsGiaTri[8] + ".\n";
                                break;
                            case 9:
                                giai += "Tính độ dài nửa chu vi p\n"
                                    + "- Áp dụng công thức: p = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: p = " + dsGiaTri[9] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy giá trị của nửa chu vi tam giác p cần tính là "
                                                        + dsGiaTri[9] + ".\n";
                                break;
                            case 10:
                                giai += "Tính diện tích S\n"
                                    + "- Áp dụng công thức: S = " + layCongThuc(TapLuatSuDung[i]) + "\n"
                                    + "   Ta tính được: S = " + dsGiaTri[10] + "\n";
                                if (i == TapLuatSuDung.Count - 1)
                                    giai += "\nKết luận: Vậy diện tích S của tam giác cần tính là "
                                                        + dsGiaTri[10] + ".\n";
                                break;
                        }
                        break;
                    }
                }
            }
            rtResult.Text += giai;
        }
        private string layCongThuc(int vtri)
        {
            string kiTuTrungGian = "";
            StreamReader sr = new StreamReader("Rules.txt");
            for (int i = 0; i < vtri + 1; i++)
                kiTuTrungGian = sr.ReadLine();
            sr.Close();
            sr.Dispose();

            kiTuTrungGian = kiTuTrungGian.Substring(kiTuTrungGian.IndexOf('.') + 1);

            return kiTuTrungGian;
        }
    }
}
