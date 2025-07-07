using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using ImageProcessing;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

namespace ortho
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            bt1.IsEnabled = false;
            bt2.IsEnabled = false;
        }

        LOCImage image0;
        double[,] oriima;
        double[,] oriobj;
        double xp, yp, k1, k2, k3, p1, p2, b1, b2, Hpixsize, Vpixsize, Hpix, Vpix;
        int orthoW, orthoH, W, H;
        String filename;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.ShowDialog(); //OFD.Filename
            filename = OFD.FileName;

            //String filename = "C:\\Users\\ASUS\\Videos\\Captures\\83039715_188795668839191_8407660474925056000_n.jpg";
            Origin.Source = new BitmapImage(new Uri(filename));
            //BitmapImage image0 = new BitmapImage(new Uri(filename));
            image0 = new LOCImage(filename, Int32Rect.Empty);

            W = image0.Width;
            H = image0.Height;

            oriima = new double[4, 2];
            oriobj = new double[4, 2];
            /*
            oriima[0, 0] = 1298.517;
            oriima[0, 1] = 665.524;
            oriima[1, 0] = 3628.861;
            oriima[1, 1] = 278.252;
            oriima[2, 0] = 1008.017;
            oriima[2, 1] = 2456.027;
            oriima[3, 0] = 3723.286;
            oriima[3, 1] = 2482.600;

            oriobj[0, 0] = 0;
            oriobj[0, 1] = 0;
            oriobj[1, 0] = 1080;
            oriobj[1, 1] = 0;
            oriobj[2, 0] = 0;
            oriobj[2, 1] = 835;
            oriobj[3, 0] = 1080;
            oriobj[3, 1] = 835;
            */

            //率定結果
            OpenFileDialog OFDt2 = new OpenFileDialog();
            OFDt2.DefaultExt = ".txt";
            OFDt2.Filter = "Text Document (.txt)|*.txt";
            //觀測數據
            OpenFileDialog OFDt = new OpenFileDialog();
            OFDt.DefaultExt = ".txt";
            OFDt.Filter = "Text Document (.txt)|*.txt";

            if (OFDt2.ShowDialog() == true)
            {
                String text2filname = OFDt2.FileName;
                String[] coefreport = File.ReadAllLines(text2filname);
                int n2 = coefreport.GetLength(0);
                String[,] coeftext = new String[n2, 3];

                for (int i = 0; i < n2; i++)
                {
                    String[] sub2 = coefreport[i].Split(new Char[1] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    int length = sub2.Length;
                    for (int j = 0; j < length; j++)
                    {
                        if (sub2[j] == "")
                        {
                            break;
                        }
                        else if (sub2[j] == "H")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            Hpix = double.Parse(coeftext[i, j + 1]);
                            coeftext[i, j + 2] = sub2[j + 2];
                            Hpixsize = double.Parse(coeftext[i, j + 2]);
                        }
                        else if (sub2[j] == "V")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            Vpix = double.Parse(coeftext[i, j + 1]);
                            coeftext[i, j + 2] = sub2[j + 2];
                            Vpixsize = double.Parse(coeftext[i, j + 2]);
                        }
                        else if (sub2[j] == "XP")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            xp = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "YP")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            yp = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "K1")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            k1 = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "K2")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            k2 = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "K3")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            k3 = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "P1")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            p1 = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "P2")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            p2 = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "B1")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            b1 = double.Parse(coeftext[i, j + 1]);
                        }
                        else if (sub2[j] == "B2")
                        {
                            coeftext[i, j + 1] = sub2[j + 1];
                            b2 = double.Parse(coeftext[i, j + 1]);
                        }
                    }
                }
            }

            if (OFDt.ShowDialog() == true)
            {
                String textfilename = OFDt.FileName;
                String[] point = File.ReadAllLines(textfilename);
                int n = point.GetLength(0);
                String[,] change = new String[n, 2];
                double[,] objWH = new double[1, 2];

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        String[] sub = point[i].Split(new Char[1] { ' ' });
                        if (i < 4)
                        {
                            change[i, j] = sub[j];
                            oriima[i, j] = double.Parse(change[i, j]);
                        }
                        else
                        {
                            change[i, j] = sub[j];
                            objWH[i - 4, j] = double.Parse(change[i, j]);
                        }
                    }
                }

                // 透鏡畸變糾正
                double xbar2, ybar2, r22, deltaX2, deltaY2;
                for (int i = 0; i < 4; i++)
                {
                    oriima[i, 0] = Hpixsize * (oriima[i, 0] - (Hpix + 1) / 2);
                    oriima[i, 1] = Vpixsize * ((Vpix + 1) / 2 - oriima[i, 1]);

                    xbar2 = oriima[i, 0] - xp;
                    ybar2 = oriima[i, 1] - yp;

                    r22 = Math.Sqrt(Math.Pow(xbar2, 2) + Math.Pow(ybar2, 2));
                    deltaX2 = xbar2 * Math.Pow(r22, 2) * k1 + xbar2 * Math.Pow(r22, 4) * k2 + xbar2 * Math.Pow(r22, 6) * k3
                        + (2 * Math.Pow(xbar2, 2) + Math.Pow(r22, 2)) * p1 + 2 * xbar2 * ybar2 * p2 + b1 * xbar2 + b2 * ybar2;
                    deltaY2 = ybar2 * Math.Pow(r22, 2) * k1 + ybar2 * Math.Pow(r22, 4) * k2 + ybar2 * Math.Pow(r22, 6) * k3
                        + 2 * xbar2 * ybar2 * p1 + (2 * Math.Pow(ybar2, 2) + Math.Pow(r22, 2)) * p2;

                    oriima[i, 0] += deltaX2;
                    oriima[i, 1] += deltaY2;                   

                    oriima[i, 0] = oriima[i, 0] / Hpixsize + (Hpix + 1) / 2;
                    oriima[i, 1] = (Vpix + 1) / 2 - oriima[i, 1] / Vpixsize;
                }
                
                double objW = objWH[0, 0], objH = objWH[0, 1], r1, r2, r3, r4, gsd;
                r1 = Math.Sqrt(Math.Pow(oriima[1, 0] - oriima[0, 0], 2) + Math.Pow(oriima[1, 1] - oriima[0, 1], 2));
                r2 = Math.Sqrt(Math.Pow(oriima[1, 0] - oriima[2, 0], 2) + Math.Pow(oriima[1, 1] - oriima[2, 1], 2));
                r3 = Math.Sqrt(Math.Pow(oriima[2, 0] - oriima[3, 0], 2) + Math.Pow(oriima[2, 1] - oriima[3, 1], 2));
                r4 = Math.Sqrt(Math.Pow(oriima[3, 0] - oriima[0, 0], 2) + Math.Pow(oriima[3, 1] - oriima[0, 1], 2));
                gsd = Math.Round((r1 / objW + r2 / objH + r3 / objW + r4 / objH) / 4, 2, MidpointRounding.AwayFromZero);

                oriobj[0, 0] = 0;
                oriobj[0, 1] = 0;
                oriobj[1, 0] = gsd * objW;
                oriobj[1, 1] = 0;
                oriobj[2, 0] = 0;
                oriobj[2, 1] = gsd * objH;
                oriobj[3, 0] = gsd * objW;
                oriobj[3, 1] = gsd * objH;
            }

            orthoW = (int)(oriobj[3, 0]);
            orthoH = (int)(oriobj[3, 1]);
            bt1.IsEnabled = true;
            bt2.IsEnabled = true;
            //MessageBox.Show("Hp = " + Hpixsize + "\nVp =" + Vpixsize + "\nxp = " + xp + "\nyp = " + yp + "\nk1 = " + k1
            //+ "\nk2 = " + k2 + "\nk3 = " + k3 + "\np1 = " + p1 + "\np2 = " + p2 + "\nb1 = " + b1 + "\nb2 = " + b2);
        }

        //八參數
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Stopwatch time1 = new Stopwatch();
            time1.Start();

            double[,] AA = new double[8, 8];
            double[,] LL = new double[8, 1];
            double[,] XX = new double[8, 1];

            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                {
                    AA[i, 0] = oriobj[i / 2, 0];
                    AA[i, 1] = oriobj[i / 2, 1];
                    AA[i, 2] = 1;
                    AA[i, 3] = 0;
                    AA[i, 4] = 0;
                    AA[i, 5] = 0;
                    AA[i, 6] = -1 * oriobj[i / 2, 0] * oriima[i / 2, 0];
                    AA[i, 7] = -1 * oriobj[i / 2, 1] * oriima[i / 2, 0];
                }
                else
                {
                    AA[i, 0] = 0;
                    AA[i, 1] = 0;
                    AA[i, 2] = 0;
                    AA[i, 3] = oriobj[i / 2, 0];
                    AA[i, 4] = oriobj[i / 2, 1];
                    AA[i, 5] = 1;
                    AA[i, 6] = -1 * oriobj[i / 2, 0] * oriima[i / 2, 1];
                    AA[i, 7] = -1 * oriobj[i / 2, 1] * oriima[i / 2, 1];
                }
            }

            for (int i = 0; i < 4; i++)
            {
                LL[i * 2, 0] = oriima[i, 0];
                LL[i * 2 + 1, 0] = oriima[i, 1];
            }

            Matrix<double> A = Matrix<double>.Build.Dense(8, 8, (i, j) => AA[i, j]);
            Matrix<double> L = Matrix<double>.Build.Dense(8, 1, (i, j) => LL[i, j]);
            Matrix<double> N = A.Transpose().Multiply(A);
            Matrix<double> X = N.Inverse().Multiply(A.Transpose()).Multiply(L);


            LOCImage orthoima = new LOCImage(orthoW, orthoH, image0.DpiX, image0.DpiY, PixelFormats.Bgr24, null);

            for (int i = 0; i < orthoW; i++)
            {
                int index;
                double oriX, oriY, deltaX = 0, deltaY = 0, r, xbar, ybar, tmp, tmp2, tmpX, tmpY;
                for (int j = 0; j < orthoH; j++)
                {
                    index = ((j - 0) * orthoW + i - 0) * 3;

                    oriX = (X[0, 0] * i + X[1, 0] * j + X[2, 0]) / (X[6, 0] * i + X[7, 0] * j + 1);
                    oriY = (X[3, 0] * i + X[4, 0] * j + X[5, 0]) / (X[6, 0] * i + X[7, 0] * j + 1);

                    // 透鏡畸變糾正
                    oriX = Hpixsize * (oriX - (Hpix + 1) / 2);
                    oriY = Vpixsize * ((Vpix + 1) / 2 - oriY);
                    tmpX = oriX;
                    tmpY = oriY;

                    do
                    {
                        tmp = deltaX;
                        tmp2 = deltaY;
                        xbar = oriX - xp;
                        ybar = oriY - yp;

                        r = Math.Sqrt(Math.Pow(xbar, 2) + Math.Pow(ybar, 2));
                        deltaX = xbar * Math.Pow(r, 2) * k1 + xbar * Math.Pow(r, 4) * k2 + xbar * Math.Pow(r, 6) * k3
                            + (2 * Math.Pow(xbar, 2) + Math.Pow(r, 2)) * p1 + 2 * xbar * ybar * p2 + b1 * xbar + b2 * ybar;
                        deltaY = ybar * Math.Pow(r, 2) * k1 + ybar * Math.Pow(r, 4) * k2 + ybar * Math.Pow(r, 6) * k3
                            + 2 * xbar * ybar * p1 + (2 * Math.Pow(ybar, 2) + Math.Pow(r, 2)) * p2;

                        oriX = tmpX - deltaX;
                        oriY = tmpY - deltaY;

                    }
                    //while (false);
                    while (Math.Abs(deltaX - tmp) > 0.000001 || Math.Abs(deltaY - tmp2) > 0.000001);

                    oriX = oriX / Hpixsize + (Hpix + 1) / 2;
                    oriY = (Vpix + 1) / 2 - oriY / Vpixsize;
                    

                    if (oriX < 0 || oriX >= W || oriY < 0 || oriY >= H)
                    {
                        continue;
                    }
                    else
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            if ((int)oriX != W - 1 && (int)oriY != H - 1)
                            {
                                orthoima.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX + 1 + ((int)oriY + 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                            else if ((int)oriX == W - 1 && (int)oriY != H - 1)
                            {
                                orthoima.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX - 1 + ((int)oriY + 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                            else if ((int)oriX != W - 1 && (int)oriY == H - 1)
                            {
                                orthoima.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX + 1 + ((int)oriY - 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                            else if ((int)oriX == W - 1 && (int)oriY == H - 1)
                            {
                                orthoima.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX - 1 + ((int)oriY - 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                        }
                    }
                }
            }

            time1.Stop();

            FileInfo fi = new FileInfo(filename);
            String direc = fi.DirectoryName, file = fi.Name, transfile = direc + "\\new_" + file;
            orthoima.Save(transfile, ImageFormat.Jpeg);

            using (var stream = new FileStream(transfile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ortho.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            double stdx, sumx = 0, averx, tmpx = 0, stdy, sumy = 0, avery, tmpy = 0, rmsx, tmpx2 = 0, rmsy, tmpy2 = 0;
            Matrix<double> V = A.Multiply(X).Subtract(L);
            Matrix<double> AX = A.Multiply(X);

            for (int i = 0; i < 8; i += 2)
            {
                sumx += V[i, 0];
            }
            averx = sumx / 4;
            for (int i = 0; i < 8; i += 2)
            {
                tmpx += Math.Pow(V[i, 0] - averx, 2);
                tmpx2 += Math.Pow(V[i, 0], 2);
            }
            stdx = Math.Sqrt(tmpx / (4 - 1));
            rmsx = Math.Sqrt(tmpx2 / 4);

            for (int i = 0; i < 8; i += 2)
            {
                sumy += V[i + 1, 0];
            }
            avery = sumy / 4;
            for (int i = 0; i < 8; i += 2)
            {
                tmpy += Math.Pow(V[i + 1, 0] - avery, 2);
                tmpy2 += Math.Pow(V[i + 1, 0], 2);
            }
            stdy = Math.Sqrt(tmpy / (4 - 1));
            rmsy = Math.Sqrt(tmpy2 / 4);

            TB1.Text = "執行時間" + String.Format("{0:N3}", Math.Round(time1.Elapsed.TotalMilliseconds / 1000, 3, MidpointRounding.AwayFromZero)) + "秒\n"
                 + "每一萬個像素" + String.Format("{0:N3}", Math.Round(time1.Elapsed.TotalMilliseconds / orthoW / orthoH / 1000 * 10000, 3, MidpointRounding.AwayFromZero)) + "秒\n";
            MessageBox.Show("點號    原始座標X    原始座標Y    轉換後座標X    轉換後座標Y    X差值    Y差值\n"
                            + "  1    " + oriima[0, 0] + "    " + oriima[0, 1] + "    " + AX[0, 0] + "    " + AX[1, 0] + "    " + V[0, 0] + "    " + V[1, 0] + "\n"
                            + "  2    " + oriima[1, 0] + "    " + oriima[1, 1] + "    " + AX[2, 0] + "    " + AX[3, 0] + "    " + V[2, 0] + "    " + V[3, 0] + "\n"
                            + "  3    " + oriima[2, 0] + "    " + oriima[2, 1] + "    " + AX[4, 0] + "    " + AX[5, 0] + "    " + V[4, 0] + "    " + V[5, 0] + "\n"
                            + "  4    " + oriima[3, 0] + "    " + oriima[3, 1] + "    " + AX[6, 0] + "    " + AX[7, 0] + "    " + V[6, 0] + "    " + V[7, 0] + "\n"
                            + "\n"
                            + "X差值均方根: " + rmsx + " " + "Y差值均方根: " + rmsy + "\n"
                            + "X差值標準差: " + stdx + " " + "Y差值標準差: " + stdy);
        }

        //雙線性
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Stopwatch time2 = new Stopwatch();
            time2.Start();

            LOCImage orthoima2 = new LOCImage(orthoW, orthoH, image0.DpiX, image0.DpiY, PixelFormats.Bgr24, null);

            for (int i = 0; i < orthoW; i++)
            {
                int index;
                double oriX, oriY, ux, uy, vx, vy, deltaX = 0, deltaY = 0, r, xbar, ybar, tmpX, tmpY, tmp, tmp2;
                for (int j = 0; j < orthoH; j++)
                {
                    index = ((j - 0) * orthoW + i - 0) * 3;
                    ux = oriima[0, 0] + i * (oriima[1, 0] - oriima[0, 0]) / orthoW;
                    uy = oriima[0, 1] + i * (oriima[1, 1] - oriima[0, 1]) / orthoW;
                    vx = oriima[2, 0] + i * (oriima[3, 0] - oriima[2, 0]) / orthoW;
                    vy = oriima[2, 1] + i * (oriima[3, 1] - oriima[2, 1]) / orthoW;
                    oriX = ux + j * (vx - ux) / orthoH;
                    oriY = uy + j * (vy - uy) / orthoH;

                    // 透鏡畸變糾正
                    oriX = Hpixsize * (oriX - (Hpix + 1) / 2);
                    oriY = Vpixsize * ((Vpix + 1) / 2 - oriY);
                    tmpX = oriX;
                    tmpY = oriY;

                    do
                    {
                        tmp = deltaX;
                        tmp2 = deltaY;
                        xbar = oriX - xp;
                        ybar = oriY - yp;

                        r = Math.Sqrt(Math.Pow(xbar, 2) + Math.Pow(ybar, 2));
                        deltaX = xbar * Math.Pow(r, 2) * k1 + xbar * Math.Pow(r, 4) * k2 + xbar * Math.Pow(r, 6) * k3
                            + (2 * Math.Pow(xbar, 2) + Math.Pow(r, 2)) * p1 + 2 * xbar * ybar * p2 + b1 * xbar + b2 * ybar;
                        deltaY = ybar * Math.Pow(r, 2) * k1 + ybar * Math.Pow(r, 4) * k2 + ybar * Math.Pow(r, 6) * k3
                            + 2 * xbar * ybar * p1 + (2 * Math.Pow(ybar, 2) + Math.Pow(r, 2)) * p2;

                        oriX = tmpX - deltaX;
                        oriY = tmpY - deltaY;
                    }
                    //while (false);
                    while (Math.Abs(deltaX - tmp) > 0.000001 || Math.Abs(deltaY - tmp2) > 0.000001);
                    
                    oriX = oriX / Hpixsize + (Hpix + 1) / 2;
                    oriY = (Vpix + 1) / 2 - oriY / Vpixsize;
                    

                    if (oriX < 0 || oriX >= W || oriY < 0 || oriY >= H)
                    { }
                    else
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            if ((int)oriX != W - 1 && (int)oriY != H - 1)
                            {
                                orthoima2.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX + 1 + ((int)oriY + 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                            else if ((int)oriX == W - 1 && (int)oriY != H - 1)
                            {
                                orthoima2.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY + 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX - 1 + ((int)oriY + 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                            else if ((int)oriX != W - 1 && (int)oriY == H - 1)
                            {
                                orthoima2.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX + 1 + ((int)oriY - 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                            else if ((int)oriX == W - 1 && (int)oriY == H - 1)
                            {
                                orthoima2.ByteData[index + k] = (Byte)(image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]
                                    + ((image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriX - (int)oriX)
                                    + ((image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) - (image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k])) * (oriY - (int)oriY)
                                    + ((image0.ByteData[((int)oriX + (int)oriY * W) * 3 + k]) - (image0.ByteData[((int)oriX - 1 + (int)oriY * W) * 3 + k])
                                        - (image0.ByteData[((int)oriX + ((int)oriY - 1) * W) * 3 + k]) + (image0.ByteData[((int)oriX - 1 + ((int)oriY - 1) * W) * 3 + k])) * (oriX - (int)oriX) * (oriY - (int)oriY));
                            }
                        }
                    }
                }
            }

            time2.Stop();

            FileInfo fi = new FileInfo(filename);
            String direc = fi.DirectoryName, file = fi.Name, transfile = direc + "\\new2_" + file;
            orthoima2.Save(transfile, ImageFormat.Jpeg);

            using (var stream = new FileStream(transfile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ortho.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            TB2.Text = "執行時間" + String.Format("{0:N3}", Math.Round(time2.Elapsed.TotalMilliseconds / 1000, 3, MidpointRounding.AwayFromZero)) + "秒\n"
                 + "每一萬個像素" + String.Format("{0:N3}", Math.Round(time2.Elapsed.TotalMilliseconds / orthoW / orthoH / 1000 * 10000, 3, MidpointRounding.AwayFromZero)) + "秒";
        }
    }
}
