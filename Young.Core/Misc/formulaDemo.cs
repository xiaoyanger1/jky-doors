using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Young.Core.Misc
{
    public class Formula
    {
        /// <summary>
        /// 计算斜率k及纵截距b值
        /// </summary>
        /// <param name="x1">坐标点x1</param>
        /// <param name="x2">坐标点x2</param>
        /// <param name="y1">坐标点y1</param>
        /// <param name="y2">坐标点y2</param>
        /// <param name="kvalue">斜率k值</param>
        /// <param name="bvalue">纵截距b值</param>
        private static void Calculate(float x1, float x2, float y1, float y2, ref float kvalue, ref float bvalue)//求方程y=kx+b 系数 k ,b
        {
            float coefficient = 1;//系数值
            try
            {
                if ((x1 == 0) || (x2 == 0) || (x1 == x2)) return; //排除为零的情况以及x1，x2相等时无法运算的情况
                //if (y1 == y2) return; //根据具体情况而定，如何这两个值相等，得到的就是一条直线
                float temp = 0;
                if (x1 >= x2)
                {
                    coefficient = (x1 / x2);
                    temp = y2 * coefficient; //将对应的函数乘以系数
                    bvalue = (temp - y1) / (coefficient - 1);
                    kvalue = (y1 - bvalue) / x1; //求出k值
                }
                else
                {
                    coefficient = x2 / x1;
                    temp = y1 * coefficient;
                    bvalue = (temp - y2) / (coefficient - 1); //求出b值
                    kvalue = (y2 - bvalue) / x2; //求出k值
                }
            }
            catch
            {
                Console.WriteLine("x系数不能为0或相等");
            }
        }

        public List<listFormula> getValue()
        {
            List<listFormula> list = new List<listFormula>();

            listFormula list1 = new listFormula();
            list1.x = 1000;
            list1.y = 100;
            list.Add(list1);

            listFormula list2 = new listFormula();
            list2.x = 2000;
            list2.y = 200;
            list.Add(list2);

            listFormula list3 = new listFormula();
            list3.x = 4000;
            list3.y = 250;
            list.Add(list3);

            listFormula list4 = new listFormula();
            list4.x = 6000;
            list4.y = 250;
            list.Add(list4);

            listFormula list5 = new listFormula();
            list5.x = 900;
            list5.y = 250;
            list.Add(list5);

            // 先进行排序
            list.Sort(delegate (listFormula x, listFormula y)
           {
               return x.x.CompareTo(y.x);
           });

            // 对数据合计
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count)
                {
                    string ss = "3";
                }
                if (i == 0)
                {
                    continue;
                }
                float k = 0, b = 0;
                Calculate(list[i - 1].x, list[i].x, list[i - 1].y, list[i].y, ref k, ref b);
                list[i - 1].k = k;
                list[i - 1].b = b;
            }
            return list;
        }

        /// <summary>
        /// 录入时获取数据
        /// </summary>
        /// <param name="vlaue"></param>
        public void insert(float x, float y)
        {
            List<listFormula> list = getValue();

            float k = 0, b = 0;
            // 判断 第一位
            if (x < list[0].x)
            {
                Calculate(x, list[0].x, y, list[0].y, ref k, ref b);
                list.Insert(0, new listFormula
                {
                    x = x,
                    y = y,
                    k = k,
                    b = b
                });
            }
            else if (x > list[list.Count - 1].x)
            {
                // 最后一位
                Calculate(list[list.Count - 1].x, x, list[list.Count - 1].y, y, ref k, ref b);
                list.Add(new listFormula
                {
                    x = x,
                    y = y,
                    k = k,
                    b = b
                });
            }
            else
            {
                // 中间位
                for (int i = 0; i < list.Count; i++)
                {
                    if (x > list[i].x && x < list[i + 1].x)
                    {
                        // 更新
                        Calculate(list[i].x, x, list[i].y, y, ref k, ref b);

                        list[i].k = k;
                        list[i].b = b;

                        k = b = 0;

                        Calculate(x, list[i + 1].x, y, list[i + 1].y, ref k, ref b);
                        // 插入
                        list.Insert(i + 1, new listFormula
                        {
                            x = x,
                            y = y,
                            k = k,
                            b = b
                        });
                        break;  // 中断
                    }
                }
            }
        }

        public void test()
        {
            float k = 0;
            float b = 0;
            Calculate(2, 3, 10, 20, ref k, ref b);

            Console.WriteLine("k:" + k);
            Console.WriteLine("b:" + b);
        }


    }

    public class listFormula
    {
        public float x { get; set; }
        public float y { get; set; }

        public float k { get; set; }
        public float b { get; set; }

    }
}
