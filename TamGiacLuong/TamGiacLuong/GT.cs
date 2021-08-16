using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamGiacLuong
{
    public class GT
    {
        List<int> listGT;
        // 11 element: A, B, C, a, b, c, ha, hb, hc, p, S;
        public GT()
        {
            for(int i = 0; i < 11; i++)
            {
                listGT.Add(-1);
            }
        }
        public List<int> ListGT { get => listGT; set => listGT = value; }
    }
}
