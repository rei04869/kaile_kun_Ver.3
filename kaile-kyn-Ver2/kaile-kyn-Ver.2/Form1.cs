using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kaile_kyn_Ver._2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // ウィンドウを画面右下に表示させる
            int left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            int top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            DesktopBounds = new Rectangle(left, top, this.Width, this.Height);

            label2.Visible = false;
            if (label1.Visible == false)
            {
                label1.Visible = true;

            }

            
            pictureBox1.ImageLocation = @"c:\image\kaile_move.gif";

            string s2 = "やぁ　僕の名前はカイル";
            string s3 = "\r\n" + "こんなご時世だから、もちろん熱は計ってパソコン触っているんだよね？？";
            string s4 = "\r\n" + "さっさと何度だったか教えてくれない？";

            string output = "";

            int waitTimeChar = 100; // 一文字の待機時間
            int waitTimeLine = 400; // 行間の待機時間


            await OutputMessage(s2);
            await Task.Delay(waitTimeLine);

            await OutputMessage(s3);
            await Task.Delay(waitTimeLine);

            await OutputMessage(s4);

            pictureBox1.ImageLocation = @"c:\image\kaile_stop.gif";

            textBox1.Visible = true;
            button1.Visible = true;

 
                // 関数：1文字ずつ表示する
                async Task OutputMessage(string s)
            {
                // foreachで1文字ずつ処理（後半）
                foreach (char c in s)
                {
                    // 1文字追加
                    output += c.ToString();

                    // ラベルに表示
                    this.label1.Text = output;

                    // 空白文字以外にディレイさせる
                    if ("" != c.ToString())
                    {
                        // ディレイ
                        await Task.Delay(waitTimeChar);
                    }
                }
            }
        }




        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //バックスペースと小数点使用可能とする
                if (e.KeyChar == 0x08 || e.KeyChar == '.')
                {
                    return;
                }

                //数字キー以外の入力をキャンセルする
                if (e.KeyChar < 0x30 || e.KeyChar > 0x39)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        bool isDelete = false;

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            isDelete = (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back);
        }

        //整数部桁数
        private readonly int LENGTH_INT_PART = 2;
        //小数部桁数
        private readonly int LENGTH_DECIMAL_PLACES = 1;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var txtBox = sender as TextBox;

            txtBox.TextChanged -= textBox1_TextChanged;

            try
            {
                if (!string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    var value = txtBox.Text;
                    //整数と小数を分割する
                    string[] values = value.Split('.');

                    int currentPoint = 0;
                    switch (values.Length)
                    {
                        case 1:
                            //------------------------------
                            //整数部のみで構成されている場合
                            //------------------------------
                            //整数部の最大桁数を超えた場合、今回の入力値を無効にする
                            if (value.Length > LENGTH_INT_PART)
                            {
                                //DELETEやBackSpaceによる削除で桁あふれが発生しているなら
                                //小数点の削除によるものなので、先頭桁から削除する
                                if (isDelete)
                                {
                                    txtBox.Text = value.Substring(value.Length - LENGTH_INT_PART);

                                    //カーソル位置を移動
                                    if (currentPoint - (values.Length - LENGTH_INT_PART) >= 0)
                                    {
                                        txtBox.SelectionStart = currentPoint -
                                            (values.Length - LENGTH_INT_PART);
                                    }
                                    else
                                    {
                                        txtBox.SelectionStart = 0;
                                    }
                                }
                                else
                                {
                                    //入力後の現カーソル位置を取得
                                    currentPoint = txtBox.SelectionStart;

                                    //カーソル位置の前の１文字が今回入力された文字、
                                    //よって、それを省いた文字列に編集する
                                    var left = value.Substring(0
                                        , currentPoint > LENGTH_INT_PART
                                            ? LENGTH_INT_PART : currentPoint - 1);
                                    var right = left.Length >= LENGTH_INT_PART
                                        ? "" : value.Substring(currentPoint);
                                    txtBox.Text = left + right;

                                    //カーソル位置を入力前の位置に戻す
                                    txtBox.SelectionStart = currentPoint - 1;
                                }
                            }
                            break;

                        case 2:
                            //----------------------------------
                            //整数部＋小数部で構成されている場合
                            //----------------------------------
                            //入力後の現カーソル位置を取得
                            currentPoint = txtBox.SelectionStart;

                            //今回の入力値が"."の場合、小数点を基準点として桁あふれ分を除外する
                            if (value.Substring(currentPoint - 1, 1) == ".")
                            {
                                //=== 整数部の処理 ===
                                var intPart = values[0];
                                if (values[0].Length > LENGTH_INT_PART)
                                {
                                    intPart = values[0].Substring(LENGTH_INT_PART - values[0].Length);
                                }

                                //=== 小数部の処理 ===
                                var decimalPart = values[1];
                                if (values[1].Length > LENGTH_DECIMAL_PLACES)
                                {
                                    decimalPart = values[1].Substring(0, LENGTH_DECIMAL_PLACES);
                                }

                                //整数と小数を結合
                                if (values[0].Length > LENGTH_INT_PART || values[1].Length > LENGTH_DECIMAL_PLACES)
                                {
                                    txtBox.Text = string.Format("{0}.{1}", intPart, decimalPart);

                                    //小数点入力時なら小数点の後ろにカーソルをセット
                                    txtBox.SelectionStart = value.IndexOf(".") + 1;
                                }
                            }
                            else
                            {
                                //=== 整数部の処理 ===
                                var intPart = values[0];
                                if (values[0].Length > LENGTH_INT_PART)
                                {
                                    //カーソル位置の前の１文字が今回入力された文字、
                                    //よって、それを省いた文字列に編集する
                                    var left = values[0].Substring(0
                                        , currentPoint > LENGTH_INT_PART
                                            ? LENGTH_INT_PART : currentPoint - 1);
                                    var right = left.Length >= LENGTH_INT_PART
                                        ? "" : values[0].Substring(currentPoint);

                                    //桁数調整後の整数部文字列
                                    intPart = left + right;
                                }

                                //=== 小数部の処理 ===
                                var decimalPart = values[1];
                                if (values[1].Length > LENGTH_DECIMAL_PLACES)
                                {
                                    //整数部と小数点を除いたときのカーソル位置を算出
                                    var tempPoint = currentPoint - values[0].Length - 1;
                                    //カーソル位置の前の１文字を除外する
                                    var left = values[1].Substring(0
                                        , tempPoint > LENGTH_DECIMAL_PLACES
                                            ? LENGTH_DECIMAL_PLACES : tempPoint - 1);
                                    var right = left.Length >= LENGTH_DECIMAL_PLACES
                                            ? "" : values[1].Substring(tempPoint);

                                    //桁数調整後の小数部文字列
                                    decimalPart = left + right;
                                }

                                //整数と小数を結合
                                if (values[0].Length > LENGTH_INT_PART || values[1].Length > LENGTH_DECIMAL_PLACES)
                                {
                                    txtBox.Text = string.Format("{0}.{1}", intPart, decimalPart);

                                    //カーソル位置を入力前の位置に戻す
                                    txtBox.SelectionStart = currentPoint - 1;
                                }
                            }

                            break;

                        default:
                            //"."で文字列を分割したときに３以上になるのなら、
                            //既に"."が存在しているのに今回"."が入力されたことを示す
                            //よって、今回の入力を無効にしてしまう。
                            {
                                currentPoint = txtBox.SelectionStart;
                                var left = value.Substring(0, currentPoint - 1);
                                var right = value.Substring(currentPoint);
                                txtBox.Text = left + right;
                                txtBox.SelectionStart = currentPoint - 1;
                            }
                            break;
                    }

                    //先頭が小数点なら先頭に 0 を入れる
                    if (txtBox.Text.StartsWith("."))
                    {
                        currentPoint = txtBox.SelectionStart;
                        txtBox.Text = "0" + txtBox.Text;
                        txtBox.SelectionStart = currentPoint + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                txtBox.TextChanged += textBox1_TextChanged;
            }
            this.label3.Text = txtBox.Text;

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("今朝計った温度は" + label3.Text + "℃でよろしいですか",
             "確認だよ",
             MessageBoxButtons.YesNo,
             MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button2);

            double Btemp = double.Parse(label3.Text);

            
            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                textBox1.Visible = false;
                button1.Visible = false;
                label2.Visible = true;
                label1.Visible = false;

                if (Btemp > 37.5)
                {

                    string s4 = "......え？";
                    string s6 = "\r\n" + "聞き間違いだと思うからもう一回聞くね？";
                    string s7 = "\r\n" + "今朝何度だった？";

                    string output1 = "";

                    int waitTimeChar1 = 50; // 一文字の待機時間
                    int waitTimeLine = 400; // 行間の待機時間

                    await OutputMessage1(s4);
                    await Task.Delay(waitTimeLine);

                    await OutputMessage1(s6);
                    await Task.Delay(waitTimeLine);

                    await OutputMessage1(s7);

                    textBox1.Visible = true;
                    button1.Visible = true;

                    // 関数：1文字ずつ表示する
                    async Task OutputMessage1(string s)
                    {
                        // foreachで1文字ずつ処理（後半）
                        foreach (char c in s)
                        {
                            // 1文字追加
                            output1 += c.ToString();

                            // ラベルに表示
                            this.label2.Text = output1;

                            // 空白文字以外にディレイさせる
                            if ("" != c.ToString())
                            {
                                // ディレイ
                                await Task.Delay(waitTimeChar1);
                            }
                        }
                    }
                }
                else
                {
                    string s5 = "ふ～んまぁ熱がない事は当然だけどね♪";

                    string output = "";

                    int waitTimeChar = 50; // 一文字の待機時間

                    await OutputMessage(s5);
                    // 関数：1文字ずつ表示する
                    async Task OutputMessage(string s)
                    {
                        // foreachで1文字ずつ処理（後半）
                        foreach (char c in s)
                        {
                            // 1文字追加
                            output += c.ToString();

                            // ラベルに表示
                            this.label2.Text = output;

                            // 空白文字以外にディレイさせる
                            if ("" != c.ToString())
                            {
                                // ディレイ
                                await Task.Delay(waitTimeChar);
                            }
                        }
                    }
                }
                
             
                
            }
        }
    }
}
