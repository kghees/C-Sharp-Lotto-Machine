using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lotto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //난수 생성해야하므로 랜덤 클래스 생성
        Random r = new Random();
        //HashSet은 중복된 요소는 걸러주기 때문에 사용
        HashSet<int> LottoNums = new HashSet<int>();

        private void button1_Click(object sender, EventArgs e)
        {
            //버튼 누르면 HashSet clear
            LottoNums.Clear();
            //무명메서드 호출 하면 GetNum 메서드 실행
            //직접 만든 workThread는 직접 글을 쓸 수 없음
            Thread t1 = new Thread(() => GetNum(tB1));
            t1.Start();
            Thread t2 = new Thread(() => GetNum(tB2));
            t2.Start();
            Thread t3 = new Thread(() => GetNum(tB3));
            t3.Start();
            Thread t4 = new Thread(() => GetNum(tB4));
            t4.Start();
            Thread t5 = new Thread(() => GetNum(tB5));
            t5.Start();
            Thread t6 = new Thread(() => GetNum(tB6));
            t6.Start();
        }

        private void GetNum(TextBox t)
        {
            int n;
            //중복된 숫자를 넣지 않기 위해 처리
            //lock : 특정 블럭의 코드(Critical Section이라 부른다)를 한번에 하나의 쓰레드만 실행할 수 있도록 해준다.
            lock (LottoNums) //thread 안정성을 위해 lock 사용
            {
                do
                {
                    n = r.Next(1, 46);
                } while (LottoNums.Contains(n)); //중복된 숫자가 아니면 탈출

                LottoNums.Add(n); //새로운 숫자를 HashSet에 추가
            }
            SetTextBox(t, n);
        }

        public delegate void SetTextBoxDelegate(TextBox t, int n);
        private void SetTextBox(TextBox t, int n)
        {
            //TextBox에 쓰는 것은 UI thread만 가능하므로
            //직접 바로 쓰면 안됨

            //UI thread가 아니므로
            //delegate를 써서 호출해야함
            //호출하면 다시 SetTextBox 메서드를 UI thread가 delegate메서드 호출
            //그래서 else문으로 간다!
            if (t.InvokeRequired) 
            {
                
                //SetTextBox 메서드를 가리키는 델리게이트 인스턴스를 생성
                SetTextBoxDelegate d = new SetTextBoxDelegate(SetTextBox);

                //delegate를 UI thread에서 실행하도록 한다.
                //invoke 메서드는 현재 thread가 UI thread가 아닌 경우, 
                //UI thread에서 delegate를 실행한다.
                this.Invoke(d, new object[] { t, n });
            }
            //UI thread가 delegate를 통해서 SetTextBox를 호출한 경우에
            else
            {
                t.Text = n.ToString();
            }
        }
    }
}
