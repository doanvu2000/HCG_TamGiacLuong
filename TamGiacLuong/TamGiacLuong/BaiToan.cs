using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamGiacLuong
{
    class BaiToan
    {
        List<int> bt;

        public BaiToan()
        {
            bt = new List<int>();
            for(int i = 0; i < 11; i++)
            {
                bt.Add(-1);
            }
        }

        public List<int> Bt { get => bt; set => bt = value; }
    }
}
